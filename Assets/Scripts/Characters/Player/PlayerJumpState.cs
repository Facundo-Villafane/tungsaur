using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerJumpState : PlayerState
{
    public PlayerJumpState(PlayerController player) : base(player) { }

    public override void Enter()
    {
        base.Enter();

        // Animación de salto
        player.Animator?.SetTrigger("Jump 0");

        // 🔊 Sonido de salto (Salto2)
        AudioManager.Instance.SonidoSalto2();
    }

    public override void Update()
    {
        base.Update();

        if (Keyboard.current == null) return;
        var kb = Keyboard.current;
        HandleAirMovement(kb);

        // Si aterrizó antes de caer → Idle / Walk / Run
        if (player.isGrounded)
        {
            if (player.InputVector.magnitude <= 0.1f)
                player.ChangeState(new PlayerIdleState(player));
            else if (player.IsRunning)
                player.ChangeState(new PlayerRunState(player));
            else
                player.ChangeState(new PlayerWalkState(player));
        }
    }

    public override void FixedUpdate()
    {
        base.FixedUpdate();
        player.MoveWithPhysics();      // Permite control en el aire
        player.ApplyExtraGravity();    // Caída más rápida
        player.HandleRotation();
    }

    private void HandleAirMovement(Keyboard kb)
    {
        Vector3 input = Vector3.zero;

        if (kb.aKey.isPressed) input.x = -1f;
        else if (kb.dKey.isPressed) input.x = 1f;

        if (kb.sKey.isPressed) input.z = -1f;
        else if (kb.wKey.isPressed) input.z = 1f;

        player.InputVector = input;
        player.IsRunning = kb.leftShiftKey.isPressed || kb.rightShiftKey.isPressed;
    }
}
