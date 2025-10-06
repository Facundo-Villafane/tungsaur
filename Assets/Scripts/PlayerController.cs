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
    
    private Rigidbody rb;
    private Vector3 currentVelocity;
    private Vector2 inputVector;
    
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        
        if (rb != null)
        {
            rb.freezeRotation = true;
            rb.useGravity = false;
        }
    }
    
    void Update()
    {
        // Capturar input usando el nuevo Input System
        inputVector = Vector2.zero;
        
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
                inputVector.y = -1f;
            else if (keyboard.wKey.isPressed || keyboard.upArrowKey.isPressed)
                inputVector.y = 1f;
        }
        
        // Normalizar para movimiento diagonal consistente
        if (inputVector.magnitude > 1f)
            inputVector.Normalize();
        
        // Invertir horizontal como en el código original
        inputVector.x = -inputVector.x;
    }
    
    void FixedUpdate()
    {
        if (usePhysics && rb != null)
        {
            MoveWithPhysics();
        }
        else
        {
            MoveWithTransform();
        }
    }
    
    private void MoveWithPhysics()
    {
        // Convertir input 2D a movimiento 3D (X, Y, 0)
        Vector3 inputDirection = new Vector3(inputVector.x, inputVector.y, 0f);
        Vector3 targetVelocity = inputDirection * moveSpeed;
        
        currentVelocity = Vector3.Lerp(
            currentVelocity, 
            targetVelocity, 
            (inputVector.magnitude > 0 ? acceleration : deceleration) * Time.fixedDeltaTime
        );
        
        currentVelocity = Vector3.ClampMagnitude(currentVelocity, maxSpeed);
        
        // Aplica velocidad en X e Y, mantiene Z
        rb.linearVelocity = new Vector3(currentVelocity.x, currentVelocity.y, rb.linearVelocity.z);
    }
    
    private void MoveWithTransform()
    {
        Vector3 inputDirection = new Vector3(inputVector.x, inputVector.y, 0f);
        Vector3 movement = inputDirection * moveSpeed * Time.fixedDeltaTime;
        transform.position += movement;
    }
    
    public Vector3 GetVelocity()
    {
        return currentVelocity;
    }
    
    public bool IsMoving()
    {
        return currentVelocity.magnitude > 0.1f;
    }
}