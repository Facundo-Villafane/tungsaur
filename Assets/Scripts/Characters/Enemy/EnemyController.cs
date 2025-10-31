using UnityEngine;
using System;
using System.Collections;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(AudioSource))]
public class EnemyController : CharacterBase
{
    public event Action OnEnemyDeath;

    [Header("Movement Settings")]
    [SerializeField] private float acceleration = 10f;
    [SerializeField] private float deceleration = 10f;
    [SerializeField] private float maxSpeed = 5f;
    public float AttackRange => attackRange;

    [Header("Physics")]
    [SerializeField] private bool usePhysics = true;
    [SerializeField] private float fallMultiplier = 2.5f;
    [SerializeField] private float groundCheckDistance = 0.2f;
    [SerializeField] private LayerMask groundLayer;

    [Header("Animation")]
    public Animator Animator => animator;

    [Header("Patrulla")]
    public Transform[] patrolPoints;
    public int currentPatrolIndex = 0;
    [HideInInspector] public int AssignedSlot = -1;

    [Header("Rotation")]
    [SerializeField] private bool flipSprite = true;
    [SerializeField] private float rotationSpeed = 10f;

    [Header("AI Settings")]
    [SerializeField] private float detectionRadius = 8f;
    [SerializeField] private float attackRange = 2f;

    [Header("Fallback Settings")]
    [SerializeField] private float fallbackForce = 1f;

    [Header("Audio")]
    [SerializeField] private AudioManager audioManager;
    public AudioManager AudioManager => audioManager;
    [SerializeField] private AudioSource audioSource;
    public AudioSource AudioSource => audioSource;

    public bool IsGrounded { get; private set; } = true;
    public bool IsMoving { get; private set; }
    public float DetectionRadius => detectionRadius;

    private bool isFalling = false;
    private bool isBouncing = false;

    public Rigidbody rb;
    public Rigidbody Rb => rb;
    private Vector3 currentVelocity;
    private EnemyState currentState;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        if (animator == null)
            animator = GetComponent<Animator>();

        if (audioManager == null)
        {
            audioManager = AudioManager.Instance;
            Debug.Log(audioManager != null
                ? "EnemyController: AudioManager asignado automáticamente desde instancia."
                : "EnemyController: AudioManager no encontrado en escena.");
        }

        if (audioSource == null)
        {
            audioSource = GetComponent<AudioSource>();
            if (audioSource == null)
                audioSource = gameObject.AddComponent<AudioSource>();

            Debug.Log("EnemyController: AudioSource asignado automáticamente.");
        }

