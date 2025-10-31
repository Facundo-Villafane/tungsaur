using UnityEngine;
using System;
using System.Collections;

public class PlayerStats : CharacterBase
{
    [Header("Propiedades del Player")]
    [SerializeField] private int gloria = 0;

    [Header("Animation")]
    [SerializeField] private Animator playerAnimator;

    // --- EVENTOS para UIController ---
    public static event Action<float, float> OnHealthChanged;
    public static event Action<float, float> OnEnergyChanged;
    public static event Action OnHealthPickUp;
    public static event Action OnEnergyPickUp;

    protected override void Awake()
    {
        base.Awake();
        playerAnimator = playerAnimator ?? GetComponent<Animator>();

        // Enviamos el estado inicial de vida y energía
        OnHealthChanged?.Invoke(CurrentHealth, MaxHealth);
        OnEnergyChanged?.Invoke(Energy, 100f); // si tenés un valor máximo distinto, usalo acá
    }

    public override void TakeDamage(float damage)
    {
        base.TakeDamage(damage);
        OnHealthChanged?.Invoke(CurrentHealth, MaxHealth);
    }

    public void Heal(float amount)
    {
        CurrentHealth = Mathf.Min(CurrentHealth + amount, MaxHealth);
        OnHealthChanged?.Invoke(CurrentHealth, MaxHealth);
        OnHealthPickUp?.Invoke();
    }

    public void UseEnergy(float amount)
    {
        Energy = Mathf.Max(0, Energy - amount);
        OnEnergyChanged?.Invoke(Energy, 100f);
    }

    public void RecoverEnergy(float amount)
    {
        Energy = Mathf.Min(Energy + amount, 100f);
        OnEnergyChanged?.Invoke(Energy, 100f);
        OnEnergyPickUp?.Invoke();
    }
}
