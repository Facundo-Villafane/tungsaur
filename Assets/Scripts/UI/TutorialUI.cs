using UnityEngine;
using TMPro;
using System.Collections;

namespace CDG.UI
{
    /// <summary>
    /// UI component for displaying tutorial instructions and input prompts
    /// </summary>
    public class TutorialUI : MonoBehaviour
    {
        [Header("UI Elements")]
        [SerializeField] private GameObject tutorialPanel;
        [SerializeField] private TextMeshProUGUI instructionText;
        [SerializeField] private TextMeshProUGUI inputPromptText;
        [SerializeField] private GameObject advancePrompt;

        [Header("Animation")]
        [SerializeField] private bool fadeIn = true;
        [SerializeField] private float fadeDuration = 0.5f;

        private CanvasGroup canvasGroup;
        private Coroutine fadeCoroutine;

        private void Awake()
        {
            // Get or add CanvasGroup for fade animations
            if (tutorialPanel != null && fadeIn)
            {
                canvasGroup = tutorialPanel.GetComponent<CanvasGroup>();
                if (canvasGroup == null)
                {
                    canvasGroup = tutorialPanel.AddComponent<CanvasGroup>();
                }
            }

            Hide();
        }

        /// <summary>
        /// Shows the tutorial UI
        /// </summary>
        public void Show()
        {
            if (tutorialPanel != null)
            {
                tutorialPanel.SetActive(true);
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
        /// Hides the tutorial UI
        /// </summary>
        public void Hide()
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
                if (tutorialPanel != null)
                {
                    tutorialPanel.SetActive(false);
                }
            }
        }

        /// <summary>
        /// Shows a tutorial step with instruction and input prompt
        /// </summary>
        public void ShowStep(string instruction, string inputPrompt = "")
        {
            if (instructionText != null)
            {
                instructionText.text = instruction;
            }

            if (inputPromptText != null)
            {
                if (!string.IsNullOrEmpty(inputPrompt))
                {
                    inputPromptText.text = inputPrompt;
                    inputPromptText.gameObject.SetActive(true);
                }
                else
                {
                    inputPromptText.gameObject.SetActive(false);
                }
            }

            // Show advance prompt
            if (advancePrompt != null)
            {
                advancePrompt.SetActive(true);
            }
        }

        /// <summary>
        /// Hides the advance prompt (used during practice waves)
        /// </summary>
        public void HideAdvancePrompt()
        {
            if (advancePrompt != null)
            {
                advancePrompt.SetActive(false);
            }
        }

        /// <summary>
        /// Fades in the tutorial panel
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
        /// Fades out the tutorial panel
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

            if (tutorialPanel != null)
            {
                tutorialPanel.SetActive(false);
            }
        }
    }
}
