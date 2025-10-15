using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(Rigidbody))]
public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] public float moveSpeed = 10f;
    [SerializeField] private float acceleration = 10f;
    [SerializeField] private float deceleration = 10f;
    [SerializeField] private float maxSpeed = 8f;
    [SerializeField] private bool collitionWithWall = false;
    [Header("Physics")]
    [SerializeField] private bool usePhysics = true;

    [Header("Animation")]
    [SerializeField] private Animator animator;

    [Header("Rotation")]
    [SerializeField] private bool flipSprite = true;
    [SerializeField] private float rotationSpeed = 10f;

    [Header("Jump")]
    [SerializeField] private float jumpForce = 5f;
    // [SerializeField] private bool isGrounded = true;
    [SerializeField] private float groundCheckDistance = 0.2f; // Distancia para el raycast
    [SerializeField] private LayerMask groundLayer;     
    [SerializeField] private float fallMultiplier = 2.5f;

    [Header("Fall Settings")]
    [SerializeField] private float fallbackForce = 1f;

    // Variables públicas para que Player_Attacks pueda accederlas
    public bool isGrounded { get; private set; } = true;
    public bool isMoving { get; private set; }
    public Animator Animator => animator;

    private bool isFallen = false;
    private bool isFalling = false;
    private bool isBouncing = false;
    private bool isRunning = false;
    private bool isDead = false;

    public bool IsDead => isDead;

    private Rigidbody rb;
    private Vector3 currentVelocity;
    private Vector3 inputVector;
    private Player_Attacks attackController;
    private PlayerState currentState;
    void Start()
    {
        rb = GetComponent<Rigidbody>();

        if (animator == null)
        {
            animator = GetComponent<Animator>();
        }
        attackController = GetComponent<Player_Attacks>(); 
        if (attackController == null)
        {
            Debug.LogWarning("Player_Attacks no encontrado en " + gameObject.name);
        }
        else
        {
            Debug.Log("Player_Attacks encontrado correctamente");
        }
    }

    void Update()
    {
        // Capturar input usando el nuevo Input System
        inputVector = Vector3.zero;

        Keyboard keyboard = Keyboard.current;
        if (keyboard != null && !isFallen)
        {
            // Horizontal (A/D o flechas)
            if (keyboard.aKey.isPressed )
                inputVector.x = -1f;
            else if (keyboard.dKey.isPressed )
                inputVector.x = 1f;

            // Vertical (W/S o flechas)
            if (keyboard.sKey.isPressed)
                inputVector.z = -1f;
            else if (keyboard.wKey.isPressed)
                inputVector.z = 1f;
        if (attackController != null)
        {
            if (keyboard.jKey.wasPressedThisFrame)
            {
                attackController.HandleKickInput();
            }
        }
        if (keyboard.kKey.wasReleasedThisFrame)
        {
            attackController.HandlePunchInput();
        }

        if (keyboard.spaceKey.wasPressedThisFrame)
        {
            PerformJump();
        }
        if (keyboard.leftShiftKey.isPressed || keyboard.rightShiftKey.isPressed)
        {
            
            isRunning = true;
          
            float xVelocity = 1f;
            animator.SetFloat("xVelocity", xVelocity);
        } else {
            isRunning = false;
        
        }

        }

        if (inputVector.magnitude > 1f)
            inputVector.Normalize();

        // Bloquear ataques si se está moviendo

        if (keyboard != null && keyboard.oKey.wasReleasedThisFrame)
        {
            PerformFellStand();
            
        }

        if (collitionWithWall && isFalling)
        {
           animator.SetTrigger("Fall");
           ApplyFallbackForceTwo(); 
           isFalling = false;
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
        // else
        // {
        //     MoveWithTransform();
        // }

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
    public void ChangeState(PlayerState newState)
    {
        currentState?.Exit();
        currentState = newState;
        currentState?.Enter();
    }
    private void MoveWithPhysics()
    {
        // Convertir input 2D a movimiento 3D (X, Y, Z)
        if (isRunning)
        {
            moveSpeed = 20f;
            maxSpeed = 20f;  // ⬅ Importante
        }
        else
        {
            moveSpeed = 10f;
            maxSpeed = 8f;   // ⬅ Valor normal
        }
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

    // private void MoveWithTransform()
    // {
    //     Vector3 inputDirection = new Vector3(inputVector.x, 0f, inputVector.z);
    //     Vector3 movement = inputDirection * moveSpeed * Time.fixedDeltaTime;
    //     transform.position += movement;

    //     isMoving = inputDirection.magnitude > 0.1f;
    // }

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
            float xVelocity = inputVector.magnitude > 0.01f ? 0.5f : 0f;
            animator.SetFloat("xVelocity", xVelocity);
        }
    }


    private void PerformFellStand() {
 
        if (animator != null && !isFallen)
        {
            isFalling = true;
            animator.SetTrigger("Fall");
            isFallen = true;
        } else if (animator != null && isFallen) {
            animator.SetTrigger("StandUp");
            isFallen = false;
            isFalling = false;
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



    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
        }
        if (collision.gameObject.CompareTag("Wall")) {
            collitionWithWall = true;
        } else {
            collitionWithWall = false;
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

    
private IEnumerator WallBounceCoroutine(float duration)
{
    float elapsed = 0f;
    float horizontalPower = 1f;
    float verticalPower = 1f;
    float direction = transform.localScale.x > 0 ? -1f : 1f;

    while (elapsed < duration)
    {
        if (isBouncing) yield return new WaitForFixedUpdate();;
        Vector3 force = new Vector3(direction * horizontalPower * Time.fixedDeltaTime, verticalPower * Time.fixedDeltaTime, 0f);
        rb.AddForce(force, ForceMode.VelocityChange);
        elapsed += Time.fixedDeltaTime;
        yield return new WaitForFixedUpdate();
    }
}

    public void ApplyFallbackForce()
        {
            if (rb != null)
            {
                StartCoroutine(SmoothFallback());
            }
        }

        private IEnumerator SmoothFallback()
        {
        float direction = transform.localScale.x > 0 ? -5f : 5f;
        float duration = 0.2f;      
        float elapsed = 0f;
        float totalForce = fallbackForce; 

        while (elapsed < duration)
        {
            // Aplicar fuerza suave cada frame
            Vector3 force = new Vector3(direction * (totalForce / duration) * Time.fixedDeltaTime, 0, 0);
            rb.AddForce(force, ForceMode.VelocityChange);
            elapsed += Time.fixedDeltaTime;
            yield return new WaitForFixedUpdate();
        }
    }

        public void ApplyFallbackForceTwo()
        {

            if (rb != null)
            {
                StartCoroutine(SmoothFallbackTwo());
            }
        }

private IEnumerator SmoothFallbackTwo()
{
    isBouncing = true;

    
    float direction = transform.localScale.x > 0 ? -1f : 1f;
    float verticalDirection = 1f;
    // Duración y fuerzas
    float duration = 0.2f;
    float elapsed = 0f;
    float totalForce = fallbackForce;
    float verticalForce = 5f;

    // Girar el sprite para mirar hacia la nueva dirección
    Vector3 localScale = transform.localScale;
    localScale.x = direction < 0 ? -Mathf.Abs(localScale.x) : Mathf.Abs(localScale.x);
    transform.localScale = localScale;

    while (elapsed < duration)
    {
        // Aplicar fuerza suave cada frame
        Vector3 force = new Vector3(direction * (totalForce / duration) * Time.fixedDeltaTime,
                                    verticalDirection * (verticalForce / duration) * Time.fixedDeltaTime,
                                    0f);
        rb.AddForce(force, ForceMode.VelocityChange);
        elapsed += Time.fixedDeltaTime;
        yield return new WaitForFixedUpdate();
    }

    isBouncing = false;
}

}
