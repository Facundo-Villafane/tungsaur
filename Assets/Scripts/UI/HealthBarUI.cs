using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace CDG.UI
{
    /// <summary>
    /// Reusable health bar UI component
    /// Can be used for player, enemies, or bosses
    /// </summary>
    public class HealthBarUI : MonoBehaviour
    {
        [Header("UI Elements")]
        [SerializeField] private Slider healthSlider;
        [SerializeField] private Image fillImage;
        [SerializeField] private TextMeshProUGUI nameText;
        [SerializeField] private TextMeshProUGUI healthText;

        [Header("Colors")]
        [SerializeField] private Color healthyColor = Color.green;
        [SerializeField] private Color warningColor = Color.yellow;
        [SerializeField] private Color criticalColor = Color.red;

        [Header("Thresholds")]
        [SerializeField] private float warningThreshold = 0.5f;
        [SerializeField] private float criticalThreshold = 0.25f;

        [Header("Animation")]
        [SerializeField] private bool animateChanges = true;
        [SerializeField] private float animationSpeed = 5f;

        private float currentHealth;
        private float maxHealth;
        private float targetFillAmount;

        private void Awake()
        {
            if (healthSlider == null)
            {
                healthSlider = GetComponent<Slider>();
            }

            if (fillImage == null && healthSlider != null)
            {
                fillImage = healthSlider.fillRect.GetComponent<Image>();
            }
        }

        private void Update()
        {
            // Smooth animation for health bar
            if (animateChanges && healthSlider != null)
            {
                float currentFill = healthSlider.value;
                if (Mathf.Abs(currentFill - targetFillAmount) > 0.001f)
                {
                    healthSlider.value = Mathf.Lerp(currentFill, targetFillAmount, Time.deltaTime * animationSpeed);
                }
            }
        }

        /// <summary>
        /// Updates the health bar with current and max health
        /// </summary>
        public void UpdateHealth(float current, float max)
        {
            currentHealth = Mathf.Clamp(current, 0f, max);
            maxHealth = max;

            float fillAmount = maxHealth > 0 ? currentHealth / maxHealth : 0f;

            if (animateChanges)
            {
                targetFillAmount = fillAmount;
            }
            else
            {
                if (healthSlider != null)
                {
                    healthSlider.value = fillAmount;
                }
            }

            // Update color based on health percentage
            UpdateColor(fillAmount);

            // Update text
            UpdateHealthText();
        }

        /// <summary>
        /// Updates the color of the health bar based on fill amount
        /// </summary>
        private void UpdateColor(float fillAmount)
        {
            if (fillImage == null) return;

            if (fillAmount <= criticalThreshold)
            {
                fillImage.color = criticalColor;
            }
            else if (fillAmount <= warningThreshold)
            {
                fillImage.color = warningColor;
            }
            else
            {
                fillImage.color = healthyColor;
            }
        }

        /// <summary>
        /// Updates the health text display
        /// </summary>
        private void UpdateHealthText()
        {
            if (healthText != null)
            {
                healthText.text = $"{Mathf.CeilToInt(currentHealth)} / {Mathf.CeilToInt(maxHealth)}";
            }
        }

        /// <summary>
        /// Sets the name displayed on the health bar
        /// </summary>
        public void SetName(string name)
        {
            if (nameText != null)
            {
                nameText.text = name;
            }
        }

        /// <summary>
        /// Initializes the health bar with max health
        /// </summary>
        public void Initialize(float max, string name = "")
        {
            maxHealth = max;
            currentHealth = max;
            UpdateHealth(max, max);

            if (!string.IsNullOrEmpty(name))
            {
                SetName(name);
            }
        }

        /// <summary>
        /// Shows or hides the health bar
        /// </summary>
        public void SetVisible(bool visible)
        {
            gameObject.SetActive(visible);
        }
    }
}
