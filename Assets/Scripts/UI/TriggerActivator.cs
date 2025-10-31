using UnityEngine;

public class TriggerActivator : MonoBehaviour
{
    [Header("Objeto que se activará al entrar en el trigger")]
    [SerializeField] private GameObject objectToActivate;

    [Header("Filtrar por tag (opcional)")]
    [SerializeField] private string triggeringTag = "Player";

    private void OnTriggerEnter(Collider other)
    {
        // Si no hay objeto asignado, avisamos
        if (objectToActivate == null)
        {
            Debug.LogWarning($"{name}: No hay objeto asignado para activar.");
            return;
        }

        // Si tiene el tag correcto (o si triggeringTag está vacío, activa igual)
        if (string.IsNullOrEmpty(triggeringTag) || other.CompareTag(triggeringTag))
        {
            objectToActivate.SetActive(true);
            Debug.Log($"{name}: Activó el objeto {objectToActivate.name}");
        }
    }
}
