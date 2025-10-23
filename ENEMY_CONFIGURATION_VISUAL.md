# 🎯 Configuración Visual: Enemigos vs Bosses

## 📊 Diagrama de Arquitectura

```
CharacterBase (Clase base abstracta)
    ├── Stats: Health, Damage, Defense, Speed
    ├── TakeDamage()
    └── Die()
        ↓
EnemyController (Enemigos normales)
    ├── Hereda de CharacterBase
    ├── State Machine (Idle, Patrol, Attack, Hit, Dead)
    ├── Detection & Attack Range
    └── Stats configurados EN EL PREFAB
        ↓
BossController (Bosses)
    ├── Hereda de EnemyController
    ├── Todo lo de EnemyController +
    ├── Sistema de Fases
    ├── Ataques Especiales
    ├── Stats configurados EN SCRIPTABLEOBJECT
    └── Integración con UIManager
```

---

## 🔧 Configuración de Enemigos Normales

### Inspector View del Prefab:

```
┌─────────────────────────────────────────┐
│ Enemy Prefab                            │
├─────────────────────────────────────────┤
│ ✓ EnemyController.cs                    │
│                                         │
│ Movement Settings:                      │
│   Acceleration: 10                      │
│   Max Speed: 5                          │
│                                         │
│ AI Settings:                            │
│   Detection Radius: 8                   │
│   Attack Range: 2                       │
│                                         │
│ Character Base (inherited):             │
│   Max Health: 100                       │
│   Base Damage: 10                       │
│   Defense: 2                            │
│   Move Speed: 5                         │
│                                         │
│ Physics:                                │
│   Use Physics: ✓                        │
│   Fall Multiplier: 2.5                  │
│   Ground Layer: Ground                  │
├─────────────────────────────────────────┤
│ ✓ Rigidbody                             │
│ ✓ Collider                              │
│ ✓ Animator                              │
└─────────────────────────────────────────┘
```

### Spawning en WaveConfig:

```
┌─────────────────────────────────────────┐
│ WaveConfigSO: "Wave_Easy"               │
├─────────────────────────────────────────┤
│ Enemy Prefab: [Enemy]  ← Prefab aquí   │
│ Total Enemies: 5                        │
│ Max Simultaneous: 3                     │
│ Spawn Interval: 2.0                     │
└─────────────────────────────────────────┘
```

**STATS = PREFAB DIRECTO ✅**

---

## 👹 Configuración de Bosses

### Paso 1: Prefab del Boss

```
┌─────────────────────────────────────────┐
│ Boss_Level1 Prefab                      │
├─────────────────────────────────────────┤
│ ✓ BossController.cs                     │
│                                         │
│ Boss Configuration:                     │
│   Boss Config: [Boss_Level1_Config] ←──┼── ScriptableObject
│   Show Health Bar: ✓                    │
│   Invulnerable During Intro: ✓          │
│                                         │
│ (Stats heredados de EnemyController     │
│  pero IGNORADOS, usa el Config SO)      │
├─────────────────────────────────────────┤
│ ✓ Rigidbody                             │
│ ✓ Collider                              │
│ ✓ Animator                              │
└─────────────────────────────────────────┘
```

### Paso 2: BossConfigSO (ScriptableObject)

```
┌──────────────────────────────────────────────┐
│ Boss_Level1_Config.asset                     │
│ (BossConfigSO)                               │
├──────────────────────────────────────────────┤
│ Boss Info:                                   │
│   Boss Name: "Brutal Brawler"                │
│   Boss Prefab: [Boss_Level1]  ← Referencia  │
│                                              │
│ Stats (ESTOS SE USAN ✅):                    │
│   Max Health: 500                            │
│   Damage: 30                                 │
│   Defense: 10                                │
│   Move Speed: 3                              │
│                                              │
│ Phases:                                      │
│   Phase Thresholds: [0.75, 0.5, 0.25]       │
│   Phase Names:                               │
│     - "Warming Up"                           │
│     - "Getting Serious"                      │
│     - "Enraged"                              │
│     - "Last Stand"                           │
│                                              │
│ Special Attacks:                             │
│   Special Attack Cooldown: 10                │
│                                              │
│ Cinematics:                                  │
│   Intro Cinematic: [BossIntro]               │
│   Defeat Cinematic: [BossDefeat]             │
│                                              │
│ Rewards:                                     │
│   Glory Reward: 200                          │
│   Score Reward: 2000                         │
│                                              │
│ Audio:                                       │
│   Boss Music: [Boss_Theme]                   │
│   Boss Defeated Sound: [Victory_Sound]       │
└──────────────────────────────────────────────┘
```

### Paso 3: Asignar en LevelConfig

```
┌──────────────────────────────────────────────┐
│ Level1Config.asset                           │
│ (LevelConfigSO)                              │
├──────────────────────────────────────────────┤
│ Level Info:                                  │
│   Level Name: "Level 1"                      │
│   Level Number: 1                            │
│                                              │
│ Cinematics:                                  │
│   Intro Cinematic: [Level1_Intro]            │
│   Outro Cinematic: [Level1_Outro]            │
│                                              │
│ Stages:                                      │
│   - Stage1Config                             │
│   - Stage2Config                             │
│   - Stage3Config                             │
│                                              │
│ Boss: ┌────────────────────────────────────┐ │
│       │ Boss Config: [Boss_Level1_Config] │ │
│       └────────────────────────────────────┘ │
│                                              │
│ Audio:                                       │
│   Level Music: [Level1_Music]                │
│   Boss Music: [Boss_Music] (sobrescribe)     │
└──────────────────────────────────────────────┘
```

