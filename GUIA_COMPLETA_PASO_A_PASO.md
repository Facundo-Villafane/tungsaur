# 🎮 GUÍA COMPLETA - Configurar tu Juego Desde CERO

**Esta es LA ÚNICA guía que necesitas seguir.** Todo en orden, paso a paso.

---

## 📋 ANTES DE EMPEZAR

**Lo que ya deberías tener:**
- Unity abierto con tu proyecto
- DialogueEditor instalado desde Asset Store
- Los scripts de managers ya importados (LevelFlowManager, CinematicsManager, etc.)

**Tiempo estimado:** 1-2 horas

---

## PARTE 1: CREAR CARPETAS (5 minutos)

### 1.1 - Estructura de Project

En la ventana **Project**, crea esta estructura de carpetas:

```
Assets/
├── Dialogues/
│   ├── Cinematics/
│   └── Tutorial/
├── ScriptableObjects/
│   ├── Cinematics/
│   │   ├── Intros/
│   │   ├── Outros/
│   │   └── Bosses/
│   ├── Waves/
│   ├── Bosses/
│   ├── Tutorial/
│   └── Levels/
└── Prefabs/
    ├── Enemies/
    └── Managers/
```

**Cómo crear carpetas:**
- Click derecho en Project → `Create > Folder`
- Nombra cada carpeta exactamente como se muestra arriba

✅ **Resultado:** Estructura de carpetas lista.

---

## PARTE 2: CREAR DIÁLOGOS (20 minutos)

### 2.1 - Intro del Nivel

#### A. Crear GameObject de conversación

1. En **Hierarchy**, click derecho → `Create Empty`
2. Renombra a: `Level1_Intro_Dialogue`
3. Con el objeto seleccionado, **Inspector** → `Add Component`
4. Busca: `NPC Conversation` (viene de DialogueEditor)
5. Click para agregarlo

#### B. Diseñar el diálogo

1. **CON EL GAMEOBJECT SELECCIONADO EN HIERARCHY**, abre: `Window > DialogueEditor`
2. Verás una ventana nueva con un nodo rojo que dice "[Root] Speech node."
3. **Click derecho en ese nodo raíz** → Selecciona `Create Speech`
4. **Click izquierdo** en un espacio vacío para colocar el nuevo nodo
5. Click en el nuevo nodo para seleccionarlo
6. En el panel derecho (propiedades):
   - **Character Name:** Narrador
   - **Dialogue:** "Bienvenido al Nivel 1..."
7. Repite para crear más nodos:
   - Click derecho en el último nodo → `Create Speech`
   - Click izquierdo para colocar
   - Edita el texto

#### C. Guardar como prefab

1. **Arrastra el GameObject** `Level1_Intro_Dialogue` desde **Hierarchy**
2. **Suéltalo** en la carpeta `Assets/Dialogues/Cinematics/` en **Project**
3. Unity crea un prefab (ícono de **cubo azul**)
4. **Elimina** el GameObject de Hierarchy (ya está guardado como prefab)

✅ **Resultado:** Prefab `Level1_Intro_Dialogue` en `Assets/Dialogues/Cinematics/`

---

### 2.2 - Tutorial (Opcional si usas DialogueEditor para tutorial)

**Repite el mismo proceso:**

1. Hierarchy → Create Empty → `Tutorial_Dialogue`
2. Add Component → `NPC Conversation`
3. Window > DialogueEditor (con el objeto seleccionado)
4. Crear nodos:
   - "¡Bienvenido al tutorial!"
   - "Usa WASD para moverte"
   - "Presiona J para atacar"
5. Guardar como prefab en `Assets/Dialogues/Tutorial/`
6. Eliminar de Hierarchy

✅ **Resultado:** Prefab `Tutorial_Dialogue` en `Assets/Dialogues/Tutorial/`

---

### 2.3 - Boss Intro

