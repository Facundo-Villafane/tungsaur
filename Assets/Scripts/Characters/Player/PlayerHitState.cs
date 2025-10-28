using UnityEngine;

public class PlayerHitState : PlayerState
{
    private float duration;
    private float timer;

    public PlayerHitState(PlayerController player, float duration = 0.5f) : base(player)
    {
        this.duration = duration;
    }

    public override void Enter()
    {
        if (player.IsDead) return;

        timer = 0f;

        if (player.Animator != null)
            player.Animator.SetTrigger("HitTook");

        // ❌ Quitar la línea que aplicaba daño
        // player.TakeDamage(damage);
    }

    public override void Update()
    {
        timer += Time.deltaTime;
        if (timer >= duration)
        {
            // Si el jugador murió durante el golpe, no volver al Idle
            if (player.IsDead)
                player.ChangeState(new PlayerDeadState(player));
            else
                player.ChangeState(new PlayerIdleState(player));
        }
    }
}
