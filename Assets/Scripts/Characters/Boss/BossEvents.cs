using UnityEngine;
using System;

public static class BossEvents
{
    public static event Action<Transform> OnBossDeath;

    public static void TriggerBossDeath(Transform bossTransform)
    {
        OnBossDeath?.Invoke(bossTransform);
    }
}
