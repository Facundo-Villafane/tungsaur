using UnityEngine;
using System;
using System.Collections;
using CDG.Data;
using CDG.UI;
using DialogueEditor;

namespace CDG.Managers
{
    /// <summary>
    /// Manages tutorial sequences with step-by-step instructions
    /// Handles player invulnerability, input prompts, and practice waves
    /// </summary>
    public class TutorialManager : MonoBehaviour
    {
        public static TutorialManager Instance { get; private set; }

        [Header("Tutorial Configuration")]
        [SerializeField] private TutorialConfigSO tutorialConfig;

        [Header("UI References")]
        [SerializeField] private TutorialUI tutorialUI;

        [Header("Practice Wave")]
        [SerializeField] private WaveManager practiceWaveManager;

        [Header("State")]
        private int currentStepIndex = 0;
        private bool isTutorialActive = false;
        private bool isWaitingForInput = false;

        // Events
        public event Action OnTutorialStarted;
        public event Action<int> OnStepChanged;
        public event Action OnTutorialCompleted;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
        }

        private void Update()
        {
            // Handle manual step advancement
            if (isTutorialActive && isWaitingForInput && tutorialConfig != null)
            {
                if (Input.GetKeyDown(tutorialConfig.advanceKey))
                {
                    AdvanceToNextStep();
                }
            }
        }

        /// <summary>
        /// Starts the tutorial sequence
        /// </summary>
        public void StartTutorial()
        {
            if (tutorialConfig == null)
            {
                Debug.LogError("TutorialManager: No tutorial config assigned!");
                return;
            }

            if (isTutorialActive)
            {
                Debug.LogWarning("TutorialManager: Tutorial already active!");
                return;
            }

            StartCoroutine(RunTutorialCoroutine());
        }

        /// <summary>
        /// Main coroutine that runs the tutorial
        /// </summary>
        private IEnumerator RunTutorialCoroutine()
        {
            isTutorialActive = true;
            currentStepIndex = 0;

            Debug.Log($"TutorialManager: Starting tutorial '{tutorialConfig.tutorialTitle}'");

            OnTutorialStarted?.Invoke();

            // Enable player invulnerability if configured
            if (tutorialConfig.playerInvulnerable)
            {
                SetPlayerInvulnerable(true);
            }

            // Choose tutorial display mode
            if (tutorialConfig.displayMode == TutorialDisplayMode.DialogueEditor)
            {
                // Use DialogueEditor conversation
                yield return StartCoroutine(ShowDialogueTutorial());
            }
            else
            {
                // Use simple text mode
                if (tutorialUI != null)
                {
                    tutorialUI.Show();
                }
                else if (UIManager.Instance != null)
                {
                    UIManager.Instance.ShowTutorial();
                }

                // Run through all tutorial steps
                for (int i = 0; i < tutorialConfig.tutorialSteps.Length; i++)
                {
                    currentStepIndex = i;
                    yield return StartCoroutine(ShowTutorialStep(i));
                }
            }

            // Run practice wave if configured
            if (tutorialConfig.practiceWave != null && practiceWaveManager != null)
            {
                Debug.Log("TutorialManager: Starting practice wave");

                if (tutorialUI != null)
                {
                    tutorialUI.ShowStep("Defeat the enemies!", "");
                }

                yield return StartCoroutine(RunPracticeWave());
            }

            // Complete tutorial
            CompleteTutorial();
        }

        /// <summary>
        /// Shows tutorial using DialogueEditor conversation
        /// </summary>
        private IEnumerator ShowDialogueTutorial()
        {
            if (tutorialConfig.tutorialConversation == null)
            {
                Debug.LogError("TutorialManager: Tutorial conversation not assigned!");
                yield return new WaitForSeconds(3f);
                yield break;
            }

            // Get NPCConversation component from GameObject
            NPCConversation conversation = tutorialConfig.tutorialConversation.GetComponent<NPCConversation>();
            if (conversation == null)
            {
                Debug.LogError("TutorialManager: GameObject does not have NPCConversation component!");
                yield return new WaitForSeconds(3f);
                yield break;
            }

            // Use ConversationManager to play the tutorial dialogue
            if (ConversationManager.Instance != null)
            {
                Debug.Log("TutorialManager: Starting DialogueEditor tutorial");

                ConversationManager.Instance.StartConversation(conversation);

                // Wait until conversation finishes
                while (ConversationManager.Instance.IsConversationActive)
                {
                    yield return null;
                }

                Debug.Log("TutorialManager: DialogueEditor tutorial completed");
            }
            else
            {
                Debug.LogError("TutorialManager: ConversationManager not found in scene!");
                yield return new WaitForSeconds(3f);
            }
        }

