using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerFallState : PlayerState
{
    private bool isFallen = false;

    public PlayerFallState(PlayerController player) : base(player) { }

    public override void Enter()
    {
        
        player.Animator?.SetTrigger("Fall");
        isFallen = true;
    }

    public override void Update()
    {
        if (Keyboard.current == null) return;
        var kb = Keyboard.current;

        // Si presiona la tecla de levantarse
        if (kb.oKey.wasPressedThisFrame && isFallen)
        {
            player.Animator?.SetTrigger("StandUp");
            isFallen = false;

            // Volvemos al estado Idle
            player.ChangeState(new PlayerIdleState(player));
        }
    }

    public override void FixedUpdate()
    {
        // Opcional: aplicar gravedad extra mientras está en el suelo
        player.ApplyExtraGravity();
    }
}
