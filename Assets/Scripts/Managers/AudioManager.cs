using UnityEngine;
using System.Collections;

namespace CDG.Managers
{
    /// <summary>
    /// Centralized audio manager for music and sound effects
    /// Handles volume control, crossfading, and audio state management
    /// </summary>
    public class AudioManager : MonoBehaviour
    {
        public static AudioManager Instance { get; private set; }

        [Header("Audio Sources")]
        [SerializeField] private AudioSource musicSource;
        [SerializeField] private AudioSource sfxSource;
        [SerializeField] private AudioSource ambienceSource;

        [Header("Volume Settings")]
        [SerializeField, Range(0f, 1f)] private float masterVolume = 1f;
        [SerializeField, Range(0f, 1f)] private float musicVolume = 0.7f;
        [SerializeField, Range(0f, 1f)] private float sfxVolume = 1f;
        [SerializeField, Range(0f, 1f)] private float ambienceVolume = 0.5f;

        [Header("Crossfade Settings")]
        [SerializeField] private float crossfadeDuration = 2f;

        private Coroutine musicFadeCoroutine;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
            DontDestroyOnLoad(gameObject);

            // Initialize audio sources
            InitializeAudioSources();
            UpdateVolumes();
        }

        /// <summary>
        /// Initializes audio sources if not assigned
        /// </summary>
        private void InitializeAudioSources()
        {
            if (musicSource == null)
            {
                GameObject musicObj = new GameObject("MusicSource");
                musicObj.transform.SetParent(transform);
                musicSource = musicObj.AddComponent<AudioSource>();
                musicSource.loop = true;
                musicSource.playOnAwake = false;
            }

            if (sfxSource == null)
            {
                GameObject sfxObj = new GameObject("SFXSource");
                sfxObj.transform.SetParent(transform);
                sfxSource = sfxObj.AddComponent<AudioSource>();
                sfxSource.loop = false;
                sfxSource.playOnAwake = false;
            }

            if (ambienceSource == null)
            {
                GameObject ambienceObj = new GameObject("AmbienceSource");
                ambienceObj.transform.SetParent(transform);
                ambienceSource = ambienceObj.AddComponent<AudioSource>();
                ambienceSource.loop = true;
                ambienceSource.playOnAwake = false;
            }
        }

        // === Music Control ===

        /// <summary>
        /// Plays music with optional crossfade
        /// </summary>
        public void PlayMusic(AudioClip clip, bool crossfade = true)
        {
            if (clip == null)
            {
                Debug.LogWarning("AudioManager: Cannot play null music clip");
                return;
            }

            if (musicSource.clip == clip && musicSource.isPlaying)
            {
                Debug.Log("AudioManager: Music already playing");
                return;
            }

            if (crossfade && musicSource.isPlaying)
            {
                if (musicFadeCoroutine != null)
                {
                    StopCoroutine(musicFadeCoroutine);
                }
                musicFadeCoroutine = StartCoroutine(CrossfadeMusicCoroutine(clip));
            }
            else
            {
                musicSource.clip = clip;
                musicSource.Play();
                Debug.Log($"AudioManager: Playing music '{clip.name}'");
            }
        }

        /// <summary>
        /// Stops music with optional fade out
        /// </summary>
        public void StopMusic(bool fade = true)
        {
            if (!musicSource.isPlaying) return;

            if (fade)
            {
                if (musicFadeCoroutine != null)
                {
                    StopCoroutine(musicFadeCoroutine);
                }
                musicFadeCoroutine = StartCoroutine(FadeOutMusicCoroutine());
            }
            else
            {
                musicSource.Stop();
            }
        }

        /// <summary>
        /// Pauses music
        /// </summary>
        public void PauseMusic()
        {
            if (musicSource.isPlaying)
            {
                musicSource.Pause();
            }
        }

        /// <summary>
        /// Resumes music
        /// </summary>
        public void ResumeMusic()
        {
            if (!musicSource.isPlaying && musicSource.clip != null)
            {
                musicSource.UnPause();
            }
        }

