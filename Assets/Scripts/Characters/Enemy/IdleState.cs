using UnityEngine;

public class IdleState : EnemyState
{
    public IdleState(EnemyController enemy) : base(enemy) { }

    public override void Enter()
    {
        if (enemy.Animator != null)
        {
            enemy.Animator.SetFloat("xVelocity", 0f);
        }
    }

    public override void Update()
    {

    }
}
