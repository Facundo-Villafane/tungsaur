using UnityEngine;
using System;
using System.Collections;
using CDG.Data;
using CDG.UI;

namespace CDG.Characters
{
    /// <summary>
    /// Boss controller that extends EnemyController with phases, special attacks, and UI integration
    /// </summary>
    [RequireComponent(typeof(Rigidbody))]
    public class BossController : EnemyController
    {
        [Header("Boss Configuration")]
        [SerializeField] private BossConfigSO bossConfig;

        [Header("Boss-Specific Settings")]
        [SerializeField] private bool showHealthBar = true;
        [SerializeField] private bool invulnerableDuringIntro = true;

        [Header("Phase System")]
        [SerializeField] private int currentPhase = 0;
        private float[] phaseThresholds;

        [Header("Special Attacks")]
        [SerializeField] private float specialAttackCooldown = 10f;
        private float lastSpecialAttackTime = -999f;

        [Header("States")]
        private bool isInIntro = false;
        private bool isDefeated = false;

        // Events
        public event Action<int> OnPhaseChanged;
        public event Action OnBossDefeated;
        public event Action OnSpecialAttackStarted;

        protected override void Start()
        {
            base.Start();

            if (bossConfig != null)
            {
                InitializeFromConfig();
            }

            // Show boss health bar
            if (showHealthBar && UIManager.Instance != null)
            {
                string bossName = bossConfig != null ? bossConfig.bossName : "Boss";
                UIManager.Instance.ShowBossHealthBar(bossName, MaxHealth);
            }
        }

        /// <summary>
        /// Initializes boss stats from BossConfigSO
        /// </summary>
        private void InitializeFromConfig()
        {
            if (bossConfig == null) return;

            // Set stats
            MaxHealth = bossConfig.maxHealth;
            CurrentHealth = MaxHealth;
            BaseDamage = bossConfig.damage;
            Defense = bossConfig.defense;
            MoveSpeed = bossConfig.moveSpeed;

            // Set phase thresholds
            phaseThresholds = bossConfig.phaseThresholds;

            // Set special attack cooldown
            specialAttackCooldown = bossConfig.specialAttackCooldown;

            Debug.Log($"BossController: Initialized '{bossConfig.bossName}' with {MaxHealth} HP");
        }

        protected override void Update()
        {
            base.Update();

            // Update boss health bar
            if (showHealthBar && UIManager.Instance != null)
            {
                UIManager.Instance.UpdateBossHealth(CurrentHealth, MaxHealth);
            }

            // Check for special attack opportunity
            if (!isInIntro && !isDefeated && CanUseSpecialAttack())
            {
                TryUseSpecialAttack();
            }
        }

        /// <summary>
        /// Override TakeDamage to handle phase transitions
        /// </summary>
        public override void TakeDamage(float damage)
        {
            // Invulnerable during intro
            if (invulnerableDuringIntro && isInIntro)
            {
                Debug.Log("BossController: Invulnerable during intro!");
                return;
            }

            // Normal damage
            float previousHealth = CurrentHealth;
            base.TakeDamage(damage);

            // Check for phase transition
            CheckPhaseTransition(previousHealth, CurrentHealth);
        }

        /// <summary>
        /// Checks if the boss should transition to a new phase
        /// </summary>
        private void CheckPhaseTransition(float previousHealth, float newHealth)
        {
            if (phaseThresholds == null || phaseThresholds.Length == 0) return;

            float previousHealthPercent = previousHealth / MaxHealth;
            float newHealthPercent = newHealth / MaxHealth;

            // Check each phase threshold
            for (int i = 0; i < phaseThresholds.Length; i++)
            {
                float threshold = phaseThresholds[i];

                // If we crossed this threshold
                if (previousHealthPercent >= threshold && newHealthPercent < threshold)
                {
                    int newPhase = i + 1;
                    if (newPhase != currentPhase)
                    {
                        TransitionToPhase(newPhase);
                    }
                }
            }
        }