1. Hierarchy → Create Empty → `Boss_Level1_Intro_Dialogue`
2. Add Component → `NPC Conversation`
3. Window > DialogueEditor
4. Crear nodos:
   - "¡Has llegado hasta aquí!"
   - "¡Prepárate para tu derrota!"
5. Guardar como prefab en `Assets/Dialogues/Cinematics/`
6. Eliminar de Hierarchy

✅ **Resultado:** Prefab `Boss_Level1_Intro_Dialogue` en `Assets/Dialogues/Cinematics/`

---

### 2.4 - Boss Defeat

1. Hierarchy → Create Empty → `Boss_Level1_Defeat_Dialogue`
2. Add Component → `NPC Conversation`
3. Window > DialogueEditor
4. Crear nodos:
   - "¡Imposible...!"
   - "Has ganado... esta vez..."
5. Guardar como prefab en `Assets/Dialogues/Cinematics/`
6. Eliminar de Hierarchy

✅ **Resultado:** Prefab `Boss_Level1_Defeat_Dialogue` en `Assets/Dialogues/Cinematics/`

---

### 2.5 - Outro del Nivel

1. Hierarchy → Create Empty → `Level1_Outro_Dialogue`
2. Add Component → `NPC Conversation`
3. Window > DialogueEditor
4. Crear nodos:
   - "¡Victoria!"
   - "Has completado el Nivel 1"
5. Guardar como prefab en `Assets/Dialogues/Cinematics/`
6. Eliminar de Hierarchy

✅ **Resultado:** Prefab `Level1_Outro_Dialogue` en `Assets/Dialogues/Cinematics/`

---

## PARTE 3: CREAR SCRIPTABLEOBJECTS - CINEMÁTICAS (15 minutos)

### 3.1 - Intro Cinematic

1. En **Project**, navega a: `Assets/ScriptableObjects/Cinematics/Intros/`
2. Click derecho → `Create > CDG > Cinematic Configuration`
3. Nombra: `Level1_Intro_Cinematic`
4. Selecciónalo y en **Inspector** configura:
   - **Cinematic Name:** Level 1 Intro
   - **Cinematic Type:** `Dialogue`
   - **Dialogue Conversation:** Arrastra el prefab `Level1_Intro_Dialogue` desde `Assets/Dialogues/Cinematics/`
   - **Lock Camera:** ✓
   - **Can Skip:** ✓

✅ **Resultado:** `Level1_Intro_Cinematic` configurado

---

### 3.2 - Boss Intro Cinematic

1. En `Assets/ScriptableObjects/Cinematics/Bosses/`
2. Create → `Create > CDG > Cinematic Configuration`
3. Nombra: `Boss_Level1_Intro_Cinematic`
4. Configura:
   - **Cinematic Type:** `Dialogue`
   - **Dialogue Conversation:** Arrastra el prefab `Boss_Level1_Intro_Dialogue`
   - **Lock Camera:** ✓
   - **Can Skip:** ✓

✅ **Resultado:** `Boss_Level1_Intro_Cinematic` configurado

---

### 3.3 - Boss Defeat Cinematic

1. En `Assets/ScriptableObjects/Cinematics/Bosses/`
2. Create → `Create > CDG > Cinematic Configuration`
3. Nombra: `Boss_Level1_Defeat_Cinematic`
4. Configura:
   - **Cinematic Type:** `Dialogue`
   - **Dialogue Conversation:** Arrastra el prefab `Boss_Level1_Defeat_Dialogue`
   - **Can Skip:** ✓

✅ **Resultado:** `Boss_Level1_Defeat_Cinematic` configurado

---

### 3.4 - Outro Cinematic

1. En `Assets/ScriptableObjects/Cinematics/Outros/`
2. Create → `Create > CDG > Cinematic Configuration`
3. Nombra: `Level1_Outro_Cinematic`
4. Configura:
   - **Cinematic Type:** `Dialogue`
   - **Dialogue Conversation:** Arrastra el prefab `Level1_Outro_Dialogue`
   - **Can Skip:** ✓

