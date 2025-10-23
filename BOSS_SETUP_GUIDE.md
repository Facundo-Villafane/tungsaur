# 🦹 Guía de Configuración de Bosses

## 📋 Diferencias: Enemigos vs Bosses

### Enemigos Normales
- Stats configurados **directamente en el prefab**
- Usan `EnemyController.cs`
- No tienen fases ni ataques especiales
- Se spawnean desde `WaveConfigSO`

### Bosses
- Stats configurados en **BossConfigSO** (ScriptableObject)
- Usan `BossController.cs` (extiende EnemyController)
- Sistema de fases basado en vida
- Ataques especiales con cooldown
- Cinemáticas de intro/derrota
- Barra de vida especial en UI

---

## 🎯 Crear un Boss Completo

### PASO 1: Crear el Prefab Base del Boss

1. En Unity, duplica tu prefab de `Enemy`
2. Renombra a `Boss_Level1`
3. **IMPORTANTE**: Reemplaza el componente:
   - ❌ Quita `EnemyController.cs`
   - ✅ Agrega `BossController.cs`

4. El prefab debe tener:
   ```
   Boss_Level1
   ├── BossController.cs        ← Nuevo script
   ├── Rigidbody
   ├── Collider (con trigger para detección)
   ├── Animator
   ├── Sprite Renderer / Model
   └── (Opcional) Efectos de partículas
   ```

5. En el componente `BossController`:
   ```
   Boss Config: [Dejar vacío por ahora]
   Show Health Bar: ✓
   Invulnerable During Intro: ✓
   ```

---

### PASO 2: Crear el BossConfigSO

1. En Unity Project:
   - Click derecho → `Create > CDG > Boss Configuration`
   - Nombra: `Boss_Level1_Config`

2. Configurar el ScriptableObject:

   ```
   ═══════════════════════════════════════
   Boss Info
   ═══════════════════════════════════════
   Boss Name: "Brutal Brawler"
   Boss Prefab: [Arrastra Boss_Level1 aquí]

   ═══════════════════════════════════════
   Stats
   ═══════════════════════════════════════
   Max Health: 500
   Damage: 30
   Defense: 10
   Move Speed: 3

   ═══════════════════════════════════════
   Phases
   ═══════════════════════════════════════
   Phase Thresholds:
   - Size: 3
   - Element 0: 0.75    ← Fase 2 a 75% vida
   - Element 1: 0.5     ← Fase 3 a 50% vida
   - Element 2: 0.25    ← Fase Final a 25% vida

   Phase Names:
   - Size: 4
   - Element 0: "Warming Up"
   - Element 1: "Getting Serious"
   - Element 2: "Enraged"
   - Element 3: "Last Stand"

   ═══════════════════════════════════════
   Special Attacks
   ═══════════════════════════════════════
   Special Attack Cooldown: 10

   ═══════════════════════════════════════
   Cinematics (Opcional)
   ═══════════════════════════════════════
   Intro Cinematic: [BossIntroCinematic]
   Defeat Cinematic: [BossDefeatCinematic]

   ═══════════════════════════════════════
   Rewards
   ═══════════════════════════════════════
   Glory Reward: 200
   Score Reward: 2000

   ═══════════════════════════════════════
   Audio (Opcional)
   ═══════════════════════════════════════
   Boss Music: [Tu música de boss]
   Boss Defeated Sound: [Sonido de derrota]
   ```

---

### PASO 3: Conectar el Config al Prefab

1. Abre el prefab `Boss_Level1`
2. Selecciona el componente `BossController`
3. Arrastra `Boss_Level1_Config` al campo **Boss Config**

---

### PASO 4: Asignar Boss al LevelConfig

1. Abre tu `Level1Config` (LevelConfigSO)
2. En la sección **Boss**:
   ```
   Boss Config: [Arrastra Boss_Level1_Config]
   ```

---

## 🎮 Cómo Funcionan las Fases

El boss cambia de fase automáticamente según su vida:

```
100% → 75%  | Fase 0: "Warming Up"        (Comportamiento normal)
 75% → 50%  | Fase 1: "Getting Serious"   (Más agresivo)
 50% → 25%  | Fase 2: "Enraged"           (Muy agresivo)
 25% → 0%   | Fase 3: "Last Stand"        (Desesperado)
```

### Eventos de Fase

Puedes suscribirte a cambios de fase:

```csharp
BossController boss = FindObjectOfType<BossController>();

boss.OnPhaseChanged += (int newPhase) => {
    Debug.Log($"Boss entró en fase {newPhase}");

    switch(newPhase) {
        case 1:
            // Aumentar velocidad de ataque
            break;
        case 2:
            // Activar ataques especiales más frecuentes
            break;
        case 3:
            // Modo berserk
            break;
    }
};
```

---

## ⚔️ Ataques Especiales del Boss

Por defecto, el boss tiene placeholders para 4 ataques especiales (uno por fase).

### Personalizar Ataques Especiales

Abre `BossController.cs` y modifica estos métodos:

