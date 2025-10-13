using UnityEngine;

/// <summary>
/// Stats específicos del Player: solo "gloria".
/// Hereda todos los demás stats de CharacterBase.
/// </summary>
public class EnemyStats : CharacterBase
{
    private EnemyController enemyController ;
    private Animator animator;

    void Start()
    {
        enemyController = GetComponent<EnemyController>();
        animator = enemyController?.Animator ?? GetComponent<Animator>();

    }
    protected override void Die()
    {
        base.Die();
        Debug.Log("Enemy murió.");
        enemyController.Die();
    }
    
    public override void TakeHit()
    {
        Debug.Log($"{gameObject.name} fue golpeado");
        if (CurrentHealth > 0 )
        {
            enemyController.TakeHit();
        }
        
    }
}
