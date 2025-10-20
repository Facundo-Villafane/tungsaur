using UnityEngine;
using System;
using System.Collections;

public class EnemySingleSpawner : MonoBehaviour
{
    [Header("Enemy Settings")]
    public GameObject enemyPrefab;

    [Header("Patrol Zone")]
    public PatrolZone patrolZone;

    [Header("Stage Settings")]
    public StageZone stageZone;

    [Header("Spawn Settings")]
    [Tooltip("Cantidad total de enemigos que este spawner generará durante el stage")]
    public int totalEnemiesToSpawn = 5;

    [Tooltip("Máximo de enemigos vivos simultáneamente")]
    public int maxEnemiesAlive = 3;

    [Tooltip("Segundos entre intentos de spawn")]
    public float spawnInterval = 1f;

    private bool isSpawning = false;
    private int currentEnemiesAlive = 0;
    private int enemiesSpawned = 0;
    private Coroutine spawnCoroutine;
    private Action onEnemyDefeatedCallback;

    public int EnemiesToSpawn => totalEnemiesToSpawn;

    private IEnumerator Start()
    {
        Debug.Log($"[Spawner: {name}] Start → esperando StageManager...");

        // Timeout de 10 segundos para evitar loops infinitos
        float timeout = 10f;
        float elapsed = 0f;

        while (StageManager.Instance == null && elapsed < timeout)
        {
            yield return null;
            elapsed += Time.deltaTime;
        }

        if (StageManager.Instance == null)
        {
            Debug.LogError($"[Spawner: {name}] StageManager no se encontró después de {timeout} segundos. Deshabilitando spawner.");
            enabled = false;
            yield break;
        }

        Debug.Log($"[Spawner: {name}] StageManager encontrado.");

        if (stageZone != null)
        {
            stageZone.RegisterSpawner(this);
            Debug.Log($"[Spawner: {name}] Registrado en StageZone '{stageZone.name}'");
        }
        else
        {
            Debug.LogWarning($"[Spawner: {name}] No tiene StageZone asignado.");
        }

        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnGameStateChanged += OnGameStateChanged;
            Debug.Log($"[Spawner: {name}] Suscrito a GameStateChanged");
        }
        else
        {
            Debug.LogWarning($"[Spawner: {name}] GameManager.Instance es null al Start");
        }
    }

    private void OnDisable()
    {
        if (GameManager.Instance != null)
            GameManager.Instance.OnGameStateChanged -= OnGameStateChanged;
    }

    private void OnGameStateChanged(GameState state)
    {
        Debug.Log($"[Spawner: {name}] OnGameStateChanged → {state}");
        if (state != GameState.Playing)
            StopSpawning();
    }

    public void StartSpawning(Action onEnemyDefeated)
    {
        Debug.Log($"[Spawner: {name}] StartSpawning llamado.");
        if (isSpawning)
        {
            Debug.Log($"[Spawner: {name}] Ya estaba spawneando, se ignora StartSpawning.");
            return;
        }

        onEnemyDefeatedCallback = onEnemyDefeated;
        isSpawning = true;
        enemiesSpawned = 0;
        currentEnemiesAlive = 0;

        spawnCoroutine = StartCoroutine(SpawnLoop());
        Debug.Log($"[Spawner: {name}] SpawnLoop iniciado.");
    }

    public void StopSpawning()
    {
        Debug.Log($"[Spawner: {name}] StopSpawning llamado. isSpawning={isSpawning}");
        if (!isSpawning) return;

        isSpawning = false;
        if (spawnCoroutine != null)
        {
            StopCoroutine(spawnCoroutine);
            spawnCoroutine = null;
            Debug.Log($"[Spawner: {name}] SpawnLoop detenido.");
        }
    }

    private IEnumerator SpawnLoop()
    {
        int loopCount = 0;
        int maxLoops = 1000; // Seguridad contra loops infinitos

        while (isSpawning && loopCount < maxLoops)
        {
            loopCount++;

            // Pausa si el juego no está en estado Playing
            if (GameManager.Instance != null && !GameManager.Instance.IsPlaying())
            {
                yield return null;
                continue;
            }

            // Calcular cuántos enemigos se pueden spawnear ahora
            int canSpawnNow = Mathf.Min(
                maxEnemiesAlive - currentEnemiesAlive,
                totalEnemiesToSpawn - enemiesSpawned
            );

            Debug.Log($"[Spawner: {name}] Loop #{loopCount} - Intentando spawn {canSpawnNow} enemigos (Spawned={enemiesSpawned}/{totalEnemiesToSpawn}, Alive={currentEnemiesAlive}/{maxEnemiesAlive})");

            // Spawnear los enemigos necesarios
            for (int i = 0; i < canSpawnNow; i++)
            {
                SpawnEnemy();
            }

            // Condición de salida: todos los enemigos spawneados y ninguno vivo
            if (enemiesSpawned >= totalEnemiesToSpawn && currentEnemiesAlive <= 0)
            {
                Debug.Log($"[Spawner: {name}] Todos los enemigos generados y ninguno vivo. Terminando SpawnLoop.");
                isSpawning = false;
                StopSpawning();
                enabled = false;
                yield break;
            }

            // Esperar el intervalo antes del próximo ciclo
            yield return new WaitForSeconds(spawnInterval);
        }

        // Advertencia si se alcanzó el límite de loops
        if (loopCount >= maxLoops)
        {
            Debug.LogError($"[Spawner: {name}] SpawnLoop alcanzó el límite de {maxLoops} iteraciones. Deteniendo para evitar freeze.");
            isSpawning = false;
            StopSpawning();
            enabled = false;
        }
    }

    private void SpawnEnemy()
    {
        if (enemyPrefab == null)
        {
            Debug.LogError($"[Spawner: {name}] EnemyPrefab es null, no se puede instanciar enemigo.");
            return;
        }

        if (enemiesSpawned >= totalEnemiesToSpawn)
        {
            Debug.Log($"[Spawner: {name}] Ya se generaron todos los enemigos ({enemiesSpawned}/{totalEnemiesToSpawn}).");
            return;
        }

        GameObject enemy = Instantiate(enemyPrefab, transform.position, Quaternion.identity);
        enemiesSpawned++;
        currentEnemiesAlive++;

        Debug.Log($"[Spawner: {name}] Enemigo spawn #{enemiesSpawned}. Enemigos vivos: {currentEnemiesAlive}");

        EnemyController controller = enemy.GetComponent<EnemyController>();
        if (controller != null)
        {
            // Usar una referencia local para evitar problemas de closure
            var spawner = this;

            // Handler para el evento de muerte
            System.Action deathHandler = null;
            deathHandler = () =>
            {
                // Desuscribirse del evento para evitar memory leaks
                if (controller != null)
                {
                    controller.OnEnemyDeath -= deathHandler;
                }

                currentEnemiesAlive = Mathf.Max(0, currentEnemiesAlive - 1);
                Debug.Log($"[Spawner: {name}] Enemigo murió. Enemigos vivos: {currentEnemiesAlive}");
                onEnemyDefeatedCallback?.Invoke();
            };

            controller.OnEnemyDeath += deathHandler;

            // Configurar zona de patrullaje si existe
            // if (patrolZone != null)
            //     controller.SetPatrolZone(patrolZone.transform);
        }
    }

}
