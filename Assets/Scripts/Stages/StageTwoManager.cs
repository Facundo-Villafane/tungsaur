using UnityEngine;
using UnityEngine.SceneManagement;
using DialogueEditor;

public class StageTwoManager : MonoBehaviour
{
    private ConversationController conversationController;
    private StageZone stageZone;
    private bool conversationStarted = false;

    private void Awake()
    {
        conversationController = GetComponent<ConversationController>();
        stageZone = GetComponent<StageZone>();
    }

    private void OnEnable()
    {
        ConversationManager.OnConversationStarted += OnConversationStarted;
        ConversationManager.OnConversationEnded += OnConversationEnded;
        stageZone.OnStageStarted += OnStageStarted;
    }

    private void OnDisable()
    {
        ConversationManager.OnConversationStarted -= OnConversationStarted;
        ConversationManager.OnConversationEnded -= OnConversationEnded;
        stageZone.OnStageStarted -= OnStageStarted;
    }

    private void OnStageStarted()
    {
        if (!conversationStarted)
        {
            conversationStarted = true;
            Debug.Log("[StageTwoManager] Stage iniciado, arrancando conversación 0...");
            conversationController.StartConversation(0);
        }
    }

    private void OnConversationStarted()
    {
        Debug.Log($"[StageTwoManager] 💬 Conversación comenzada: {conversationController.CurrentConversation?.name}");
    }

    private void OnConversationEnded()
    {
        int index = conversationController.CurrentIndex;

        if (index == 0)
        {
            stageZone.EndStage();

            string currentScene = SceneManager.GetActiveScene().name;

            if (currentScene != "Level1")
            {
                if (GameManager.Instance != null)
                {
                    Debug.Log("[StageTwoManager] Tutorial completado. Cargando escena 'Level1'...");
                    GameManager.Instance.LoadScene("Level1");
                }
                else
                {
                    Debug.LogWarning("[StageTwoManager] GameManager.Instance es null. No se puede cargar 'Level1'.");
                }
            }
            else
            {
                Debug.Log("[StageTwoManager] Ya estamos en 'Level1'. No se recarga, se continúa con el flujo normal.");
            }
        }
    }
}