```csharp
private void SpecialAttackPhase1()
{
    Debug.Log("Boss usa ataque especial fase 1!");

    // TU CÓDIGO AQUÍ
    // Ejemplo: Lanzar proyectiles
    // SpawnProjectiles();
}

private void SpecialAttackPhase2()
{
    Debug.Log("Boss usa ataque especial fase 2!");

    // TU CÓDIGO AQUÍ
    // Ejemplo: Salto aplastante
    // PerformGroundSlam();
}

private void SpecialAttackPhase3()
{
    Debug.Log("Boss usa ataque especial fase 3!");

    // TU CÓDIGO AQUÍ
    // Ejemplo: Invocar enemigos
    // SummonMinions();
}

private void SpecialAttackFinal()
{
    Debug.Log("Boss usa ataque especial final!");

    // TU CÓDIGO AQUÍ
    // Ejemplo: Área de efecto masiva
    // UltimateAttack();
}
```

**El cooldown se controla automáticamente** desde el `BossConfigSO`.

---

## 🎬 Cinemáticas del Boss

### Crear Cinemática de Intro

1. Click derecho → `Create > CDG > Cinematic Configuration`
2. Nombra: `BossIntroCinematic`
3. Configura:
   ```
   Cinematic Name: "Boss Intro"
   Cinematic Type: Dialogue (o Timeline)

   Dialogue Conversation: [Conversación de diálogo del boss]
   Lock Camera: ✓
   Can Skip: ✓
   Skip Key: Space
   ```

### Secuencia Automática

Cuando el boss se spawnea:
1. Se reproduce la cinemática de intro
2. Boss está invulnerable
3. Al terminar cinemática → `EndIntro()` se llama
4. Boss se vuelve vulnerable
5. Barra de vida aparece

---

## 🎨 Mini-Bosses

Los mini-bosses son más simples (sin fases):

### Crear MiniBossConfigSO

1. Click derecho → `Create > CDG > MiniBoss Configuration`
2. Configurar:
   ```
   MiniBoss Name: "Elite Guard"
   MiniBoss Prefab: [Tu prefab]

   Max Health: 200
   Damage: 20
   Defense: 5
   Move Speed: 4

   Special Attack Cooldown: 8
   Has Intro Animation: ✓
   ```

3. Asignar en `StageConfigSO`:
   ```
   Mini Boss Config: [Tu MiniBossConfig]
   ```

**NOTA**: Los mini-bosses usan `EnemyController` normal, no `BossController`.

---

## 🔗 Integración con el Flujo del Nivel

El boss se spawnea automáticamente desde `LevelFlowManager`:

```
Stages completos
    ↓
LevelFlowManager detecta Boss Config
    ↓
Reproduce Boss Intro Cinematic
    ↓
Spawnea Boss usando BossConfigSO
    ↓
Muestra barra de vida del boss
    ↓
Boss fight...
    ↓
Boss derrotado
    ↓
Reproduce Boss Defeat Cinematic
    ↓
Otorga recompensas
```

---

## 📊 Comparación de Stats

### Enemigo Normal
```
Health: 100
Damage: 10
Defense: 2
Speed: 5
```

### Mini-Boss
```
Health: 200
Damage: 20
Defense: 5
Speed: 4
Special Attacks: ✓
```

### Boss Final
```
Health: 500
Damage: 30
Defense: 10
Speed: 3
Fases: 4
Special Attacks: ✓
Cinematics: ✓
Barra de vida: ✓
```

---

## 🧪 Testing del Boss

### Probar Boss Individualmente

1. Crea una escena de prueba
2. Arrastra el prefab `Boss_Level1`
3. Agrega `UIManager` (para la barra de vida)
4. Play → El boss debe:
   - Mostrar barra de vida
   - Cambiar de fase según daño
   - Usar ataques especiales

### Probar Boss en Nivel Completo

1. Configura `LevelConfigSO` con boss
2. Play desde inicio del nivel
3. Completa todos los stages
4. Debe aparecer cinemática de boss
5. Boss fight comienza

---

## ⚠️ Troubleshooting

### "Boss no aparece"
✅ Verifica que `LevelConfigSO` tenga el `BossConfigSO` asignado

### "Boss no tiene barra de vida"
✅ Verifica que `UIManager` esté en la escena
✅ Verifica que `Show Health Bar` esté en ✓

### "Boss no cambia de fase"
✅ Verifica que `Phase Thresholds` estén configurados
✅ Revisa que el boss esté recibiendo daño

### "Ataques especiales no funcionan"
✅ Verifica `Special Attack Cooldown` en el config
✅ Implementa los métodos de ataque en `BossController.cs`

---

## 🎓 Resumen

| Aspecto | Enemigos | Mini-Bosses | Bosses |
|---------|----------|-------------|--------|
| Script | EnemyController | EnemyController | BossController |
| Config | Prefab directo | MiniBossConfigSO | BossConfigSO |
| Fases | ❌ | ❌ | ✅ (4 fases) |
| Ataques Especiales | ❌ | ✅ Simple | ✅ Por fase |
| Cinemáticas | ❌ | Opcional | ✅ Intro/Derrota |
| Barra de Vida | ❌ | ❌ | ✅ Especial |
| Spawning | WaveManager | StageConfig | LevelConfig |

---

**¡Listo! Ahora sabes cómo configurar enemigos normales, mini-bosses y bosses finales! 🎮**
