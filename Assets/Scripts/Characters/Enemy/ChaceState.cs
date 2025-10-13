using UnityEngine;

public class ChaseState : EnemyState
{
    private Transform player;

    public ChaseState(EnemyController enemy) : base(enemy) { }

    public override void Enter()
    {
        GameObject playerObj = GameObject.FindWithTag("Player");
        if (playerObj != null)
        {
            player = playerObj.transform;
        }

        if (enemy.Animator != null )
        {
            enemy.Animator.SetFloat("xVelocity", 0.5f);
        }
    }

    public override void Update()
    {
        if (player == null)
        {
            enemy.ChangeState(new IdleState(enemy));
            return;
        }

        float distance = Vector3.Distance(enemy.transform.position, player.position);

        // Si el jugador se aleja demasiado, volver a Idle
        if (distance > enemy.DetectionRadius)
        {
            enemy.ChangeState(new IdleState(enemy));
            return;
        }

        // Si está en rango de ataque, después podemos cambiar a un AttackState
        if (distance <= enemy.AttackRange)
        {
            // TODO: Cambiar a estado de ataque
            return;
        }

        // Movimiento hacia el jugador
        Vector3 direction = (player.position - enemy.transform.position).normalized;
        Vector3 move = direction * enemy.MoveSpeed * Time.deltaTime;

        enemy.transform.position += new Vector3(move.x, 0f, move.z);
    }

    public override void Exit()
    {
        if (enemy.Animator != null)
        {
            enemy.Animator.SetFloat("xVelocity", 0f);
        }
    }
}
