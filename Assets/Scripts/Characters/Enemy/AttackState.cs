using UnityEngine;

public class AttackState : EnemyState
{
    private Transform target;
    private float attackCooldown = 2f;
    private float timer;

    private float approachDistance = 4f;
    private float approachChance = 0.3f;
    private float moveSpeed;
    private AudioManager audioManager => enemy.AudioManager;



    public AttackState(EnemyController enemy, Transform target) : base(enemy)
    {
        this.target = target;
        moveSpeed = enemy.MoveSpeed;
    }

    public override void Enter()
    {
        timer = 0f;
    }

    public override void Update()
    {
        if (target == null || SlotManager.Instance?.Player == null)
        {
            enemy.ChangeState(new IdleState(enemy));
            return;
        }

        PlayerController player = target.GetComponent<PlayerController>();
        if (player == null || player.IsDead)
        {
            enemy.ChangeState(new CirclePatrolState(enemy));
            return;
        }

        float distance = Vector3.Distance(enemy.transform.position, target.position);

        if (distance > enemy.AttackRange)
        {
            if (distance <= approachDistance && Random.value < approachChance * Time.deltaTime * 5f)
            {
                Vector3 direction = (target.position - enemy.transform.position).normalized;
                direction.y = 0;
                enemy.SetVelocity(direction * moveSpeed);
                enemy.HandleRotation();
            }
            else
            {
                enemy.SetVelocity(Vector3.zero);
                enemy.ChangeState(new CirclePatrolState(enemy));
            }
        }
        else
        {
            enemy.SetVelocity(Vector3.zero);
            timer -= Time.deltaTime;

            if (timer <= 0f)
            {
                if (enemy.Animator != null)
                    enemy.Animator.SetTrigger("Up Punch");


                // ⚔️ Aplicar daño, pero el Player decide qué hacer con él
                player.TakeDamage(40f);



                timer = attackCooldown;
                if (audioManager != null)

                    audioManager.SonidoAtaqueEspada1(enemy.AudioSource);
                else
                    Debug.LogWarning("AttackState: AudioManager no asignado.");
             }

            enemy.HandleRotation();
        }
    }
}
