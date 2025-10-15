using UnityEngine;

public class SlotManager : MonoBehaviour
{
    public static SlotManager Instance { get; private set; }

    [SerializeField] private float circleRadius = 2.5f;
    [SerializeField] private int maxSlots = 8;

    private Transform player;
    public Transform Player => player;
    public Vector3 PlayerPosition => player != null ? player.position : Vector3.zero;
    private bool[] slotOccupied;

    private void Awake()
    {
        if (Instance != null) Destroy(gameObject);
        Instance = this;

        slotOccupied = new bool[maxSlots];
    }

    private void Start()
    {
        player = GameObject.FindWithTag("Player").transform;
    }

public Vector3 RequestSlot(EnemyController enemy)
{
    System.Collections.Generic.List<int> freeSlots = new System.Collections.Generic.List<int>();

    // Buscar slots libres
    for (int i = 0; i < maxSlots; i++)
    {
        if (!slotOccupied[i])
        {
            freeSlots.Add(i);
        }
    }

    if (freeSlots.Count > 0)
    {
        // Elegir un slot libre aleatorio
        int randomIndex = Random.Range(0, freeSlots.Count);
        int chosenSlot = freeSlots[randomIndex];

        slotOccupied[chosenSlot] = true;
        enemy.AssignedSlot = chosenSlot;
        return GetSlotPosition(chosenSlot);
    }

    // Si no hay slots libres, quedarse donde está
    return enemy.transform.position;
}


    public void ReleaseSlot(int slotIndex)
    {
        if (slotIndex >= 0 && slotIndex < slotOccupied.Length)
        {
            slotOccupied[slotIndex] = false;
        }
    }

    public Vector3 GetSlotPosition(int index)
    {
        float angle = (360f / maxSlots) * index;
        Vector3 offset = new Vector3(
            Mathf.Cos(angle * Mathf.Deg2Rad),
            0,
            Mathf.Sin(angle * Mathf.Deg2Rad)
        ) * circleRadius;
        return player.position + offset;
    }
}