✅ **Resultado:** `Level1_Outro_Cinematic` configurado

---

## PARTE 4: CREAR SCRIPTABLEOBJECTS - WAVES (10 minutos)

### 4.1 - Wave de Tutorial

1. En `Assets/ScriptableObjects/Waves/`
2. Click derecho → `Create > CDG > Wave Configuration`
3. Nombra: `Wave_Tutorial_Practice`
4. Configura:
   - **Wave Name:** Tutorial Practice
   - **Enemy Prefab:** (tu prefab de enemigo, si lo tienes)
   - **Total Enemies:** 3
   - **Max Simultaneous:** 2
   - **Spawn Interval:** 3

✅ **Resultado:** `Wave_Tutorial_Practice` creado

---

### 4.2 - Waves para Stage 1

Crea 2 waves:

**Wave 1:**
1. En `Assets/ScriptableObjects/Waves/`
2. Create → `Create > CDG > Wave Configuration`
3. Nombra: `Wave_Stage1_Easy`
4. Configura:
   - **Total Enemies:** 4
   - **Max Simultaneous:** 2

**Wave 2:**
1. Create → `Create > CDG > Wave Configuration`
2. Nombra: `Wave_Stage1_Medium`
3. Configura:
   - **Total Enemies:** 6
   - **Max Simultaneous:** 3

✅ **Resultado:** 2 waves para Stage 1 creadas

---

## PARTE 5: CREAR SCRIPTABLEOBJECTS - TUTORIAL (5 minutos)

### 5.1 - Tutorial Config

1. En `Assets/ScriptableObjects/Tutorial/`
2. Create → `Create > CDG > Tutorial Configuration`
3. Nombra: `GameTutorial`
4. Configura:

**Opción A - Texto Simple:**
- **Display Mode:** `SimpleText`
- **Tutorial Steps:** (agregar 3 elementos)
  - "¡Bienvenido!"
  - "Usa WASD para moverte"
  - "Presiona J para atacar"
- **Practice Wave:** Arrastra `Wave_Tutorial_Practice`
- **Player Invulnerable:** ✓

**Opción B - DialogueEditor:**
- **Display Mode:** `DialogueEditor`
- **Tutorial Conversation:** Arrastra el prefab `Tutorial_Dialogue` desde `Assets/Dialogues/Tutorial/`
- **Practice Wave:** Arrastra `Wave_Tutorial_Practice`
- **Player Invulnerable:** ✓

✅ **Resultado:** `GameTutorial` configurado

---

## PARTE 6: CREAR SCRIPTABLEOBJECTS - STAGES (10 minutos)

### 6.1 - Stage 1 Config

1. En `Assets/ScriptableObjects/` (puedes crear subcarpeta `Stages/`)
2. Create → `Create > CDG > Stage Configuration`
3. Nombra: `Stage1_Config`
4. Configura:
   - **Stage Name:** Stage 1
   - **Stage Number:** 1
   - **Waves:** (Size = 2)
     - Element 0: Arrastra `Wave_Stage1_Easy`
     - Element 1: Arrastra `Wave_Stage1_Medium`

✅ **Resultado:** `Stage1_Config` creado

---

### 6.2 - Stages 2 y 3 (Copias)

1. Selecciona `Stage1_Config` en Project
2. **Ctrl+D** para duplicar
3. Renombra: `Stage2_Config`
4. Cambia:
   - **Stage Name:** Stage 2
   - **Stage Number:** 2

5. Duplica nuevamente para `Stage3_Config`
6. Cambia:
   - **Stage Name:** Stage 3
   - **Stage Number:** 3

✅ **Resultado:** 3 stages configurados

---

## PARTE 7: CREAR SCRIPTABLEOBJECTS - BOSS (10 minutos)

### 7.1 - Boss Config

