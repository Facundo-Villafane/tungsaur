using UnityEngine;
using System;
using System.Collections.Generic;

public class StageZone : MonoBehaviour
{
    [Header("Stage Settings")]
    [SerializeField] private List<MonoBehaviour> spawners = new List<MonoBehaviour>(); // Lista de MonoBehaviour que implementan IEnemySpawner
   

    public event Action OnStageCompleted;
    public event Action OnStageStarted;

    private int totalEnemies = 0;
    private int enemiesDefeated = 0;
    private bool stageActive = false;

    private void Start()
    {
        
        Debug.Log($"[StageZone: {name}] Awake → intentando registrar en StageManager.");
        if (StageManager.Instance != null)
        {
            StageManager.Instance.RegisterStage(this);
            Debug.Log($"[StageZone: {name}] Registrado correctamente en StageManager.");
        }
        else
        {
            Debug.LogWarning($"[StageZone: {name}] StageManager.Instance es null en Awake. (El manager podría estar inicializándose después)");
        }
       
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log($"[StageZone: {name}] OnTriggerEnter con {other.name}");
   

        if (other.CompareTag("Player"))
        {
            if (StageManager.Instance == null)
            {
                Debug.LogError($"[StageZone: {name}] StageManager.Instance es null al entrar el jugador.");
                return;
            }

            Debug.Log($"[StageZone: {name}] Jugador entró. Preparando Stage {StageManager.Instance.CurrentStageIndex}...");
            StageManager.Instance.PrepareStage(StageManager.Instance.CurrentStageIndex);

            Debug.Log($"[StageZone: {name}] Iniciando Stage...");
            StageManager.Instance.StartStage();
        }
    }

    /// <summary>
    /// Inicializa el stage (antes de empezar la oleada)
    /// </summary>
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

    /// <summary>
    /// Comienza la oleada/enemigos
    /// </summary>
    public void StartStage()
    {
        Debug.Log($"[StageZone: {name}] StartStage llamado. stageActive={stageActive}");
        if (stageActive)
        {
            Debug.Log($"[StageZone: {name}] Stage ya estaba activo, se ignora StartStage.");
            return;
        }
        stageActive = true;

        foreach (var spawnerObj in spawners)
        {
            if (spawnerObj == null)
            {
                Debug.LogWarning($"[StageZone: {name}] Spawner null en StartStage, se ignora.");
                continue;
            }

            IEnemySpawner spawner = spawnerObj as IEnemySpawner;
            if (spawner == null)
            {
                Debug.LogWarning($"[StageZone: {name}] El objeto '{spawnerObj.name}' no implementa IEnemySpawner.");
                continue;
            }

            Debug.Log($"[StageZone: {name}] Activando spawner '{spawnerObj.name}'");
            spawner.StartSpawning(OnEnemyDefeated);
        }

        Debug.Log($"[StageZone: {name}] Todos los spawners activados.");
        OnStageStarted?.Invoke();
    }

    /// <summary>
    /// Llamado por los spawners cada vez que un enemigo muere
    /// </summary>
    private void OnEnemyDefeated()
    {
        enemiesDefeated++;
        Debug.Log($"[StageZone: {name}] Enemigo derrotado. {enemiesDefeated}/{totalEnemies}");

        if (enemiesDefeated >= totalEnemies)
        {
            Debug.Log($"[StageZone: {name}] Todos los enemigos derrotados. Terminando stage...");
            EndStage();
        }
    }

    /// <summary>
    /// Termina el stage
    /// </summary>
    public void EndStage()
    {
        Debug.Log($"[StageZone: {name}] EndStage llamado. stageActive={stageActive}");
        stageActive = false;
        OnStageCompleted?.Invoke();

        if (StageManager.Instance != null)
        {
            StageManager.Instance.EndStage();
        }
        else
        {
            Debug.LogError($"[StageZone: {name}] StageManager.Instance es null en EndStage.");
        }
    }

    /// <summary>
    /// Permite registrar spawners por código si es necesario
    /// </summary>
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
            Debug.Log($"[StageZone: {name}] Spawner '{spawner.name}' registrado por código.");
        }
    }
}
