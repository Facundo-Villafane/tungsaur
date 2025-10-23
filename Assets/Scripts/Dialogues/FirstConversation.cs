using UnityEngine;
using DialogueEditor;
public class FirstConversation : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    [SerializeField] private NPCConversation m_node;   
    void Start()
    {
         ConversationManager.Instance.StartConversation(m_node);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