1. En `Assets/ScriptableObjects/Bosses/`
2. Create → `Create > CDG > Boss Configuration`
3. Nombra: `Boss_Level1_Config`
4. Configura:
   - **Boss Name:** Boss del Nivel 1
   - **Max Health:** 500
   - **Intro Cinematic:** Arrastra `Boss_Level1_Intro_Cinematic`
   - **Defeat Cinematic:** Arrastra `Boss_Level1_Defeat_Cinematic`
   - **Show Health Bar:** ✓
   - **Phase Thresholds:** (Size = 3)
     - Element 0: 0.75
     - Element 1: 0.5
     - Element 2: 0.25

✅ **Resultado:** `Boss_Level1_Config` configurado

---

## PARTE 8: CREAR SCRIPTABLEOBJECT - LEVEL (10 minutos)

### 8.1 - Level 1 Config (EL MÁS IMPORTANTE)

Este conecta TODO.

1. En `Assets/ScriptableObjects/Levels/`
2. Create → `Create > CDG > Level Configuration`
3. Nombra: `Level1_Config`
4. Configura:

```
┌─────────────────────────────────────────┐
│ LEVEL INFO                              │
├─────────────────────────────────────────┤
│ Level Name: Level 1                     │
│ Level Number: 1                         │
│                                         │
│ CINEMATICS                              │
├─────────────────────────────────────────┤
│ Intro Cinematic:                        │
│   → [Level1_Intro_Cinematic]            │
│                                         │
│ Outro Cinematic:                        │
│   → [Level1_Outro_Cinematic]            │
│                                         │
│ STAGES                                  │
├─────────────────────────────────────────┤
│ Stages: (Size = 3)                      │
│   Element 0: [Stage1_Config]            │
│   Element 1: [Stage2_Config]            │
│   Element 2: [Stage3_Config]            │
│                                         │
│ BOSS                                    │
├─────────────────────────────────────────┤
│ Boss Config:                            │
│   → [Boss_Level1_Config]                │
│                                         │
│ REWARDS                                 │
├─────────────────────────────────────────┤
│ Glory Reward: 100                       │
│ Score Reward: 1000                      │
└─────────────────────────────────────────┘
```

**Cómo asignar cada uno:**
- Click en el círculo al lado del campo
- O arrastra el asset desde Project

✅ **Resultado:** `Level1_Config` - TODO CONECTADO

---

## PARTE 9: CONFIGURAR HIERARCHY - MANAGERS (30 minutos)

Ahora configuramos la escena con los managers.

### 9.1 - GameManager (si no lo tienes)

1. Hierarchy → Create Empty
2. Renombra: `GameManager`
3. Add Component → `Game Manager`

✅ **Resultado:** GameManager en escena

---

### 9.2 - LevelFlowManager

1. Hierarchy → Create Empty
2. Renombra: `LevelFlowManager`
3. Add Component → `Level Flow Manager`
4. En Inspector:
   - **Level Config:** Arrastra `Level1_Config` desde Project
   - **Skip Cinematics:** □ (dejar sin marcar)

✅ **Resultado:** LevelFlowManager en escena

---

### 9.3 - CinematicsManager

1. Hierarchy → Create Empty
2. Renombra: `CinematicsManager`
3. Add Component → `Cinematics Manager`
4. En Inspector:
   - **Fade Panel:** (dejar None por ahora)
   - **Skip Prompt UI:** (dejar None por ahora)
   - **Cinematic Camera Object:** (dejar None por ahora)
   - **Playable Director:** (dejar None por ahora)

✅ **Resultado:** CinematicsManager en escena

---

### 9.4 - TutorialManager

1. Hierarchy → Create Empty
2. Renombra: `TutorialManager`
3. Add Component → `Tutorial Manager`
4. En Inspector:
   - **Tutorial Config:** Arrastra `GameTutorial` desde Project
   - **Tutorial UI:** (dejar None por ahora)
   - **Practice Wave Manager:** (crear después)

