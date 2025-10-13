using UnityEngine;

public class CharacterBase : MonoBehaviour
{
    [Header("Stats Generales")]
    [SerializeField] private float maxHealth = 100f;
    [SerializeField] private float currentHealth = 100f;
    [SerializeField] private float energy = 50f;
    [SerializeField] private float defense = 5f;
    [SerializeField] private float baseDamage = 10f;
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private bool isStunned = false;
    [SerializeField] private bool tookHit = false;

    // ----------- PROPIEDADES -----------

    public float MaxHealth
    {
        get => maxHealth;
        set
        {
            maxHealth = Mathf.Max(1, value); // nunca menos de 1
            currentHealth = Mathf.Min(currentHealth, maxHealth);
        }
    }

    public float CurrentHealth
    {
        get => currentHealth;
        set
        {
            currentHealth = Mathf.Clamp(value, 0, MaxHealth);
            if (currentHealth <= 0) Die();
        }
    }

    public float Energy
    {
        get => energy;
        set => energy = Mathf.Clamp(value, 0, 9999);
    }

    public float Defense
    {
        get => defense;
        set => defense = Mathf.Max(0, value);
    }

    public float BaseDamage
    {
        get => baseDamage;
        set => baseDamage = Mathf.Max(0, value);
    }

    public float MoveSpeed
    {
        get => moveSpeed;
        set => moveSpeed = Mathf.Max(0, value);
    }

    public bool IsStunned
    {
        get => isStunned;
        set => isStunned = value;
    }
    public bool TookHit
    {
        get => tookHit;
        set => tookHit = value;
    }

    // ----------- MÉTODOS -----------

    protected virtual void Awake()
    {
        currentHealth = maxHealth;
    }

    protected virtual void Die()
    {
        Debug.Log($"{gameObject.name} murió");
        
    }

    public virtual void TakeDamage(float damage)
    {
        // Reducción por defensa
        float finalDamage = Mathf.Max(0, damage - Defense);
        CurrentHealth -= finalDamage;
        Debug.Log($"{gameObject.name} recibió {finalDamage} de daño. Vida restante: {CurrentHealth}");
    }

    public virtual void TakeHit()
    {
        Debug.Log($"{gameObject.name} fue golpeado");
    }


}
