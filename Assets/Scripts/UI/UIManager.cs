using UnityEngine;
using System;

namespace CDG.UI
{
    /// <summary>
    /// Central UI manager for all UI panels and HUD elements
    /// Manages health bars, score, pause menu, game over, etc.
    /// </summary>
    public class UIManager : MonoBehaviour
    {
        public static UIManager Instance { get; private set; }

        [Header("HUD Panels")]
        [SerializeField] private GameObject hudPanel;
        [SerializeField] private HealthBarUI playerHealthBar;
        [SerializeField] private HealthBarUI bossHealthBar;
        [SerializeField] private ScoreUI scoreUI;
        [SerializeField] private ObjectiveUI objectiveUI;

        [Header("Menu Panels")]
        [SerializeField] private GameObject pauseMenuPanel;
        [SerializeField] private GameObject gameOverPanel;
        [SerializeField] private GameObject levelCompletePanel;

        [Header("Tutorial UI")]
        [SerializeField] private GameObject tutorialPanel;

        // Events
        public event Action OnPauseMenuOpened;
        public event Action OnPauseMenuClosed;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;

            // Initialize all panels as hidden
            HideAllPanels();
        }

        private void Start()
        {
            // Show HUD by default
            ShowHUD();

            // Subscribe to GameManager events
            if (GameManager.Instance != null)
            {
                GameManager.Instance.OnGameStateChanged += HandleGameStateChanged;
            }
        }

        private void OnDestroy()
        {
            // Unsubscribe from events
            if (GameManager.Instance != null)
            {
                GameManager.Instance.OnGameStateChanged -= HandleGameStateChanged;
            }
        }

        /// <summary>
        /// Handles game state changes from GameManager
        /// </summary>
        private void HandleGameStateChanged(GameState newState)
        {
            switch (newState)
            {
                case GameState.Playing:
                    ShowHUD();
                    HidePauseMenu();
                    break;

                case GameState.Paused:
                    ShowPauseMenu();
                    break;

                case GameState.GameOver:
                    ShowGameOver();
                    break;
            }
        }

        // === HUD Management ===

        public void ShowHUD()
        {
            if (hudPanel != null)
            {
                hudPanel.SetActive(true);
            }
        }

        public void HideHUD()
        {
            if (hudPanel != null)
            {
                hudPanel.SetActive(false);
            }
        }

        /// <summary>
        /// Updates player health bar
        /// </summary>
        public void UpdatePlayerHealth(float currentHealth, float maxHealth)
        {
            if (playerHealthBar != null)
            {
                playerHealthBar.UpdateHealth(currentHealth, maxHealth);
            }
        }

        /// <summary>
        /// Updates boss health bar
        /// </summary>
        public void UpdateBossHealth(float currentHealth, float maxHealth, string bossName = "Boss")
        {
            if (bossHealthBar != null)
            {
                bossHealthBar.UpdateHealth(currentHealth, maxHealth);
                bossHealthBar.SetName(bossName);
            }
        }

        /// <summary>
        /// Shows the boss health bar
        /// </summary>
        public void ShowBossHealthBar(string bossName, float maxHealth)
        {
            if (bossHealthBar != null)
            {
                bossHealthBar.gameObject.SetActive(true);
                bossHealthBar.SetName(bossName);
                bossHealthBar.UpdateHealth(maxHealth, maxHealth);
            }
        }

        /// <summary>
        /// Hides the boss health bar
        /// </summary>
        public void HideBossHealthBar()
        {
            if (bossHealthBar != null)
            {
                bossHealthBar.gameObject.SetActive(false);
            }
        }

        /// <summary>
        /// Updates score display
        /// </summary>
        public void UpdateScore(int score)
        {
            if (scoreUI != null)
            {
                scoreUI.UpdateScore(score);
            }
        }

        /// <summary>
        /// Shows an objective text (e.g., "Defeat all enemies")
        /// </summary>
        public void ShowObjective(string objectiveText)
        {
            if (objectiveUI != null)
            {
                objectiveUI.ShowObjective(objectiveText);
            }
        }

        /// <summary>
        /// Hides the objective text
        /// </summary>
        public void HideObjective()
        {
            if (objectiveUI != null)
            {
                objectiveUI.HideObjective();
            }
        }

        // === Pause Menu ===

        public void ShowPauseMenu()
        {
            if (pauseMenuPanel != null)
            {
                pauseMenuPanel.SetActive(true);
                OnPauseMenuOpened?.Invoke();
            }
        }

        public void HidePauseMenu()
        {
            if (pauseMenuPanel != null)
            {
                pauseMenuPanel.SetActive(false);
                OnPauseMenuClosed?.Invoke();
            }
        }

        public void OnResumeButtonClicked()
        {
            if (GameManager.Instance != null)
            {
                GameManager.Instance.ResumeGame();
            }
        }

        public void OnRestartButtonClicked()
        {
            // TODO: Implement restart functionality
            Debug.Log("Restart button clicked");
        }

        public void OnMainMenuButtonClicked()
        {
            // TODO: Load main menu scene
            Debug.Log("Main menu button clicked");
        }

        // === Game Over ===

        public void ShowGameOver()
        {
            if (gameOverPanel != null)
            {
                gameOverPanel.SetActive(true);
            }
            HideHUD();
        }

        public void HideGameOver()
        {
            if (gameOverPanel != null)
            {
                gameOverPanel.SetActive(false);
            }
        }

        // === Level Complete ===

        public void ShowLevelComplete(int score, int glory)
        {
            if (levelCompletePanel != null)
            {
                levelCompletePanel.SetActive(true);
                // TODO: Pass score and glory to the panel
            }
            HideHUD();
        }

        public void HideLevelComplete()
        {
            if (levelCompletePanel != null)
            {
                levelCompletePanel.SetActive(false);
            }
        }

        // === Tutorial ===

        public void ShowTutorial()
        {
            if (tutorialPanel != null)
            {
                tutorialPanel.SetActive(true);
            }
        }

        public void HideTutorial()
        {
            if (tutorialPanel != null)
            {
                tutorialPanel.SetActive(false);
            }
        }

        // === Utility ===

        private void HideAllPanels()
        {
            if (hudPanel != null) hudPanel.SetActive(false);
            if (pauseMenuPanel != null) pauseMenuPanel.SetActive(false);
            if (gameOverPanel != null) gameOverPanel.SetActive(false);
            if (levelCompletePanel != null) levelCompletePanel.SetActive(false);
            if (tutorialPanel != null) tutorialPanel.SetActive(false);
            if (bossHealthBar != null) bossHealthBar.gameObject.SetActive(false);
        }
    }
}
