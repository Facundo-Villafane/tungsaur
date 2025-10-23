# 📁 Guía de Estructura de Carpetas para ScriptableObjects

## 🗂️ Estructura Completa

```
Assets/
│
├── Scripts/
│   └── ScriptableObjects/              ← C# Scripts de los SO (YA CREADOS ✅)
│       ├── LevelConfigSO.cs
│       ├── StageConfigSO.cs
│       ├── WaveConfigSO.cs
│       ├── CinematicConfigSO.cs
│       ├── BossConfigSO.cs
│       ├── MiniBossConfigSO.cs
│       └── TutorialConfigSO.cs
│
├── ScriptableObjects/                  ← ASSETS de ScriptableObjects (CREAR AQUÍ)
│   │
│   ├── Levels/                         ← Configuración completa de niveles
│   │   ├── Level1Config.asset
│   │   ├── Level2Config.asset
│   │   └── TutorialLevelConfig.asset
│   │
│   ├── Stages/                         ← Stages individuales por nivel
│   │   ├── Level1/
│   │   │   ├── Stage1Config.asset
│   │   │   ├── Stage2Config.asset
│   │   │   └── Stage3Config.asset
│   │   │
│   │   └── Level2/
│   │       ├── Stage1Config.asset
│   │       ├── Stage2Config.asset
│   │       └── Stage3Config.asset
│   │
│   ├── Waves/                          ← Oleadas de enemigos
│   │   ├── Easy/
│   │   │   ├── Wave_Intro.asset
│   │   │   ├── Wave_Basic_1.asset
│   │   │   ├── Wave_Basic_2.asset
│   │   │   └── Wave_Tutorial.asset
│   │   │
│   │   ├── Medium/
│   │   │   ├── Wave_Mixed_1.asset
│   │   │   ├── Wave_Mixed_2.asset
│   │   │   └── Wave_Aggressive.asset
│   │   │
│   │   └── Hard/
│   │       ├── Wave_Elite.asset
│   │       ├── Wave_Boss_Minions.asset
│   │       └── Wave_Final.asset
│   │
│   ├── Bosses/                         ← Configuración de bosses y mini-bosses
│   │   ├── Level1/
│   │   │   ├── Boss_Level1_Config.asset
│   │   │   └── MiniBoss_Elite_Guard.asset
│   │   │
│   │   └── Level2/
│   │       ├── Boss_Level2_Config.asset
│   │       └── MiniBoss_Heavy_Brute.asset
│   │
│   ├── Cinematics/                     ← Configuraciones de cinemáticas
│   │   ├── Intros/
│   │   │   ├── Level1_Intro.asset
│   │   │   ├── Level2_Intro.asset
│   │   │   └── Tutorial_Intro.asset
│   │   │
│   │   ├── Outros/
│   │   │   ├── Level1_Outro.asset
│   │   │   ├── Level2_Outro.asset
│   │   │   └── Tutorial_Complete.asset
│   │   │
│   │   ├── Bosses/
│   │   │   ├── Boss_Level1_Intro.asset
│   │   │   ├── Boss_Level1_Defeat.asset
│   │   │   ├── Boss_Level2_Intro.asset
│   │   │   └── Boss_Level2_Defeat.asset
│   │   │
│   │   └── Stages/
│   │       ├── Stage_Transition_1.asset
│   │       └── Stage_Transition_2.asset
│   │
│   └── Tutorial/                       ← Configuración del tutorial
│       └── GameTutorial.asset
│
├── Dialogues/                          ← Conversaciones del DialogueEditor
│   ├── Cinematics/
│   │   ├── Level1_Intro_Dialogue.asset
│   │   ├── Level2_Intro_Dialogue.asset
│   │   ├── Boss_Level1_Intro_Dialogue.asset
│   │   ├── Boss_Level1_Defeat_Dialogue.asset
│   │   └── Tutorial_Intro_Dialogue.asset
│   │
│   ├── Tutorial/
│   │   └── Tutorial_Conversation.asset
│   │
│   └── NPCs/
│       ├── Merchant_Dialogue.asset
│       ├── Trainer_Dialogue.asset
│       └── Ally_Dialogue.asset
│
└── (Otras carpetas del proyecto...)
```

---

## 📝 Instrucciones Paso a Paso

### 1. Crear la Estructura Base

En Unity, en la pestaña **Project**:

1. Click derecho en `Assets/` → `Create > Folder`
2. Nombrar: `ScriptableObjects`
3. Dentro de `ScriptableObjects`, crear subcarpetas:
   - `Levels`
   - `Stages`
   - `Waves`
   - `Bosses`
   - `Cinematics`
   - `Tutorial`