        /// <summary>
        /// Transitions to a new phase
        /// </summary>
        private void TransitionToPhase(int newPhase)
        {
            currentPhase = newPhase;

            string phaseName = "Phase " + currentPhase;
            if (bossConfig != null && bossConfig.phaseNames != null && currentPhase < bossConfig.phaseNames.Length)
            {
                phaseName = bossConfig.phaseNames[currentPhase];
            }

            Debug.Log($"BossController: Transitioning to {phaseName}");

            OnPhaseChanged?.Invoke(currentPhase);

            // Show objective UI
            if (UIManager.Instance != null)
            {
                UIManager.Instance.ShowObjective(phaseName);
            }

            // TODO: Trigger phase-specific behavior
            // - Change attack patterns
            // - Increase speed/damage
            // - Enable new abilities
        }

        /// <summary>
        /// Checks if the boss can use a special attack
        /// </summary>
        private bool CanUseSpecialAttack()
        {
            return (Time.time - lastSpecialAttackTime) >= specialAttackCooldown;
        }

        /// <summary>
        /// Attempts to use a special attack
        /// </summary>
        private void TryUseSpecialAttack()
        {
            // TODO: Implement special attack logic based on phase
            // For now, just reset cooldown
            lastSpecialAttackTime = Time.time;
            OnSpecialAttackStarted?.Invoke();

            Debug.Log($"BossController: Special attack used in phase {currentPhase}!");

            // Example: Different attacks per phase
            switch (currentPhase)
            {
                case 0:
                    SpecialAttackPhase1();
                    break;
                case 1:
                    SpecialAttackPhase2();
                    break;
                case 2:
                    SpecialAttackPhase3();
                    break;
                default:
                    SpecialAttackFinal();
                    break;
            }
        }

        // === Special Attack Implementations (Placeholders) ===

        private void SpecialAttackPhase1()
        {
            Debug.Log("Boss uses Phase 1 special attack!");
            // TODO: Implement phase 1 special attack
        }

        private void SpecialAttackPhase2()
        {
            Debug.Log("Boss uses Phase 2 special attack!");
            // TODO: Implement phase 2 special attack
        }

        private void SpecialAttackPhase3()
        {
            Debug.Log("Boss uses Phase 3 special attack!");
            // TODO: Implement phase 3 special attack
        }

        private void SpecialAttackFinal()
        {
            Debug.Log("Boss uses Final Phase special attack!");
            // TODO: Implement final phase special attack
        }

        /// <summary>
        /// Override Die to handle boss-specific death
        /// </summary>
        public override void Die()
        {
            if (isDefeated) return;

            isDefeated = true;

            Debug.Log($"BossController: '{bossConfig?.bossName ?? "Boss"}' defeated!");

            // Hide boss health bar
            if (UIManager.Instance != null)
            {
                UIManager.Instance.HideBossHealthBar();
            }

            // Award rewards
            if (bossConfig != null && GameManager.Instance != null)
            {
                GameManager.Instance.AddScore(1, bossConfig.scoreReward);
            }

            // Notify listeners
            OnBossDefeated?.Invoke();

            // Call base death
            base.Die();
        }

        // === Public API for Cinematics/Intro ===

        /// <summary>
        /// Starts the boss intro (called by cinematic or level flow)
        /// </summary>
        public void StartIntro()
        {
            isInIntro = true;
            Debug.Log("BossController: Intro started");

            // TODO: Play intro animation
        }

        /// <summary>
        /// Ends the boss intro and makes boss vulnerable
        /// </summary>
        public void EndIntro()
        {
            isInIntro = false;
            Debug.Log("BossController: Intro ended, boss is now active");

            // TODO: Transition to combat state
        }

        /// <summary>
        /// Gets the current phase
        /// </summary>
        public int GetCurrentPhase() => currentPhase;

        /// <summary>
        /// Gets the boss configuration
        /// </summary>
        public BossConfigSO GetBossConfig() => bossConfig;

        /// <summary>
        /// Checks if boss is in intro
        /// </summary>
        public bool IsInIntro() => isInIntro;

        /// <summary>
        /// Checks if boss is defeated
        /// </summary>
        public bool IsDefeated() => isDefeated;
    }
}
