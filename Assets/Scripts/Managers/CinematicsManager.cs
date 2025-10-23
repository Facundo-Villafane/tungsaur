using UnityEngine;
using UnityEngine.Playables;
using System;
using System.Collections;
using CDG.Data;
using DialogueEditor;

namespace CDG.Managers
{
    /// <summary>
    /// Manages cinematic playback using Unity Timeline or Dialogue system
    /// Handles camera control, skipping, and state management
    /// </summary>
    public class CinematicsManager : MonoBehaviour
    {
        public static CinematicsManager Instance { get; private set; }

        [Header("Playable Director")]
        [SerializeField] private PlayableDirector playableDirector;

        [Header("Cinematic Camera")]
        [SerializeField] private Camera cinematicCamera;
        [SerializeField] private GameObject cinematicCameraObject;

        [Header("UI")]
        [SerializeField] private GameObject skipPromptUI;
        [SerializeField] private CanvasGroup fadeCanvasGroup;

        [Header("Settings")]
        [SerializeField] private float fadeDuration = 1f;

        // State
        private bool isPlayingCinematic = false;
        private CinematicConfigSO currentCinematic;
        private Coroutine cinematicCoroutine;

        // Events
        public event Action<CinematicConfigSO> OnCinematicStarted;
        public event Action<CinematicConfigSO> OnCinematicCompleted;
        public event Action<CinematicConfigSO> OnCinematicSkipped;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;

            // Auto-find PlayableDirector if not assigned
            if (playableDirector == null)
            {
                playableDirector = GetComponent<PlayableDirector>();
            }

            // Initialize fade canvas
            if (fadeCanvasGroup != null)
            {
                fadeCanvasGroup.alpha = 0f;
            }
        }

        private void Update()
        {
            // Handle skip input during cinematics
            if (isPlayingCinematic && currentCinematic != null)
            {
                if (currentCinematic.canSkip && Input.GetKeyDown(currentCinematic.skipKey))
                {
                    SkipCinematic();
                }
            }
        }

        /// <summary>
        /// Plays a cinematic based on configuration
        /// </summary>
        public void PlayCinematic(CinematicConfigSO cinematicConfig, Action onComplete = null)
        {
            if (cinematicConfig == null)
            {
                Debug.LogWarning("CinematicsManager: Cannot play null cinematic config");
                onComplete?.Invoke();
                return;
            }

            if (isPlayingCinematic)
            {
                Debug.LogWarning("CinematicsManager: Already playing a cinematic");
                return;
            }

            cinematicCoroutine = StartCoroutine(PlayCinematicCoroutine(cinematicConfig, onComplete));
        }

        /// <summary>
        /// Main coroutine for playing cinematics
        /// </summary>
        private IEnumerator PlayCinematicCoroutine(CinematicConfigSO cinematicConfig, Action onComplete)
        {
            isPlayingCinematic = true;
            currentCinematic = cinematicConfig;

            Debug.Log($"CinematicsManager: Playing '{cinematicConfig.cinematicName}'");

            // Notify listeners
            OnCinematicStarted?.Invoke(cinematicConfig);

            // Fade in
            yield return StartCoroutine(FadeIn());

            // Lock camera if needed
            if (cinematicConfig.lockCamera && GameManager.Instance != null)
            {
                GameManager.Instance.ChangeCameraState(CameraState.Locked);
            }

            // Enable cinematic camera if available
            if (cinematicCameraObject != null)
            {
                cinematicCameraObject.SetActive(true);
            }

            // Show skip prompt
            if (cinematicConfig.canSkip && skipPromptUI != null)
            {
                skipPromptUI.SetActive(true);
            }

            // Play cinematic based on type
            switch (cinematicConfig.cinematicType)
            {
                case CinematicType.Timeline:
                    yield return StartCoroutine(PlayTimelineCinematic(cinematicConfig));
                    break;

                case CinematicType.Dialogue:
                    yield return StartCoroutine(PlayDialogueCinematic(cinematicConfig));
                    break;

                case CinematicType.Custom:
                    Debug.LogWarning("CinematicsManager: Custom cinematic type not implemented");
                    yield return new WaitForSeconds(3f);
                    break;
            }

            // Hide skip prompt
            if (skipPromptUI != null)
            {
                skipPromptUI.SetActive(false);
            }

            // Disable cinematic camera
            if (cinematicCameraObject != null)
            {
                cinematicCameraObject.SetActive(false);
            }

            // Fade out
            yield return StartCoroutine(FadeOut());

            // Restore camera state
            if (GameManager.Instance != null)
            {
                GameManager.Instance.ChangeCameraState(CameraState.Free);
            }

            // Notify completion
            OnCinematicCompleted?.Invoke(cinematicConfig);

            Debug.Log($"CinematicsManager: Finished '{cinematicConfig.cinematicName}'");

            isPlayingCinematic = false;
            currentCinematic = null;

            // Invoke callback
            onComplete?.Invoke();
        }

