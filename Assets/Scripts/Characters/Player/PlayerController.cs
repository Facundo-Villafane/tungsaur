using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(PlayerCombat))]
public class PlayerController : CharacterBase
{
    [Header("Movement Settings")]
    [SerializeField] private float acceleration = 10f;
    [SerializeField] private float deceleration = 10f;
    [SerializeField] private float maxSpeed = 8f;

    [Header("Physics")]
    [SerializeField] private bool usePhysics = true;

    [Header("Rotation")]
    [SerializeField] private bool flipSprite = true;
    [SerializeField] private float rotationSpeed = 10f;

    [Header("Jump")]
    [SerializeField] private float jumpForce = 5f;
    [SerializeField] private float groundCheckDistance = 0.2f;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private float fallMultiplier = 2.5f;

    [Header("Fall Settings")]
    [SerializeField] private float fallbackForce = 1f;

    [Header("Attack Cooldown")]
    [SerializeField] private float attackCooldown = 1f; // tiempo entre ataques
    private float lastAttackTime = -999f;
    public float AttackCooldown => attackCooldown;
    public float LastAttackTime => lastAttackTime;

    [Header("UI Controller")]
    [SerializeField] private UIController uiController;

    private PlayerState currentState;

    public Rigidbody rb { get; private set; }
    private Vector3 currentVelocity;
    public Vector3 InputVector { get; set; }
    public bool IsMoving => currentVelocity.magnitude > 0.1f;
    public bool IsRunning { get; set; } = false;
    public bool isGrounded { get; private set; } = true;
    private bool isFallen = false;
    private bool isBouncing = false;

    public PlayerCombat Combat { get; private set; }
    public Animator Animator => animator;
    [SerializeField] private AudioManager audioManager;
    public AudioManager AudioManager => audioManager;

    [SerializeField] private AudioSource audioSource;
    public AudioSource AudioSource => audioSource;

    protected override void Awake()
    {
        base.Awake();
        rb = GetComponent<Rigidbody>();
        Combat = GetComponent<PlayerCombat>();
    }

    private void Start()
    {
        ChangeState(new PlayerIdleState(this));

        if (uiController != null)
        {
            uiController.UpdateHealth(CurrentHealth, MaxHealth);
            uiController.UpdateEnergy(Energy, 100f);
        }

        if (audioManager == null)
        {
            audioManager = AudioManager.Instance;
            Debug.Log(audioManager != null
                ? "PlayerController: AudioManager asignado automáticamente desde instancia."
                : "PlayerController: AudioManager no encontrado en escena.");
        }

        if (audioSource == null)
        {
            audioSource = GetComponent<AudioSource>();
            if (audioSource == null)
                audioSource = gameObject.AddComponent<AudioSource>();

            Debug.Log("PlayerController: AudioSource asignado automáticamente.");
        }
    }

    private void Update()
    {
        currentState?.Update();

        if (uiController != null)
        {
            uiController.UpdateHealth(CurrentHealth, MaxHealth);
            uiController.UpdateEnergy(Energy, 100f);
        }

        if (IsDead && !(currentState is PlayerDeadState))
        {
            ChangeState(new PlayerDeadState(this));
            return;
        }

        // Ataque (solo si pasó el cooldown)
        if (Keyboard.current?.kKey.wasPressedThisFrame == true && CanAttack())
        {
            RegisterAttack();
            ChangeState(new PlayerAttacksState(this));
        }

        // Salto
        if (Keyboard.current?.spaceKey.wasPressedThisFrame == true && isGrounded)
        {
            PerformJump();
            ChangeState(new PlayerJumpState(this));
        }
    }

    private void FixedUpdate()
    {
        CheckGrounded();
        currentState?.FixedUpdate();
    }

    public void ChangeState(PlayerState newState)
    {
        if (IsDead && !(newState is PlayerDeadState))
            return;

        currentState?.Exit();
        currentState = newState;
        currentState.Enter();
    }

    public bool CanAttack()
    {
        return Time.time >= lastAttackTime + attackCooldown;
    }

    public void RegisterAttack()
    {
        lastAttackTime = Time.time;
    }

