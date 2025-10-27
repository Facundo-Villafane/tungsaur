using UnityEngine;
using DialogueEditor;
public class FirstConversation : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    [SerializeField] private NPCConversation m_node1;  
    [SerializeField] private NPCConversation m_node2;  
    void Start()
    {
        ConversationManager.Instance.StartConversation(m_node1);
    }

    // Update is called once per frame
    void Update()
    {

    }
    
    public void StartSecondConversation()
    {
        ConversationManager.Instance.StartConversation(m_node2);
    }
}
