using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerIdleState : PlayerState
{
    public PlayerIdleState(PlayerController player) : base(player) { }
    public override void Enter()
    {
        
    }

    public override void Update()
    {

      
        if (Keyboard.current == null) return;
        var kb = Keyboard.current;

        // ----------------- MOVIMIENTO -----------------
        HandleMovement(kb);
        if (player.InputVector.magnitude > 0.1f)
        {
           
        if (player.IsRunning)
            player.ChangeState(new PlayerRunState(player));
        else
            player.ChangeState(new PlayerWalkState(player));

        return;
    }

        player.HandleRotation();

        // ----------------- SALTO -----------------
        if (kb.spaceKey.wasPressedThisFrame && player.isGrounded)
        {
            player.PerformJump();
        }

        // ----------------- ATAQUES -----------------
        HandleAttacks(kb);

        // ----------------- FALL / STAND -----------------
        if (kb.oKey.wasPressedThisFrame)
        {
            // Cambiamos de estado a PlayerFallState
            player.ChangeState(new PlayerFallState(player));
        }

    }

    public override void FixedUpdate()
    {
        player.MoveWithPhysics();
        player.ApplyExtraGravity();
    }

    private void HandleMovement(Keyboard kb)
    {
        Vector3 input = Vector3.zero;

        // Movimiento horizontal
        if (kb.aKey.isPressed) input.x = -1f;
        else if (kb.dKey.isPressed) input.x = 1f;

        // Movimiento vertical
        if (kb.sKey.isPressed) input.z = -1f;
        else if (kb.wKey.isPressed) input.z = 1f;

        player.InputVector = input;

        // Correr
        player.IsRunning = kb.leftShiftKey.isPressed || kb.rightShiftKey.isPressed;
    }

    private void HandleAttacks(Keyboard kb)
    {
        if (player.Combat == null || player.Animator == null) return;

        // Kick
        if (kb.jKey.wasPressedThisFrame)
        {
            if (player.isGrounded && !player.IsMoving)
                player.Animator.SetTrigger("Kick 0");
            else if (!player.isGrounded)
                player.Animator.SetTrigger("Jump Kick");

            player.Combat.TryDealDamage();
        }

        // Punch
        if (kb.kKey.wasPressedThisFrame)
        {
          

            player.Combat.TryDealDamage();
        }
    }
    
}
