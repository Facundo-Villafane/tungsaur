using UnityEngine;
using System;
using System.Collections;

[RequireComponent(typeof(Rigidbody))]
public class EnemyController : MonoBehaviour
{
    public event Action OnEnemyDeath;

    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 1f;
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
    [SerializeField] private Animator animator;
    public Animator Animator => animator;

    [Header("Rotation")]
    [SerializeField] private bool flipSprite = true;
    [SerializeField] private float rotationSpeed = 10f;

    [Header("AI Settings")]
    [SerializeField] private float detectionRadius = 8f;
    [SerializeField] private float attackRange = 1.5f;

    [Header("Fallback Settings")]
    [SerializeField] private float fallbackForce = 1f;

    public bool IsGrounded { get; private set; } = true;
    public bool IsMoving { get; private set; }
    public float DetectionRadius => detectionRadius;
    public float MoveSpeed => moveSpeed;

    public bool isDead = false;
    private bool isFallen = false;
    private bool isFalling = false;
    private bool isBouncing = false;

    private Rigidbody rb;
    private Vector3 currentVelocity;
    private EnemyState currentState;

    

    private void Start()
    {
        rb = GetComponent<Rigidbody>();

        if (animator == null)
        {
            animator = GetComponent<Animator>();
        }

        // Estado inicial
        ChangeState(new IdleState(this));
    }

    private void Update()
    {
        currentState?.Update();
        if (isDead) return;

        if (isFalling)
        {
            animator.SetTrigger("Fall");
            ApplyFallbackForceTwo();
            isFalling = false;
        }

    }

    private void FixedUpdate()
    {
        currentState?.FixedUpdate();

        if (isDead) return;

        CheckGrounded();

        if (usePhysics && rb != null)
        {
            MoveWithPhysics();
            ApplyExtraGravity();
        }

        HandleRotation();
        UpdateAnimator();

        
    }

    private void CheckGrounded()
    {
        Ray ray = new Ray(transform.position, Vector3.down);
        IsGrounded = Physics.Raycast(ray, groundCheckDistance, groundLayer);
    }

    private void MoveWithPhysics()
    {
        Vector3 targetVelocity = Vector3.zero; // Por ahora no tiene input, se moverá por IA
        currentVelocity = Vector3.Lerp(
            currentVelocity,
            targetVelocity,
            (targetVelocity.magnitude > 0 ? acceleration : deceleration) * Time.fixedDeltaTime
        );

        currentVelocity = Vector3.ClampMagnitude(currentVelocity, maxSpeed);

        rb.linearVelocity = new Vector3(currentVelocity.x, rb.linearVelocity.y, currentVelocity.z);

        IsMoving = targetVelocity.magnitude > 0.1f;
    }

    private void ApplyExtraGravity()
    {
        if (rb.linearVelocity.y < 0 && !IsGrounded)
        {
            rb.AddForce(Physics.gravity * (fallMultiplier - 1f), ForceMode.Acceleration);
        }
    }

    private void HandleRotation()
    {
        if (currentVelocity.x != 0)
        {
            if (flipSprite)
            {
                Vector3 localScale = transform.localScale;
                localScale.x = currentVelocity.x < 0 ? -Mathf.Abs(localScale.x) : Mathf.Abs(localScale.x);
                transform.localScale = localScale;
            }
            else
            {
                float targetAngle = currentVelocity.x > 0 ? 180f : 0f;
                Quaternion targetRotation = Quaternion.Euler(0, targetAngle, 0);
                transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, rotationSpeed * Time.fixedDeltaTime);
            }
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

    public void TakeHit()
    {
        if (!isDead)
        {
            animator?.SetTrigger("HitTook");
        }
    }

    public void Die()
    {
        if (isDead) return;

        OnEnemyDeath?.Invoke();
        isDead = true;

        animator?.SetTrigger("Fall");
        StartCoroutine(DestroyAfterDelay(5f));
    }

    private IEnumerator DestroyAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        Destroy(gameObject);
    }

    public void ChangeState(EnemyState newState)
    {
        currentState?.Exit();
        currentState = newState;
        currentState?.Enter();
    }

    // Rebote contra paredes o golpes
    public void ApplyFallbackForceTwo()
    {
        if (rb != null)
        {
            StartCoroutine(SmoothFallbackTwo());
        }
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
}
