using UnityEngine;

public class AttackState : EnemyState
{
    private Transform target;
    private float attackCooldown = 1.5f;
    private float timer;

    public AttackState(EnemyController enemy, Transform target) : base(enemy)
    {
        this.target = target;
    }

    public override void Enter()
    {
        if (enemy.Animator != null)
            enemy.Animator.SetTrigger("Up Punch");

        timer = attackCooldown;
    }

public override void Update()
{
    if (target == null)
    {
        enemy.ChangeState(new IdleState(enemy));
        return;
    }

    float distance = Vector3.Distance(enemy.transform.position, target.position);

    // Si el jugador se sale del rango, volver a perseguir
    if (distance > enemy.AttackRange)
    {
        enemy.ChangeState(new ChaseState(enemy));
        return;
    }

    // Ataque por cooldown
    timer -= Time.deltaTime;
    if (timer <= 0f)
    {
        if (enemy.Animator != null)
            enemy.Animator.SetTrigger("Up Punch"); // Repetir ataque
        timer = attackCooldown;
    }
}

}
