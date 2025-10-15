using UnityEngine;
using System.Collections;

public class EnemySingleSpawner : MonoBehaviour
{
    [Header("Enemy Settings")]
    public GameObject enemyPrefab;

    [Header("Patrol Zone")]
    public PatrolZone patrolZone; 

    [Header("Spawn Settings")]
    public int maxEnemiesAlive = 3;
    public float spawnInterval = 1f;

    private bool isSpawning = false;
    private int currentEnemiesAlive = 0;
    private Coroutine spawnCoroutine;

private IEnumerator Start()
{
    // Esperar a que GameManager exista
    yield return new WaitUntil(() => GameManager.Instance != null);

    // Esperar a que el juego esté en Playing
    yield return new WaitUntil(() => GameManager.Instance.IsPlaying());

    // Suscribirse al evento de cambio de estado
    GameManager.Instance.OnGameStateChanged += OnGameStateChanged;

    // Iniciar el spawn
    StartSpawning();
}

    private void OnDisable()
    {
        if (GameManager.Instance != null)
            GameManager.Instance.OnGameStateChanged -= OnGameStateChanged;
    }

    private void OnGameStateChanged(GameState newState)
    {
        if (newState == GameState.Playing)
            ResumeSpawning();
        else
            StopSpawning();
    }

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
            StartSpawning();
    }

   private IEnumerator SpawnLoop()
{
    while (isSpawning)
    {
        if (GameManager.Instance != null && GameManager.Instance.IsPlaying())
        {
            // Spawnea los enemigos que falten para llegar al máximo
            int enemiesToSpawn = maxEnemiesAlive - currentEnemiesAlive;
            for (int i = 0; i < enemiesToSpawn; i++)
            {
                SpawnEnemy();
            }
        }

        yield return new WaitForSeconds(spawnInterval);
    }
}


    private void SpawnEnemy()
    {
        if (enemyPrefab == null) return;

        GameObject enemy = Instantiate(enemyPrefab, transform.position, Quaternion.identity);
        currentEnemiesAlive++;

        EnemyController controller = enemy.GetComponent<EnemyController>();
        if (controller != null)
        {
            controller.OnEnemyDeath += EnemyDied;

            // if (patrolZone != null)
            // {
            //     controller.SetPatrolZone(patrolZone.transform);
            // }

        }
    }

    private void EnemyDied()
    {
        currentEnemiesAlive = Mathf.Max(0, currentEnemiesAlive - 1);
    }
}
