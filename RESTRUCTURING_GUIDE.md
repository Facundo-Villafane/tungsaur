# 🎮 Beat Em Up 2D - Sistema de Managers Completo

## 📋 Resumen de Implementación

Se ha creado una arquitectura completa y escalable para tu beat em up 2D con los siguientes sistemas:

### ✅ Sistemas Implementados

1. **LevelFlowManager** - Control del flujo completo del nivel
2. **CinematicsManager** - Sistema de cinemáticas con Timeline y Diálogos
3. **UIManager** - Sistema de UI completo (HUD, barras de vida, menús)
4. **WaveManager** - Sistema de oleadas avanzado
5. **BossController** - Controlador de jefes con fases
6. **TutorialManager** - Sistema de tutorial guiado
7. **AudioManager** - Control centralizado de audio

---

## 🗂️ Estructura de Archivos Creados

```
Assets/Scripts/
├── ScriptableObjects/
│   ├── LevelConfigSO.cs           # Configuración completa de nivel
│   ├── StageConfigSO.cs           # Configuración de stage individual
│   ├── WaveConfigSO.cs            # Configuración de oleada
│   ├── CinematicConfigSO.cs       # Configuración de cinemática
│   ├── BossConfigSO.cs            # Configuración de jefe
│   ├── MiniBossConfigSO.cs        # Configuración de mini-jefe
│   └── TutorialConfigSO.cs        # Configuración de tutorial
│
├── Managers/
│   ├── LevelFlowManager.cs        # Manager principal del flujo
│   ├── CinematicsManager.cs       # Manager de cinemáticas
│   ├── WaveManager.cs             # Manager de oleadas
│   ├── TutorialManager.cs         # Manager de tutorial
│   └── AudioManager.cs            # Manager de audio
│
├── Characters/
│   └── Boss/
│       └── BossController.cs      # Controlador de jefe
│
└── UI/
    ├── UIManager.cs               # Manager central de UI
    ├── HealthBarUI.cs             # Barra de vida reusable
    ├── ScoreUI.cs                 # Visualización de score
    ├── ObjectiveUI.cs             # Texto de objetivos
    └── TutorialUI.cs              # UI del tutorial
```

---

## 🎯 Flujo del Juego

Tu juego seguirá esta secuencia:

```
Cinemática Intro
    ↓
Tutorial (con oleada de práctica)
    ↓
Cinemática post-tutorial
    ↓
Nivel 1 - Stage 1 (waves + mini-boss)
    ↓
Nivel 1 - Stage 2 (waves + mini-boss)
    ↓
Nivel 1 - Stage 3 (waves + mini-boss)
    ↓
Boss Fight (con fases)
    ↓
Cinemática Outro
```

---

## 🛠️ Cómo Configurar Todo

### 1. Crear ScriptableObjects

En Unity, crea los siguientes assets:

#### 📁 Nivel Completo
1. Click derecho en Project → `Create > CDG > Level Configuration`
2. Nombra: `Level1Config`
3. Configura:
   - Level Name: "Level 1"
   - Level Number: 1
   - Intro Cinematic: (asignar después)
   - Stages: (arrastrar 3 StageConfigs)
   - Boss Config: (asignar después)
   - Level Music: (tu audio clip)

#### 📁 Stages Individuales
1. Click derecho → `Create > CDG > Stage Configuration`
2. Crea 3 stages: `Stage1Config`, `Stage2Config`, `Stage3Config`
3. Para cada stage:
   - Stage Name: "Stage 1/2/3"
   - Waves: (arrastrar WaveConfigs)
   - Mini Boss Config: (opcional)

#### 📁 Waves (Oleadas)
1. Click derecho → `Create > CDG > Wave Configuration`
2. Crea múltiples waves: `Wave1_Easy`, `Wave2_Medium`, etc.
3. Configura:
   - Enemy Prefab: Tu prefab de enemigo
   - Total Enemies: 5
   - Max Simultaneous Enemies: 3
   - Spawn Interval: 2 segundos

#### 📁 Boss
1. Click derecho → `Create > CDG > Boss Configuration`
2. Nombra: `Level1BossConfig`
3. Configura:
   - Boss Name: "Boss Name"
   - Boss Prefab: Tu prefab de jefe
   - Max Health: 500
   - Phase Thresholds: `0.75, 0.5, 0.25` (4 fases)
   - Special Attack Cooldown: 10 segundos

