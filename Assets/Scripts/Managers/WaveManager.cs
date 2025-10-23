using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using CDG.Data;

namespace CDG.Managers
{
    /// <summary>
    /// Advanced wave spawner that supports mini-bosses and uses WaveConfigSO
    /// Replaces/extends EnemySingleSpawner_Safe with more features
    /// </summary>
    public class WaveManager : MonoBehaviour
    {
        [Header("Wave Configuration")]
        [SerializeField] private List<WaveConfigSO> waves = new List<WaveConfigSO>();

        [Header("Spawn Points")]
        [SerializeField] private Transform[] spawnPoints;
        [SerializeField] private float spawnRadius = 2f;

        [Header("State")]
        [SerializeField] private bool autoStart = false;
        private int currentWaveIndex = 0;
        private bool isSpawning = false;
        private List<GameObject> activeEnemies = new List<GameObject>();

        // Events
        public event Action<int> OnWaveStarted;
        public event Action<int> OnWaveCompleted;
        public event Action OnAllWavesCompleted;
        public event Action<GameObject> OnEnemySpawned;
        public event Action<GameObject> OnEnemyDefeated;

        private void Start()
        {
            if (autoStart && waves.Count > 0)
            {
                StartWaves();
            }
        }

        /// <summary>
        /// Starts spawning all configured waves
        /// </summary>
        public void StartWaves()
        {
            if (waves.Count == 0)
            {
                Debug.LogWarning("WaveManager: No waves configured!");
                return;
            }

            if (isSpawning)
            {
                Debug.LogWarning("WaveManager: Already spawning waves!");
                return;
            }

            StartCoroutine(SpawnWavesCoroutine());
        }

        /// <summary>
        /// Main coroutine that spawns all waves sequentially
        /// </summary>
        private IEnumerator SpawnWavesCoroutine()
        {
            isSpawning = true;
            currentWaveIndex = 0;

            for (int i = 0; i < waves.Count; i++)
            {
                currentWaveIndex = i;
                WaveConfigSO wave = waves[i];

                Debug.Log($"WaveManager: Starting Wave {i + 1}/{waves.Count} - {wave.waveName}");

                yield return StartCoroutine(SpawnWaveCoroutine(wave, i));

                Debug.Log($"WaveManager: Wave {i + 1} completed!");
            }

            isSpawning = false;
            OnAllWavesCompleted?.Invoke();
            Debug.Log("WaveManager: All waves completed!");
        }

        /// <summary>
        /// Spawns a single wave based on WaveConfigSO
        /// </summary>
        private IEnumerator SpawnWaveCoroutine(WaveConfigSO waveConfig, int waveIndex)
        {
            OnWaveStarted?.Invoke(waveIndex);

            // Initial delay
            yield return new WaitForSeconds(waveConfig.initialDelay);

            int enemiesSpawned = 0;
            int totalEnemies = waveConfig.totalEnemies;

            while (enemiesSpawned < totalEnemies)
            {
                // Check if we can spawn more enemies
                if (GetActiveEnemyCount() < waveConfig.maxSimultaneousEnemies)
                {
                    SpawnEnemy(waveConfig, enemiesSpawned);
                    enemiesSpawned++;

                    // Wait before spawning next enemy
                    yield return new WaitForSeconds(waveConfig.spawnInterval);
                }
                else
                {
                    // Wait a bit before checking again
                    yield return new WaitForSeconds(0.5f);
                }
            }

            // Wait for all enemies to be defeated
            while (GetActiveEnemyCount() > 0)
            {
                yield return new WaitForSeconds(0.5f);
            }

            OnWaveCompleted?.Invoke(waveIndex);
        }

