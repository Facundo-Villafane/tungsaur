using UnityEngine;

public class HitState : EnemyState
{
    private float duration = 0.5f; // duración del golpe
    private float timer = 0f;

    public HitState(EnemyController enemy, float duration = 0.5f) : base(enemy)
    {
        this.duration = duration;
    }

    public override void Enter()
    {
        timer = 0f;

        // Detener movimiento
        enemy.SetVelocity(Vector3.zero);

        // Animación de daño
        if (enemy.Animator != null)
        {
            enemy.Animator.SetTrigger("HitTook");
            
        }

        
        
    }

    public override void Update()
    {
        timer += Time.deltaTime;
        if (timer >= duration)
        {
            // Volver al estado anterior o patrulla
            enemy.ChangeState(new CirclePatrolState(enemy));
        }
    }

    public override void Exit()
    {
        // nada que limpiar por ahora
    }
}
