using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float acceleration = 10f;
    [SerializeField] private float deceleration = 10f;
    [SerializeField] private float maxSpeed = 8f;

    [Header("Physics")]
    [SerializeField] private bool usePhysics = true;

    [Header("Animation")]
    [SerializeField] private Animator animator;

    [Header("Rotation")]
    [SerializeField] private bool flipSprite = true;
    [SerializeField] private float rotationSpeed = 10f;

    [Header("Jump")]
    [SerializeField] private float jumpForce = 5f;
    [SerializeField] private bool isGrounded = true;
    [SerializeField] private float groundCheckDistance = 0.2f; // Distancia para el raycast
    [SerializeField] private LayerMask groundLayer;     
    [SerializeField] private float fallMultiplier = 2.5f;

    private bool isMoving;
    private bool isFallen = false;
    private Rigidbody rb;
    private Vector3 currentVelocity;
    private Vector3 inputVector;

    void Start()
    {
        rb = GetComponent<Rigidbody>();

        if (animator == null)
        {
            animator = GetComponent<Animator>();
        }
    }

    void Update()
    {
        // Capturar input usando el nuevo Input System
        inputVector = Vector3.zero;

        Keyboard keyboard = Keyboard.current;
        if (keyboard != null)
        {
            // Horizontal (A/D o flechas)
            if (keyboard.aKey.isPressed || keyboard.leftArrowKey.isPressed)
                inputVector.x = -1f;
            else if (keyboard.dKey.isPressed || keyboard.rightArrowKey.isPressed)
                inputVector.x = 1f;

            // Vertical (W/S o flechas)
            if (keyboard.sKey.isPressed || keyboard.downArrowKey.isPressed)
                inputVector.z = -1f;
            else if (keyboard.wKey.isPressed || keyboard.upArrowKey.isPressed)
                inputVector.z = 1f;
        }

        if (inputVector.magnitude > 1f)
            inputVector.Normalize();

        // Bloquear ataques si se está moviendo
        if (keyboard != null && keyboard.jKey.wasReleasedThisFrame)
        {
            if (isGrounded  && !isMoving)
                PerformKick();
            else if (!isGrounded)
                PerformJumpKick();
        }

        if (keyboard != null && keyboard.kKey.wasReleasedThisFrame && isGrounded && !isMoving)
        {
            PerformUpPunch();
        }

        if (keyboard != null && keyboard.spaceKey.wasPressedThisFrame)
        {
            PerformJump();
        }
        if (keyboard != null && keyboard.pKey.wasReleasedThisFrame)
        {
            PerformFellStand();
        }
    }

    void FixedUpdate()
    {
        CheckGrounded();

        if (usePhysics && rb != null)
        {
            MoveWithPhysics();
            ApplyExtraGravity();
        }
        else
        {
            MoveWithTransform();
        }

        HandleRotation();
        UpdateAnimator();
    }

    private void CheckGrounded()
    {
        Ray ray = new Ray(transform.position, Vector3.down);
        if (Physics.Raycast(ray, groundCheckDistance, groundLayer))
        {
            isGrounded = true;
        }
        else
        {
            isGrounded = false;
        }
    }

    private void MoveWithPhysics()
    {
        // Convertir input 2D a movimiento 3D (X, Y, Z)
        Vector3 inputDirection = new Vector3(inputVector.x, 0f, inputVector.z);
        Vector3 targetVelocity = inputDirection * moveSpeed;

        currentVelocity = Vector3.Lerp(
            currentVelocity,
            targetVelocity,
            (inputVector.magnitude > 0 ? acceleration : deceleration) * Time.fixedDeltaTime
        );

        currentVelocity = Vector3.ClampMagnitude(currentVelocity, maxSpeed);

        rb.linearVelocity = new Vector3(currentVelocity.x, rb.linearVelocity.y, currentVelocity.z);

      
        isMoving = inputDirection.magnitude > 0.1f;
    }

    private void MoveWithTransform()
    {
        Vector3 inputDirection = new Vector3(inputVector.x, 0f, inputVector.z);
        Vector3 movement = inputDirection * moveSpeed * Time.fixedDeltaTime;
        transform.position += movement;

        isMoving = inputDirection.magnitude > 0.1f;
    }

    private void ApplyExtraGravity()
    {
        if (rb.linearVelocity.y < 0 && !isGrounded)
        {
            rb.AddForce(Physics.gravity * (fallMultiplier - 1f), ForceMode.Acceleration);
        }
    }

    public Vector3 GetVelocity()
    {
        return currentVelocity;
    }

    public bool IsMoving()
    {
        return currentVelocity.magnitude > 0.1f;
    }

    private void UpdateAnimator()
    {
        if (animator != null)
        {
            float xVelocity = inputVector.magnitude > 0.01f ? 1f : 0f;
            animator.SetFloat("xVelocity", xVelocity);
        }
    }

    private void PerformKick()
    {
        if (animator != null)
        {
            animator.SetTrigger("Kick 0");
        }
    }
    private void PerformFellStand() {
        if (animator != null && !isFallen)
        {
            animator.SetTrigger("Fall");
        } else if (animator != null && isFallen) {
            animator.SetTrigger("StandUp");
        }
        isFallen = !isFallen;
    }
    private void PerformJumpKick()
    {
        if (animator != null)
        {
            animator.SetTrigger("Jump Kick");
        }
    }

    private void PerformJump()
    {
        if (isGrounded && rb != null)
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            animator?.SetTrigger("Jump 0");
            isGrounded = false;
        }
    }

    private void PerformUpPunch()
    {
        if (animator != null)
        {
            animator.SetTrigger("Up Punch");
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
        }
    }

    private void HandleRotation()
    {
        if (Mathf.Abs(inputVector.x) > 0.01f)
        {
            if (flipSprite)
            {
                Vector3 localScale = transform.localScale;
                if (inputVector.x < 0)
                    localScale.x = -Mathf.Abs(localScale.x); // Mira a la derecha
                else
                    localScale.x = Mathf.Abs(localScale.x); // Mira a la izquierda

                transform.localScale = localScale;
            }
            else
            {
                float targetAngle = inputVector.x > 0 ? 180f : 0f;
                Quaternion targetRotation = Quaternion.Euler(0, targetAngle, 0);
                transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, rotationSpeed * Time.fixedDeltaTime);
            }
        }
    }
}
