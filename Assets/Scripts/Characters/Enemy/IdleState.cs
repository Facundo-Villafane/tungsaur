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
        // Buscar al jugador por tag
        GameObject player = GameObject.FindWithTag("Player");
        if (player == null) return;

        float distance = Vector3.Distance(enemy.transform.position, player.transform.position);

        // Si el jugador está dentro del radio de detección, pasar a ChaseState
        if (distance <= enemy.DetectionRadius)
        {
            enemy.ChangeState(new ChaseState(enemy));
        }
    }
}
