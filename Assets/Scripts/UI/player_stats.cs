using UnityEngine;
using System;

public class Player : MonoBehaviour
{
    [Header("Health Stats")]
    public float MinimumHP = 0f;
    public float CurrentHP = 100f;
    public float MaximumHP = 100f;
    
    [Header("Energy Stats")]
    public float MinimumENERGY = 0f;
    public float CurrentENERGY = 0f;
    public float MaximumENERGY = 100f;
    
    // Eventos para notificar cambios en la UI
    public static event Action<float, float> OnHealthChanged;
    public static event Action<float, float> OnEnergyChanged;
    public static event Action OnHealthPickUp;
    public static event Action OnEnergyPickUp;
    
    private UIController uiController;
    
    void Start()
    {
        uiController = FindObjectOfType<UIController>();
        
        // Inicializar UI con valores actuales
        OnHealthChanged?.Invoke(CurrentHP, MaximumHP);
        OnEnergyChanged?.Invoke(CurrentENERGY, MaximumENERGY);
    }
    
    // Método para recibir daño
    public void TakeDamage(float damage)
    {
        CurrentHP = Mathf.Clamp(CurrentHP - damage, MinimumHP, MaximumHP);
        OnHealthChanged?.Invoke(CurrentHP, MaximumHP);
    }
    
    // Método para recuperar vida
    public void Heal(float amount)
    {
        CurrentHP = Mathf.Clamp(CurrentHP + amount, MinimumHP, MaximumHP);
        OnHealthChanged?.Invoke(CurrentHP, MaximumHP);
    }
    
    // Método para ganar energía
    public void GainEnergy(float amount)
    {
        CurrentENERGY = Mathf.Clamp(CurrentENERGY + amount, MinimumENERGY, MaximumENERGY);
        OnEnergyChanged?.Invoke(CurrentENERGY, MaximumENERGY);
    }
    
    // Método para usar energía
    public void UseEnergy(float amount)
    {
        CurrentENERGY = Mathf.Clamp(CurrentENERGY - amount, MinimumENERGY, MaximumENERGY);
        OnEnergyChanged?.Invoke(CurrentENERGY, MaximumENERGY);
    }
    
    // Método cuando se recoge un pickup de vida
    public void GetHealthPickUp(float amount)
    {
        Heal(amount);
        OnHealthPickUp?.Invoke();
    }
    
    // Método cuando se recoge un pickup de energía
    public void GetEnergyPickUp(float amount)
    {
        GainEnergy(amount);
        OnEnergyPickUp?.Invoke();
    }
    
    // Método para testear en el inspector
    [ContextMenu("Test Take Damage 20")]
    void TestDamage20() => TakeDamage(20f);
    
    [ContextMenu("Test Heal 20")]
    void TestHeal20() => Heal(20f);
    
    [ContextMenu("Test Gain Energy 33")]
    void TestGainEnergy33() => GainEnergy(33f);
    
    [ContextMenu("Test Health PickUp")]
    void TestHealthPickUp() => GetHealthPickUp(25f);
    
    [ContextMenu("Test Energy PickUp")]
    void TestEnergyPickUp() => GetEnergyPickUp(33f);
}