        ValidarAudioSource();
        ChangeState(new CirclePatrolState(this));
    }

    private void ValidarAudioSource()
    {
        if (audioSource == null)
        {
            Debug.LogError($"[EnemyController: {name}] AudioSource no asignado.");
            return;
        }

        if (!audioSource.enabled)
        {
            Debug.LogWarning($"[EnemyController: {name}] AudioSource estaba deshabilitado. Se habilita automáticamente.");
            audioSource.enabled = true;
        }

        if (!audioSource.gameObject.activeInHierarchy)
        {
            Debug.LogWarning($"[EnemyController: {name}] GameObject del AudioSource estaba inactivo. Se activa automáticamente.");
            audioSource.gameObject.SetActive(true);
        }
    }

    private void Update()
    {
        if (IsDead && !(currentState is EnemyDeadState))
        {
            Debug.Log("Cambiando a estado Dead del enemy");
            ChangeState(new EnemyDeadState(this));
            return;
        }

        currentState?.Update();

        if (isFalling)
        {
            animator.SetTrigger("Fall");
            ApplyFallbackForce();
            isFalling = false;
        }
    }

    private void FixedUpdate()
    {
        if (IsDead) return;
        currentState?.FixedUpdate();

        CheckGrounded();

        if (usePhysics && rb != null)
        {
            MoveWithPhysics();
            ApplyExtraGravity();
        }

        HandleRotation();
        UpdateAnimator();
    }

    public void SetVelocity(Vector3 velocity)
    {
        currentVelocity.x = velocity.x;
        currentVelocity.z = velocity.z;
    }

    private void CheckGrounded()
    {
        Ray ray = new Ray(transform.position, Vector3.down);
        IsGrounded = Physics.Raycast(ray, groundCheckDistance, groundLayer);
    }

    private void MoveWithPhysics()
    {
        if (currentVelocity.sqrMagnitude > 0.001f)
        {
            currentVelocity = Vector3.ClampMagnitude(currentVelocity, maxSpeed);
            rb.linearVelocity = new Vector3(currentVelocity.x, rb.linearVelocity.y, currentVelocity.z);
            IsMoving = currentVelocity.magnitude > 0.1f;
        }
        else
        {
            Vector3 horizontalVelocity = new Vector3(rb.linearVelocity.x, 0, rb.linearVelocity.z);
            horizontalVelocity = Vector3.Lerp(horizontalVelocity, Vector3.zero, deceleration * Time.fixedDeltaTime);
            rb.linearVelocity = new Vector3(horizontalVelocity.x, rb.linearVelocity.y, horizontalVelocity.z);
            IsMoving = horizontalVelocity.magnitude > 0.1f;
        }
    }

    private void ApplyExtraGravity()
    {
        if (rb.linearVelocity.y < 0 && !IsGrounded)
        {
            rb.AddForce(Physics.gravity * (fallMultiplier - 1f), ForceMode.Acceleration);
        }
    }

    public void HandleRotation()
    {
        if (SlotManager.Instance == null || SlotManager.Instance.Player == null)
        {
            Debug.LogWarning($"[EnemyController: {name}] No se puede rotar: SlotManager o Player es null");
            return;
        }

        Vector3 playerPos = SlotManager.Instance.Player.position;
        Vector3 direction = playerPos - transform.position;

        if (direction.sqrMagnitude < 0.001f)
            return;

        if (flipSprite)
        {
            Vector3 localScale = transform.localScale;
            localScale.x = direction.x < 0 ? -Mathf.Abs(localScale.x) : Mathf.Abs(localScale.x);
            transform.localScale = localScale;
        }
        else
        {
            float targetAngle = direction.x > 0 ? 180f : 0f;
            Quaternion targetRotation = Quaternion.Euler(0, targetAngle, 0);
            transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, rotationSpeed * Time.fixedDeltaTime);
        }
    }

    private void UpdateAnimator()
    {
        if (animator != null)
        {
            float xVelocity = IsMoving ? 0.5f : 0f;
            animator.SetFloat("xVelocity", xVelocity);
        }
    }

    public override void TakeHit()
    {
        if (!IsDead)
        {
            ChangeState(new HitState(this));

            if (audioManager != null && audioSource != null && audioSource.enabled)
            {
                audioManager.SonidoDañoEnemigo1(audioSource);
                Debug.Log("AudioManager asignado: " + audioManager.gameObject.name);
                Debug.Log("Clip asignado: " + audioManager.DañoEnemigo1?.name);
            }
            else
            {
                Debug.LogWarning("EnemyController: AudioManager o AudioSource no asignado o desactivado.");
            }
        }
    }

    public override void TakeDamage(float amount)
    {
        base.TakeDamage(amount);
        Debug.Log($"Enemy recibió {amount} de daño.");
        if (!IsDead)
        {
            TakeHit();
        }
    }

    public override void Die()
    {
        if (IsDead) return;
        base.Die();

        if (audioManager != null && audioSource != null && audioSource.enabled)
            audioManager.SonidoEnemigoMuriendo(audioSource);

        OnEnemyDeath?.Invoke();
    }

    public void SetHorizontalVelocity(float x)
    {
        currentVelocity.x = x;
    }

    public void ChangeState(EnemyState newState)
    {
        currentState?.Exit();
        currentState = newState;
        currentState?.Enter();
    }

    public void ApplyFallbackForce()
    {
        if (rb != null)
            StartCoroutine(SmoothFallback());
    }

    private IEnumerator SmoothFallback()
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
}
