using UnityEngine;

/// <summary>
/// Stats específicos del Player: solo "gloria".
/// Hereda todos los demás stats de CharacterBase.
/// </summary>
public class EnemyStats : CharacterBase
{
    


    protected override void Die()
    {
        base.Die();
        Debug.Log("Enemy murió.");
    }
}
