using UnityEngine;
using System;
using System.Collections;
using CDG.Data;

namespace CDG.Managers
{
    /// <summary>
    /// Controls the complete flow of a level
    /// Sequence: Intro Cinematic -> Stages (with waves/mini-bosses) -> Boss -> Outro Cinematic
    /// </summary>
    public class LevelFlowManager : MonoBehaviour
    {
        public static LevelFlowManager Instance { get; private set; }

        [Header("Level Configuration")]
        [SerializeField] private LevelConfigSO levelConfig;

        [Header("Current State")]
        [SerializeField] private LevelFlowState currentState = LevelFlowState.Idle;
        private int currentStageIndex = 0;

        [Header("Debug")]
        [SerializeField] private bool skipCinematics = false;

        // Events
        public event Action<LevelFlowState> OnStateChanged;
        public event Action<int> OnStageStarted;
        public event Action<int> OnStageCompleted;
        public event Action OnBossStarted;
        public event Action OnLevelCompleted;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
        }

        private void Start()
        {
            if (levelConfig == null)
            {
                Debug.LogError("LevelFlowManager: No LevelConfigSO assigned!");
                return;
            }

            // Start the level flow
            StartCoroutine(RunLevelFlow());
        }

        /// <summary>
        /// Main coroutine that runs the entire level sequence
        /// </summary>
        private IEnumerator RunLevelFlow()
        {
            // 1. Intro Cinematic
            if (levelConfig.introCinematic != null && !skipCinematics)
            {
                ChangeState(LevelFlowState.IntroCinematic);
                yield return StartCoroutine(PlayCinematic(levelConfig.introCinematic));
            }

            // 2. Play Stages
            for (int i = 0; i < levelConfig.stages.Count; i++)
            {
                currentStageIndex = i;
                yield return StartCoroutine(PlayStage(levelConfig.stages[i], i));
            }

            // 3. Boss Fight
            if (levelConfig.bossConfig != null)
            {
                yield return StartCoroutine(PlayBossFight(levelConfig.bossConfig));
            }

            // 4. Outro Cinematic
            if (levelConfig.outroCinematic != null && !skipCinematics)
            {
                ChangeState(LevelFlowState.OutroCinematic);
                yield return StartCoroutine(PlayCinematic(levelConfig.outroCinematic));
            }

            // 5. Level Complete
            CompleteLeve();
        }

        /// <summary>
        /// Plays a single stage with its waves and optional mini-boss
        /// </summary>
        private IEnumerator PlayStage(StageConfigSO stageConfig, int stageIndex)
        {
            Debug.Log($"Starting Stage {stageIndex + 1}: {stageConfig.stageName}");

            // Optional stage intro cinematic
            if (stageConfig.introCinematic != null && !skipCinematics)
            {
                yield return StartCoroutine(PlayCinematic(stageConfig.introCinematic));
            }

            ChangeState(LevelFlowState.Stage);
            OnStageStarted?.Invoke(stageIndex);

            // Play all waves in this stage
            for (int i = 0; i < stageConfig.waves.Count; i++)
            {
                WaveConfigSO wave = stageConfig.waves[i];
                Debug.Log($"Starting Wave {i + 1} in Stage {stageIndex + 1}");

                yield return StartCoroutine(PlayWave(wave));
            }

            // Optional mini-boss
            if (stageConfig.miniBossConfig != null)
            {
                Debug.Log($"Mini-Boss spawning in Stage {stageIndex + 1}");
                yield return StartCoroutine(PlayMiniBoss(stageConfig.miniBossConfig));
            }

            // Stage completed
            OnStageCompleted?.Invoke(stageIndex);
            Debug.Log($"Stage {stageIndex + 1} completed!");

            // Award rewards
            if (GameManager.Instance != null)
            {
                GameManager.Instance.AddScore(1, stageConfig.scoreReward);
            }

            yield return new WaitForSeconds(2f); // Brief pause between stages
        }

        /// <summary>
        /// Spawns and manages a wave of enemies
        /// </summary>
        private IEnumerator PlayWave(WaveConfigSO waveConfig)
        {
            // Wait for initial delay
            yield return new WaitForSeconds(waveConfig.initialDelay);

            // TODO: Integrate with WaveManager or existing spawner
            // For now, this is a placeholder that waits for wave duration
            Debug.Log($"Wave '{waveConfig.waveName}' started - {waveConfig.totalEnemies} enemies");

            // Simulate wave duration (replace with actual spawn system)
            float waveDuration = waveConfig.totalEnemies * waveConfig.spawnInterval;
            yield return new WaitForSeconds(waveDuration);

            Debug.Log($"Wave '{waveConfig.waveName}' completed");
        }