---

### 2. Crear Subcarpetas Organizadas

#### En `Stages/`:
```
Stages/
├── Level1/
├── Level2/
└── Tutorial/
```

#### En `Waves/`:
```
Waves/
├── Easy/
├── Medium/
└── Hard/
```

#### En `Bosses/`:
```
Bosses/
├── Level1/
└── Level2/
```

#### En `Cinematics/`:
```
Cinematics/
├── Intros/
├── Outros/
├── Bosses/
└── Stages/
```

---

### 3. Crear Assets por Carpeta

#### 📂 `Assets/ScriptableObjects/Waves/Easy/`

1. Click derecho → `Create > CDG > Wave Configuration`
2. Nombrar: `Wave_Tutorial`
3. Configurar:
   ```
   Wave Name: "Tutorial Wave"
   Enemy Prefab: [Tu enemigo básico]
   Total Enemies: 3
   Max Simultaneous: 2
   Spawn Interval: 2.0
   ```

**Repetir** para crear:
- `Wave_Basic_1.asset`
- `Wave_Basic_2.asset`
- `Wave_Intro.asset`

---

#### 📂 `Assets/ScriptableObjects/Stages/Level1/`

1. Click derecho → `Create > CDG > Stage Configuration`
2. Nombrar: `Stage1Config`
3. Configurar:
   ```
   Stage Name: "Stage 1"
   Stage Number: 1
   Waves: [Arrastra Wave_Basic_1, Wave_Basic_2]
   Mini Boss Config: [Opcional]
   ```

**Repetir** para `Stage2Config.asset` y `Stage3Config.asset`

---

#### 📂 `Assets/ScriptableObjects/Bosses/Level1/`

1. Click derecho → `Create > CDG > Boss Configuration`
2. Nombrar: `Boss_Level1_Config`
3. Configurar stats, fases, etc.

---

#### 📂 `Assets/ScriptableObjects/Cinematics/Intros/`

1. Click derecho → `Create > CDG > Cinematic Configuration`
2. Nombrar: `Level1_Intro`
3. Configurar:
   ```
   Cinematic Type: Dialogue
   Dialogue Conversation: [Tu diálogo de DialogueEditor]
   Lock Camera: ✓
   Can Skip: ✓
   ```

---

#### 📂 `Assets/ScriptableObjects/Levels/`

1. Click derecho → `Create > CDG > Level Configuration`
2. Nombrar: `Level1Config`
3. Configurar:
   ```
   Level Name: "Level 1"
   Intro Cinematic: [Level1_Intro]
   Stages: [Stage1Config, Stage2Config, Stage3Config]
   Boss Config: [Boss_Level1_Config]
   Outro Cinematic: [Level1_Outro]
   ```

---

## 🎯 Convenciones de Nombres

### Levels:
- `Level1Config.asset`
- `Level2Config.asset`
- `TutorialLevelConfig.asset`

### Stages:
- `Stage1Config.asset` (dentro de carpeta Level1/)
- `Stage2Config.asset`
- `Stage3Config.asset`

### Waves:
- `Wave_[Dificultad]_[Número].asset`
- Ejemplos:
  - `Wave_Easy_1.asset`
  - `Wave_Medium_Mixed.asset`
  - `Wave_Hard_Elite.asset`

### Bosses:
- `Boss_Level[N]_Config.asset`
- `MiniBoss_[Nombre].asset`
- Ejemplos:
  - `Boss_Level1_Config.asset`
  - `MiniBoss_Elite_Guard.asset`

### Cinematics:
- `[Nivel]_[Tipo].asset`
- Ejemplos:
  - `Level1_Intro.asset`
  - `Boss_Level1_Intro.asset`
  - `Tutorial_Complete.asset`

### Dialogues:
- `[Contexto]_Dialogue.asset`
- Ejemplos:
  - `Level1_Intro_Dialogue.asset`
  - `Boss_Intro_Dialogue.asset`
  - `Tutorial_Conversation.asset`

---

## 📊 Relaciones entre Assets

### Ejemplo: Level 1 Completo

