using System;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [SerializeField] private AudioSource audioSource;

    [Header("Ataques de espada")]
    public AudioClip AtaqueEspada1;
    public AudioClip AtaqueEspada2;
    public AudioClip Desenvaine1;
    public AudioClip MissAttack;

    [Header("Daño recibido")]
    public AudioClip DañoEnemigo1;
    public AudioClip DañoEnemigo2;
    public AudioClip DañoPlayer;
    public AudioClip EnemigoMuriendo;
    public AudioClip RisaMalvada;

    [Header("Movimiento del personaje")]
    public AudioClip Salto1;
    public AudioClip Salto2;
    public AudioClip Salto3;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            Debug.Log("AudioManager inicializado correctamente.");
        }
        else
        {
            Debug.LogWarning("Se intentó crear un segundo AudioManager. Este será destruido.");
            Destroy(gameObject);
        }

        if (audioSource == null)
            audioSource = GetComponent<AudioSource>();

        if (audioSource == null)
            Debug.LogError("AudioSource no encontrado en AudioManager.");
    }

    // 🔊 Reproduce desde el AudioSource interno
    private void Reproducir(AudioClip clip)
    {
        Reproducir(clip, audioSource);
    }

    // 🔊 Reproduce desde cualquier AudioSource externo
    public void Reproducir(AudioClip clip, AudioSource source)
    {
        if (clip == null)
        {
            Debug.LogWarning("Intentaste reproducir un clip nulo.");
            return;
        }

        if (source == null)
        {
            Debug.LogWarning("AudioSource externo nulo.");
            return;
        }

        Debug.Log("Reproduciendo clip: " + clip.name + " desde " + source.gameObject.name);
        source.PlayOneShot(clip);
    }

    // Métodos públicos con opción de AudioSource externo
    public void SonidoAtaqueEspada1(AudioSource source = null) => Reproducir(AtaqueEspada1, source ?? audioSource);
    public void SonidoAtaqueEspada2(AudioSource source = null) => Reproducir(AtaqueEspada2, source ?? audioSource);
    public void SonidoDesenvaine(AudioSource source = null) => Reproducir(Desenvaine1, source ?? audioSource);
    public void SonidoMissAttack(AudioSource source = null) => Reproducir(MissAttack, source ?? audioSource);
    public void SonidoDañoEnemigo1(AudioSource source = null) => Reproducir(DañoEnemigo1, source ?? audioSource);
    public void SonidoDañoEnemigo2(AudioSource source = null) => Reproducir(DañoEnemigo2, source ?? audioSource);
    public void SonidoDañoPlayer(AudioSource source = null) => Reproducir(DañoPlayer, source ?? audioSource);
    public void SonidoSalto1(AudioSource source = null) => Reproducir(Salto1, source ?? audioSource);
    public void SonidoSalto2(AudioSource source = null) => Reproducir(Salto2, source ?? audioSource);
    public void SonidoSalto3(AudioSource source = null) => Reproducir(Salto3, source ?? audioSource);
    public void SonidoEnemigoMuriendo(AudioSource source = null) => Reproducir(EnemigoMuriendo, source ?? audioSource);
    public void SonidoRisaMalvada(AudioSource source = null) => Reproducir(RisaMalvada, source ?? audioSource);

}
