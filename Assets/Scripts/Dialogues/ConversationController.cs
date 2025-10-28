using UnityEngine;
using DialogueEditor;

public class ConversationController : MonoBehaviour
{
    [SerializeField] private NPCConversation[] conversations; // All your dialogues in order
    private int currentIndex = -1; // Start at -1 to indicate "no conversation started yet"

    public int CurrentIndex => currentIndex;
    public NPCConversation CurrentConversation =>
        (currentIndex >= 0 && currentIndex < conversations.Length) ? conversations[currentIndex] : null;


    // Starts any conversation by index
    public void StartConversation(int index)
    {
        if (index < 0 || index >= conversations.Length)
        {
            Debug.LogWarning($"⚠️ Invalid conversation index: {index}");
            return;
        }

        currentIndex = index;
        ConversationManager.Instance.StartConversation(conversations[index]);
        Debug.Log($"💬 Starting conversation {index}: {conversations[index].name}");
    }

    // Starts the next one (if available)
    public void StartNextConversation()
    {
        int nextIndex = currentIndex + 1;

        if (nextIndex < conversations.Length)
        {
            currentIndex = nextIndex;
            ConversationManager.Instance.StartConversation(conversations[nextIndex]);
            Debug.Log($"➡️ Starting next conversation: {conversations[nextIndex].name}");
        }
        else
        {
            Debug.Log("🏁 No more conversations left.");
        }
    }

    // Optionally, restart from the beginning
    public void RestartConversations()
    {
        currentIndex = -1;
        Debug.Log("🔁 Conversations reset. Ready to start again from index 0.");
    }
}
