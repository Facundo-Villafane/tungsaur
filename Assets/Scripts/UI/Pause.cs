using UnityEngine;
using UnityEngine.SceneManagement;

public class Pause : MonoBehaviour
{
    [Header("Configuración de Pausa")]
    [Tooltip("Tecla para activar/desactivar la pausa")]
    [SerializeField] private KeyCode pauseKey = KeyCode.Escape;
    
    [Header("UI")]
    [Tooltip("Canvas que se mostrará al pausar el juego")]
    [SerializeField] private GameObject pauseCanvas;
    
    [Header("Escenas")]
    [Tooltip("Índice de la escena del menú principal")]
    [SerializeField] private int mainMenuSceneIndex = 0;
    
    // Estado de pausa
    private bool isPaused = false;
    
    void Start()
    {
        // Asegurarse de que el juego empiece sin pausa
        Time.timeScale = 1f;
        
        // Ocultar el canvas de pausa al inicio
        if (pauseCanvas != null)
        {
            pauseCanvas.SetActive(false);
        }
    }
    
    void Update()
    {
        // Detectar cuando se presiona la tecla de pausa
        if (Input.GetKeyDown(pauseKey))
        {
            if (isPaused)
            {
                SetUnpause();
            }
            else
            {
                SetPause();
            }
        }
    }
    
    /// <summary>
    /// Pausa el juego, establece TimeScale a 0 y muestra el canvas de pausa
    /// </summary>
    public void SetPause()
    {
        isPaused = true;
        Time.timeScale = 0f;
        
        if (pauseCanvas != null)
        {
            pauseCanvas.SetActive(true);
        }
        else
        {
            Debug.LogWarning("No se ha asignado el Canvas de pausa en el inspector");
        }
        
        // Opcional: Desbloquear el cursor para poder interactuar con el menú de pausa
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
    
    /// <summary>
    /// Quita la pausa del juego, establece TimeScale a 1 y oculta el canvas de pausa
    /// </summary>
    public void SetUnpause()
    {
        isPaused = false;
        Time.timeScale = 1f;
        
        if (pauseCanvas != null)
        {
            pauseCanvas.SetActive(false);
        }
        
        // Opcional: Volver a bloquear el cursor si es necesario para tu juego
        // Cursor.lockState = CursorLockMode.Locked;
        // Cursor.visible = false;
    }
    
    /// <summary>
    /// Vuelve al menú principal cargando la escena especificada
    /// </summary>
    public void BackToMainMenu()
    {
        // Restablecer TimeScale antes de cambiar de escena
        Time.timeScale = 1f;
        
        // Cargar la escena del menú principal
        SceneManager.LoadScene(mainMenuSceneIndex);
    }
    
    /// <summary>
    /// Se ejecuta cuando el objeto es destruido
    /// Asegura que TimeScale vuelva a 1 si el objeto se destruye mientras está pausado
    /// </summary>
    void OnDestroy()
    {
        Time.timeScale = 1f;
    }

    /// <summary>
    /// Se ejecuta cuando la aplicación pierde el foco (opcional)
    /// Puedes descomentar si quieres pausar automáticamente cuando el jugador cambia de ventana
    /// </summary>
    /*
    void OnApplicationFocus(bool hasFocus)
    {
        if (!hasFocus && !isPaused)
        {
            SetPause();
        }
    }
    */

    public void CloseGame()
    {
            Application.Quit();
    }
}