✅ **Resultado:** TutorialManager en escena

---

### 9.5 - UIManager

1. Hierarchy → Create Empty
2. Renombra: `UIManager`
3. Add Component → `UI Manager`

✅ **Resultado:** UIManager en escena

---

### 9.6 - AudioManager

1. Hierarchy → Create Empty
2. Renombra: `AudioManager`
3. Add Component → `Audio Manager`

✅ **Resultado:** AudioManager en escena

---

### 9.7 - ConversationManager (DialogueEditor)

**IMPORTANTE:** Este prefab viene con DialogueEditor.

1. Busca en tus assets el prefab `ConversationManager` (puede estar en `Assets/DialogueEditor/`)
2. Arrástralo a Hierarchy
3. **Debe ser hijo de un Canvas**:
   - Si no tienes Canvas: Hierarchy → Create → UI → Canvas
   - Arrastra ConversationManager para que sea hijo del Canvas

✅ **Resultado:** ConversationManager en escena (hijo de Canvas)

---

### 9.8 - WaveManager para Tutorial

1. Hierarchy → Create Empty
2. Renombra: `WaveManager_Tutorial`
3. Add Component → `Wave Manager`
4. En Inspector:
   - **Spawn Points:** (crear después)
   - **Enemy Container:** (crear después)

5. **Asignar al TutorialManager:**
   - Selecciona `TutorialManager` en Hierarchy
   - En Inspector, busca **Practice Wave Manager**
   - Arrastra `WaveManager_Tutorial` desde Hierarchy

✅ **Resultado:** WaveManager_Tutorial en escena y asignado

---

### 9.9 - Enemy Container

1. Hierarchy → Create Empty
2. Renombra: `EnemyContainer`
3. **Asignar al WaveManager:**
   - Selecciona `WaveManager_Tutorial`
   - En Inspector, campo **Enemy Container**
   - Arrastra `EnemyContainer` desde Hierarchy

✅ **Resultado:** EnemyContainer en escena y asignado

---

### 9.10 - Spawn Points

1. Hierarchy → Create Empty
2. Renombra: `SpawnPoint_1`
3. Mueve el transform a una posición en tu escena (ej: X=5, Y=0, Z=0)
4. Duplica (Ctrl+D) 2 veces más:
   - `SpawnPoint_2` (ej: X=-5, Y=0, Z=0)
   - `SpawnPoint_3` (ej: X=0, Y=0, Z=5)

5. **Asignar al WaveManager:**
   - Selecciona `WaveManager_Tutorial`
   - En Inspector, campo **Spawn Points** → Size = 3
   - Arrastra cada SpawnPoint a cada elemento

✅ **Resultado:** 3 spawn points en escena y asignados

---

## RESUMEN DE HIERARCHY (Lo que deberías tener)

```
Hierarchy:
├── Canvas
│   └── ConversationManager  ← Prefab de DialogueEditor
├── GameManager
├── LevelFlowManager         ← Tiene Level1_Config asignado
├── CinematicsManager
├── TutorialManager          ← Tiene GameTutorial asignado
├── UIManager
├── AudioManager
├── WaveManager_Tutorial     ← Tiene spawn points y container asignados
├── EnemyContainer
├── SpawnPoint_1
├── SpawnPoint_2
└── SpawnPoint_3
```

---

## RESUMEN DE PROJECT (Lo que deberías tener)

