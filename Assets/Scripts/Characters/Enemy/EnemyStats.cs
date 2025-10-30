using UnityEngine;

/// <summary>
/// Stats específicos del enemigo.
/// Hereda todos los demás stats de CharacterBase.
/// </summary>
public class EnemyStats : CharacterBase
{
    private EnemyController enemyController;

    void Start()
    {
        enemyController = GetComponent<EnemyController>();
        animator = enemyController?.Animator ?? GetComponent<Animator>();
    }

    public override void TakeHit()
    {
        Debug.Log($"{gameObject.name} fue golpeado");

        if (CurrentHealth > 0)
        {
            // 🔊 Reproducir sonido de daño desde AudioManager serializado
            if (enemyController.AudioManager != null)
            {
                enemyController.AudioManager.SonidoDañoEnemigo1();
                Debug.Log("Sonido de daño reproducido desde EnemyStats.");
            }
            else
            {
                Debug.LogWarning("EnemyStats: AudioManager no asignado en EnemyController.");
            }

            // Cambiar estado
            enemyController.TakeHit();
        }
    }
}
