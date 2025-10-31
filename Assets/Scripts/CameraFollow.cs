using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target;
    public float offset = 0f;
    public float smoothSpeed = 5f;

    private GameManager gameManager;
    private float maxX; // 👈 Guarda la posición máxima en X alcanzada por el target

    void Start()
    {
        gameManager = FindFirstObjectByType<GameManager>();
        if (gameManager == null)
        {
            Debug.LogError("[CameraFollow] No se encontró un GameManager en la escena.");
        }

        if (target != null)
            maxX = target.position.x; // Inicializa el límite con la posición inicial del jugador
    }

    void Update()
    {
        if (!target || gameManager == null) return;

        if (gameManager.CanCameraMove())
        {
            // Solo actualizar si el jugador avanza hacia la derecha
            if (target.position.x > maxX)
            {
                maxX = target.position.x;
            }

            // La cámara solo sigue hasta maxX
            Vector3 desiredPosition = new Vector3(maxX + offset, 3.68f, -9.76f);
            transform.position = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed * Time.deltaTime);
        }
    }
}
