using UnityEngine;

/// <summary>
/// Stats específicos del Player: solo "gloria".
/// Hereda todos los demás stats de CharacterBase.
/// </summary>
public class PlayerStats : CharacterBase
{
    [Header("Propiedades del Player")]
    [SerializeField] private int gloria = 0;

    public int Gloria
    {
        get => gloria;
        set => gloria = Mathf.Max(0, value); // nunca negativa
    }

    public void AddGloria(int amount)
    {
        Gloria += Mathf.Max(0, amount);
    }

    public void RemoveGloria(int amount)
    {
        Gloria -= Mathf.Max(0, amount);
    }

    protected override void Die()
    {
        base.Die();
        Debug.Log("Player murió. Mostrar pantalla de Game Over o reinicio.");
    }
}
