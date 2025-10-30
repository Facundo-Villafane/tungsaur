using UnityEngine;

[RequireComponent(typeof(PlayerController))]
public class PlayerCombat : MonoBehaviour
{
    [Header("Ataques")]
    [SerializeField] private float attackRange = 1.5f;
    [SerializeField] private LayerMask enemyLayers;

    private PlayerController playerController;

    private void Awake()
    {
        playerController = GetComponent<PlayerController>();
        if (playerController == null)
        {
            Debug.LogError("PlayerController no encontrado en " + gameObject.name);
        }
    }

    public void TryDealDamage()
    {
        // Evita atacar si todavía está en cooldown
        if (!CanDealDamage())
            return;

        // Detecta enemigos cercanos en 3D
        Collider[] hitEnemies = Physics.OverlapSphere(transform.position, attackRange, enemyLayers);

        foreach (Collider enemy in hitEnemies)
        {
            EnemyController enemyController = enemy.GetComponent<EnemyController>();
            BossController bossController = enemy.GetComponent<BossController>();

            if (enemyController != null && !enemyController.IsDead)
            {
                Debug.Log("Golpeo al enemigo: " + enemy.name);
                enemyController.TakeDamage(playerController.BaseDamage);
            }
            else if (bossController != null && !bossController.IsDead)
            {
                Debug.Log("Golpeo al boss: " + enemy.name);
                bossController.TakeDamage(playerController.BaseDamage);
                bossController.TakeHit();
            }
        }
    }

    private bool CanDealDamage()
    {
        // Evita aplicar daño si el cooldown del ataque no terminó
        return Time.time >= playerController.LastAttackTime + playerController.AttackCooldown;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}
