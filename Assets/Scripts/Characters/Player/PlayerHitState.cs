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

        // Animación de daño
        if (player.Animator != null)
            player.Animator.SetTrigger("HitTook");

        // Sonido de daño
        AudioManager.Instance.SonidoDañoPlayer();
    }

    public override void Update()
    {
        timer += Time.deltaTime;


        // Si se mueve, ir a movimiento
        if (player.InputVector.magnitude > 0.1f)
        {
            player.ChangeState(new PlayerWalkState(player));
            return;
        }

        // Si terminó el tiempo de reacción, volver a Idle
        if (timer >= duration)
        {
            player.ChangeState(new PlayerIdleState(player));
        }
    }
}
