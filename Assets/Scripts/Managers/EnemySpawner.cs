using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EnemySpawner : MonoBehaviour
{
    [Header("Enemy Prefabs")]
    [SerializeField] private List<GameObject> enemyPrefabs = new List<GameObject>();

    [Header("Spawn Settings")]
    [SerializeField] private Transform[] spawnPoints;
    [SerializeField] private float spawnInterval = 3f;
    [SerializeField] private int maxEnemiesAlive = 3;

    [Header("Patrol Zone")]
    [SerializeField] private PatrolZone patrolZone; 

    [Header("Debug")]
    [SerializeField] private bool autoStart = true;
    [SerializeField] private bool isSpawning = false;

    private int currentEnemiesAlive = 0;
    private Coroutine spawnCoroutine;

    private void Start()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnGameStateChanged += OnGameStateChanged;
        }
        else
        {
            Debug.LogWarning("EnemySpawner: GameManager.Instance es null en Start()");
        }

        if (autoStart)
        {
            StartSpawning();
        }
    }

    private void OnDisable()
    {
        if (GameManager.Instance != null)
            GameManager.Instance.OnGameStateChanged -= OnGameStateChanged;
    }

    private void OnGameStateChanged(GameState newState)
    {
        switch (newState)
        {
            case GameState.Playing:
                ResumeSpawning();
                break;
            case GameState.Paused:
            case GameState.GameOver:
                StopSpawning();
                break;
        }
    }

    // ==================== PUBLIC API ====================

    public void StartSpawning()
    {
        if (isSpawning) return;

        isSpawning = true;
        spawnCoroutine = StartCoroutine(SpawnLoop());
    }

    public void StopSpawning()
    {
        if (!isSpawning) return;

        isSpawning = false;
        if (spawnCoroutine != null)
        {
            StopCoroutine(spawnCoroutine);
            spawnCoroutine = null;
        }
    }

    public void ResumeSpawning()
    {
        if (!isSpawning)
        {
            StartSpawning();
        }
    }

    public void EnemyDied()
    {
        currentEnemiesAlive = Mathf.Max(0, currentEnemiesAlive - 1);
    }

    // ==================== SPAWN LOGIC ====================

    private IEnumerator SpawnLoop()
    {
        while (isSpawning)
        {
            if (currentEnemiesAlive < maxEnemiesAlive && GameManager.Instance != null && GameManager.Instance.IsPlaying())
            {
                SpawnEnemy();
            }
            yield return new WaitForSeconds(spawnInterval);
        }
    }

    private void SpawnEnemy()
    {
        if (enemyPrefabs.Count == 0 || spawnPoints.Length == 0) return;

        GameObject prefab = enemyPrefabs[Random.Range(0, enemyPrefabs.Count)];
        Transform spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];

        GameObject enemy = Instantiate(prefab, spawnPoint.position, spawnPoint.rotation);
        currentEnemiesAlive++;

        EnemyController controller = enemy.GetComponent<EnemyController>();
        if (controller != null)
        {
            controller.OnEnemyDeath += EnemyDied;

            // Asignar la PatrolZone al enemigo
            if (patrolZone != null)
            {
                controller.SetPatrolZone(patrolZone.transform);
            }
        }
    }

    // ==================== GIZMOS ====================

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        if (spawnPoints != null)
        {
            foreach (var point in spawnPoints)
            {
                if (point != null)
                    Gizmos.DrawWireSphere(point.position, 0.5f);
            }
        }
    }
}