        /// <summary>
        /// Spawns and waits for mini-boss defeat
        /// </summary>
        private IEnumerator PlayMiniBoss(MiniBossConfigSO miniBossConfig)
        {
            ChangeState(LevelFlowState.MiniBoss);

            // TODO: Spawn mini-boss using miniBossConfig
            Debug.Log($"Mini-Boss '{miniBossConfig.miniBossName}' spawned!");

            // Placeholder: wait for defeat (replace with actual boss controller)
            yield return new WaitForSeconds(10f);

            Debug.Log($"Mini-Boss '{miniBossConfig.miniBossName}' defeated!");

            // Award rewards
            if (GameManager.Instance != null)
            {
                GameManager.Instance.AddScore(1, miniBossConfig.scoreReward);
            }
        }

        /// <summary>
        /// Boss fight sequence
        /// </summary>
        private IEnumerator PlayBossFight(BossConfigSO bossConfig)
        {
            Debug.Log($"Boss Fight: {bossConfig.bossName}");

            // Boss intro cinematic
            if (bossConfig.introCinematic != null && !skipCinematics)
            {
                yield return StartCoroutine(PlayCinematic(bossConfig.introCinematic));
            }

            ChangeState(LevelFlowState.Boss);
            OnBossStarted?.Invoke();

            // TODO: Spawn boss using BossController
            Debug.Log($"Boss '{bossConfig.bossName}' spawned!");

            // Placeholder: wait for boss defeat (replace with actual boss controller)
            yield return new WaitForSeconds(20f);

            Debug.Log($"Boss '{bossConfig.bossName}' defeated!");

            // Boss defeat cinematic
            if (bossConfig.defeatCinematic != null && !skipCinematics)
            {
                yield return StartCoroutine(PlayCinematic(bossConfig.defeatCinematic));
            }

            // Award rewards
            if (GameManager.Instance != null)
            {
                GameManager.Instance.AddScore(1, bossConfig.scoreReward);
            }
        }

        /// <summary>
        /// Plays a cinematic (Timeline or Dialogue)
        /// </summary>
        private IEnumerator PlayCinematic(CinematicConfigSO cinematicConfig)
        {
            Debug.Log($"Playing cinematic: {cinematicConfig.cinematicName}");

            // Lock camera if needed
            if (cinematicConfig.lockCamera && GameManager.Instance != null)
            {
                GameManager.Instance.SetCameraState(CameraState.Locked);
            }

            // TODO: Integrate with CinematicsManager
            switch (cinematicConfig.cinematicType)
            {
                case CinematicType.Timeline:
                    // Play Unity Timeline
                    Debug.Log("Playing Timeline cinematic (placeholder)");
                    yield return new WaitForSeconds(5f); // Placeholder
                    break;

                case CinematicType.Dialogue:
                    // Play dialogue using existing DialogueEditor
                    if (cinematicConfig.dialogueConversation != null)
                    {
                        Debug.Log("Playing Dialogue cinematic");
                        // TODO: Trigger ConversationManager
                        yield return new WaitForSeconds(3f); // Placeholder
                    }
                    break;

                case CinematicType.Custom:
                    Debug.Log("Custom cinematic (placeholder)");
                    yield return new WaitForSeconds(3f);
                    break;
            }

            Debug.Log($"Cinematic '{cinematicConfig.cinematicName}' finished");
        }

        /// <summary>
        /// Called when the entire level is completed
        /// </summary>
        private void CompleteLeve()
        {
            ChangeState(LevelFlowState.Completed);
            OnLevelCompleted?.Invoke();

            Debug.Log($"Level '{levelConfig.levelName}' completed!");

            // Award level rewards
            if (GameManager.Instance != null)
            {
                GameManager.Instance.AddScore(1, levelConfig.scoreReward);
            }

            // TODO: Transition to next level or victory screen
        }

        /// <summary>
        /// Changes the current flow state and notifies listeners
        /// </summary>
        private void ChangeState(LevelFlowState newState)
        {
            if (currentState != newState)
            {
                currentState = newState;
                OnStateChanged?.Invoke(currentState);
                Debug.Log($"LevelFlowState changed to: {currentState}");
            }
        }

        // Public API
        public LevelFlowState GetCurrentState() => currentState;
        public int GetCurrentStageIndex() => currentStageIndex;
        public LevelConfigSO GetLevelConfig() => levelConfig;
    }

    /// <summary>
    /// States for the level flow sequence
    /// </summary>
    public enum LevelFlowState
    {
        Idle,
        IntroCinematic,
        Stage,
        MiniBoss,
        Boss,
        OutroCinematic,
        Completed
    }
}
