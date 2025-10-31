using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class BossSpawner : MonoBehaviour, IEnemySpawner
{
    [Header("Boss Settings")]
    public GameObject bossPrefab;

    [Header("Stage Settings")]
    public StageZone stageZone;

    [Header("Spawn Settings")]
    [Tooltip("Cantidad total de bosses que este spawner generará durante el stage")]
    public int totalBossesToSpawn = 1;

    [Tooltip("Máximo de bosses vivos simultáneamente")]
    public int maxBossesAlive = 1;

    [Tooltip("Segundos entre intentos de spawn")]
    public float spawnInterval = 2f;

    // Estado interno
    private bool isActive = false;
    private int bossesSpawned = 0;
    private List<BossController> activeBosses = new List<BossController>();
    private Coroutine spawnCoroutine;
    private Action onBossDefeatedCallback;
    public int EnemiesToSpawn => totalBossesToSpawn;


    public int BossesToSpawn => totalBossesToSpawn;

    private void Start()
    {
        StartCoroutine(Initialize());
    }

    private IEnumerator Initialize()
    {
        Debug.Log($"[BossSpawner: {name}] Inicializando...");

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

        if (stageZone != null)
        {
            stageZone.RegisterSpawner(this);
            Debug.Log($"[BossSpawner: {name}] Registrado en StageZone '{stageZone.name}'");
        }

        Debug.Log($"[BossSpawner: {name}] Inicialización completada.");
    }

    public void StartSpawning(Action onBossDefeated = null)
    {
        if (isActive)
        {
            Debug.LogWarning($"[BossSpawner: {name}] Ya está activo. Ignorando StartSpawning.");
            return;
        }

        Debug.Log($"[BossSpawner: {name}] Iniciando aparición del Boss...");
        isActive = true;
        bossesSpawned = 0;
        activeBosses.Clear();
        onBossDefeatedCallback = onBossDefeated;

        if (spawnCoroutine != null)
            StopCoroutine(spawnCoroutine);

        spawnCoroutine = StartCoroutine(SpawnRoutine());
    }

    public void StopSpawning()
    {
        Debug.Log($"[BossSpawner: {name}] Deteniendo aparición del Boss...");
        isActive = false;

        if (spawnCoroutine != null)
        {
            StopCoroutine(spawnCoroutine);
            spawnCoroutine = null;
        }
    }

    private IEnumerator SpawnRoutine()
    {
        while (isActive && bossesSpawned < totalBossesToSpawn)
        {
            activeBosses.RemoveAll(b => b == null || b.IsDead);

            int canSpawn = Mathf.Min(
                maxBossesAlive - activeBosses.Count,
                totalBossesToSpawn - bossesSpawned
            );

            if (canSpawn > 0)
            {
                for (int i = 0; i < canSpawn; i++)
                    SpawnSingleBoss();
            }

            yield return new WaitForSeconds(spawnInterval);
        }

        Debug.Log($"[BossSpawner: {name}] Todos los bosses instanciados. Esperando que sean derrotados...");

        while (activeBosses.Exists(b => b != null && !b.IsDead))
        {
            activeBosses.RemoveAll(b => b == null || b.IsDead);
            yield return new WaitForSeconds(0.5f);
        }

        Debug.Log($"[BossSpawner: {name}] ✅ Boss derrotado.");

        stageZone?.OnEnemyDefeated(); 

        onBossDefeatedCallback?.Invoke();

        isActive = false;
        enabled = false;
    }

    private void SpawnSingleBoss()
    {
        if (bossPrefab == null)
        {
            Debug.LogError($"[BossSpawner: {name}] BossPrefab es null!");
            return;
        }

        if (bossesSpawned >= totalBossesToSpawn)
        {
            Debug.LogWarning($"[BossSpawner: {name}] Ya se instanciaron todos los bosses.");
            return;
        }

        GameObject bossObj = Instantiate(bossPrefab, transform.position, Quaternion.identity);

        if (!bossObj.activeSelf)
        {
            Debug.LogWarning($"[BossSpawner: {name}] El Boss instanciado estaba inactivo. Activando...");
            bossObj.SetActive(true);
        }

        BossController controller = bossObj.GetComponent<BossController>();

        if (controller != null)
        {
            ValidarBoss(controller);
            activeBosses.Add(controller);
            bossesSpawned++;
            Debug.Log($"[BossSpawner: {name}] Boss #{bossesSpawned} instanciado. Activos: {activeBosses.Count}");
        }
        else
        {
            Debug.LogError($"[BossSpawner: {name}] El prefab no tiene BossController!");
            Destroy(bossObj);
        }
    }

    private void ValidarBoss(BossController boss)
    {
        if (boss.AudioSource == null)
        {
            Debug.LogWarning($"[BossSpawner: {name}] Boss sin AudioSource. Se asigna automáticamente.");
            AudioSource source = boss.GetComponent<AudioSource>();
            if (source == null) source = boss.gameObject.AddComponent<AudioSource>();
            boss.AudioSource.enabled = true;
        }

        if (!boss.AudioSource.enabled || !boss.AudioSource.gameObject.activeInHierarchy)
        {
            Debug.LogWarning($"[BossSpawner: {name}] AudioSource desactivado o inactivo. Se activa.");
            boss.AudioSource.enabled = true;
            boss.AudioSource.gameObject.SetActive(true);
        }

        Vector3 pos = boss.transform.position;
        if (pos.y < -10f || pos.y > 50f)
        {
            Debug.LogWarning($"[BossSpawner: {name}] Boss fuera de rango vertical. Reubicando...");
            boss.transform.position = new Vector3(pos.x, Mathf.Clamp(pos.y, 0f, 10f), pos.z);
        }

        Renderer renderer = boss.GetComponent<Renderer>();
        if (renderer == null)
        {
            Debug.LogWarning($"[BossSpawner: {name}] Boss sin Renderer. ¿Está oculto?");
        }
    }

    private void OnDisable()
    {
        StopSpawning();
    }

    private void OnDestroy()
    {
        activeBosses.Clear();
    }
}