        /// <summary>
        /// Spawns a single enemy
        /// </summary>
        private void SpawnEnemy(WaveConfigSO waveConfig, int enemyIndex)
        {
            if (waveConfig.enemyPrefab == null)
            {
                Debug.LogError("WaveManager: Enemy prefab not assigned in wave config!");
                return;
            }

            // Determine spawn position
            Vector3 spawnPosition = GetSpawnPosition(waveConfig, enemyIndex);

            // Instantiate enemy
            GameObject enemy = Instantiate(waveConfig.enemyPrefab, spawnPosition, Quaternion.identity);
            activeEnemies.Add(enemy);

            // Subscribe to enemy death
            EnemyController enemyController = enemy.GetComponent<EnemyController>();
            if (enemyController != null)
            {
                enemyController.OnEnemyDeath += () => HandleEnemyDeath(enemy);
            }
            else
            {
                Debug.LogWarning("WaveManager: Spawned enemy does not have EnemyController!");
            }

            OnEnemySpawned?.Invoke(enemy);

            Debug.Log($"WaveManager: Spawned enemy at {spawnPosition}");
        }

        /// <summary>
        /// Gets the spawn position for an enemy
        /// </summary>
        private Vector3 GetSpawnPosition(WaveConfigSO waveConfig, int enemyIndex)
        {
            if (waveConfig.useRandomSpawnPoints)
            {
                // Random spawn from available spawn points
                if (spawnPoints != null && spawnPoints.Length > 0)
                {
                    Transform randomSpawn = spawnPoints[UnityEngine.Random.Range(0, spawnPoints.Length)];
                    return randomSpawn.position + GetRandomOffset();
                }
                else
                {
                    Debug.LogWarning("WaveManager: No spawn points assigned, using default position");
                    return transform.position + GetRandomOffset();
                }
            }
            else
            {
                // Use predefined positions from config
                if (waveConfig.spawnPositions != null && waveConfig.spawnPositions.Length > 0)
                {
                    int posIndex = enemyIndex % waveConfig.spawnPositions.Length;
                    return waveConfig.spawnPositions[posIndex];
                }
                else
                {
                    Debug.LogWarning("WaveManager: No spawn positions in config, using random");
                    return GetSpawnPosition(waveConfig, enemyIndex);
                }
            }
        }

        /// <summary>
        /// Gets a random offset for spawn position variation
        /// </summary>
        private Vector3 GetRandomOffset()
        {
            return new Vector3(
                UnityEngine.Random.Range(-spawnRadius, spawnRadius),
                0f,
                UnityEngine.Random.Range(-spawnRadius, spawnRadius)
            );
        }

        /// <summary>
        /// Handles enemy death event
        /// </summary>
        private void HandleEnemyDeath(GameObject enemy)
        {
            if (activeEnemies.Contains(enemy))
            {
                activeEnemies.Remove(enemy);
                OnEnemyDefeated?.Invoke(enemy);
                Debug.Log($"WaveManager: Enemy defeated. {GetActiveEnemyCount()} enemies remaining.");
            }
        }

        /// <summary>
        /// Gets the number of active enemies
        /// </summary>
        private int GetActiveEnemyCount()
        {
            // Clean up null references
            activeEnemies.RemoveAll(enemy => enemy == null);
            return activeEnemies.Count;
        }

        /// <summary>
        /// Stops all wave spawning
        /// </summary>
        public void StopWaves()
        {
            StopAllCoroutines();
            isSpawning = false;
            Debug.Log("WaveManager: Stopped all waves");
        }

        /// <summary>
        /// Clears all active enemies
        /// </summary>
        public void ClearAllEnemies()
        {
            foreach (GameObject enemy in activeEnemies)
            {
                if (enemy != null)
                {
                    Destroy(enemy);
                }
            }
            activeEnemies.Clear();
            Debug.Log("WaveManager: Cleared all enemies");
        }

        // Public API
        public int GetCurrentWaveIndex() => currentWaveIndex;
        public int GetTotalWaves() => waves.Count;
        public bool IsSpawning() => isSpawning;
        public int GetActiveEnemiesCount() => GetActiveEnemyCount();

        // Gizmos for spawn points visualization
        private void OnDrawGizmos()
        {
            if (spawnPoints == null || spawnPoints.Length == 0) return;

            Gizmos.color = Color.red;
            foreach (Transform spawnPoint in spawnPoints)
            {
                if (spawnPoint != null)
                {
                    Gizmos.DrawWireSphere(spawnPoint.position, spawnRadius);
                    Gizmos.DrawLine(spawnPoint.position, spawnPoint.position + Vector3.up * 2f);
                }
            }
        }
    }
}