**STATS = SCRIPTABLEOBJECT ✅**

---

## 🔄 Flujo de Datos

### Enemigos Normales:
```
WaveConfigSO → Enemy Prefab → EnemyController
                                    ↓
                            Stats del Prefab
```

### Bosses:
```
LevelConfigSO → BossConfigSO → Boss Prefab → BossController
                      ↓                            ↓
                  Stats del SO              InitializeFromConfig()
                                                   ↓
                                          Stats aplicados al Boss
```

---

## 📋 Tabla Comparativa

| Aspecto | Enemigo Normal | Boss |
|---------|----------------|------|
| **Prefab** | Enemy.prefab | Boss_Level1.prefab |
| **Script** | EnemyController.cs | BossController.cs |
| **Config Source** | Inspector del Prefab | BossConfigSO (ScriptableObject) |
| **Max Health** | 100 (en prefab) | 500 (en SO) |
| **Damage** | 10 (en prefab) | 30 (en SO) |
| **Defense** | 2 (en prefab) | 10 (en SO) |
| **Speed** | 5 (en prefab) | 3 (en SO) |
| **Fases** | ❌ | ✅ (configurables) |
| **Ataques Especiales** | ❌ | ✅ (con cooldown) |
| **Cinemáticas** | ❌ | ✅ (intro/derrota) |
| **Barra de Vida** | ❌ | ✅ (dedicada) |
| **Spawning** | WaveManager | LevelFlowManager |
| **Recompensas** | Ninguna | Glory + Score |

---

## 🎬 Secuencia de Boss Fight

```
┌─────────────────────────────────────────────┐
│ 1. LevelFlowManager detecta BossConfigSO    │
└─────────────────┬───────────────────────────┘
                  ↓
┌─────────────────────────────────────────────┐
│ 2. Reproduce Boss Intro Cinematic           │
│    (Boss invulnerable)                      │
└─────────────────┬───────────────────────────┘
                  ↓
┌─────────────────────────────────────────────┐
│ 3. Instancia Boss desde BossConfigSO        │
│    BossController.InitializeFromConfig()    │
└─────────────────┬───────────────────────────┘
                  ↓
┌─────────────────────────────────────────────┐
│ 4. UIManager muestra barra de vida del boss│
└─────────────────┬───────────────────────────┘
                  ↓
┌─────────────────────────────────────────────┐
│ 5. Boss.EndIntro() → vulnerable             │
└─────────────────┬───────────────────────────┘
                  ↓
┌─────────────────────────────────────────────┐
│ 6. BOSS FIGHT                               │
│    ├─ 100% → 75%: Fase 0                    │
│    ├─  75% → 50%: Fase 1 (OnPhaseChanged)   │
│    ├─  50% → 25%: Fase 2 (OnPhaseChanged)   │
│    └─  25% →  0%: Fase 3 (OnPhaseChanged)   │
└─────────────────┬───────────────────────────┘
                  ↓
┌─────────────────────────────────────────────┐
│ 7. Boss derrotado (CurrentHealth = 0)       │
│    ├─ UIManager oculta barra               │
│    ├─ Reproduce Defeat Cinematic            │
│    └─ Otorga recompensas                    │
└─────────────────┬───────────────────────────┘
                  ↓
┌─────────────────────────────────────────────┐
│ 8. LevelFlowManager → Outro Cinematic       │
└─────────────────────────────────────────────┘
```

---

## 💡 Por Qué Esta Arquitectura

### ✅ Enemigos con Prefab Directo
- **Rápido**: Solo editas el prefab
- **Simple**: Pocos enemigos normales, no necesitan complejidad
- **Reutilizable**: Mismo prefab en múltiples waves

### ✅ Bosses con ScriptableObject
- **Escalable**: Crear nuevo boss = crear nuevo SO
- **Data-Driven**: Sin tocar código
- **Complejo**: Fases, cinemáticas, recompensas
- **Centralizado**: Toda la configuración en un lugar
- **Versionable**: Fácil de balancear stats

---

## 🔧 Ejemplo Práctico

### Balancear Enemigo Normal:
1. Abre prefab `Enemy`
2. Cambia `Max Health: 100 → 120`
3. Save
4. ✅ Todos los spawns usan el nuevo valor

### Balancear Boss:
1. Abre `Boss_Level1_Config` (SO)
2. Cambia `Max Health: 500 → 600`
3. Cambia `Phase Thresholds: [0.75, 0.5, 0.25] → [0.8, 0.6, 0.3]`
4. Save
5. ✅ Boss actualizado sin tocar el prefab

---

## 🎯 Resumen Visual

```
ENEMIGOS: Prefab → Stats directos
┌─────────┐
│ Prefab  │ Health: 100
│ Enemy   │ Damage: 10
└─────────┘ Speed: 5


BOSSES: Prefab → ScriptableObject → Stats
┌─────────┐      ┌──────────────┐
│ Prefab  │─────→│ BossConfigSO │ Health: 500
│ Boss    │      │ Level1Config │ Damage: 30
└─────────┘      └──────────────┘ Phases: 4
                                  Cinematics: ✓
```

---

**¡Ahora deberías entender perfectamente cómo configurar enemigos y bosses! 🎮**
