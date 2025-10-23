using UnityEngine;
using System.Collections.Generic;

namespace CDG.Data
{
    /// <summary>
    /// ScriptableObject that defines a single stage within a level
    /// Contains wave configurations and optional mini-boss
    /// </summary>
    [CreateAssetMenu(fileName = "New Stage Config", menuName = "CDG/Stage Configuration")]
    public class StageConfigSO : ScriptableObject
    {
        [Header("Stage Info")]
        public string stageName = "Stage 1";
        public int stageNumber = 1;

        [Header("Waves")]
        [Tooltip("List of enemy waves for this stage")]
        public List<WaveConfigSO> waves = new List<WaveConfigSO>();

        [Header("Mini-Boss (Optional)")]
        [Tooltip("If set, a mini-boss will spawn after all waves")]
        public MiniBossConfigSO miniBossConfig;

        [Header("Cinematics (Optional)")]
        [Tooltip("Play a short cinematic before this stage starts")]
        public CinematicConfigSO introCinematic;

        [Header("Stage Completion")]
        public int gloryReward = 50;
        public int scoreReward = 500;
    }
}
