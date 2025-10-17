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
            enemy.GetComponent<CharacterBase>()?.TakeDamage(playerStats.BaseDamage);
            enemy.GetComponent<CharacterBase>()?.TakeHit();
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}
