using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerRunState : PlayerState
{
    public PlayerRunState(PlayerController player) : base(player) {}

    public override void Enter()
    {
        base.Enter();
        player.Animator?.SetFloat("xVelocity", 1f);
    }

    public override void Update()
    {
        base.Update();

        if (Keyboard.current == null) return;
        var kb = Keyboard.current;

        HandleMovement(kb);

        // Si no se está moviendo → Idle
        if (player.InputVector.magnitude <= 0.1f)
        {
            player.ChangeState(new PlayerIdleState(player));
            return;
        }

        // Si dejó de correr → Walk
        if (!player.IsRunning)
        {
            player.ChangeState(new PlayerWalkState(player));
            return;
        }
    }

    public override void FixedUpdate()
    {
        base.FixedUpdate();
        player.MoveWithPhysics();
        player.HandleRotation();
    }

    public override void Exit()
    {
        base.Exit();
        player.Animator?.SetFloat("xVelocity", 0f);
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
}
