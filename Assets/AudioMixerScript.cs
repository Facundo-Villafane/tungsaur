using UnityEngine;
using UnityEngine.Audio;

public class AudioControl : MonoBehaviour
{
    [SerializeField] private AudioMixer mixer;

    public void SetFXVolume(float value)
    {
        mixer.SetFloat("FXVolume", Mathf.Log10(Mathf.Clamp(value, 0.001f, 1f)) * 20f);
    }

    public void SetMusicVolume(float value)
    {
        mixer.SetFloat("MusicVolume", Mathf.Log10(Mathf.Clamp(value, 0.001f, 1f)) * 20f);
    }
}