#### 📁 Cinemáticas
1. Click derecho → `Create > CDG > Cinematic Configuration`
2. Crea: `IntroCinematic`, `TutorialCinematic`, `BossCinematic`, etc.
3. Configura:
   - Cinematic Type: `Dialogue` o `Timeline`
   - Dialogue Conversation: Arrastra el **prefab GameObject** con NPCConversation desde `Assets/Dialogues/`
   - Timeline Asset: (si usas Timeline en vez de Dialogue)
   - Can Skip: ✓

#### 📁 Tutorial
1. Click derecho → `Create > CDG > Tutorial Configuration`
2. Configura:
   - Tutorial Steps:
     ```
     "Welcome to the game!"
     "Use WASD to move"
     "Press J to punch, K to kick"
     "Defeat these enemies!"
     ```
   - Input Prompts:
     ```
     "WASD - Move"
     "J - Punch | K - Kick"
     "Space - Jump"
     ```
   - Practice Wave: (asignar una WaveConfig fácil)
   - Player Invulnerable: ✓

---

### 2. Setup de Escena

#### A. Managers (crear GameObjects vacíos)

1. **LevelFlowManager**
   ```
   Crear GameObject: "LevelFlowManager"
   Agregar componente: LevelFlowManager.cs
   Asignar: Level Config SO
   ```

2. **CinematicsManager**
   ```
   Crear GameObject: "CinematicsManager"
   Agregar componente: CinematicsManager.cs
   Asignar:
   - Playable Director (Timeline)
   - Cinematic Camera
   - Skip Prompt UI
   - Fade Canvas Group
   ```

3. **UIManager**
   ```
   Crear GameObject: "UIManager"
   Agregar componente: UIManager.cs
   Crear UI Canvas con:
   - HUD Panel
   - Player Health Bar (HealthBarUI.cs)
   - Boss Health Bar (HealthBarUI.cs)
   - Score UI (ScoreUI.cs)
   - Objective UI (ObjectiveUI.cs)
   - Pause Menu Panel
   - Game Over Panel
   - Level Complete Panel
   ```

4. **WaveManager** (uno por stage)
   ```
   Crear GameObject: "WaveManager_Stage1"
   Agregar componente: WaveManager.cs
   Asignar:
   - Waves (lista de WaveConfigSO)
   - Spawn Points (transforms vacíos)
   - Spawn Radius: 2
   ```

5. **TutorialManager**
   ```
   Crear GameObject: "TutorialManager"
   Agregar componente: TutorialManager.cs
   Asignar:
   - Tutorial Config SO
   - Tutorial UI
   - Practice Wave Manager
   ```

6. **AudioManager**
   ```
   Crear GameObject: "AudioManager" (DontDestroyOnLoad)
   Agregar componente: AudioManager.cs
   Se autoconfigura automáticamente
   ```

#### B. UI Setup

Crea un Canvas con estos elementos:

```
Canvas
├── HUD Panel
│   ├── Player Health Bar
│   ├── Score Text
│   └── Objective Text
│
├── Boss Health Bar (inicialmente oculto)
│
├── Pause Menu Panel (inicialmente oculto)
│   ├── Resume Button
│   ├── Restart Button
│   └── Main Menu Button
│
├── Game Over Panel (inicialmente oculto)
│
├── Level Complete Panel (inicialmente oculto)
│
└── Tutorial Panel (inicialmente oculto)
    ├── Instruction Text
    ├── Input Prompt Text
    └── Advance Prompt ("Press SPACE")
```

---

## 💡 Cómo Usar en Código

### Iniciar el Nivel

El **LevelFlowManager** se inicia automáticamente en `Start()`:

```csharp
// Se ejecuta automáticamente
// Secuencia: Cinemática Intro → Stages → Boss → Cinemática Outro
```

### Reproducir Cinemática Manualmente

```csharp
CinematicsManager.Instance.PlayCinematic(cinematicConfig, () => {
    Debug.Log("Cinemática terminada!");
});
```

### Actualizar UI

```csharp
// Actualizar vida del jugador
UIManager.Instance.UpdatePlayerHealth(currentHealth, maxHealth);

// Mostrar objetivo
UIManager.Instance.ShowObjective("Defeat all enemies!");

// Mostrar barra de jefe
UIManager.Instance.ShowBossHealthBar("Boss Name", maxHealth);
```

### Reproducir Audio

```csharp
// Música con crossfade
AudioManager.Instance.PlayMusic(levelMusicClip, crossfade: true);

// Efectos de sonido
AudioManager.Instance.PlaySFX(punchSoundClip);

// Música de jefe
AudioManager.Instance.PlayMusic(bossMusicClip, crossfade: true);
```

