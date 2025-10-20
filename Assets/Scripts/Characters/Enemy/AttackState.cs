using UnityEngine;

public class AttackState : EnemyState
{
    private Transform target;
    private float attackCooldown = 1.5f;
    private float timer;

    private float approachDistance = 4f; // distancia desde la que puede decidir acercarse
    private float approachChance = 0.3f;  // 30% de chance de acercarse
    private float moveSpeed;

    public AttackState(EnemyController enemy, Transform target) : base(enemy)
    {
        this.target = target;
        moveSpeed = enemy.MoveSpeed;
    }

    public override void Enter()
    {
        timer = 0f; // Reiniciar timer al entrar al estado
    }

    public override void Update()
    {
        // Validación segura del target y SlotManager
        if (target == null || SlotManager.Instance == null || SlotManager.Instance.Player == null)
        {
            enemy.ChangeState(new IdleState(enemy));
            return;
        }

        // Obtener el PlayerController de forma segura
        PlayerController player = target.GetComponent<PlayerController>();
        if (player == null || player.CurrentHealth <= 0f)
        {
            enemy.ChangeState(new CirclePatrolState(enemy));
            return;
        }

        float distance = Vector3.Distance(enemy.transform.position, target.position);

        // Si el jugador está fuera del rango de ataque
        if (distance > enemy.AttackRange)
        {
            // Decidir aleatoriamente si acercarse
            if (distance <= approachDistance && Random.value < approachChance * Time.deltaTime * 5f)
            {
                // Moverse hacia el jugador
                Vector3 direction = (target.position - enemy.transform.position).normalized;
                direction.y = 0; // ignorar eje Y
                enemy.SetVelocity(direction * moveSpeed);

                // Rotar hacia el jugador mientras se acerca
                enemy.HandleRotation();
            }
            else
            {
                // Si no decide acercarse, vuelve a patrullar
                enemy.SetVelocity(Vector3.zero);
                enemy.ChangeState(new CirclePatrolState(enemy));
            }
        }
        else
        {
            // Está en rango -> atacar
            enemy.SetVelocity(Vector3.zero);
            timer -= Time.deltaTime;

            if (timer <= 0f)
            {
                // Animación de ataque
                if (enemy.Animator != null)
                    enemy.Animator.SetTrigger("Up Punch");

                // Aplicar daño al jugador
                player.ChangeState(new PlayerHitState(player, 10f));

                timer = attackCooldown;
            }

            // Rotar hacia el jugador mientras ataca
            enemy.HandleRotation();
        }
    }
}
