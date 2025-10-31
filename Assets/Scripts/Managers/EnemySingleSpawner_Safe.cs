using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Versión más segura y simple del spawner que previene crashes y loops infinitos
/// </summary>
public class EnemySingleSpawner_Safe : MonoBehaviour, IEnemySpawner
{
    [Header("Enemy Settings")]
    public GameObject enemyPrefab;

    [Header("Stage Settings")]
    public StageZone stageZone;

    [Header("Spawn Settings")]
    [Tooltip("Cantidad total de enemigos que este spawner generará durante el stage")]
    public int totalEnemiesToSpawn = 5;

    [Tooltip("Máximo de enemigos vivos simultáneamente")]
    public int maxEnemiesAlive = 3;

    [Tooltip("Segundos entre intentos de spawn")]
    public float spawnInterval = 2f;

    // Estado interno
    private bool isActive = false;
    private int enemiesSpawned = 0;
    private List<EnemyController> activeEnemies = new List<EnemyController>();
    private Coroutine spawnCoroutine;
    private Action onAllEnemiesDefeatedCallback;

    public int EnemiesToSpawn => totalEnemiesToSpawn;

    private void Start()
    {
        StartCoroutine(Initialize());
    }

    private IEnumerator Initialize()
    {
        Debug.Log($"[SpawnerSafe: {name}] Inicializando...");

        // Esperar StageManager con timeout
        float timeout = 5f;
        float elapsed = 0f;

        while (StageManager.Instance == null && elapsed < timeout)
        {
            yield return null;
            elapsed += Time.deltaTime;
        }

        if (StageManager.Instance == null)
        {
            Debug.LogError($"[SpawnerSafe: {name}] StageManager no encontrado. Deshabilitando spawner.");
            enabled = false;
            yield break;
        }

        // Registrar en StageZone
        if (stageZone != null)
        {
            stageZone.RegisterSpawner(this);
            Debug.Log($"[SpawnerSafe: {name}] Registrado en StageZone '{stageZone.name}'");
        }

        Debug.Log($"[SpawnerSafe: {name}] Inicialización completada.");
    }

    public void StartSpawning(Action onAllEnemiesDefeated)
    {
        if (isActive)
        {
            Debug.LogWarning($"[SpawnerSafe: {name}] Ya está activo. Ignorando StartSpawning.");
            return;
        }

        Debug.Log($"[SpawnerSafe: {name}] Iniciando spawning...");
        onAllEnemiesDefeatedCallback = onAllEnemiesDefeated;
        isActive = true;
        enemiesSpawned = 0;
        activeEnemies.Clear();

        if (spawnCoroutine != null)
        {
            StopCoroutine(spawnCoroutine);
        }

        spawnCoroutine = StartCoroutine(SpawnRoutine());
    }

    public void StopSpawning()
    {
        Debug.Log($"[SpawnerSafe: {name}] Deteniendo spawning...");
        isActive = false;

        if (spawnCoroutine != null)
        {
            StopCoroutine(spawnCoroutine);
            spawnCoroutine = null;
        }
    }

    private IEnumerator SpawnRoutine()
    {
        while (isActive && enemiesSpawned < totalEnemiesToSpawn)
        {
            // Limpiar referencias nulas de la lista
            activeEnemies.RemoveAll(e => e == null || e.IsDead);

            // Verificar cuántos podemos spawnear
            int canSpawn = Mathf.Min(
                maxEnemiesAlive - activeEnemies.Count,
                totalEnemiesToSpawn - enemiesSpawned
            );

            if (canSpawn > 0)
            {
                for (int i = 0; i < canSpawn; i++)
                {
                    SpawnSingleEnemy();
                }
            }

            // Esperar intervalo
            yield return new WaitForSeconds(spawnInterval);
        }

        // Esperar a que todos los enemigos mueran
        Debug.Log($"[SpawnerSafe: {name}] Todos spawneados. Esperando que mueran...");

        while (activeEnemies.Count > 0)
        {
            activeEnemies.RemoveAll(e => e == null || e.IsDead);
            yield return new WaitForSeconds(0.5f);
        }

        // Notificar que terminó
        Debug.Log($"[SpawnerSafe: {name}] Todos los enemigos derrotados.");
       
        isActive = false;
        enabled = false;
    }

private void SpawnSingleEnemy()
{
    if (enemyPrefab == null || enemiesSpawned >= totalEnemiesToSpawn) return;

    GameObject enemyObj = Instantiate(enemyPrefab, transform.position, Quaternion.identity);
    EnemyController controller = enemyObj.GetComponent<EnemyController>();

    if (controller != null)
    {
        activeEnemies.Add(controller);
        enemiesSpawned++;

        Debug.Log($"[SpawnerSafe: {name}] Enemigo #{enemiesSpawned} spawneado. Activos: {activeEnemies.Count}");

        // ✅ Suscripción segura
        Action deathHandler = null;
        deathHandler = () =>
        {
            controller.OnEnemyDeath -= deathHandler; // desuscribirse inmediatamente
            OnEnemyDied(controller);
        };

        controller.OnEnemyDeath += deathHandler;
    }
    else
    {
        Destroy(enemyObj);
    }
}

private void OnEnemyDied(EnemyController enemy)
{
    if (enemy == null) return;

    // Limpiar de la lista de activos
    activeEnemies.Remove(enemy);
    Debug.Log($"[SpawnerSafe: {name}] Enemigo murió. Activos: {activeEnemies.Count}, Spawned: {enemiesSpawned}/{totalEnemiesToSpawn}");

    // Notificar al StageZone
    stageZone?.OnEnemyDefeated();

    // 🔹 NO usar activeEnemies.Count para disparar callback del spawner
}




private void OnDestroy()
{
    activeEnemies.Clear();
}

    private void OnDisable()
    {
        StopSpawning();
    }
}
