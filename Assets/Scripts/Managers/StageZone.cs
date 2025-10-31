using UnityEngine;
using System;
using System.Collections.Generic;

public class StageZone : MonoBehaviour
{
    [Header("Stage Settings")]
    [SerializeField] private List<MonoBehaviour> spawners = new List<MonoBehaviour>();

    public event Action OnStageCompleted;
    public event Action OnStageStarted;

    [SerializeField] private int totalEnemies = 0;
    [SerializeField] private int enemiesDefeated = 0;
    public bool stageActive = false;

    // ❌ ELIMINADO: Ya no se registra en Start()
    // El StageManager carga todos los stages automáticamente desde la jerarquía

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log($"[StageZone: {name}] OnTriggerEnter con {other.name}");

        if (!other.CompareTag("Player"))
            return;

        if (StageManager.Instance == null)
        {
            Debug.LogError($"[StageZone: {name}] StageManager.Instance es null al entrar el jugador.");
            return;
        }

        // Evita reiniciar el stage si ya está activo
        if (stageActive)
        {
            Debug.Log($"[StageZone: {name}] Stage ya activo, no se reinicia.");
            return;
        }

        Debug.Log($"[StageZone: {name}] Jugador entró. Iniciando Stage...");
        StageManager.Instance.StartStage();
        Collider trigger = GetComponent<Collider>();
        trigger.enabled = false; // Desactiva el trigger para evitar múltiples activaciones
    }

    public void Initialize()
    {
        Debug.Log($"[StageZone: {name}] Initialize llamado.");
        stageActive = false;
        enemiesDefeated = 0;
        totalEnemies = 0;

        foreach (var spawnerObj in spawners)
        {
            if (spawnerObj == null)
            {
                Debug.LogWarning($"[StageZone: {name}] Hay un spawner null en la lista.");
                continue;
            }

            IEnemySpawner spawner = spawnerObj as IEnemySpawner;
            if (spawner == null)
            {
                Debug.LogWarning($"[StageZone: {name}] El objeto '{spawnerObj.name}' no implementa IEnemySpawner.");
                continue;
            }

            Debug.Log($"[StageZone: {name}] Contando spawner '{spawnerObj.name}' con {spawner.EnemiesToSpawn} enemigos.");
            totalEnemies += spawner.EnemiesToSpawn;
        }

        Debug.Log($"[StageZone: {name}] Total de enemigos esperados: {totalEnemies}");
    }

    public void StartStage()
    {
        if (GameManager.Instance.GetCameraState() != CameraState.Locked)
        {
            GameManager.Instance.ChangeCameraState(CameraState.Locked);
        }
       
        if (stageActive)
        {
            Debug.Log($"[StageZone: {name}] StartStage ignorado (ya activo).");
            return;
        }

        Debug.Log($"[StageZone: {name}] Iniciando Stage...");
        stageActive = true;

        foreach (var spawnerObj in spawners)
        {
            if (spawnerObj == null) continue;

            IEnemySpawner spawner = spawnerObj as IEnemySpawner;
            if (spawner == null)
            {
                Debug.LogWarning($"[StageZone: {name}] '{spawnerObj.name}' no implementa IEnemySpawner.");
                continue;
            }

            Debug.Log($"[StageZone: {name}] Activando spawner '{spawnerObj.name}'...");
            spawner.StartSpawning(OnEnemyDefeated);
        }

        OnStageStarted?.Invoke();
    }

    public void OnEnemyDefeated()
    {
        enemiesDefeated++;
        Debug.Log($"[StageZone: {name}] Enemigo derrotado. {enemiesDefeated}/{totalEnemies}");

        if (enemiesDefeated >= totalEnemies)
        {
            Debug.Log($"[StageZone: {name}] Todos los enemigos derrotados.");
            EndStage();
        }
    }

    public void EndStage()
    {
        if (!stageActive)
        {
            Debug.Log($"[StageZone: {name}] EndStage ignorado (ya inactivo).");
            return;
        }

        Debug.Log($"[StageZone: {name}] EndStage → marcando stage como completado.");
        stageActive = false;
        OnStageCompleted?.Invoke();

        if (GameManager.Instance.GetCameraState() != CameraState.Free)
        {
            GameManager.Instance.ChangeCameraState(CameraState.Free);
        }
    }

    public void RegisterSpawner(MonoBehaviour spawner)
    {
        if (spawner == null)
        {
            Debug.LogWarning($"[StageZone: {name}] Intentando registrar spawner null.");
            return;
        }

        if (!(spawner is IEnemySpawner))
        {
            Debug.LogWarning($"[StageZone: {name}] El objeto '{spawner.name}' no implementa IEnemySpawner.");
            return;
        }

        if (!spawners.Contains(spawner))
        {
            spawners.Add(spawner);
            Debug.Log($"[StageZone: {name}] Spawner '{spawner.name}' registrado.");
        }
    }
}