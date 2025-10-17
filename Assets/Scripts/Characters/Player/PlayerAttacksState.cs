using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerAttacksState : PlayerState
{
    public PlayerAttacksState(PlayerController player) : base(player) { }

    public override void Enter()
    {
        // Reiniciar triggers si quieres
    }

    public override void Update()
    {
        if (Keyboard.current == null) return;
        var kb = Keyboard.current;

        // Kick
        if (kb.jKey.wasPressedThisFrame)
        {
         
            if (player.isGrounded && !player.IsMoving)
                player.Animator.SetTrigger("Kick 0");
            else
                player.Animator.SetTrigger("Jump Kick");

            player.Combat?.TryDealDamage();
        }

        // Punch
        if (kb.kKey.wasPressedThisFrame)
        {
            if (player.isGrounded && !player.IsMoving)
                player.Animator.SetTrigger("Up Punch");
            else
                player.Animator.SetTrigger("Jump Punch");

            player.Combat?.TryDealDamage();
        }
    }
}
