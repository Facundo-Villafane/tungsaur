using UnityEngine;

public class StageSixBossFlow : MonoBehaviour
{
    private StageZone stageZone;
    private BossSpawner bossSpawner;

    private void Awake()
    {
        stageZone = GetComponent<StageZone>();
        bossSpawner = GetComponentInChildren<BossSpawner>(true);

        if (bossSpawner != null)
            Debug.Log($"[StageSixBossFlow] ✅ Spawner detectado automáticamente: {bossSpawner.name}");
        else
            Debug.LogWarning("[StageSixBossFlow] ⚠️ No se encontró spawner en hijos. Verificá jerarquía.");
    }

    private void OnEnable()
    {
        stageZone.OnStageStarted += OnStageStarted;
    }

    private void OnDisable()
    {
        stageZone.OnStageStarted -= OnStageStarted;
    }

    private void OnStageStarted()
    {
        if (bossSpawner == null)
        {
            Debug.LogError("[StageSixBossFlow] ❌ bossSpawner es null. No se puede iniciar el boss.");
            return;
        }

        Debug.Log("[StageSixBossFlow] Stage iniciado. Activando boss...");
        bossSpawner.StartSpawning();
    }
}
