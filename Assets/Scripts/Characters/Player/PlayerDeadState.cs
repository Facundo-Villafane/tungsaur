using UnityEngine;

public class PlayerDeadState : PlayerState
{
    public PlayerDeadState(PlayerController player) : base(player) { }

public override void Enter()
 {
    player.IsDead = true;

    player.Animator.ResetTrigger("Jump 0");
    player.Animator.ResetTrigger("Kick 0");
    player.Animator.ResetTrigger("Up Punch");
    player.Animator.ResetTrigger("Fall");

    player.Animator.SetTrigger("Fall"); // animación de muerte
       Debug.Log("Jugador en estado muerto");
    
 } 
}
