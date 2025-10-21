using UnityEngine;
using System;
using System.Collections.Generic;

public class StageManager : MonoBehaviour
{
    public static StageManager Instance { get; private set; }

    [Header("Stage Info")]
    [SerializeField] private int currentLevel = 1;
    [SerializeField] private int currentStageIndex = 0; 
    [SerializeField] private StageState currentStageState = StageState.Locked;

    [Header("Stages")]
    [SerializeField] private List<StageZone> stages = new List<StageZone>();

    public event Action<StageState> OnStageStateChanged;
    public event Action<int, int> OnStageProgressed; 
    public event Action<StageZone> OnStageStarted;
    public event Action<StageZone> OnStageEnded;

    public int CurrentStageIndex => currentStageIndex; 

    public StageZone CurrentStage =>
        (currentStageIndex >= 0 && currentStageIndex < stages.Count) ? stages[currentStageIndex] : null;

    private void Awake()
    {
        Debug.Log($"[StageManager] Awake en {gameObject.name}");

        if (Instance != null && Instance != this)
        {
            Debug.LogWarning($"[StageManager] Ya existe una instancia ({Instance.gameObject.name}), destruyendo {gameObject.name}");
            Destroy(gameObject);
            return;
        }

        Instance = this;
        Debug.Log("[StageManager] Instancia establecida correctamente.");
    }

    private void Start()
    {
        Debug.Log("[StageManager] Start");
        if (stages.Count > 0)
        {
            Debug.Log($"[StageManager] Preparando Stage 0: {stages[0].name}");
            PrepareStage(0);
        }
        else
        {
            Debug.LogWarning("[StageManager] No hay stages asignados.");
        }
    }

    public void RegisterStage(StageZone stageZone)
    {
        if (stageZone == null) return;

        Debug.Log($"[StageManager] Registrando StageZone: {stageZone.name}");
        if (!stages.Contains(stageZone))
        {
            stages.Add(stageZone);
        }

        // 🔗 Suscribir evento al completarse el stage
        stageZone.OnStageCompleted += () => OnStageCompleted(stageZone);
    }

    public void PrepareStage(int index)
    {
        Debug.Log($"[StageManager] PrepareStage index={index}");
        if (index < 0 || index >= stages.Count)
        {
            Debug.LogWarning("[StageManager] Índice fuera de rango.");
            return;
        }

        currentStageIndex = index;
        ChangeStageState(StageState.Locked);

        StageZone stage = stages[index];
        Debug.Log($"[StageManager] Inicializando Stage: {stage.name}");
        stage.Initialize();
    }

    public void StartStage()
    {
        Debug.Log("[StageManager] StartStage llamado");
        if (CurrentStage == null)
        {
            Debug.LogError("[StageManager] CurrentStage es null en StartStage");
            return;
        }

        ChangeStageState(StageState.Active);
        Debug.Log($"[StageManager] Iniciando Stage: {CurrentStage.name}");
        CurrentStage.StartStage();
        OnStageStarted?.Invoke(CurrentStage);
    }

    /// <summary>
    /// 🔁 Ahora este método solo reacciona al fin del stage.
    /// Ya no llama a CurrentStage.EndStage() para evitar loops.
    /// </summary>
    private void OnStageCompleted(StageZone completedStage)
    {
        Debug.Log($"[StageManager] Stage completado: {completedStage.name}");

        ChangeStageState(StageState.Completed);
        OnStageEnded?.Invoke(completedStage);

        AdvanceStage();
    }

    private void AdvanceStage()
    {
        Debug.Log("[StageManager] AdvanceStage");
        currentStageIndex++;
        OnStageProgressed?.Invoke(currentLevel, currentStageIndex);

        if (currentStageIndex < stages.Count)
        {
            Debug.Log($"[StageManager] Avanzando a Stage {currentStageIndex}");
            PrepareStage(currentStageIndex);
        }
        else
        {
            Debug.Log("[StageManager] Nivel completado 🎉");
            // Aquí podrías avisar al GameManager para cargar el siguiente nivel.
        }
    }

    private void ChangeStageState(StageState newState)
    {
        if (currentStageState == newState) return;
        Debug.Log($"[StageManager] Cambiando estado: {currentStageState} → {newState}");
        currentStageState = newState;
        OnStageStateChanged?.Invoke(newState);
    }
}

public enum StageState
{
    Locked,
    Active,
    Completed
}
