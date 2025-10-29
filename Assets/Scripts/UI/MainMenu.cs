using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class MainMenu : MonoBehaviour
{
    [Header("Canvas de Créditos")]
    [Tooltip("Canvas que contiene los créditos del juego")]
    [SerializeField] private GameObject creditsCanvas;
    
    [Header("Canvas de Carga")]
    [Tooltip("Canvas que se muestra mientras se carga el juego")]
    [SerializeField] private GameObject loadingCanvas;
    
    [Header("Configuración de Escenas")]
    [Tooltip("Índice de la escena del juego principal")]
    [SerializeField] private int gameSceneIndex = 1;
    
    [Header("Opciones de Carga")]
    [Tooltip("Tiempo de espera antes de cargar la escena (en segundos)")]
    [SerializeField] private float loadingDelay = 0.5f;
    
    void Start()
    {
        // Asegurarse de que el TimeScale esté en 1 al iniciar el menú
        Time.timeScale = 1f;
        
        // Ocultar los canvas al inicio
        if (creditsCanvas != null)
        {
            creditsCanvas.SetActive(false);
        }
        
        if (loadingCanvas != null)
        {
            loadingCanvas.SetActive(false);
        }
        
        // Configurar el cursor para el menú
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
    
    /// <summary>
    /// Muestra el canvas de créditos
    /// </summary>
    public void ShowCredits()
    {
        if (creditsCanvas != null)
        {
            creditsCanvas.SetActive(true);
        }
        else
        {
            Debug.LogWarning("No se ha asignado el Canvas de créditos en el inspector");
        }
    }
    
    /// <summary>
    /// Oculta el canvas de créditos
    /// </summary>
    public void HideCredits()
    {
        if (creditsCanvas != null)
        {
            creditsCanvas.SetActive(false);
        }
    }
    
    /// <summary>
    /// Cierra el juego
    /// </summary>
    public void CloseGame()
    {
        Debug.Log("Cerrando el juego...");
        
        #if UNITY_EDITOR
            // Si estamos en el editor de Unity, detener el modo Play
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            // En una build, cerrar la aplicación
            Application.Quit();
        #endif
    }
    
    /// <summary>
    /// Inicia el juego mostrando un canvas de carga y luego cargando la escena
    /// </summary>
    public void StartGame()
    {
        // Mostrar el canvas de carga
        if (loadingCanvas != null)
        {
            loadingCanvas.SetActive(true);
        }
        
        // Iniciar la corrutina para cargar la escena
        StartCoroutine(LoadGameScene());
    }
    
    /// <summary>
    /// Corrutina para cargar la escena del juego con un pequeño delay
    /// </summary>
    private IEnumerator LoadGameScene()
    {
        // Esperar un momento para que el jugador vea la pantalla de carga
        yield return new WaitForSeconds(loadingDelay);
        
        // Cargar la escena del juego de forma asíncrona para una transición más suave
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(gameSceneIndex);
        
        // Esperar hasta que la escena termine de cargar
        while (!asyncLoad.isDone)
        {
            // Opcional: Aquí podrías actualizar una barra de progreso
            // float progress = Mathf.Clamp01(asyncLoad.progress / 0.9f);
            // Debug.Log("Progreso de carga: " + (progress * 100) + "%");
            
            yield return null;
        }
    }
    
    /// <summary>
    /// Método alternativo para cargar escena de forma directa (sin corrutina)
    /// Puedes usar este si prefieres no usar la pantalla de carga
    /// </summary>
    public void StartGameDirect()
    {
        SceneManager.LoadScene(gameSceneIndex);
    }
    
    /// <summary>
    /// Abre una URL en el navegador (útil para enlaces a redes sociales)
    /// </summary>
    public void OpenURL(string url)
    {
        Application.OpenURL(url);
    }
    
    /// <summary>
    /// Reproduce un sonido de click (opcional)
    /// Requiere un AudioSource en el GameObject
    /// </summary>
    public void PlayClickSound()
    {
        AudioSource audioSource = GetComponent<AudioSource>();
        if (audioSource != null)
        {
            audioSource.Play();
        }
    }
}
