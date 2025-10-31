using UnityEngine;
using DialogueEditor;

public class StageFourManager : MonoBehaviour
{
    private ConversationController conversationController;
    private StageZone stageZone;
    private bool conversationStarted = false; // Para que no se inicie varias veces

    private void Awake()
    {
        conversationController = GetComponent<ConversationController>();
        stageZone = GetComponent<StageZone>();
    }

    private void OnEnable()
    {
        ConversationManager.OnConversationStarted += OnConversationStarted;
        ConversationManager.OnConversationEnded += OnConversationEnded;

        // Suscribirse al evento del StageZone
        stageZone.OnStageStarted += OnStageStarted;
    }

    private void OnDisable()
    {
        ConversationManager.OnConversationStarted -= OnConversationStarted;
        ConversationManager.OnConversationEnded -= OnConversationEnded;

        stageZone.OnStageStarted -= OnStageStarted;
    }

    // Se ejecuta cuando StageZone indica que el stage empezó
    private void OnStageStarted()
    {
        if (!conversationStarted)
        {
            conversationStarted = true;
            Debug.Log("Stage iniciado, arrancando conversación 0...");
            conversationController.StartConversation(0);
        }
    }

    private void OnConversationStarted()
    {
        Debug.Log($"💬 Conversación comenzada: {conversationController.CurrentConversation?.name}");
    }

    private void OnConversationEnded()
    {
        stageZone.EndStage();
    }
}
