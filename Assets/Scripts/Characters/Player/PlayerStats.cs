using UnityEngine;

/// <summary>
/// Stats específicos del Player: gloria, experiencia, etc.
/// NO hereda de CharacterBase, para evitar duplicación.
/// </summary>
public class PlayerStats : MonoBehaviour
{
    [Header("Propiedades del Player")]
    [SerializeField] private int gloria = 0;

    public int Gloria
    {
        get => gloria;
        set => gloria = Mathf.Max(0, value);
    }

    public void AddGloria(int amount)
    {
        Gloria += Mathf.Max(0, amount);
    }

    public void RemoveGloria(int amount)
    {
        Gloria -= Mathf.Max(0, amount);
    }
}
