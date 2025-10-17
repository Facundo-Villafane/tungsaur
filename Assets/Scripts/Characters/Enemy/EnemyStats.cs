using UnityEngine;

/// <summary>
/// Stats específicos del Player: solo "gloria".
/// Hereda todos los demás stats de CharacterBase.
/// </summary>
public class EnemyStats : CharacterBase
{
    private EnemyController enemyController ;

    void Start()
    {
        enemyController = GetComponent<EnemyController>();
        animator = enemyController?.Animator ?? GetComponent<Animator>();

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
