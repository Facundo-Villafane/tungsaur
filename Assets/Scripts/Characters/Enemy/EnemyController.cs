using UnityEngine;
using System;
using System.Collections;

[RequireComponent(typeof(Rigidbody))]
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
    // private Transform patrolZone; // Ahora siempre asignada por el spawner
    public Transform[] patrolPoints; // Se llenará dinámicamente
    public int currentPatrolIndex = 0;
    [HideInInspector] public int AssignedSlot = -1; // patrullaje inteligente

    [Header("Rotation")]
    [SerializeField] private bool flipSprite = true;
    [SerializeField] private float rotationSpeed = 10f;

    [Header("AI Settings")]
    [SerializeField] private float detectionRadius = 8f;
    [SerializeField] private float attackRange = 2f;

    [Header("Fallback Settings")]
    [SerializeField] private float fallbackForce = 1f;

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

        
        // if (patrolZone == null)
        // {
        //     patrolPoints = new Transform[0];
        //     Debug.Log("PatrolZone no asignada todavía. El spawner debe asignarla.");
        // }

    ChangeState(new CirclePatrolState(this));
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
    // Solo aplicar la interpolación si currentVelocity tiene magnitud
    // Esto permite que los estados controlen el movimiento
    if (currentVelocity.sqrMagnitude > 0.001f)
    {
        currentVelocity = Vector3.ClampMagnitude(currentVelocity, maxSpeed);
        rb.linearVelocity = new Vector3(currentVelocity.x, rb.linearVelocity.y, currentVelocity.z);
        IsMoving = currentVelocity.magnitude > 0.1f;
    }
    else
    {
        // Si no hay velocidad objetivo, aplicar deceleración
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
    // Validación mejorada
    if (SlotManager.Instance == null || SlotManager.Instance.Player == null)
    {
        Debug.LogWarning($"[EnemyController: {name}] No se puede rotar: SlotManager o Player es null");
        return;
    }

    Vector3 playerPos = SlotManager.Instance.Player.position;
    Vector3 direction = playerPos - transform.position;

    // Evitar rotación si la dirección es muy pequeña
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
        }
    }

    public override void Die()
    {
        if (IsDead) return;
        base.Die(); 
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
