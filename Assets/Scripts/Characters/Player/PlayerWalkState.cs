using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerWalkState : PlayerState
{
    public PlayerWalkState(PlayerController player) : base(player) { }

    public override void Enter()
    {
        base.Enter();
        player.Animator?.SetFloat("xVelocity", 0.5f);
    }

    public override void Update()
    {
        base.Update();

        // Recalcular input cada frame
        if (Keyboard.current != null)
            HandleMovement(Keyboard.current);

        // Si no hay input → Idle
        if (player.InputVector.magnitude <= 0.1f)
        {
            player.ChangeState(new PlayerIdleState(player));
            return;
        }

        // Si presionó Shift → Run
        if (player.IsRunning)
        {
            player.ChangeState(new PlayerRunState(player));
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

        if (kb.aKey.isPressed) input.x = -1f;
        else if (kb.dKey.isPressed) input.x = 1f;

        if (kb.sKey.isPressed) input.z = -1f;
        else if (kb.wKey.isPressed) input.z = 1f;

        // Normalizamos para que diagonal no sea >1
        if (input.sqrMagnitude > 1f)
            input.Normalize();

        player.InputVector = input;

        player.IsRunning = kb.leftShiftKey.isPressed || kb.rightShiftKey.isPressed;
    }
}
