using UnityEngine;
using TMPro;
using System.Collections;

namespace CDG.UI
{
    /// <summary>
    /// Displays objectives and mission text to the player
    /// </summary>
    public class ObjectiveUI : MonoBehaviour
    {
        [Header("UI Elements")]
        [SerializeField] private GameObject objectivePanel;
        [SerializeField] private TextMeshProUGUI objectiveText;

        [Header("Animation")]
        [SerializeField] private bool fadeIn = true;
        [SerializeField] private float fadeDuration = 0.5f;

        private CanvasGroup canvasGroup;
        private Coroutine fadeCoroutine;

        private void Awake()
        {
            // Get or add CanvasGroup for fade animations
            if (objectivePanel != null)
            {
                canvasGroup = objectivePanel.GetComponent<CanvasGroup>();
                if (canvasGroup == null && fadeIn)
                {
                    canvasGroup = objectivePanel.AddComponent<CanvasGroup>();
                }
            }

            HideObjective();
        }

        /// <summary>
        /// Shows an objective with optional fade-in
        /// </summary>
        public void ShowObjective(string text)
        {
            if (objectiveText != null)
            {
                objectiveText.text = text;
            }

            if (objectivePanel != null)
            {
                objectivePanel.SetActive(true);
            }

            if (fadeIn && canvasGroup != null)
            {
                if (fadeCoroutine != null)
                {
                    StopCoroutine(fadeCoroutine);
                }
                fadeCoroutine = StartCoroutine(FadeInCoroutine());
            }
        }

        /// <summary>
        /// Hides the objective with optional fade-out
        /// </summary>
        public void HideObjective()
        {
            if (fadeIn && canvasGroup != null)
            {
                if (fadeCoroutine != null)
                {
                    StopCoroutine(fadeCoroutine);
                }
                fadeCoroutine = StartCoroutine(FadeOutCoroutine());
            }
            else
            {
                if (objectivePanel != null)
                {
                    objectivePanel.SetActive(false);
                }
            }
        }

        /// <summary>
        /// Updates the objective text without hiding/showing
        /// </summary>
        public void UpdateObjective(string text)
        {
            if (objectiveText != null)
            {
                objectiveText.text = text;
            }
        }

        /// <summary>
        /// Fades in the objective panel
        /// </summary>
        private IEnumerator FadeInCoroutine()
        {
            float elapsed = 0f;
            canvasGroup.alpha = 0f;

            while (elapsed < fadeDuration)
            {
                elapsed += Time.deltaTime;
                canvasGroup.alpha = Mathf.Clamp01(elapsed / fadeDuration);
                yield return null;
            }

            canvasGroup.alpha = 1f;
        }

        /// <summary>
        /// Fades out the objective panel
        /// </summary>
        private IEnumerator FadeOutCoroutine()
        {
            float elapsed = 0f;
            float startAlpha = canvasGroup.alpha;

            while (elapsed < fadeDuration)
            {
                elapsed += Time.deltaTime;
                canvasGroup.alpha = Mathf.Clamp01(startAlpha - (elapsed / fadeDuration));
                yield return null;
            }

            canvasGroup.alpha = 0f;

            if (objectivePanel != null)
            {
                objectivePanel.SetActive(false);
            }
        }
    }
}
