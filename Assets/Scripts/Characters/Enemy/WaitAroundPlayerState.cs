using UnityEngine;

public class WaitAroundPlayerState : EnemyState
{
    private Transform player;
    private float waitTime;
    private float timer;

    // Probabilidad de atacar mientras espera
    private float randomAttackChance = 0.25f; // 25%
    
    public WaitAroundPlayerState(EnemyController enemy, Transform player) : base(enemy)
    {
        this.player = player;
    }

    public override void Enter()
    {
        // Tiempo de espera variable para que no todos actúen sincronizados
        waitTime = Random.Range(1.5f, 3.5f);
        timer = 0f;

        // Detener al enemigo
        enemy.SetVelocity(Vector3.zero);
    }

    public override void Update()
    {
        if (enemy.IsDead) return;

        if (player == null)
        {
            
            enemy.ChangeState(new CirclePatrolState(enemy));
            return;
        }

        // Girar constantemente hacia el jugador mientras espera
        enemy.HandleRotation();

        float distance = Vector3.Distance(enemy.transform.position, player.position);

        // Si el jugador se aleja del radio de detección, vuelve a patrullar
        if (distance > enemy.DetectionRadius)
        {
            enemy.ChangeState(new CirclePatrolState(enemy));
            return;
        }

        // Contador de espera
        timer += Time.deltaTime;

        // Chance aleatoria de atacar durante la espera
        if (timer >= waitTime * 0.5f) // solo después de la mitad del tiempo
        {
            
            if (Random.value < randomAttackChance)
            {
                enemy.ChangeState(new AttackState(enemy, player));
                return;
            }
        }

        // Al terminar el tiempo → volver a patrullar
        if (timer >= waitTime)
        {
            enemy.ChangeState(new CirclePatrolState(enemy));
        }
    }

    public override void Exit()
    {
        enemy.SetVelocity(Vector3.zero);
    }
}
