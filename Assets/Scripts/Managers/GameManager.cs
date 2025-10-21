using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;
using System;

public class GameManager : MonoBehaviour
{
    // ==================== SINGLETON ====================
    public static GameManager Instance { get; private set; }

    [Header("Game State")]
    [SerializeField] private GameState currentState = GameState.Playing;
    [SerializeField] private CameraState currentCameraState = CameraState.Locked;

    [Header("Player Stats")]
    [SerializeField] private int player1Score = 0;
    [SerializeField] private int player2Score = 0;

    // ==================== EVENTS ====================
    public event Action<GameState> OnGameStateChanged;
    public event Action<CameraState> OnCameraStateChanged;
    public event Action<int, int> OnLivesChanged; // player, newLives
    public event Action<int, int> OnScoreChanged; // player, newScore

    private void Awake()
    {
        // Singleton
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
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
    }

    // ==================== INIT ====================

    private void InitializeGame()
    {
        ResetStats();
        ChangeGameState(GameState.Playing);
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
        UpdateCameraState(); // 🔹 aplica el nuevo estado
        Debug.Log($"[GameManager] Estado de cámara: {newState}");
    }

    /// <summary>
    /// Lógica que aplica el cambio de cámara (por ejemplo, activar o desactivar scripts de control)
    /// </summary>
    private void UpdateCameraState()
    {
        // Ejemplo genérico — ajusta según tu setup
        var playerCam = Camera.main;

        if (playerCam == null)
        {
            Debug.LogWarning("[GameManager] No se encontró la cámara principal.");
            return;
        }

        switch (currentCameraState)
        {
            case CameraState.Locked:
                // Aquí podrías bloquear la rotación o movimiento de cámara
                // playerCam.GetComponent<FreeCameraController>()?.enabled = false;
                // playerCam.GetComponent<FollowPlayer>()?.enabled = true;
                Debug.Log("Cámara bloqueada al jugador.");
                break;

            case CameraState.Free:
                // Aquí podrías habilitar un modo libre
                // playerCam.GetComponent<FollowPlayer>()?.enabled = false;
                // playerCam.GetComponent<FreeCameraController>()?.enabled = true;
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
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        ChangeGameState(GameState.Playing);
    }

    public void LoadScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
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

// ==================== ENUMS ====================

public enum GameState
{
    Menu,
    Playing,
    Paused,
    GameOver
}

public enum CameraState
{
    Locked,
    Free
}