```
Level1Config.asset
├── Intro Cinematic: Level1_Intro.asset
│   └── Dialogue: Level1_Intro_Dialogue.asset
│
├── Stages:
│   ├── Stage1Config.asset
│   │   ├── Waves: [Wave_Easy_1, Wave_Easy_2]
│   │   └── Mini Boss: MiniBoss_Elite_Guard.asset
│   │
│   ├── Stage2Config.asset
│   │   ├── Waves: [Wave_Medium_1, Wave_Medium_2]
│   │   └── Mini Boss: [Ninguno]
│   │
│   └── Stage3Config.asset
│       ├── Waves: [Wave_Hard_1, Wave_Hard_2]
│       └── Mini Boss: MiniBoss_Heavy_Brute.asset
│
├── Boss Config: Boss_Level1_Config.asset
│   ├── Intro Cinematic: Boss_Level1_Intro.asset
│   │   └── Dialogue: Boss_Intro_Dialogue.asset
│   └── Defeat Cinematic: Boss_Level1_Defeat.asset
│       └── Dialogue: Boss_Defeat_Dialogue.asset
│
└── Outro Cinematic: Level1_Outro.asset
    └── Dialogue: Level1_Outro_Dialogue.asset
```

---

## 🔍 Búsqueda Rápida

### Por Tipo:
- **Niveles**: `Assets/ScriptableObjects/Levels/`
- **Stages**: `Assets/ScriptableObjects/Stages/[LevelX]/`
- **Waves**: `Assets/ScriptableObjects/Waves/[Dificultad]/`
- **Bosses**: `Assets/ScriptableObjects/Bosses/[LevelX]/`
- **Cinematics**: `Assets/ScriptableObjects/Cinematics/[Tipo]/`
- **Dialogues**: `Assets/Dialogues/[Contexto]/`

### Por Nivel:
Para encontrar todo relacionado con Level 1:
1. `ScriptableObjects/Levels/Level1Config.asset` ← Punto de entrada
2. `ScriptableObjects/Stages/Level1/` ← Stages
3. `ScriptableObjects/Bosses/Level1/` ← Boss
4. `ScriptableObjects/Cinematics/*/Level1_*.asset` ← Cinemáticas
5. `Dialogues/Cinematics/Level1_*.asset` ← Diálogos

---

## ✅ Checklist de Creación

### Para un Nuevo Nivel:

- [ ] Crear carpeta en `Stages/LevelX/`
- [ ] Crear 3 StageConfigs (uno por stage)
- [ ] Crear WaveConfigs necesarias en `Waves/`
- [ ] Crear BossConfig en `Bosses/LevelX/`
- [ ] Crear MiniBossConfigs (si hay)
- [ ] Crear cinemáticas en `Cinematics/Intros/` y `Cinematics/Outros/`
- [ ] Crear diálogos en DialogueEditor
- [ ] Crear CinematicConfigs que usen los diálogos
- [ ] Crear LevelConfig en `Levels/`
- [ ] Asignar todas las referencias en LevelConfig

---

## 🎨 Visualización en Unity

Tu Project window debería verse así:

```
▼ Assets
  ▼ ScriptableObjects
    ▼ Levels
      • Level1Config
      • TutorialLevelConfig
    ▼ Stages
      ▼ Level1
        • Stage1Config
        • Stage2Config
        • Stage3Config
    ▼ Waves
      ▼ Easy
        • Wave_Tutorial
        • Wave_Basic_1
      ▼ Medium
        • Wave_Mixed_1
    ▼ Bosses
      ▼ Level1
        • Boss_Level1_Config
        • MiniBoss_Elite_Guard
    ▼ Cinematics
      ▼ Intros
        • Level1_Intro
        • Tutorial_Intro
      ▼ Bosses
        • Boss_Level1_Intro
  ▼ Dialogues
    ▼ Cinematics
      • Level1_Intro_Dialogue
    ▼ Tutorial
      • Tutorial_Conversation
```

---

## 💡 Tips de Organización

1. **Prefijos consistentes**: Usa `Level1_`, `Boss_`, `Wave_`, etc.
2. **Subcarpetas por nivel**: Mantén cada nivel autocontenido
3. **Reutilización**: Las waves pueden compartirse entre stages
4. **Nombres descriptivos**: `Wave_Mixed_1` mejor que `Wave1`
5. **Inspector names**: Configura nombres legibles en cada SO

---

## 🚀 Plantillas Recomendadas

### Wave Fácil:
```
Total Enemies: 3-5
Max Simultaneous: 2-3
Spawn Interval: 2-3s
```

### Wave Media:
```
Total Enemies: 5-8
Max Simultaneous: 3-4
Spawn Interval: 1.5-2s
```

### Wave Difícil:
```
Total Enemies: 8-12
Max Simultaneous: 4-5
Spawn Interval: 1-1.5s
```

---

**¡Sigue esta estructura para mantener tu proyecto organizado y escalable! 📁✨**