### Iniciar Tutorial

```csharp
TutorialManager.Instance.StartTutorial();
```

---

## 🔧 Integración con Sistemas Existentes

### Conectar con StageManager

Tu `StageManager` existente puede trabajar con el nuevo `LevelFlowManager`:

```csharp
// En LevelFlowManager, reemplazar el PlayStage placeholder con:
StageManager.Instance.StartStage(stageIndex);
```

### Conectar con EnemySingleSpawner_Safe

El `WaveManager` reemplaza tu spawner actual. Para migrar:

1. Usar `WaveManager` en vez de `EnemySingleSpawner_Safe`
2. O integrar `WaveManager` dentro de `StageZone`

### Conectar PlayerController con UIManager

Agregar a `PlayerController.cs`:

```csharp
void Start() {
    if (UIManager.Instance != null) {
        UIManager.Instance.UpdatePlayerHealth(CurrentHealth, MaxHealth);
    }
}

public override void TakeDamage(float damage) {
    base.TakeDamage(damage);

    if (UIManager.Instance != null) {
        UIManager.Instance.UpdatePlayerHealth(CurrentHealth, MaxHealth);
    }
}
```

---

## 🎨 Mejores Prácticas

### ✅ DO's

- **Usar ScriptableObjects** para TODA la configuración (niveles, waves, jefes)
- **Serialized Fields** para referencias en Inspector
- **Eventos** para comunicación entre managers
- **Placeholders** mientras no tengas assets finales
- Probar cada sistema individualmente antes de conectarlos

### ❌ DON'Ts

- No hardcodear valores en código
- No instanciar managers manualmente (son Singletons)
- No destruir managers con DontDestroyOnLoad
- No olvidar asignar referencias en Inspector

---

## 🧪 Testing Checklist

### 1. Audio
- [ ] Música se reproduce correctamente
- [ ] Crossfade funciona
- [ ] SFX se escuchan

### 2. UI
- [ ] Barra de vida del jugador se actualiza
- [ ] Barra de vida del jefe aparece/desaparece
- [ ] Score aumenta correctamente
- [ ] Objetivos se muestran

### 3. Cinemáticas
- [ ] Cinemáticas se reproducen
- [ ] Se pueden saltar con Space
- [ ] Cámara se bloquea durante cinemáticas
- [ ] Fade in/out funcionan

### 4. Tutorial
- [ ] Pasos se muestran correctamente
- [ ] Oleada de práctica funciona
- [ ] Jugador es invulnerable
- [ ] Se completa correctamente

### 5. Oleadas
- [ ] Enemigos se spawnean correctamente
- [ ] Respetan max simultáneos
- [ ] Se completan las waves
- [ ] Mini-jefes aparecen

### 6. Boss
- [ ] Jefe aparece correctamente
- [ ] Fases cambian según vida
- [ ] Ataques especiales funcionan
- [ ] Barra de vida se actualiza

### 7. Flujo del Nivel
- [ ] Intro → Tutorial → Stages → Boss → Outro
- [ ] Transiciones suaves
- [ ] No hay errores en consola

---

## 📦 Próximos Pasos Recomendados

1. **Crear prefabs de UI** para reusar en todas las escenas
2. **Configurar Timeline** para cinemáticas complejas
3. **Crear mini-boss prefabs** extendiendo `EnemyController`
4. **Diseñar ataques especiales** para cada fase del jefe
5. **Agregar sistema de guardado** para progreso
6. **Implementar sistema de desbloqueo** de niveles

---

## 🐛 Debugging

Si algo no funciona:

1. **Check Console** - Todos los managers tienen logs detallados
2. **Verificar Singletons** - Solo debe haber UNA instancia
3. **Inspector** - Todas las referencias deben estar asignadas
4. **Eventos** - Suscripciones/desuscripciones correctas

---

## 📞 Arquitectura en Resumen

```
LevelFlowManager (Cerebro)
    ↓
├── CinematicsManager (Cutscenes)
├── StageManager (Stages/Zones)
├── WaveManager (Spawning)
├── BossController (Boss fights)
├── TutorialManager (Tutorial)
├── UIManager (HUD/Menus)
└── AudioManager (Music/SFX)
```

**Todo está desacoplado con eventos, ScriptableObjects, y Singletons.**

---

## 🎓 Ventajas de esta Arquitectura

✅ **Escalable** - Agregar niveles = crear ScriptableObjects
✅ **Limpia** - Separación clara de responsabilidades
✅ **Flexible** - Fácil modificar comportamiento sin tocar código
✅ **Reusable** - Componentes se pueden usar en múltiples escenas
✅ **Testeable** - Cada sistema funciona independientemente

