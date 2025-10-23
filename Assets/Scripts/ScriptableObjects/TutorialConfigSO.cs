using UnityEngine;

namespace CDG.Data
{
    /// <summary>
    /// ScriptableObject that defines a tutorial step
    /// Used to guide the player through game mechanics
    /// </summary>
    [CreateAssetMenu(fileName = "New Tutorial Config", menuName = "CDG/Tutorial Configuration")]
    public class TutorialConfigSO : ScriptableObject
    {
        [Header("Tutorial Info")]
        public string tutorialTitle = "Tutorial";

        [Header("Tutorial Steps")]
        [Tooltip("List of tutorial instructions in order")]
        [TextArea(2, 5)]
        public string[] tutorialSteps;

        [Header("Input Display")]
        [Tooltip("Show input prompts for each step")]
        public bool showInputPrompts = true;

        [Tooltip("Key bindings to display (e.g., 'WASD to Move', 'J to Punch')")]
        public string[] inputPrompts;

        [Header("Practice Wave")]
        [Tooltip("Optional practice wave for tutorial")]
        public WaveConfigSO practiceWave;

        [Header("Tutorial Settings")]
        [Tooltip("Player cannot die during tutorial")]
        public bool playerInvulnerable = true;

        [Tooltip("Auto-advance to next step or require player input")]
        public bool autoAdvance = false;

        [Tooltip("Key to advance tutorial manually")]
        public KeyCode advanceKey = KeyCode.Space;

        [Header("Completion")]
        [Tooltip("Cinematic to play after tutorial completes")]
        public CinematicConfigSO completionCinematic;
    }
}