        /// <summary>
        /// Shows a single tutorial step
        /// </summary>
        private IEnumerator ShowTutorialStep(int stepIndex)
        {
            if (stepIndex >= tutorialConfig.tutorialSteps.Length)
            {
                yield break;
            }

            string stepText = tutorialConfig.tutorialSteps[stepIndex];
            string inputPrompt = "";

            // Get input prompt if available
            if (tutorialConfig.showInputPrompts &&
                tutorialConfig.inputPrompts != null &&
                stepIndex < tutorialConfig.inputPrompts.Length)
            {
                inputPrompt = tutorialConfig.inputPrompts[stepIndex];
            }

            Debug.Log($"TutorialManager: Step {stepIndex + 1}/{tutorialConfig.tutorialSteps.Length}: {stepText}");

            // Show step on UI
            if (tutorialUI != null)
            {
                tutorialUI.ShowStep(stepText, inputPrompt);
            }

            OnStepChanged?.Invoke(stepIndex);

            // Wait for advancement
            if (tutorialConfig.autoAdvance)
            {
                // Auto-advance after a delay
                yield return new WaitForSeconds(3f);
            }
            else
            {
                // Wait for player input
                isWaitingForInput = true;
                while (isWaitingForInput)
                {
                    yield return null;
                }
            }
        }

        /// <summary>
        /// Advances to the next tutorial step
        /// </summary>
        private void AdvanceToNextStep()
        {
            isWaitingForInput = false;
        }

        /// <summary>
        /// Runs the practice wave
        /// </summary>
        private IEnumerator RunPracticeWave()
        {
            if (practiceWaveManager == null)
            {
                Debug.LogWarning("TutorialManager: No WaveManager assigned for practice wave!");
                yield return new WaitForSeconds(3f);
                yield break;
            }

            // Clear existing waves and set practice wave
            // (Assuming WaveManager has a method to set a single wave)
            // For now, just start the wave manager
            practiceWaveManager.StartWaves();

            // Wait for wave to complete
            bool waveCompleted = false;
            Action onWaveComplete = () => waveCompleted = true;

            practiceWaveManager.OnAllWavesCompleted += onWaveComplete;

            while (!waveCompleted)
            {
                yield return null;
            }

            practiceWaveManager.OnAllWavesCompleted -= onWaveComplete;

            Debug.Log("TutorialManager: Practice wave completed!");
        }

        /// <summary>
        /// Completes the tutorial
        /// </summary>
        private void CompleteTutorial()
        {
            Debug.Log("TutorialManager: Tutorial completed!");

            // Disable player invulnerability
            if (tutorialConfig.playerInvulnerable)
            {
                SetPlayerInvulnerable(false);
            }

            // Hide tutorial UI
            if (tutorialUI != null)
            {
                tutorialUI.Hide();
            }
            else if (UIManager.Instance != null)
            {
                UIManager.Instance.HideTutorial();
            }

            OnTutorialCompleted?.Invoke();

            isTutorialActive = false;

            // Play completion cinematic if configured
            if (tutorialConfig.completionCinematic != null && CinematicsManager.Instance != null)
            {
                CinematicsManager.Instance.PlayCinematic(tutorialConfig.completionCinematic);
            }
        }

        /// <summary>
        /// Sets player invulnerability state
        /// </summary>
        private void SetPlayerInvulnerable(bool invulnerable)
        {
            // Find player
            PlayerController player = FindFirstObjectByType<PlayerController>();
            if (player != null)
            {
                // TODO: Add invulnerability flag to CharacterBase
                Debug.Log($"TutorialManager: Player invulnerability set to {invulnerable}");
            }
            else
            {
                Debug.LogWarning("TutorialManager: Could not find PlayerController!");
            }
        }

        /// <summary>
        /// Skips the tutorial
        /// </summary>
        public void SkipTutorial()
        {
            if (!isTutorialActive) return;

            Debug.Log("TutorialManager: Skipping tutorial");

            StopAllCoroutines();

            // Clean up
            if (tutorialConfig.playerInvulnerable)
            {
                SetPlayerInvulnerable(false);
            }

            if (tutorialUI != null)
            {
                tutorialUI.Hide();
            }

            isTutorialActive = false;

            OnTutorialCompleted?.Invoke();
        }

        // Public API
        public bool IsTutorialActive() => isTutorialActive;
        public int GetCurrentStepIndex() => currentStepIndex;
        public TutorialConfigSO GetTutorialConfig() => tutorialConfig;
    }
}
