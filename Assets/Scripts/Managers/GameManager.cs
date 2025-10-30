using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;
using System;
using System.Collections;

public class GameManager : MonoBehaviour
{
    // ==================== SINGLETON ====================
    public static GameManager Instance { get; private set; }

    [Header("Game State")]
    [SerializeField] private GameState currentState = GameState.Playing;
    [SerializeField] private CameraState currentCameraState = CameraState.Locked;

    [Header("Scene Management")]
    [SerializeField] private SceneData[] scenes; // Array de escenas configurables
    [SerializeField] private int currentSceneIndex = 0;

    [Header("Player Stats")]
    [SerializeField] private int player1Score = 0;
    [SerializeField] private int player2Score = 0;

    // ==================== EVENTS ====================
    public event Action<GameState> OnGameStateChanged;
    public event Action<CameraState> OnCameraStateChanged;
    public event Action<int, int> OnLivesChanged;
    public event Action<int, int> OnScoreChanged;
    public event Action<string> OnSceneLoaded; // Nueva: cuando se carga una escena

    private void Awake()
    {
        // Singleton persistente
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        // Suscribirse al evento de carga de escenas
        SceneManager.sceneLoaded += OnSceneLoadedCallback;
    }

    private void OnDestroy()
    {
        // Desuscribirse para evitar memory leaks
        SceneManager.sceneLoaded -= OnSceneLoadedCallback;
    }

    private void Start()
    {
        InitializeGame();
    }

    private void Update()
    {
        var keyboard = Keyboard.current;

        if (keyboard != null && keyboard.pKey.wasPressedThisFrame)
        {
            TogglePause();
        }

        if (keyboard != null && keyboard.iKey.wasPressedThisFrame)
        {
            ToggleCameraState();
        }

        // Debug: Cargar siguiente nivel
        if (keyboard != null && keyboard.nKey.wasPressedThisFrame)
        {
            LoadNextScene();
        }
    }

    // ==================== INIT ====================

    private void InitializeGame()
    {
        ResetStats();
        ChangeGameState(GameState.Playing);
        Debug.Log("[GameManager] Juego inicializado.");
    }

    // ==================== SCENE MANAGEMENT ====================

    /// <summary>
    /// Callback que se ejecuta cuando se carga una nueva escena
    /// </summary>
    private void OnSceneLoadedCallback(Scene scene, LoadSceneMode mode)
    {
        Debug.Log($"[GameManager] ═══════════════════════════════════════");
        Debug.Log($"[GameManager] Escena cargada: {scene.name}");
        Debug.Log($"[GameManager] ═══════════════════════════════════════");

        // Esperar a que el StageManager cargue sus stages
        StartCoroutine(VerifySceneSetup());

        OnSceneLoaded?.Invoke(scene.name);
    }

    /// <summary>
    /// Verifica que la escena se haya cargado correctamente
    /// </summary>
    private IEnumerator VerifySceneSetup()
    {
        // Esperar 2 frames para que Start() se ejecute en todos los objetos
        yield return null;
        yield return null;

        if (StageManager.Instance != null)
        {
            if (StageManager.Instance.HasStages)
            {
                Debug.Log($"[GameManager] ✅ Escena con stages lista. Total: {StageManager.Instance.CurrentStageIndex}");
            }
            else
            {
                Debug.Log($"[GameManager] ℹ️ Escena sin stages (menú/cutscene)");
            }
        }
        else
        {
            Debug.Log("[GameManager] ℹ️ No hay StageManager en esta escena");
        }
    }

    /// <summary>
    /// Cargar escena por nombre
    /// </summary>
    public void LoadScene(string sceneName)
    {
        Debug.Log($"[GameManager] Cargando escena: {sceneName}");
        StartCoroutine(LoadSceneAsync(sceneName));
    }

    /// <summary>
    /// Cargar escena por índice del array
    /// </summary>
    public void LoadScene(int sceneIndex)
    {
        if (sceneIndex < 0 || sceneIndex >= scenes.Length)
        {
            Debug.LogError($"[GameManager] Índice de escena inválido: {sceneIndex}");
            return;
        }

        currentSceneIndex = sceneIndex;
        SceneData sceneData = scenes[sceneIndex];
        LoadScene(sceneData.sceneName);
    }

    /// <summary>
    /// Cargar la siguiente escena en el array
    /// </summary>
    public void LoadNextScene()
    {
        int nextIndex = currentSceneIndex + 1;

        if (nextIndex >= scenes.Length)
        {
            Debug.Log("[GameManager] No hay más escenas. Volviendo al menú.");
            LoadScene(0); // Volver al inicio
            return;
        }

        LoadScene(nextIndex);
    }

    /// <summary>
    /// Cargar escena de forma asíncrona (con loading screen si quieres)
    /// </summary>
    private IEnumerator LoadSceneAsync(string sceneName)
    {
        // Opcional: Mostrar pantalla de carga
        ChangeGameState(GameState.Loading);

        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName);

