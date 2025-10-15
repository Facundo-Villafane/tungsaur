using UnityEngine;

public class PatrolZone : MonoBehaviour
{
    [Header("Patrol Points")]
    public Transform[] patrolPoints;

    private void Awake()
    {
        // Si no asignaste puntos manualmente, tomamos todos los hijos del objeto
        if (patrolPoints == null || patrolPoints.Length == 0)
        {
            int childCount = transform.childCount;
            patrolPoints = new Transform[childCount];
            for (int i = 0; i < childCount; i++)
            {
                patrolPoints[i] = transform.GetChild(i);
            }
        }
    }
}
