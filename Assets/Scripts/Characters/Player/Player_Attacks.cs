using UnityEngine;

[RequireComponent(typeof(PlayerController))]
[RequireComponent(typeof(PlayerCombat))]
public class Player_Attacks : MonoBehaviour
{
    private PlayerController playerController;
    private Animator animator;
    private PlayerCombat combat;

    void Start()
    {
        playerController = GetComponent<PlayerController>();
        animator = playerController?.Animator ?? GetComponent<Animator>();
        combat = GetComponent<PlayerCombat>();
    }

    public void HandleKickInput()
    {
        if (!CanAttack()) return;

        if (playerController.isGrounded && !playerController.isMoving)
            PerformKick();
        else if (!playerController.isGrounded)
            PerformJumpKick();

        combat?.TryDealDamage();
    }

    public void HandlePunchInput()
    {
        if (!CanAttack()) return;

        if (playerController.isGrounded && !playerController.isMoving)
            PerformUpPunch();
        else if (!playerController.isGrounded)
            PerformJumpPunch();

        combat?.TryDealDamage();
    }

    private bool CanAttack()
    {
        return playerController != null && animator != null;
    }

    // --------------------- Animaciones --------------------- //
    private void PerformKick() => animator?.SetTrigger("Kick 0");
    private void PerformJumpKick() => animator?.SetTrigger("Jump Kick");
    private void PerformUpPunch() => animator?.SetTrigger("Up Punch");
    private void PerformJumpPunch() => animator?.SetTrigger("Jump Punch");
}
