using UnityEngine;

public class PlayerHitState : PlayerState
{
    private float duration = 0.5f; // duración del golpe
    private float timer = 0f;

    public PlayerHitState(PlayerController player, float duration = 0.5f) : base(player)
    {
        this.duration = duration;
    }

    public override void Enter()
    {
        timer = 0f;
        player.moveSpeed = 0;
        if (player.Animator != null)
        {
            player.Animator.SetTrigger("HitTook");
        }
    }

    public override void Update()
    {


    }

    public override void Exit()
    {
        // nada que limpiar por ahora
    }
}