        /// <summary>
        /// Plays a Timeline-based cinematic
        /// </summary>
        private IEnumerator PlayTimelineCinematic(CinematicConfigSO cinematicConfig)
        {
            if (playableDirector == null)
            {
                Debug.LogError("CinematicsManager: PlayableDirector not assigned!");
                yield return new WaitForSeconds(3f);
                yield break;
            }

            if (cinematicConfig.timelineAsset == null)
            {
                Debug.LogError("CinematicsManager: Timeline asset not assigned!");
                yield return new WaitForSeconds(3f);
                yield break;
            }

            // Set and play the timeline
            playableDirector.playableAsset = cinematicConfig.timelineAsset;
            playableDirector.Play();

            // Wait for timeline to finish
            while (playableDirector.state == PlayState.Playing)
            {
                yield return null;
            }
        }

        /// <summary>
        /// Plays a dialogue-based cinematic
        /// </summary>
        private IEnumerator PlayDialogueCinematic(CinematicConfigSO cinematicConfig)
        {
            if (cinematicConfig.dialogueConversation == null)
            {
                Debug.LogError("CinematicsManager: Dialogue conversation not assigned!");
                yield return new WaitForSeconds(3f);
                yield break;
            }

            // Use existing ConversationManager to play dialogue
            if (ConversationManager.Instance != null)
            {
                ConversationManager.Instance.StartConversation(cinematicConfig.dialogueConversation);

                // Wait until dialogue finishes
                while (ConversationManager.Instance.IsConversationActive)
                {
                    yield return null;
                }
            }
            else
            {
                Debug.LogError("CinematicsManager: ConversationManager not found in scene!");
                yield return new WaitForSeconds(3f);
            }
        }

        /// <summary>
        /// Skips the current cinematic
        /// </summary>
        public void SkipCinematic()
        {
            if (!isPlayingCinematic || currentCinematic == null || !currentCinematic.canSkip)
            {
                return;
            }

            Debug.Log($"CinematicsManager: Skipping '{currentCinematic.cinematicName}'");

            // Stop timeline if playing
            if (playableDirector != null && playableDirector.state == PlayState.Playing)
            {
                playableDirector.Stop();
            }

            // Stop dialogue if playing
            if (ConversationManager.Instance != null && ConversationManager.Instance.IsConversationActive)
            {
                ConversationManager.Instance.EndConversation();
            }

            // Notify listeners
            OnCinematicSkipped?.Invoke(currentCinematic);

            // Stop the coroutine
            if (cinematicCoroutine != null)
            {
                StopCoroutine(cinematicCoroutine);
            }

            // Clean up
            if (skipPromptUI != null) skipPromptUI.SetActive(false);
            if (cinematicCameraObject != null) cinematicCameraObject.SetActive(false);
            if (GameManager.Instance != null) GameManager.Instance.ChangeCameraState(CameraState.Free);

            isPlayingCinematic = false;
            currentCinematic = null;
        }

        /// <summary>
        /// Fades in from black
        /// </summary>
        private IEnumerator FadeIn()
        {
            if (fadeCanvasGroup == null) yield break;

            float elapsed = 0f;
            fadeCanvasGroup.alpha = 1f;

            while (elapsed < fadeDuration)
            {
                elapsed += Time.deltaTime;
                fadeCanvasGroup.alpha = 1f - (elapsed / fadeDuration);
                yield return null;
            }

            fadeCanvasGroup.alpha = 0f;
        }

        /// <summary>
        /// Fades out to black
        /// </summary>
        private IEnumerator FadeOut()
        {
            if (fadeCanvasGroup == null) yield break;

            float elapsed = 0f;
            fadeCanvasGroup.alpha = 0f;

            while (elapsed < fadeDuration)
            {
                elapsed += Time.deltaTime;
                fadeCanvasGroup.alpha = elapsed / fadeDuration;
                yield return null;
            }

            fadeCanvasGroup.alpha = 1f;
        }

        // Public API
        public bool IsPlayingCinematic() => isPlayingCinematic;
        public CinematicConfigSO GetCurrentCinematic() => currentCinematic;
    }
}
