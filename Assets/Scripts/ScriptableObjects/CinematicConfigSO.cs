using UnityEngine;
using UnityEngine.Playables;
using DialogueEditor;

namespace CDG.Data
{
    /// <summary>
    /// ScriptableObject that defines a cinematic/cutscene
    /// Can use Unity Timeline or simple dialogue sequences
    /// </summary>
    [CreateAssetMenu(fileName = "New Cinematic Config", menuName = "CDG/Cinematic Configuration")]
    public class CinematicConfigSO : ScriptableObject
    {
        [Header("Cinematic Info")]
        public string cinematicName = "Intro Cinematic";

        [Header("Playback Type")]
        [Tooltip("Use Timeline for complex cinematics, Dialogue for simple conversations")]
        public CinematicType cinematicType = CinematicType.Dialogue;

        [Header("Timeline (for CinematicType.Timeline)")]
        [Tooltip("Unity Timeline asset for complex cinematics")]
        public PlayableAsset timelineAsset;

        [Header("Dialogue (for CinematicType.Dialogue)")]
        [Tooltip("Dialogue conversation to play")]
        public NPCConversation dialogueConversation;

        [Header("Camera")]
        [Tooltip("Lock camera during cinematic")]
        public bool lockCamera = true;

        [Header("Skippable")]
        [Tooltip("Allow player to skip this cinematic")]
        public bool canSkip = true;

        [Tooltip("Key to skip (default: Space)")]
        public KeyCode skipKey = KeyCode.Space;

        [Header("Audio")]
        public AudioClip backgroundMusic;
    }

    public enum CinematicType
    {
        Timeline,    // Complex cinematics with camera movements, animations
        Dialogue,    // Simple dialogue-only cinematics
        Custom       // Custom implementation
    }
}
