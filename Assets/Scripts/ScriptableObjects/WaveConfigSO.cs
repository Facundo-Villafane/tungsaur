using UnityEngine;

namespace CDG.Data
{
    /// <summary>
    /// ScriptableObject that defines a single enemy wave
    /// Configures enemy types, count, and spawn behavior
    /// </summary>
    [CreateAssetMenu(fileName = "New Wave Config", menuName = "CDG/Wave Configuration")]
    public class WaveConfigSO : ScriptableObject
    {
        [Header("Wave Info")]
        public string waveName = "Wave 1";

        [Header("Enemy Configuration")]
        [Tooltip("Enemy prefab to spawn")]
        public GameObject enemyPrefab;

        [Tooltip("Total enemies to spawn in this wave")]
        public int totalEnemies = 5;

        [Tooltip("Maximum enemies alive at the same time")]
        public int maxSimultaneousEnemies = 3;

        [Header("Spawn Timing")]
        [Tooltip("Delay before first enemy spawns")]
        public float initialDelay = 0f;

        [Tooltip("Time between each enemy spawn")]
        public float spawnInterval = 2f;

        [Header("Spawn Positions")]
        [Tooltip("Use random spawn points or specific ones")]
        public bool useRandomSpawnPoints = true;

        [Tooltip("If not random, enemies will spawn at these positions")]
        public Vector3[] spawnPositions;
    }
}
