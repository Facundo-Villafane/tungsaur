using UnityEngine;
using System;

public class EnemyController : MonoBehaviour
{
    public event Action OnEnemyDeath;

    // Acá podés poner tu lógica de IA, movimiento, etc.

    public void Die()
    {
        // Acá iría animación, efectos, puntaje, etc.
        OnEnemyDeath?.Invoke();
        Destroy(gameObject);
    }
}
