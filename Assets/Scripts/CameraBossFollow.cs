using UnityEngine;

public class CameraBossFollow : MonoBehaviour
{
    [Header("Configuración de zoom")]
    [SerializeField] private float zoomFOV = 30f;
    [SerializeField] private float zoomDuration = 1.5f;
    [SerializeField] private Vector3 offset = new Vector3(0, 2f, -6f);

    private Camera cam;
    private Transform bossTarget;
    private float defaultFOV;
    private float zoomTimer = 0f;
    private bool zoomActivo = false;

    void Start()
    {
        cam = Camera.main;
        if (cam != null)
        {
            defaultFOV = cam.fieldOfView;
        }
        else
        {
            Debug.LogError("[CameraBossFollow] No se encontró la cámara principal.");
        }
    }

    void Update()
    {
        if (!zoomActivo || bossTarget == null || cam == null) return;

        // Movimiento hacia el boss
        Vector3 destino = bossTarget.position + offset;
        transform.position = Vector3.Lerp(transform.position, destino, Time.deltaTime * 3f);

        // Zoom progresivo
        zoomTimer += Time.deltaTime;
        cam.fieldOfView = Mathf.Lerp(defaultFOV, zoomFOV, zoomTimer / zoomDuration);

        if (zoomTimer >= zoomDuration)
        {
            zoomActivo = false;
            Debug.Log("[CameraBossFollow] Zoom completo al boss.");
        }
    }

    public void ActivarZoom(Transform boss)
    {
        bossTarget = boss;
        zoomTimer = 0f;
        zoomActivo = true;
        Debug.Log("[CameraBossFollow] Zoom iniciado hacia el boss.");
    }
}
