using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;
using System;

public class GameManager : MonoBehaviour
{
    // ==================== SINGLETON ====================
    public static GameManager Instance { get; private set; }

    [Header("Game State")]
    [SerializeField] private GameState currentState = GameState.Menu;
    [SerializeField] private CameraState currentCameraState = CameraState.Locked;

    [Header("Player Stats")]
    [SerializeField] private int player1Lives = 3;
    [SerializeField] private int player2Lives = 3;
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

    private void InitializeGame()
    {
        ResetStats();
        ChangeGameState(GameState.Menu);
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
        Debug.Log($"[GameManager] Estado de cámara: {newState}");
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

    public void RemoveLife(int player)
    {
        if (player == 1)
        {
            player1Lives--;
            OnLivesChanged?.Invoke(1, player1Lives);
            if (player1Lives <= 0) GameOver();
        }
        else if (player == 2)
        {
            player2Lives--;
            OnLivesChanged?.Invoke(2, player2Lives);
            if (player2Lives <= 0) GameOver();
        }
    }

    public int GetLives(int player)
    {
        return player == 1 ? player1Lives : player2Lives;
    }

    // ==================== GAME FLOW ====================

    public void GameOver()
    {
        ChangeGameState(GameState.GameOver);
        Debug.Log("[GameManager] Game Over!");
        // Podés mostrar pantalla de fin, reinicio, etc.
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
        player1Lives = 3;
        player2Lives = 3;
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
