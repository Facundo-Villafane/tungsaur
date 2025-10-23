using UnityEngine;

namespace CDG.Data
{
    /// <summary>
    /// ScriptableObject that defines a mini-boss configuration
    /// Similar to boss but simpler (no phases, shorter fights)
    /// </summary>
    [CreateAssetMenu(fileName = "New MiniBoss Config", menuName = "CDG/MiniBoss Configuration")]
    public class MiniBossConfigSO : ScriptableObject
    {
        [Header("MiniBoss Info")]
        public string miniBossName = "MiniBoss Name";
        public GameObject miniBossPrefab;

        [Header("Stats")]
        public float maxHealth = 200f;
        public float damage = 20f;
        public float defense = 5f;
        public float moveSpeed = 4f;

        [Header("Behavior")]
        [Tooltip("Time between special attacks")]
        public float specialAttackCooldown = 8f;

        [Header("Spawn")]
        [Tooltip("Show intro animation/effect when spawning")]
        public bool hasIntroAnimation = true;

        [Header("Rewards")]
        public int gloryReward = 50;
        public int scoreReward = 500;

        [Header("Audio")]
        public AudioClip spawnSound;
        public AudioClip defeatSound;
    }
}