    public void MoveWithPhysics()
    {
        float currentMoveSpeed = IsRunning ? MoveSpeed * 2f : MoveSpeed;
        float currentMaxSpeed = IsRunning ? maxSpeed * 2f : maxSpeed;

        Vector3 inputDir = new Vector3(InputVector.x, 0f, InputVector.z);
        Vector3 targetVelocity = inputDir * currentMoveSpeed;

        currentVelocity = Vector3.Lerp(currentVelocity, targetVelocity,
            (inputDir.magnitude > 0 ? acceleration : deceleration) * Time.fixedDeltaTime);

        currentVelocity = Vector3.ClampMagnitude(currentVelocity, currentMaxSpeed);
        rb.linearVelocity = new Vector3(currentVelocity.x, rb.linearVelocity.y, currentVelocity.z);
    }

    public void HandleRotation()
    {
        if (Mathf.Abs(InputVector.x) < 0.01f) return;

        if (flipSprite)
        {
            Vector3 localScale = transform.localScale;
            localScale.x = InputVector.x < 0 ? -Mathf.Abs(localScale.x) : Mathf.Abs(localScale.x);
            transform.localScale = localScale;
        }
        else
        {
            float targetAngle = InputVector.x > 0 ? 180f : 0f;
            transform.rotation = Quaternion.Lerp(transform.rotation,
                Quaternion.Euler(0, targetAngle, 0),
                rotationSpeed * Time.fixedDeltaTime);
        }
    }

    public void ApplyExtraGravity()
    {
        if (!isGrounded && rb.linearVelocity.y < 0)
            rb.AddForce(Physics.gravity * (fallMultiplier - 1f), ForceMode.Acceleration);
    }

    public void PerformJump()
    {
        if (!isGrounded) return;
        rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        animator?.SetTrigger("Jump 0");
        AudioManager.Instance.SonidoSalto2();
        isGrounded = false;
    }

    private void CheckGrounded()
    {
        Ray ray = new Ray(transform.position, Vector3.down);
        isGrounded = Physics.Raycast(ray, groundCheckDistance, groundLayer);
    }

    public override void TakeDamage(float damage)
    {
        if (IsDead) return;

        base.TakeDamage(damage);

        if (!IsDead)
            ChangeState(new PlayerHitState(this));
        else
            ChangeState(new PlayerDeadState(this));
    }

    public void Heal(float amount)
    {
        CurrentHealth = Mathf.Min(MaxHealth, CurrentHealth + amount);
        uiController?.UpdateHealth(CurrentHealth, MaxHealth);
        uiController?.ShowHealthPickupIndicator();
    }

    public void UseEnergy(float amount)
    {
        Energy = Mathf.Max(0f, Energy - amount);
        uiController?.UpdateEnergy(Energy, 100f);
    }

    public void GainEnergy(float amount)
    {
        Energy = Mathf.Min(100f, Energy + amount);
        uiController?.UpdateEnergy(Energy, 100f);
        uiController?.ShowEnergyPickupIndicator();
    }

    public void ApplyFallbackForceTwo()
    {
        if (rb != null) StartCoroutine(SmoothFallbackTwo());
    }

    private IEnumerator SmoothFallbackTwo()
    {
        isBouncing = true;
        float direction = transform.localScale.x > 0 ? -1f : 1f;
        float verticalDirection = 1f;
        float duration = 0.2f;
        float elapsed = 0f;
        float totalForce = fallbackForce;
        float verticalForce = 5f;

        Vector3 localScale = transform.localScale;
        localScale.x = direction < 0 ? -Mathf.Abs(localScale.x) : Mathf.Abs(localScale.x);
        transform.localScale = localScale;

        while (elapsed < duration)
        {
            Vector3 force = new Vector3(
                direction * (totalForce / duration) * Time.fixedDeltaTime,
                verticalDirection * (verticalForce / duration) * Time.fixedDeltaTime,
                0f
            );
            rb.AddForce(force, ForceMode.VelocityChange);
            elapsed += Time.fixedDeltaTime;
            yield return new WaitForFixedUpdate();
        }

        isBouncing = false;
    }

    public void StartFall()
    {
        if (!isFallen)
            ChangeState(new PlayerFallState(this));
    }
}