        /// <summary>
        /// Crossfades between current music and new music
        /// </summary>
        private IEnumerator CrossfadeMusicCoroutine(AudioClip newClip)
        {
            float elapsed = 0f;
            float startVolume = musicSource.volume;

            // Fade out current music
            while (elapsed < crossfadeDuration / 2f)
            {
                elapsed += Time.deltaTime;
                musicSource.volume = Mathf.Lerp(startVolume, 0f, elapsed / (crossfadeDuration / 2f));
                yield return null;
            }

            // Switch clip
            musicSource.Stop();
            musicSource.clip = newClip;
            musicSource.Play();

            // Fade in new music
            elapsed = 0f;
            while (elapsed < crossfadeDuration / 2f)
            {
                elapsed += Time.deltaTime;
                musicSource.volume = Mathf.Lerp(0f, musicVolume * masterVolume, elapsed / (crossfadeDuration / 2f));
                yield return null;
            }

            musicSource.volume = musicVolume * masterVolume;
            Debug.Log($"AudioManager: Crossfaded to '{newClip.name}'");
        }

        /// <summary>
        /// Fades out music and stops
        /// </summary>
        private IEnumerator FadeOutMusicCoroutine()
        {
            float elapsed = 0f;
            float startVolume = musicSource.volume;

            while (elapsed < crossfadeDuration)
            {
                elapsed += Time.deltaTime;
                musicSource.volume = Mathf.Lerp(startVolume, 0f, elapsed / crossfadeDuration);
                yield return null;
            }

            musicSource.Stop();
            musicSource.volume = musicVolume * masterVolume;
        }

        // === SFX Control ===

        /// <summary>
        /// Plays a sound effect
        /// </summary>
        public void PlaySFX(AudioClip clip, float volumeScale = 1f)
        {
            if (clip == null)
            {
                Debug.LogWarning("AudioManager: Cannot play null SFX clip");
                return;
            }

            sfxSource.PlayOneShot(clip, volumeScale * sfxVolume * masterVolume);
        }

        /// <summary>
        /// Plays a random sound effect from an array
        /// </summary>
        public void PlayRandomSFX(AudioClip[] clips, float volumeScale = 1f)
        {
            if (clips == null || clips.Length == 0)
            {
                Debug.LogWarning("AudioManager: No SFX clips provided");
                return;
            }

            AudioClip randomClip = clips[Random.Range(0, clips.Length)];
            PlaySFX(randomClip, volumeScale);
        }

        // === Ambience Control ===

        /// <summary>
        /// Plays ambience audio
        /// </summary>
        public void PlayAmbience(AudioClip clip)
        {
            if (clip == null)
            {
                Debug.LogWarning("AudioManager: Cannot play null ambience clip");
                return;
            }

            if (ambienceSource.clip == clip && ambienceSource.isPlaying)
            {
                return;
            }

            ambienceSource.clip = clip;
            ambienceSource.Play();
            Debug.Log($"AudioManager: Playing ambience '{clip.name}'");
        }

        /// <summary>
        /// Stops ambience audio
        /// </summary>
        public void StopAmbience()
        {
            if (ambienceSource.isPlaying)
            {
                ambienceSource.Stop();
            }
        }

        // === Volume Control ===

        /// <summary>
        /// Sets master volume
        /// </summary>
        public void SetMasterVolume(float volume)
        {
            masterVolume = Mathf.Clamp01(volume);
            UpdateVolumes();
        }

        /// <summary>
        /// Sets music volume
        /// </summary>
        public void SetMusicVolume(float volume)
        {
            musicVolume = Mathf.Clamp01(volume);
            UpdateVolumes();
        }

        /// <summary>
        /// Sets SFX volume
        /// </summary>
        public void SetSFXVolume(float volume)
        {
            sfxVolume = Mathf.Clamp01(volume);
            UpdateVolumes();
        }

        /// <summary>
        /// Sets ambience volume
        /// </summary>
        public void SetAmbienceVolume(float volume)
        {
            ambienceVolume = Mathf.Clamp01(volume);
            UpdateVolumes();
        }

        /// <summary>
        /// Updates all audio source volumes
        /// </summary>
        private void UpdateVolumes()
        {
            if (musicSource != null)
            {
                musicSource.volume = musicVolume * masterVolume;
            }

            if (ambienceSource != null)
            {
                ambienceSource.volume = ambienceVolume * masterVolume;
            }

            // Note: SFX volume is applied per-clip in PlaySFX
        }

        // === Public Getters ===

        public float GetMasterVolume() => masterVolume;
        public float GetMusicVolume() => musicVolume;
        public float GetSFXVolume() => sfxVolume;
        public float GetAmbienceVolume() => ambienceVolume;

        public bool IsMusicPlaying() => musicSource != null && musicSource.isPlaying;
        public AudioClip GetCurrentMusic() => musicSource != null ? musicSource.clip : null;
    }
}
