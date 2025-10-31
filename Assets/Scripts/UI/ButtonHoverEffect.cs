using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// Componente que agrega un efecto de escala al hacer hover sobre un botón
/// Usa el sistema de eventos de Unity UI para detectar cuando el mouse entra y sale
/// </summary>
public class ButtonHoverEffect : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [Header("Configuración de Escala")]
    [Tooltip("Escala normal del botón (1 = tamaño original)")]
    [SerializeField] private float normalScale = 1f;

    [Tooltip("Escala cuando el mouse está sobre el botón")]
    [SerializeField] private float hoverScale = 1.15f;

    [Header("Configuración de Animación")]
    [Tooltip("Velocidad de la transición de escala")]
    [SerializeField] private float scaleSpeed = 10f;

    [Tooltip("Usar animación suave (smooth) o instantánea")]
    [SerializeField] private bool smoothTransition = true;

    [Header("Efectos Adicionales (Opcional)")]
    [Tooltip("Reproducir sonido al hacer hover")]
    [SerializeField] private AudioClip hoverSound;

    [Tooltip("AudioSource para reproducir el sonido (si no se asigna, buscará uno)")]
    [SerializeField] private AudioSource audioSource;

    // Variables privadas
    private Vector3 targetScale;
    private Vector3 originalScale;
    private RectTransform rectTransform;
    private bool isHovering = false;

    void Awake()
    {
        // Obtener el RectTransform del botón
        rectTransform = GetComponent<RectTransform>();

        if (rectTransform == null)
        {
            Debug.LogError("ButtonHoverEffect requiere un RectTransform. Asegúrate de que está en un elemento UI.");
            enabled = false;
            return;
        }

        // Guardar la escala original
        originalScale = rectTransform.localScale;
        targetScale = originalScale * normalScale;

        // Buscar AudioSource si no está asignado y hay un sonido configurado
        if (hoverSound != null && audioSource == null)
        {
            audioSource = GetComponent<AudioSource>();
            if (audioSource == null)
            {
                audioSource = gameObject.AddComponent<AudioSource>();
            }
        }

        // Configurar el AudioSource
        if (audioSource != null)
        {
            audioSource.playOnAwake = false;
        }
    }

    void Update()
    {
        // Animar la escala si está activada la transición suave
        if (smoothTransition && rectTransform.localScale != targetScale)
        {
            rectTransform.localScale = Vector3.Lerp(
                rectTransform.localScale,
                targetScale,
                Time.unscaledDeltaTime * scaleSpeed
            );

            // Ajustar exactamente cuando está muy cerca del objetivo
            if (Vector3.Distance(rectTransform.localScale, targetScale) < 0.001f)
            {
                rectTransform.localScale = targetScale;
            }
        }
    }

    /// <summary>
    /// Se llama cuando el puntero del mouse entra en el área del botón
    /// </summary>
    public void OnPointerEnter(PointerEventData eventData)
    {
        isHovering = true;
        targetScale = originalScale * hoverScale;

        // Aplicar escala inmediatamente si no hay transición suave
        if (!smoothTransition)
        {
            rectTransform.localScale = targetScale;
        }

        // Reproducir sonido de hover si está configurado
        if (hoverSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(hoverSound);
        }
    }

    /// <summary>
    /// Se llama cuando el puntero del mouse sale del área del botón
    /// </summary>
    public void OnPointerExit(PointerEventData eventData)
    {
        isHovering = false;
        targetScale = originalScale * normalScale;

        // Aplicar escala inmediatamente si no hay transición suave
        if (!smoothTransition)
        {
            rectTransform.localScale = targetScale;
        }
    }

    /// <summary>
    /// Resetea el efecto a su estado original
    /// Útil si necesitas desactivar el efecto temporalmente
    /// </summary>
    public void ResetEffect()
    {
        isHovering = false;
        targetScale = originalScale * normalScale;
        rectTransform.localScale = targetScale;
    }

    /// <summary>
    /// Permite cambiar la escala de hover en tiempo de ejecución
    /// </summary>
    public void SetHoverScale(float newHoverScale)
    {
        hoverScale = newHoverScale;

        // Actualizar la escala objetivo si actualmente está en hover
        if (isHovering)
        {
            targetScale = originalScale * hoverScale;
        }
    }

    /// <summary>
    /// Permite cambiar la velocidad de la animación en tiempo de ejecución
    /// </summary>
    public void SetScaleSpeed(float newSpeed)
    {
        scaleSpeed = Mathf.Max(0.1f, newSpeed); // Asegurar que no sea negativa o cero
    }

    void OnDisable()
    {
        // Resetear la escala cuando se desactiva el componente
        if (rectTransform != null)
        {
            rectTransform.localScale = originalScale * normalScale;
        }
    }
}
