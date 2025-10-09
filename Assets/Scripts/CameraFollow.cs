using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target;
    public float offset;
    public float smoothSpeed = 5f;
    private GameManager gameManager;

    void Start()
    {
        // Buscar automáticamente el GameManager en la escena (opcional: podés asignarlo manualmente también)
        gameManager = FindFirstObjectByType<GameManager>();
        if (gameManager == null)
        {
            Debug.LogError("[CameraFollow] No se encontró un GameManager en la escena.");
        }
    }

    void Update()
    {
        if (!target) return;
        if (gameManager == null) return;

        // Solo mover la cámara si el estado es "Free"
        if (gameManager.CanCameraMove())
        {
            Vector3 desiredPosition = new Vector3(offset + target.position.x, 3.68f, -9.76f);
            transform.position = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed * Time.deltaTime);
        }
        // Si no es Free, la cámara queda fija donde estaba
    }
}