---

## 💬 Integración con DialogueEditor

### ✅ DialogueEditor - Completamente Integrado

Tu **DialogueEditor** está 100% integrado en el nuevo sistema y es el medio principal para mostrar texto al jugador.

### Dónde se Usa:

#### 1. **Cinemáticas** (CinematicConfigSO)
```
Create > CDG > Cinematic Configuration
├─ Cinematic Type: Dialogue  ← Selecciona esto
└─ Dialogue Conversation: Arrastra el prefab GameObject desde Assets/Dialogues/
```

**Resultado**: Las cinemáticas usan `ConversationManager` para mostrar diálogos.

#### 2. **Tutorial** (TutorialConfigSO)
```
Create > CDG > Tutorial Configuration
├─ Display Mode: DialogueEditor  ← Selecciona esto
└─ Tutorial Conversation: Arrastra el prefab GameObject desde Assets/Dialogues/Tutorial/
```

**Resultado**: El tutorial usa diálogos completos con portraits y branching.

#### 3. **Boss Intro/Defeat** (BossConfigSO)
```
Create > CDG > Boss Configuration
├─ Intro Cinematic: [CinematicConfig con diálogo]
└─ Defeat Cinematic: [CinematicConfig con diálogo]
```

**Resultado**: Cinemáticas automáticas cuando el boss aparece/muere.

### Flujo Completo con Diálogos:

```
[DialogueEditor: Intro] → Cinemática Intro
    ↓
[DialogueEditor: Tutorial] → Tutorial guiado
    ↓
[DialogueEditor: Post-Tutorial] → Transición
    ↓
Nivel 1 (Stages + Waves)
    ↓
[DialogueEditor: Boss Intro] → Boss aparece
    ↓
Boss Fight
    ↓
[DialogueEditor: Boss Defeat] → Victoria
    ↓
[DialogueEditor: Outro] → Final del nivel
```

### 📚 Documentación Detallada:

Para más información sobre cómo usar DialogueEditor:
- Ver **[DIALOGUE_INTEGRATION_GUIDE.md](DIALOGUE_INTEGRATION_GUIDE.md)**

---

## 🛠️ Tool de Unity Editor

### Crear Estructura de Carpetas Automáticamente

Incluido: **FolderStructureCreator** (Unity Editor Tool)

**Ubicación:** `Tools > CDG > Create ScriptableObject Folders`

**Función:**
- Crea automáticamente toda la estructura de carpetas recomendada
- Configurable (número de niveles)
- No duplica carpetas existentes

**Crea:**
```
Assets/ScriptableObjects/
├── Levels/
├── Stages/Level1/, Level2/, ...
├── Waves/Easy/, Medium/, Hard/
├── Bosses/Level1/, Level2/, ...
├── Cinematics/Intros/, Outros/, Bosses/
└── Tutorial/

Assets/Dialogues/
├── Cinematics/
├── Tutorial/
└── NPCs/
```

---

## 🐛 Errores Corregidos

Durante la implementación se corrigieron estos errores:

1. ✅ **Missing DialogueEditor imports** - Agregado `using DialogueEditor;`
2. ✅ **Invalid BossController overrides** - Cambiado a métodos privados
3. ✅ **Deprecated FindObjectOfType** - Actualizado a `FindFirstObjectByType`
4. ✅ **SetCameraState → ChangeCameraState** - Nombre de método corregido

**Resultado**: Compilación sin errores ✅

---

## 📖 Guías Adicionales

Este proyecto incluye 6 guías completas:

1. **RESTRUCTURING_GUIDE.md** (este archivo) - Guía general
2. **[BOSS_SETUP_GUIDE.md](BOSS_SETUP_GUIDE.md)** - Configurar bosses paso a paso
3. **[ENEMY_CONFIGURATION_VISUAL.md](ENEMY_CONFIGURATION_VISUAL.md)** - Diagramas visuales
4. **[DIALOGUE_INTEGRATION_GUIDE.md](DIALOGUE_INTEGRATION_GUIDE.md)** - DialogueEditor completo
5. **[FOLDER_STRUCTURE_GUIDE.md](FOLDER_STRUCTURE_GUIDE.md)** - Organización de carpetas
6. **[IMPLEMENTATION_SUMMARY.md](IMPLEMENTATION_SUMMARY.md)** - Resumen técnico completo

---

**¡Todo listo para empezar a configurar tu juego! 🚀**
