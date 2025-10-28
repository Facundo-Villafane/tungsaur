using UnityEngine;
using DialogueEditor;

public class StageOneManager : MonoBehaviour
{
    private ConversationController conversationController;
    private StageZone stageZone;

    private void Awake()
    {
        conversationController = GetComponent<ConversationController>();
        stageZone = GetComponent<StageZone>();
    }

    private void OnEnable()
    {
        // Suscribirse a los callbacks sin parámetros
        ConversationManager.OnConversationStarted += OnConversationStarted;
        ConversationManager.OnConversationEnded += OnConversationEnded;
    }

    private void OnDisable()
    {
        ConversationManager.OnConversationStarted -= OnConversationStarted;
        ConversationManager.OnConversationEnded -= OnConversationEnded;
    }

    private void Start()
    {
        // Inicia la primera conversación
        conversationController.StartConversation(0);
    }

    private void OnConversationStarted()
    {
        Debug.Log($"💬 Conversación comenzada: {conversationController.CurrentConversation?.name}");
    }

    private void OnConversationEnded()
    {
        int index = conversationController.CurrentIndex;

        if (index == 0)
        {
            Debug.Log("✅ Conversación 0 terminada. Haciendo debug...");

            // Debug adicional
            Debug.Log("🔹 Debug: flujo correcto después de conversación 0.");

            // Iniciar siguiente conversación
            conversationController.StartNextConversation();
        }
        else if (index == 1)
        {
            Debug.Log("✅ Conversación 1 terminada. Haciendo debug...");

            Debug.Log("🔹 Debug: flujo correcto después de conversación 1.");

            stageZone.EndStage();
        }
    }
}
