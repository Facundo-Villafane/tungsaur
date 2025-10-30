using UnityEngine;
using System.Collections;

public class BossController : CharacterBase
{
    [Header("Boss Settings")]
    [SerializeField] private float attackRange = 5f;
    [SerializeField] private float attackDamage = 20f;
    [SerializeField] private float dashForce = 20f;
    [SerializeField] private float dashDuration = 1f;

    [Header("Velocidades")]
    [SerializeField] private float searchSpeed = 6f;
    [SerializeField] private float attackSpeed = 8f;

    [Header("Movimiento físico")]
    [SerializeField] private float acceleration = 10f;
    [SerializeField] private float deceleration = 10f;
    [SerializeField] private float maxSpeed = 6f;

    [Header("Área de movimiento del boss")]
    [SerializeField] private Vector2 minBounds = new Vector2(-10f, -5f);
    [SerializeField] private Vector2 maxBounds = new Vector2(10f, 5f);

    [Header("Audio")]
    [SerializeField] private AudioManager audioManager;
    public AudioManager AudioManager => audioManager;

    [SerializeField] private AudioSource audioSource;
    public AudioSource AudioSource => audioSource;

    private Rigidbody rb;
    private Vector3 currentVelocity;
    private bool IsMoving;
    private bool isVulnerable = false;
    private bool hasHitDuringDash = false;

    protected void Start()
    {
        rb = GetComponent<Rigidbody>();
        if (animator == null) animator = GetComponent<Animator>();

        if (audioManager == null)
            Debug.LogWarning("BossController: AudioManager no asignado desde el Inspector.");
        else
            Debug.Log("BossController: AudioManager asignado correctamente: " + audioManager.gameObject.name);

        if (audioSource == null)
            Debug.LogWarning("BossController: AudioSource no asignado desde el Inspector.");
        else if (!audioSource.enabled)
            Debug.LogWarning("BossController: AudioSource está desactivado.");
        else
            Debug.Log("BossController: AudioSource asignado correctamente: " + audioSource.gameObject.name);

        StartCoroutine(SecuenciaDeComportamiento());
    }

    private void Update()
    {
        if (IsDead || SlotManager.Instance?.Player == null) return;
        FacePlayer();
    }

    private IEnumerator SecuenciaDeComportamiento()
    {
        while (!IsDead)
        {
            yield return Idle(2f, vulnerable: true);
            yield return BuscarJugador(2f);
            yield return AtacarSiEnRango();
            yield return Idle(2f, vulnerable: true);
            yield return AtacarSiEnRango();
            yield return Dash();
            yield return Dash();
            yield return Idle(2f, vulnerable: true);
        }
    }

    private IEnumerator Idle(float duration, bool vulnerable = false)
    {
        if (vulnerable)
        {
            isVulnerable = true;
            animator.SetTrigger("Iddle");
            Debug.Log("Boss entra en estado vulnerable.");
        }

        yield return new WaitForSeconds(duration);

        if (vulnerable)
        {
            isVulnerable = false;
            Debug.Log("Boss sale de estado vulnerable.");
        }
    }

    private IEnumerator BuscarJugador(float duration)
    {
        float elapsed = 0f;
        while (elapsed < duration)
        {
            if (SlotManager.Instance?.Player == null) yield break;

            Vector3 direction = (SlotManager.Instance.Player.position - transform.position).normalized;
            SetVelocity(direction * searchSpeed);
            MoveWithPhysics();

            elapsed += Time.deltaTime;
            yield return null;
        }
    }

    private IEnumerator AtacarSiEnRango()
    {
        if (SlotManager.Instance?.Player == null) yield break;

        float distancia = Vector3.Distance(transform.position, SlotManager.Instance.Player.position);
        Debug.Log($"Boss detecta distancia al jugador: {distancia}");

        if (distancia < attackRange)
        {
            animator.SetTrigger("PrepareAttack");
            Debug.Log("Boss prepara ataque.");
            yield return new WaitForSeconds(0.5f);

            if (!isVulnerable && Vector3.Distance(transform.position, SlotManager.Instance.Player.position) < 1.5f)
            {
                Golpear();
            }
        }
    }

    private void Golpear()
    {
        if (isVulnerable) return;

        var target = SlotManager.Instance.Player.GetComponent<CharacterBase>();
        if (target != null)
        {
            target.TakeDamage(attackDamage);
            Debug.Log($"Boss golpea al jugador e inflige {attackDamage} de daño.");

            if (audioManager != null && audioSource != null && audioSource.enabled)
            {
                audioManager.SonidoAtaqueEspada1(audioSource);
                Debug.Log("Boss reproduce sonido de ataque.");
            }
            else
            {
                Debug.LogWarning("BossController: AudioManager o AudioSource no asignado o desactivado.");
            }
        }
        else
        {
            Debug.LogWarning("BossController: No se encontró CharacterBase en el jugador.");
        }
    }

    private IEnumerator Dash()
    {
        if (SlotManager.Instance?.Player == null) yield break;

        hasHitDuringDash = false;
        Vector3 direction = (SlotManager.Instance.Player.position - transform.position).normalized;
        rb.useGravity = false;
        rb.linearVelocity = direction * dashForce;

        Debug.Log("Boss inicia dash.");

        float elapsed = 0f;
        while (elapsed < dashDuration)
        {
            if (SlotManager.Instance?.Player == null) break;

            Vector3 clampedPosition = transform.position;
            clampedPosition.x = Mathf.Clamp(clampedPosition.x, minBounds.x, maxBounds.x);
            clampedPosition.y = Mathf.Clamp(clampedPosition.y, minBounds.y, maxBounds.y);
            transform.position = clampedPosition;

            if (!hasHitDuringDash && Vector3.Distance(transform.position, SlotManager.Instance.Player.position) < 1f)
            {
                Golpear();
                hasHitDuringDash = true;
                Debug.Log("Boss golpea durante el dash.");
            }

            elapsed += Time.deltaTime;
            yield return null;
        }

        rb.linearVelocity = Vector3.zero;
        rb.useGravity = true;

        Debug.Log("Boss termina dash.");
    }

    private void SetVelocity(Vector3 velocity)
    {
        currentVelocity.x = velocity.x;
        currentVelocity.z = velocity.z;
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

        if (animator != null)
        {
            animator.SetFloat("xVelocity", IsMoving ? 0.5f : 0f);
        }
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

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Vector3 center = new Vector3((minBounds.x + maxBounds.x) / 2f, (minBounds.y + maxBounds.y) / 2f, transform.position.z);
        Vector3 size = new Vector3(Mathf.Abs(maxBounds.x - minBounds.x), Mathf.Abs(maxBounds.y - minBounds.y), 0.1f);
        Gizmos.DrawWireCube(center, size);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }

    public override void TakeDamage(float amount)
    {
        if (IsDead) return;

        base.TakeDamage(amount);
        Debug.Log($"Boss recibió {amount} de daño.");
    }

    public override void Die()
    {
        if (IsDead) return;

        base.Die();

        StopAllCoroutines();
        rb.linearVelocity = Vector3.zero;
        rb.useGravity = true;

        animator.SetTrigger("Fall");
        Debug.Log("Boss ha muerto.");

        //BossEvents.TriggerBossDeath(transform);
    }

    private void ActivarCinematicaFinal()
    {
        Debug.Log("Cinemática final activada.");
    }
    public void ApplyFallbackForce()
    {
        Debug.Log("BossController: ApplyFallbackForce fue llamado desde la animación.");
        // Podés agregar lógica de retroceso, rebote o efectos acá
    }

}
