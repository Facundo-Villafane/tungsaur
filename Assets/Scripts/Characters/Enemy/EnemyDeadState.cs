using UnityEngine;

public class EnemyDeadState : EnemyState
{
    public EnemyDeadState(EnemyController enemy) : base(enemy) { }

    public override void Enter()
    {

        enemy.Animator.ResetTrigger("Jump 0");
        enemy.Animator.ResetTrigger("Kick 0");
        enemy.Animator.ResetTrigger("Up Punch");
        enemy.Animator.ResetTrigger("Fall");
        enemy.Animator.SetTrigger("Fall");

        Debug.Log("Enemy en estado muerto");
        enemy.Die();
    }
}
