using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Versión segura del BossSpawner: controla el spawn, 
/// evita crashes y detecta muertes usando CharacterBase.IsDead.
/// </summary>
public class BossSpawner : MonoBehaviour, IEnemySpawner
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
    private List<BossController> activeEnemies = new List<BossController>();
    private Coroutine spawnCoroutine;

    public int EnemiesToSpawn => totalEnemiesToSpawn;

    private void Start()
    {
        StartCoroutine(Initialize());
    }

    private IEnumerator Initialize()
    {
        Debug.Log($"[BossSpawner: {name}] Inicializando...");

        // Esperar a que el StageManager exista
        float timeout = 5f;
        float elapsed = 0f;

        while (StageManager.Instance == null && elapsed < timeout)
        {
            yield return null;
            elapsed += Time.deltaTime;
        }

        if (StageManager.Instance == null)
        {
            Debug.LogError($"[BossSpawner: {name}] StageManager no encontrado. Deshabilitando spawner.");
            enabled = false;
            yield break;
        }

        // Registrar en StageZone
        if (stageZone != null)
        {
            stageZone.RegisterSpawner(this);
            Debug.Log($"[BossSpawner: {name}] Registrado en StageZone '{stageZone.name}'");
        }

        Debug.Log($"[BossSpawner: {name}] Inicialización completada.");
    }

    public void StartSpawning(Action onAllEnemiesDefeated = null)
    {
        if (isActive)
        {
            Debug.LogWarning($"[BossSpawner: {name}] Ya está activo. Ignorando StartSpawning.");
            return;
        }

        Debug.Log($"[BossSpawner: {name}] Iniciando spawning...");
        isActive = true;
        enemiesSpawned = 0;
        activeEnemies.Clear();

        if (spawnCoroutine != null)
            StopCoroutine(spawnCoroutine);

        spawnCoroutine = StartCoroutine(SpawnRoutine());
    }

    public void StopSpawning()
    {
        Debug.Log($"[BossSpawner: {name}] Deteniendo spawning...");
        isActive = false;

        if (spawnCoroutine != null)
        {
            StopCoroutine(spawnCoroutine);
            spawnCoroutine = null;
        }
    }

    private IEnumerator SpawnRoutine()
    {
        // Etapa 1: Spawnear enemigos hasta llegar al total
        while (isActive && enemiesSpawned < totalEnemiesToSpawn)
        {
            // Limpiar referencias nulas o muertas
            activeEnemies.RemoveAll(e => e == null || e.IsDead);

            // Verificar cuántos podemos spawnear
            int canSpawn = Mathf.Min(
                maxEnemiesAlive - activeEnemies.Count,
                totalEnemiesToSpawn - enemiesSpawned
            );

            if (canSpawn > 0)
            {
                for (int i = 0; i < canSpawn; i++)
                    SpawnSingleEnemy();
            }

            yield return new WaitForSeconds(spawnInterval);
        }

        Debug.Log($"[BossSpawner: {name}] Todos spawneados. Esperando que mueran...");

        // Etapa 2: Esperar hasta que no quede ninguno vivo
        while (activeEnemies.Exists(e => e != null && !e.IsDead))
        {
            activeEnemies.RemoveAll(e => e == null || e.IsDead);
            yield return new WaitForSeconds(0.5f);
        }

        // Etapa 3: Todos muertos
        Debug.Log($"[BossSpawner: {name}] ✅ Todos los enemigos derrotados.");
        stageZone?.OnEnemyDefeated(); // Notifica al stage si es necesario

        isActive = false;
        enabled = false;
    }

    private void SpawnSingleEnemy()
    {
        if (enemyPrefab == null)
        {
            Debug.LogError($"[BossSpawner: {name}] EnemyPrefab es null!");
            return;
        }

        if (enemiesSpawned >= totalEnemiesToSpawn)
        {
            Debug.LogWarning($"[BossSpawner: {name}] Ya se spawnearon todos los enemigos.");
            return;
        }

        // Instanciar enemigo
        GameObject enemyObj = Instantiate(enemyPrefab, transform.position, Quaternion.identity);
        BossController controller = enemyObj.GetComponent<BossController>();

        if (controller != null)
        {
            activeEnemies.Add(controller);
            enemiesSpawned++;
            Debug.Log($"[BossSpawner: {name}] Enemigo #{enemiesSpawned} spawneado. Activos: {activeEnemies.Count}");
        }
        else
        {
            Debug.LogError($"[BossSpawner: {name}] El prefab no tiene BossController!");
            Destroy(enemyObj);
        }
    }

    private void OnDisable()
    {
        StopSpawning();
    }

    private void OnDestroy()
    {
        activeEnemies.Clear();
    }
}
