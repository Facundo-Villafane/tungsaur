using UnityEngine;

namespace CDG.Data
{
    /// <summary>
    /// ScriptableObject that defines a boss configuration
    /// Includes stats, phases, and special attacks
    /// </summary>
    [CreateAssetMenu(fileName = "New Boss Config", menuName = "CDG/Boss Configuration")]
    public class BossConfigSO : ScriptableObject
    {
        [Header("Boss Info")]
        public string bossName = "Boss Name";
        public GameObject bossPrefab;

        [Header("Stats")]
        public float maxHealth = 500f;
        public float damage = 30f;
        public float defense = 10f;
        public float moveSpeed = 3f;

        [Header("Phases")]
        [Tooltip("Health thresholds for phase transitions (e.g., 0.75, 0.5, 0.25 for 3 phases)")]
        public float[] phaseThresholds = new float[] { 0.75f, 0.5f, 0.25f };

        [Tooltip("Phase names for UI display")]
        public string[] phaseNames = new string[] { "Phase 1", "Phase 2", "Phase 3", "Final Phase" };

        [Header("Special Attacks")]
        [Tooltip("Cooldown between special attacks")]
        public float specialAttackCooldown = 10f;

        [Header("Cinematics")]
        [Tooltip("Cinematic to play when boss appears")]
        public CinematicConfigSO introCinematic;

        [Tooltip("Cinematic to play when boss is defeated")]
        public CinematicConfigSO defeatCinematic;

        [Header("Rewards")]
        public int gloryReward = 200;
        public int scoreReward = 2000;

        [Header("Audio")]
        public AudioClip bossMusic;
        public AudioClip bossDefeatedSound;
    }
}
