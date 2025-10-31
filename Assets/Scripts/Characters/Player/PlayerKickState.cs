using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerKickState : PlayerState
{
    private float attackDuration = 0.2f; // Duración estimada del ataque
    private float attackTimer;

    public PlayerKickState(PlayerController player) : base(player) { }

    public override void Enter()
    {
        attackTimer = attackDuration;
        

        if (player.isGrounded)
            player.Animator.SetTrigger("Kick 0");
        else if (!player.isGrounded) { // ataque en el aire
            player.Animator.SetTrigger("Kick 0");
        }
        

        player.Combat?.TryDealDamage();
    }

    public override void Update()
    {
        attackTimer -= Time.deltaTime;

        // Si el jugador se mueve, pasar a movimiento
        if (player.InputVector.magnitude > 0.1f)
        {
            player.ChangeState(new PlayerWalkState(player));
            return;
        }

        // Si terminó el ataque, volver a Idle
        if (attackTimer <= 0f)
        {
            player.ChangeState(new PlayerIdleState(player));
        }
    }
}
