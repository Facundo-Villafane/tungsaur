using UnityEngine;
using System;

public class StageManager : MonoBehaviour
{
    public static StageManager Instance { get; private set; }

    [Header("Stage Info")]
    [SerializeField] private int currentLevel = 1;
    [SerializeField] private int currentStage = 1;
    [SerializeField] private StageState currentStageState = StageState.Locked;

    public event Action<StageState> OnStageStateChanged;
    public event Action<int, int> OnStageProgressed; // level, stage

    private void Awake() {
        if (Instance != null && Instance != this) {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private void Start() {
        StartStage();
    }

    public void StartStage() {
        ChangeStageState(StageState.Free);
        // Podrías iniciar spawners acá
    }

    public void EndStage() {
        ChangeStageState(StageState.Locked);
        AdvanceStage();
        // Avisar al GameManager si querés cambiar de escena, etc.
        GameManager.Instance.ChangeGameState(GameState.Menu); 
    }

    public void ChangeStageState(StageState newState) {
        if (currentStageState == newState) return;
        currentStageState = newState;
        OnStageStateChanged?.Invoke(newState);
    }

    private void AdvanceStage() {
        currentStage++;
        OnStageProgressed?.Invoke(currentLevel, currentStage);
    }

    public bool IsStageFree() => currentStageState == StageState.Free;


}

    public enum StageState
{
    Locked,
    Free
}