```
Assets/
├── Dialogues/
│   ├── Cinematics/
│   │   ├── Level1_Intro_Dialogue (prefab)
│   │   ├── Boss_Level1_Intro_Dialogue (prefab)
│   │   ├── Boss_Level1_Defeat_Dialogue (prefab)
│   │   └── Level1_Outro_Dialogue (prefab)
│   └── Tutorial/
│       └── Tutorial_Dialogue (prefab - opcional)
│
└── ScriptableObjects/
    ├── Cinematics/
    │   ├── Intros/
    │   │   └── Level1_Intro_Cinematic
    │   ├── Outros/
    │   │   └── Level1_Outro_Cinematic
    │   └── Bosses/
    │       ├── Boss_Level1_Intro_Cinematic
    │       └── Boss_Level1_Defeat_Cinematic
    │
    ├── Waves/
    │   ├── Wave_Tutorial_Practice
    │   ├── Wave_Stage1_Easy
    │   └── Wave_Stage1_Medium
    │
    ├── Tutorial/
    │   └── GameTutorial
    │
    ├── Stages/ (opcional subcarpeta)
    │   ├── Stage1_Config
    │   ├── Stage2_Config
    │   └── Stage3_Config
    │
    ├── Bosses/
    │   └── Boss_Level1_Config
    │
    └── Levels/
        └── Level1_Config  ← EL MÁS IMPORTANTE
```

---

## ✅ CHECKLIST FINAL

Antes de dar Play, verifica que tienes:

**EN PROJECT:**
- [ ] 5 prefabs de diálogo en `Assets/Dialogues/`
- [ ] 4 CinematicConfigSO en `Assets/ScriptableObjects/Cinematics/`
- [ ] 3 WaveConfigSO en `Assets/ScriptableObjects/Waves/`
- [ ] 1 TutorialConfigSO en `Assets/ScriptableObjects/Tutorial/`
- [ ] 3 StageConfigSO
- [ ] 1 BossConfigSO en `Assets/ScriptableObjects/Bosses/`
- [ ] 1 LevelConfigSO en `Assets/ScriptableObjects/Levels/` (CON TODO ASIGNADO)

**EN HIERARCHY:**
- [ ] GameManager
- [ ] LevelFlowManager (con Level1_Config asignado)
- [ ] CinematicsManager
- [ ] TutorialManager (con GameTutorial asignado)
- [ ] UIManager
- [ ] AudioManager
- [ ] Canvas > ConversationManager
- [ ] WaveManager_Tutorial (con spawn points asignados)
- [ ] EnemyContainer
- [ ] 3 SpawnPoints

---

## 🎮 PROBAR EL JUEGO

1. **Guarda la escena** (Ctrl+S)
2. **Dale Play**
3. **Debería suceder:**
   - Se muestra el diálogo de intro (Level1_Intro_Dialogue)
   - Luego inicia el tutorial
   - Aparecen enemigos de práctica
   - Continúa el flujo del nivel

**Si algo no funciona:**
- Revisa Console para errores
- Verifica que todos los campos están asignados (no deben decir "None")
- Asegúrate de que ConversationManager está en la escena

---

## 🐛 PROBLEMAS COMUNES

### "Diálogo no se muestra"
- ✅ Verifica que ConversationManager está en Hierarchy (hijo de Canvas)
- ✅ Verifica que el CinematicConfigSO tiene `Cinematic Type = Dialogue`
- ✅ Verifica que el prefab de diálogo está asignado

### "Type mismatch al asignar diálogo"
- ✅ Debes arrastrar el **prefab GameObject** (cubo azul) desde Project
- ✅ NO arrastres desde Hierarchy
- ✅ NO arrastres el componente NPCConversation

### "Create Speech no aparece"
- ✅ Debes tener el GameObject seleccionado en Hierarchy ANTES de abrir DialogueEditor
- ✅ El GameObject debe tener el componente `NPC Conversation`

---

## ✨ ¡LISTO!

Ahora tienes TODO configurado correctamente. Sigue esta guía paso a paso y todo funcionará.

**Orden de ejecución en runtime:**
1. Intro Cinematic (diálogo)
2. Tutorial (con wave de práctica)
3. Stage 1 (2 waves)
4. Stage 2 (2 waves)
5. Stage 3 (2 waves)
6. Boss Fight (con intro y defeat cinematics)
7. Outro Cinematic (diálogo)

---

**💡 TIP:** Guarda esta escena configurada como una "escena template" para futuros niveles.
