using UnityEngine;
using System.Collections;

public class BossController : CharacterBase
{
    [Header("Boss Settings")]
    [SerializeField] private float attackRange = 5f;
    [SerializeField] private float attackCooldown = 2f;
    [SerializeField] private float dashCooldown = 6f;
    [SerializeField] private float dashForce = 20f;
    [SerializeField] private float dashDuration = 1f;
    [SerializeField] private float attackDamage = 20f;

    [Header("Área de movimiento del boss")]
    [SerializeField] private Vector2 minBounds = new Vector2(-10f, -5f);
    [SerializeField] private Vector2 maxBounds = new Vector2(10f, 5f);

    private float lastAttackTime;
    private float lastDashTime;

    private enum BossState { Idle, Attacking, Dashing }
    private BossState currentState = BossState.Idle;

    private Rigidbody rb;
    private bool hasHitDuringDash = false;

    protected void Start()
    {
        lastAttackTime = Time.time;
        lastDashTime = Time.time;
        rb = GetComponent<Rigidbody>();
        if (animator == null) animator = GetComponent<Animator>();
        Debug.Log("Boss en estado idle");
    }

    private void Update()
    {
        if (IsDead || SlotManager.Instance?.Player == null) return;

        FacePlayer();

        switch (currentState)
        {
            case BossState.Idle:
                if (Time.time >= lastDashTime + dashCooldown && IsPlayerInRange())
                {
                    currentState = BossState.Dashing;
                    lastDashTime = Time.time;
                    StartCoroutine(DashRoutine());
                }
                else if (Time.time >= lastAttackTime + attackCooldown && IsPlayerInRange())
                {
                    currentState = BossState.Attacking;
                    lastAttackTime = Time.time;
                }
                break;

            case BossState.Attacking:
                HandleAttack();
                break;

            case BossState.Dashing:
                // Movimiento controlado por la corutina
                break;
        }
    }

    private bool IsPlayerInRange()
    {
        float distance = Vector3.Distance(transform.position, SlotManager.Instance.Player.position);
        return distance <= attackRange;
    }

    private void FacePlayer()
    {
        if (SlotManager.Instance?.Player == null) return;

        Vector3 direction = SlotManager.Instance.Player.position - transform.position;

        if (direction.x > 0)
            transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
        else
            transform.localScale = new Vector3(-Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
    }

    private void HandleAttack()
    {
        if (SlotManager.Instance?.Player == null) return;

        Vector3 direction = (SlotManager.Instance.Player.position - transform.position).normalized;
        rb.MovePosition(transform.position + direction * MoveSpeed * Time.deltaTime);

        if (Vector3.Distance(transform.position, SlotManager.Instance.Player.position) < 1f)
        {
            Golpear();
            currentState = BossState.Idle;
            Debug.Log("Boss vuelve a estado idle");
        }
    }

    private void Golpear()
    {
        SlotManager.Instance.Player.GetComponent<CharacterBase>()?.TakeDamage(attackDamage);
        Debug.Log("Boss golpeó al jugador");
    }

    private IEnumerator DashRoutine()
    {
        if (SlotManager.Instance?.Player == null)
        {
            currentState = BossState.Idle;
            yield break;
        }

        hasHitDuringDash = false;

        Vector3 direction = (SlotManager.Instance.Player.position - transform.position).normalized;
        rb.useGravity = false;
        rb.linearVelocity = direction * dashForce;

        Debug.Log("Dash iniciado con dirección: " + direction);

        float elapsed = 0f;
        while (elapsed < dashDuration)
        {
            if (SlotManager.Instance?.Player == null) break;

            // Limitar posición del boss dentro del rectángulo
            Vector3 clampedPosition = transform.position;
            clampedPosition.x = Mathf.Clamp(clampedPosition.x, minBounds.x, maxBounds.x);
            clampedPosition.y = Mathf.Clamp(clampedPosition.y, minBounds.y, maxBounds.y);
            transform.position = clampedPosition;

            if (!hasHitDuringDash && Vector3.Distance(transform.position, SlotManager.Instance.Player.position) < 1f)
            {
                Golpear();
                hasHitDuringDash = true;
                Debug.Log("Golpe durante el dash");
            }

            elapsed += Time.deltaTime;
            yield return null;
        }

        rb.linearVelocity = Vector3.zero;
        rb.useGravity = true;
        currentState = BossState.Idle;
        Debug.Log("Dash finalizado");
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Vector3 center = new Vector3(
            (minBounds.x + maxBounds.x) / 2f,
            (minBounds.y + maxBounds.y) / 2f,
            transform.position.z
        );
        Vector3 size = new Vector3(
            Mathf.Abs(maxBounds.x - minBounds.x),
            Mathf.Abs(maxBounds.y - minBounds.y),
            0.1f
        );
        Gizmos.DrawWireCube(center, size);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}