        // Esperar mientras carga
        while (!asyncLoad.isDone)
        {
            float progress = Mathf.Clamp01(asyncLoad.progress / 0.9f);
            Debug.Log($"[GameManager] Cargando escena: {progress * 100}%");
            yield return null;
        }

        Debug.Log($"[GameManager] Escena '{sceneName}' cargada completamente.");
        ChangeGameState(GameState.Playing);
    }

    /// <summary>
    /// Obtener información de la escena actual
    /// </summary>
    public SceneData GetCurrentSceneData()
    {
        if (currentSceneIndex >= 0 && currentSceneIndex < scenes.Length)
            return scenes[currentSceneIndex];

        return null;
    }

    /// <summary>
    /// Recargar la escena actual
    /// </summary>
    public void ReloadCurrentScene()
    {
        string currentScene = SceneManager.GetActiveScene().name;
        LoadScene(currentScene);
    }

    // ==================== GAME STATE ====================

    public void ChangeGameState(GameState newState)
    {
        if (currentState == newState) return;

        currentState = newState;
        OnGameStateChanged?.Invoke(newState);

        switch (newState)
        {
            case GameState.Menu:
            case GameState.Playing:
                Time.timeScale = 1f;
                break;
            case GameState.Paused:
            case GameState.GameOver:
            case GameState.Loading:
                Time.timeScale = 0f;
                break;
        }

        Debug.Log($"[GameManager] Estado de juego: {newState}");
    }

    public GameState GetCurrentState() => currentState;
    public bool IsPlaying() => currentState == GameState.Playing;

    // ==================== CAMERA STATE ====================

    private void ToggleCameraState()
    {
        ChangeCameraState(currentCameraState == CameraState.Locked
            ? CameraState.Free
            : CameraState.Locked);
    }

    public void ChangeCameraState(CameraState newState)
    {
        if (currentCameraState == newState) return;

        currentCameraState = newState;
        OnCameraStateChanged?.Invoke(newState);
        UpdateCameraState();
        Debug.Log($"[GameManager] Estado de cámara: {newState}");
    }

    private void UpdateCameraState()
    {
        var playerCam = Camera.main;

        if (playerCam == null)
        {
            Debug.LogWarning("[GameManager] No se encontró la cámara principal.");
            return;
        }

        switch (currentCameraState)
        {
            case CameraState.Locked:
                Debug.Log("Cámara bloqueada al jugador.");
                break;

            case CameraState.Free:
                Debug.Log("Cámara libre activada.");
                break;
        }
    }

    public CameraState GetCameraState() => currentCameraState;
    public bool CanCameraMove() => currentCameraState == CameraState.Free;

    // ==================== PLAYER STATS ====================

    public void AddScore(int player, int points)
    {
        if (player == 1)
            player1Score += points;
        else if (player == 2)
            player2Score += points;

        OnScoreChanged?.Invoke(player, GetScore(player));
    }

    public int GetScore(int player)
    {
        return player == 1 ? player1Score : player2Score;
    }

    // ==================== GAME FLOW ====================

    public void GameOver()
    {
        ChangeGameState(GameState.GameOver);
        Debug.Log("[GameManager] Game Over!");
    }

    public void RestartGame()
    {
        ResetStats();
        ReloadCurrentScene();
        ChangeGameState(GameState.Playing);
    }

    // ==================== PAUSE ====================

    public void PauseGame() => ChangeGameState(GameState.Paused);
    public void ResumeGame() => ChangeGameState(GameState.Playing);

    public void TogglePause()
    {
        if (currentState == GameState.Playing)
            PauseGame();
        else if (currentState == GameState.Paused)
            ResumeGame();
    }

    // ==================== RESET ====================

    private void ResetStats()
    {
        player1Score = 0;
        player2Score = 0;
    }
}

// ==================== SCENE DATA CLASS ====================

[System.Serializable]
public class SceneData
{
    public string sceneName;      // Nombre de la escena en Build Settings
    public string displayName;    // Nombre para mostrar ("Tutorial", "Nivel 1")
    public SceneType sceneType;   // Tipo de escena
    public bool hasStages;        // Si esta escena tiene stages

    public SceneData(string sceneName, string displayName, SceneType type, bool hasStages = false)
    {
        this.sceneName = sceneName;
        this.displayName = displayName;
        this.sceneType = type;
        this.hasStages = hasStages;
    }
}

// ==================== ENUMS ====================

public enum GameState
{
    Menu,
    Playing,
    Paused,
    GameOver,
    Loading // Nuevo estado
}

public enum CameraState
{
    Locked,
    Free
}

public enum SceneType
{
    Menu,
    Tutorial,
    Level,
    Boss,
    Cutscene
}