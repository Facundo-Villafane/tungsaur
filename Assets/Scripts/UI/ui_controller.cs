using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class UIController : MonoBehaviour
{
    [Header("Health Bar References")]
    [SerializeField] private Image HPBarRedIndicator;
    [SerializeField] private Image HPBarYellowIndicator;

    [Header("Energy Bar References")]
    [SerializeField] private Image MPBarBlueIndicator1;
    [SerializeField] private Image MPBarBlueIndicator2;
    [SerializeField] private Image MPBarBlueIndicator3;

    [Header("Indicator References")]
    [SerializeField] private Image healthPickupIndicator;
    [SerializeField] private Image energyPickupIndicator;
    [SerializeField] private Image dangerIndicator;

    [Header("Auto-Find Settings")]
    [Tooltip("Buscar indicadores automáticamente por nombre al iniciar")]
    [SerializeField] private bool autoFindIndicators = true;

    [Header("Animation Settings")]
    [SerializeField] private float healthBarSpeed = 2f;
    [SerializeField] private float energyBarSpeed = 2f;
    [SerializeField] private float pickupIndicatorDuration = 0.5f;
    [SerializeField] private float dangerPulseDuration = 2f;
    [SerializeField] private float yellowBarDelay = 1f;

    // Variables internas para vida
    private float targetHealthFill = 1f;
    private float currentRedFill = 1f;
    private float currentYellowFill = 1f;
    private float yellowTargetFill = 1f;
    private bool isYellowWaiting = false;
    private float yellowWaitTimer = 0f;

    // Variables internas para energía
    private float targetEnergy1Fill = 0f;
    private float targetEnergy2Fill = 0f;
    private float targetEnergy3Fill = 0f;
    private float currentEnergy1Fill = 0f;
    private float currentEnergy2Fill = 0f;
    private float currentEnergy3Fill = 0f;

    // Variables para peligro
    private bool isDangerActive = false;
    private Coroutine dangerPulseCoroutine;

private void OnEnable()
{
    PlayerStats.OnHealthChanged += UpdateHealth;
    PlayerStats.OnEnergyChanged += UpdateEnergy;
    PlayerStats.OnHealthPickUp += ShowHealthPickupIndicator;
    PlayerStats.OnEnergyPickUp += ShowEnergyPickupIndicator;
}

private void OnDisable()
{
    PlayerStats.OnHealthChanged -= UpdateHealth;
    PlayerStats.OnEnergyChanged -= UpdateEnergy;
    PlayerStats.OnHealthPickUp -= ShowHealthPickupIndicator;
    PlayerStats.OnEnergyPickUp -= ShowEnergyPickupIndicator;
}

    private void Start()
    {
        // Auto-buscar indicadores si está activado
        if (autoFindIndicators)
        {
            FindIndicators();
        }

        // Inicializar indicadores invisibles
        if (healthPickupIndicator != null)
            SetIndicatorAlpha(healthPickupIndicator, 0f);
        if (energyPickupIndicator != null)
            SetIndicatorAlpha(energyPickupIndicator, 0f);
        if (dangerIndicator != null)
            SetIndicatorAlpha(dangerIndicator, 0f);
    }

    // Método para buscar automáticamente los indicadores por nombre
    public void FindIndicators()
    {
        // Buscar en todo el Canvas
        Image[] allImages = FindObjectsOfType<Image>();

        foreach (Image img in allImages)
        {
            if (healthPickupIndicator == null && img.name == "HealthPickupIndicator")
            {
                healthPickupIndicator = img;
            }
            else if (energyPickupIndicator == null && img.name == "EnergyPickupIndicator")
            {
                energyPickupIndicator = img;
            }
            else if (dangerIndicator == null && img.name == "DangerIndicator")
            {
                dangerIndicator = img;
            }
        }

    }

    // Método para asignar manualmente desde el Inspector (botón)
    [ContextMenu("Buscar Indicadores Ahora")]
    void FindIndicatorsManually()
    {
        FindIndicators();
    }

    private void Update()
    {
        UpdateHealthBars();
        UpdateEnergyBars();
    }

    // ==================== HEALTH BAR LOGIC ====================

    public void UpdateHealth(float currentHP, float maxHP)
    {
        float newTargetFill = currentHP / maxHP;


        // Detectar si es daño o curación
        if (newTargetFill < targetHealthFill)
        {
            // Recibiendo daño
            targetHealthFill = newTargetFill;
            yellowTargetFill = newTargetFill;
            isYellowWaiting = true;
            yellowWaitTimer = 0f;
        }
        else if (newTargetFill > targetHealthFill)
        {
            // Curando
            targetHealthFill = newTargetFill;
            yellowTargetFill = newTargetFill;
            isYellowWaiting = false;
        }

        // Verificar estado de peligro
        CheckDangerState(currentHP, maxHP);
    }

    public void UpdateHealthBars()
    {
        // Actualizar barra roja (siempre se mueve hacia el target)
        if (HPBarRedIndicator != null)
        {
            currentRedFill = Mathf.Lerp(currentRedFill, targetHealthFill, Time.deltaTime * healthBarSpeed);
            HPBarRedIndicator.fillAmount = currentRedFill;
        }

        // Actualizar barra amarilla con delay
        if (HPBarYellowIndicator != null)
        {
            if (isYellowWaiting)
            {
                // Incrementar temporizador
                yellowWaitTimer += Time.deltaTime;

                // Después de 1 segundo, permitir que baje
                if (yellowWaitTimer >= yellowBarDelay)
                {
                    isYellowWaiting = false;
                }
            }

            if (!isYellowWaiting)
            {
                // Mover la barra amarilla hacia su target
                currentYellowFill = Mathf.Lerp(currentYellowFill, yellowTargetFill, Time.deltaTime * healthBarSpeed);
            }

            HPBarYellowIndicator.fillAmount = currentYellowFill;
        }
    }

    // ==================== ENERGY BAR LOGIC ====================

    public void UpdateEnergy(float currentEnergy, float maxEnergy)
    {

        // Barra 1: 0-33
        if (currentEnergy <= 33f)
        {
            targetEnergy1Fill = currentEnergy / 33f;
            targetEnergy2Fill = 0f;
            targetEnergy3Fill = 0f;
        }
        // Barra 2: 34-66
        else if (currentEnergy <= 66f)
        {
            targetEnergy1Fill = 1f;
            targetEnergy2Fill = (currentEnergy - 33f) / 33f;
            targetEnergy3Fill = 0f;
        }
        // Barra 3: 67-100
        else
        {
            targetEnergy1Fill = 1f;
            targetEnergy2Fill = 1f;
            targetEnergy3Fill = (currentEnergy - 66f) / 34f;
        }

  
    }

    public void UpdateEnergyBars()
    {
        // Actualizar las tres barras con interpolación suave
        if (MPBarBlueIndicator1 != null)
        {
            currentEnergy1Fill = Mathf.Lerp(currentEnergy1Fill, targetEnergy1Fill, Time.deltaTime * energyBarSpeed);
            MPBarBlueIndicator1.fillAmount = currentEnergy1Fill;
        }

        if (MPBarBlueIndicator2 != null)
        {
            currentEnergy2Fill = Mathf.Lerp(currentEnergy2Fill, targetEnergy2Fill, Time.deltaTime * energyBarSpeed);
            MPBarBlueIndicator2.fillAmount = currentEnergy2Fill;
        }

        if (MPBarBlueIndicator3 != null)
        {
            currentEnergy3Fill = Mathf.Lerp(currentEnergy3Fill, targetEnergy3Fill, Time.deltaTime * energyBarSpeed);
            MPBarBlueIndicator3.fillAmount = currentEnergy3Fill;
        }
    }

    // ==================== INDICATOR LOGIC ====================

    public void ShowHealthPickupIndicator()
    {
        

        if (healthPickupIndicator == null)
        {
           
            return;
        }

        StartCoroutine(FlashIndicator(healthPickupIndicator));
    }

    public void ShowEnergyPickupIndicator()
    {
       

        if (energyPickupIndicator == null)
        {
          
            return;
        }

        StartCoroutine(FlashIndicator(energyPickupIndicator));
    }

    IEnumerator FlashIndicator(Image indicator)
    {
        

        float elapsed = 0f;

        // Fade in
        while (elapsed < pickupIndicatorDuration)
        {
            elapsed += Time.deltaTime;
            float alpha = Mathf.Lerp(0f, 1f, elapsed / pickupIndicatorDuration);
            SetIndicatorAlpha(indicator, alpha);
            yield return null;
        }

        SetIndicatorAlpha(indicator, 1f);
        elapsed = 0f;

        // Fade out
        while (elapsed < pickupIndicatorDuration)
        {
            elapsed += Time.deltaTime;
            float alpha = Mathf.Lerp(1f, 0f, elapsed / pickupIndicatorDuration);
            SetIndicatorAlpha(indicator, alpha);
            yield return null;
        }

        SetIndicatorAlpha(indicator, 0f);
        
    }

    public void CheckDangerState(float currentHP, float maxHP)
    {
        bool shouldShowDanger = (currentHP / maxHP) < 0.25f;

        if (shouldShowDanger && !isDangerActive)
        {
            isDangerActive = true;
            if (dangerIndicator != null)
            {
                dangerPulseCoroutine = StartCoroutine(DangerPulse());
            }
        }
        else if (!shouldShowDanger && isDangerActive)
        {
            isDangerActive = false;
            if (dangerPulseCoroutine != null)
            {
                StopCoroutine(dangerPulseCoroutine);
            }
            if (dangerIndicator != null)
            {
                SetIndicatorAlpha(dangerIndicator, 0f);
            }
        }
    }

    public IEnumerator DangerPulse()
    {
        while (isDangerActive)
        {
            float elapsed = 0f;

            // Fade in
            while (elapsed < dangerPulseDuration)
            {
                elapsed += Time.deltaTime;
                float alpha = Mathf.Lerp(0f, 1f, elapsed / dangerPulseDuration);
                SetIndicatorAlpha(dangerIndicator, alpha);
                yield return null;
            }

            elapsed = 0f;

            // Fade out
            while (elapsed < dangerPulseDuration)
            {
                elapsed += Time.deltaTime;
                float alpha = Mathf.Lerp(1f, 0f, elapsed / dangerPulseDuration);
                SetIndicatorAlpha(dangerIndicator, alpha);
                yield return null;
            }
        }
    }

    public void SetIndicatorAlpha(Image image, float alpha)
    {
        if (image != null)
        {
            Color color = image.color;
            color.a = alpha;
            image.color = color;
        }
    }
}