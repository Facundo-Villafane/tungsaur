using UnityEngine;
using TMPro;

namespace CDG.UI
{
    /// <summary>
    /// Displays the player's score with optional animation
    /// </summary>
    public class ScoreUI : MonoBehaviour
    {
        [Header("UI Elements")]
        [SerializeField] private TextMeshProUGUI scoreText;

        [Header("Animation")]
        [SerializeField] private bool animateScore = true;
        [SerializeField] private float animationSpeed = 100f;

        private int currentScore = 0;
        private int targetScore = 0;

        private void Update()
        {
            // Animate score counting up
            if (animateScore && currentScore != targetScore)
            {
                int difference = targetScore - currentScore;
                int step = Mathf.CeilToInt(animationSpeed * Time.deltaTime);

                if (Mathf.Abs(difference) < step)
                {
                    currentScore = targetScore;
                }
                else
                {
                    currentScore += (difference > 0) ? step : -step;
                }

                UpdateScoreText();
            }
        }

        /// <summary>
        /// Updates the score display
        /// </summary>
        public void UpdateScore(int newScore)
        {
            targetScore = newScore;

            if (!animateScore)
            {
                currentScore = targetScore;
                UpdateScoreText();
            }
        }

        /// <summary>
        /// Adds to the current score
        /// </summary>
        public void AddScore(int amount)
        {
            UpdateScore(targetScore + amount);
        }

        /// <summary>
        /// Resets the score to zero
        /// </summary>
        public void ResetScore()
        {
            currentScore = 0;
            targetScore = 0;
            UpdateScoreText();
        }

        /// <summary>
        /// Updates the text display
        /// </summary>
        private void UpdateScoreText()
        {
            if (scoreText != null)
            {
                scoreText.text = $"Score: {currentScore:N0}";
            }
        }
    }
}
