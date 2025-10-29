using UnityEngine;

[RequireComponent(typeof(PlayerController))]
public class PlayerCombat : MonoBehaviour
{
    [Header("Ataques")]
    [SerializeField] private float attackRange = 1.5f;
    [SerializeField] private LayerMask enemyLayers;

    private PlayerController playerStats;

    private void Awake()
    {
        playerStats = GetComponent<PlayerController>();
        if (playerStats == null)
        {
            Debug.LogError("PlayerStats no encontrado en " + gameObject.name);
        }
    }
    public void TryDealDamage()
    {
        // Detecta enemigos cercanos en 3D
        Collider[] hitEnemies = Physics.OverlapSphere(transform.position, attackRange, enemyLayers);

        foreach (Collider enemy in hitEnemies)
        {
            EnemyController enemyController = enemy.GetComponent<EnemyController>();
            BossController bossController = enemy.GetComponent<BossController>();
           if (enemyController != null && !enemyController.IsDead)
             {
                Debug.Log("Golpeo al enemigo: " + enemy.name);
                enemyController.TakeDamage(playerStats.BaseDamage);
                enemyController.TakeHit();
             }
            else if (bossController != null && !bossController.IsDead)
             {
                Debug.Log("Golpeo al boss: " + enemy.name);
                bossController.TakeDamage(playerStats.BaseDamage);
                bossController.TakeHit();
             }
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}
