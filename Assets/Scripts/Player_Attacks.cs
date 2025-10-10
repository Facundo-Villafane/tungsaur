using UnityEngine;

[RequireComponent(typeof(PlayerController))]
public class Player_Attacks : MonoBehaviour
{
    private PlayerController playerController;
    private Animator animator;

    void Start()
    {
        playerController = GetComponent<PlayerController>();
        if (playerController == null)
        {
            return;
        }

        animator = playerController.Animator;

        if (animator == null)
        {
            animator = GetComponent<Animator>();
        }
    }

    public void HandleKickInput()
    {
        if (playerController == null || animator == null)
        {
            return;
        }

        if (playerController.isGrounded && !playerController.isMoving)
        {
            PerformKick();
        }
        else if (!playerController.isGrounded)
        {
            PerformJumpKick();
        }
    }

    public void HandlePunchInput()
    {
        if (playerController.isGrounded && !playerController.isMoving)
        {
            PerformUpPunch();
        }
        else if (!playerController.isGrounded)
        {
            PerformJumpPunch();
        }
    }

    //------------------------------------- Ataques básicos --------------------------------------------//
    private void PerformKick()
    {
        if (animator != null)
        {
            animator.SetTrigger("Kick 0");
        }
    }

    private void PerformJumpKick()
    {
        if (animator != null)
        {
            animator.SetTrigger("Jump Kick");
        }
    }

    private void PerformUpPunch()
    {
        if (animator != null)
        {
            animator.SetTrigger("Up Punch");
        }
    }

    private void PerformJumpPunch()
    {
        if (animator != null)
        {
            animator.SetTrigger("Jump Punch");
        }
    }
}