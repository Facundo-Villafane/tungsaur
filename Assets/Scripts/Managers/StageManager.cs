using UnityEngine;
using System;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class StageManager : MonoBehaviour
{
    public static StageManager Instance { get; private set; }

    [Header("Stage Info")]
    [SerializeField] private int currentLevel = 1;
    [SerializeField] private int currentStageIndex = 0;
    [SerializeField] private StageState currentStageState = StageState.Locked;

    [Header("Stages (Auto-populated)")]
    [SerializeField] private List<StageZone> stages = new List<StageZone>();

    private Transform stageZonesParent;
    private bool hasStages = false;

    public event Action<StageState> OnStageStateChanged;
    public event Action<int, int> OnStageProgressed;
    public event Action<StageZone> OnStageStarted;
    public event Action<StageZone> OnStageEnded;

    public int CurrentStageIndex => currentStageIndex;
    public bool HasStages => hasStages;
    private bool isWaitingForSceneChange = false;
    private float sceneChangeTimer = 0f;

    public StageZone CurrentStage =>
        (currentStageIndex >= 0 && currentStageIndex < stages.Count) ? stages[currentStageIndex] : null;



    private void Awake()
    {
        Debug.Log($"[StageManager] Awake en {gameObject.name}");

        // ⚠️ Importante: NO usar DontDestroyOnLoad en StageManager
        // Cada escena tiene su propio StageManager

        if (Instance != null && Instance != this)
        {
            Debug.LogWarning($"[StageManager] Ya existe una instancia ({Instance.gameObject.name}), destruyendo {gameObject.name}");
            Destroy(gameObject);
            return;
        }

        Instance = this;
        Debug.Log("[StageManager] Instancia establecida correctamente.");

        // ✅ Cargar stages automáticamente al iniciar
        LoadStagesInOrder();
    }

    private void OnDestroy()
    {
        // Limpiar la instancia al destruir (cuando cambia de escena)
        if (Instance == this)
        {
            Instance = null;
            Debug.Log("[StageManager] Instancia limpiada al cambiar de escena.");
        }
    }

    /// <summary>
    /// ✅ Busca automáticamente el GameObject "StageZones" en la escena
    /// </summary>
    private void FindStageZonesParent()
    {
        // Opción 1: Buscar por nombre (más simple y confiable)
        GameObject stageZonesObject = GameObject.Find("StageZones");

        if (stageZonesObject != null)
        {
            stageZonesParent = stageZonesObject.transform;
            Debug.Log($"[StageManager] StageZones encontrado automáticamente: {stageZonesParent.name}");
            return;
        }

        // Opción 2: Buscar hijo directo de este objeto
        stageZonesParent = transform.Find("StageZones");

        if (stageZonesParent != null)
        {
            Debug.Log($"[StageManager] StageZones encontrado como hijo: {stageZonesParent.name}");
            return;
        }

        // Opción 3: Buscar por tag (si tienes tag "StageZones")
        try
        {
            stageZonesObject = GameObject.FindGameObjectWithTag("StageZones");

            if (stageZonesObject != null)
            {
                stageZonesParent = stageZonesObject.transform;
                Debug.Log($"[StageManager] StageZones encontrado por tag: {stageZonesParent.name}");
                return;
            }
        }
        catch (UnityException)
        {
            // Tag no existe, continuar sin error
        }

        Debug.LogWarning("[StageManager] No se encontró GameObject 'StageZones' en la escena. (Esto es normal para escenas sin stages como menús)");
    }

    /// <summary>
    /// ✅ Carga los stages en el orden exacto de la jerarquía de Unity
    /// </summary>
    private void LoadStagesInOrder()
    {
        stages.Clear();
        hasStages = false;

        // Buscar automáticamente el contenedor
        FindStageZonesParent();

        if (stageZonesParent == null)
        {
            Debug.Log("[StageManager] Esta escena no tiene StageZones.");
            NotifyGameManagerNoStages();
            return;
        }

        Debug.Log($"[StageManager] Cargando stages desde: {stageZonesParent.name}");

        // Recorrer hijos en orden de jerarquía
        for (int i = 0; i < stageZonesParent.childCount; i++)
        {
            Transform child = stageZonesParent.GetChild(i);

            // Ignorar hijos inactivos
            if (!child.gameObject.activeInHierarchy)
            {
                Debug.Log($"[StageManager] Ignorando hijo inactivo: {child.name}");
                continue;
            }

            StageZone stageZone = child.GetComponent<StageZone>();

            if (stageZone != null)
            {
                stages.Add(stageZone);

                // Suscribir evento al completarse el stage
                stageZone.OnStageCompleted += () => OnStageCompleted(stageZone);

                Debug.Log($"[StageManager] Stage {stages.Count - 1} cargado: {stageZone.name}");
            }
            else
            {
                Debug.LogWarning($"[StageManager] El hijo '{child.name}' no tiene componente StageZone.");
            }
        }

        hasStages = stages.Count > 0;

        if (hasStages)
        {
            Debug.Log($"[StageManager] ✅ Total de stages cargados: {stages.Count}");
            NotifyGameManagerStagesLoaded();
        }
        else
        {
            Debug.LogWarning("[StageManager] No se encontraron StageZones válidos en el contenedor.");
            NotifyGameManagerNoStages();
        }
    }

    /// <summary>
    /// Notifica al GameManager que los stages fueron cargados exitosamente
    /// </summary>
    private void NotifyGameManagerStagesLoaded()
    {
        if (GameManager.Instance != null)
        {
            Debug.Log($"[StageManager] Notificando a GameManager: {stages.Count} stages cargados.");
            // Aquí puedes agregar un evento si GameManager necesita saberlo
        }
    }

    /// <summary>
    /// Notifica al GameManager que esta escena no tiene stages
    /// </summary>
    private void NotifyGameManagerNoStages()
    {
        if (GameManager.Instance != null)
        {
            Debug.Log("[StageManager] Notificando a GameManager: Esta escena no tiene stages.");
            // Útil para escenas de menú, cutscenes, etc.
        }
    }

    private void Start()
    {
        Debug.Log("[StageManager] Start");

        if (!hasStages)
        {
            Debug.Log("[StageManager] Esta escena no tiene stages, StageManager en modo pasivo.");
            return;
        }

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






    /// <summary>
    /// ⚠️ DEPRECADO: Este método ya no debe usarse.
    /// Los stages se cargan automáticamente en Awake() mediante LoadStagesInOrder()
    /// </summary>
    [Obsolete("RegisterStage ya no es necesario. Los stages se cargan automáticamente desde la jerarquía.")]
    public void RegisterStage(StageZone stageZone)
    {
        Debug.LogWarning($"[StageManager] RegisterStage() está deprecado. El stage {stageZone.name} se carga automáticamente.");
    }

    public void PrepareStage(int index)
    {
        if (!hasStages)
        {
            Debug.LogWarning("[StageManager] No se puede preparar stage, esta escena no tiene stages.");
            return;
        }

        Debug.Log($"[StageManager] PrepareStage index={index}");
        if (index < 0 || index >= stages.Count)
        {
            Debug.LogWarning($"[StageManager] Índice fuera de rango: {index} (total: {stages.Count})");
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
        if (!hasStages)
        {
            Debug.LogWarning("[StageManager] No se puede iniciar stage, esta escena no tiene stages.");
            return;
        }
        Debug.Log($"[StageManager] Llamando StartStage en: {CurrentStage?.name}, estado activo: {CurrentStage?.stageActive}");


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
            Debug.Log("[StageManager] ¡Todos los stages completados! 🎉");
            OnLevelCompleted();
        }
    }

    /// <summary>
    /// Se llama cuando todos los stages de la escena están completados
    /// </summary>
    private void OnLevelCompleted()
    {
        Debug.Log("[StageManager] Nivel completado. Notificando al GameManager...");

        // Notificar al GameManager para que cargue la siguiente escena
        if (GameManager.Instance != null)
        {
            // Puedes agregar un delay antes de cargar la siguiente escena
            StartCoroutine(LoadNextSceneAfterDelay(3f));
        }
        else
        {
            Debug.LogWarning("[StageManager] GameManager.Instance es null.");
        }
    }

    /// <summary>
    /// Espera unos segundos antes de cargar la siguiente escena
    /// </summary>
    private System.Collections.IEnumerator LoadNextSceneAfterDelay(float delay)
    {
        Debug.Log($"[StageManager] Cargando siguiente escena en {delay} segundos...");
        yield return new WaitForSeconds(delay);

        LoadNextScene();
    }

    private void ChangeStageState(StageState newState)
    {
        if (currentStageState == newState) return;
        Debug.Log($"[StageManager] Cambiando estado: {currentStageState} → {newState}");
        currentStageState = newState;
        OnStageStateChanged?.Invoke(newState);
    }

    // ✅ Método helper para debugging en el Inspector
    [ContextMenu("Debug: Mostrar Orden de Stages")]
    private void DebugStageOrder()
    {
        if (!hasStages || stages.Count == 0)
        {
            Debug.Log("=== NO HAY STAGES EN ESTA ESCENA ===");
            return;
        }

        Debug.Log("=== ORDEN DE STAGES ===");
        for (int i = 0; i < stages.Count; i++)
        {
            Debug.Log($"Stage {i}: {stages[i].name}");
        }
        Debug.Log($"Total: {stages.Count} stages");
    }

    /// <summary>
    /// Método público para recargar stages manualmente (útil para testing)
    /// </summary>
    [ContextMenu("Debug: Recargar Stages")]
    public void ReloadStages()
    {
        Debug.Log("[StageManager] Recargando stages manualmente...");
        LoadStagesInOrder();

        if (hasStages && stages.Count > 0)
        {
            PrepareStage(0);
        }
    }
    public void LoadNextScene()
    {
        int currentIndex = SceneManager.GetActiveScene().buildIndex;
        int nextIndex = currentIndex + 1;

        if (nextIndex < SceneManager.sceneCountInBuildSettings)
        {
            SceneManager.LoadScene(nextIndex);
            Debug.Log("Go to EndGame");
        }
        else
        {
            Debug.LogWarning("No hay más escenas en el Build Settings.");
        }
    }
}

public enum StageState
{
    Locked,
    Active,
    Completed
}