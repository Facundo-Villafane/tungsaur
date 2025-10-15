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

        if (enemy.Animator != null)
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

        // Calcular distancia en 3D
        float distance = Vector3.Distance(enemy.transform.position, player.position);

        // Si el jugador se aleja demasiado, volver a Idle
        if (distance > enemy.DetectionRadius)
        {
            enemy.ChangeState(new IdleState(enemy));
            return;
        }

        // Si está en rango de ataque, cambiar a AttackState
        if (distance <= enemy.AttackRange)
        {
            enemy.ChangeState(new AttackState(enemy, player));
            return;
        }

        // Movimiento hacia el jugador en el plano horizontal (X, Z)
        Vector3 targetPosition = player.position;
        Vector3 currentPosition = enemy.transform.position;
        
        // Calcular dirección solo en el plano horizontal (ignorando Y)
        Vector3 direction = new Vector3(
            targetPosition.x - currentPosition.x,
            0,
            targetPosition.z - currentPosition.z
        ).normalized;

        // Aplicar velocidad en ambos ejes (X y Z)
        Vector3 targetVelocity = direction * enemy.MoveSpeed;
        enemy.SetVelocity(targetVelocity);
    }

    public override void Exit()
    {
        if (enemy.Animator != null)
        {
            enemy.Animator.SetFloat("xVelocity", 0f);
        }
        
        // Detener movimiento al salir
        enemy.SetVelocity(Vector3.zero);
    }
}