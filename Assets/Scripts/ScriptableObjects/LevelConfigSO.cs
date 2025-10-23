using UnityEngine;
using System.Collections.Generic;

namespace CDG.Data
{
    /// <summary>
    /// ScriptableObject that defines a complete level configuration
    /// Includes cinematics, stages, waves, and boss data
    /// </summary>
    [CreateAssetMenu(fileName = "New Level Config", menuName = "CDG/Level Configuration")]
    public class LevelConfigSO : ScriptableObject
    {
        [Header("Level Info")]
        public string levelName = "Level 1";
        public int levelNumber = 1;

        [Header("Cinematics")]
        [Tooltip("Cinematic to play before the level starts")]
        public CinematicConfigSO introCinematic;

        [Tooltip("Cinematic to play after completing all stages")]
        public CinematicConfigSO outroCinematic;

        [Header("Stages")]
        [Tooltip("List of stages in this level (e.g., Level 1 has 3 stages)")]
        public List<StageConfigSO> stages = new List<StageConfigSO>();

        [Header("Boss")]
        [Tooltip("Boss configuration for this level")]
        public BossConfigSO bossConfig;

        [Header("Audio")]
        public AudioClip levelMusic;
        public AudioClip bossMusic;

        [Header("Rewards")]
        public int gloryReward = 100;
        public int scoreReward = 1000;
    }
}
