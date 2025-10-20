using System;

/// <summary>
/// Interfaz común para todos los spawners de enemigos
/// </summary>
public interface IEnemySpawner
{
    int EnemiesToSpawn { get; }
    void StartSpawning(Action onEnemyDefeated);
    void StopSpawning();
}
