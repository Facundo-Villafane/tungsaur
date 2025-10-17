using UnityEngine;

public class PlayerDeadState : PlayerState
{
    public PlayerDeadState(PlayerController player) : base(player) { }

    public override void Enter()
    {
        // Cancela cualquier animación previa
        
        player.Animator.ResetTrigger("Jump 0");
        player.Animator.ResetTrigger("Kick 0");
        player.Animator.ResetTrigger("Up Punch");
        player.Animator.ResetTrigger("Fall");
        player.Animator.SetTrigger("Fall"); // Activar animación de muerte
        // player.isFalled = true;
        Debug.Log("Jugador en estado muerto");
    }

    public override void Update()
    {
        
    }

    public override void FixedUpdate()
    {
       
    }
}
