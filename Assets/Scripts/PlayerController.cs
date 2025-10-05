using UnityEngine;

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
    private Vector3 inputDirection;
    
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
        // Capturar input (negativo para corregir la dirección)
        float horizontal = -Input.GetAxisRaw("Horizontal"); // Invertido
        float vertical = Input.GetAxisRaw("Vertical");     // Invertido
        
        // Ahora usa X e Y en lugar de X y Z
        inputDirection = new Vector3(horizontal, vertical, 0f).normalized;
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
        Vector3 targetVelocity = inputDirection * moveSpeed;
        
        currentVelocity = Vector3.Lerp(
            currentVelocity, 
            targetVelocity, 
            (inputDirection.magnitude > 0 ? acceleration : deceleration) * Time.fixedDeltaTime
        );
        
        currentVelocity = Vector3.ClampMagnitude(currentVelocity, maxSpeed);
        
        // Aplica velocidad en X e Y, mantiene Z
        rb.velocity = new Vector3(currentVelocity.x, currentVelocity.y, rb.velocity.z);
    }
    
    private void MoveWithTransform()
    {
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