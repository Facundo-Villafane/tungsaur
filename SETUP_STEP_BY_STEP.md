# 🎮 Guía Paso a Paso - Configurar el Flujo Completo del Juego

## 🎯 Objetivo
Crear el flujo completo del juego desde cero:
```
Cinemática Intro → Tutorial → Cinemática Post-Tutorial → Level 1 → Boss → Outro
```

---

## 📋 PARTE 1: PREPARACIÓN (5 minutos)

### Paso 1.1: Crear Estructura de Carpetas

En Unity:

1. En la pestaña **Project**, click en `Assets/`
2. Menu superior: `Tools > CDG > Create ScriptableObject Folders`
3. En la ventana que aparece:
   - Number of Levels: **1** (por ahora)
   - Dejar todas las opciones marcadas ✓
4. Click en **"Create Folder Structure"**

✅ **Resultado**: Se crean todas las carpetas automáticamente.

---

### Paso 1.2: Verificar que Todo Compiló

1. Abrir la consola: `Window > General > Console` (Ctrl+Shift+C)
2. **No debe haber errores rojos** ❌
3. Si hay errores, cópialos y me los envías

✅ **Resultado**: Proyecto compilando sin errores.

---

## 📋 PARTE 2: CINEMÁTICA INICIAL (10 minutos)

### Paso 2.1: Crear Conversación en DialogueEditor

1. Menu: `Window > Dialogue Editor`
2. En la ventana del Dialogue Editor:
   - Click en **"New Conversation"**
   - Aparece un panel vacío

3. **Crear nodos de diálogo:**
   - Click derecho en el espacio vacío → `Create Node > Speech Node`
   - En el nodo creado:
     - **Name:** "Intro_Node1"
     - **Text:** "Bienvenido a Tungsaur..."
     - **Icon:** (dejar vacío por ahora)

4. **Crear más nodos:**
   - Click derecho → `Create Node > Speech Node`
   - **Name:** "Intro_Node2"
   - **Text:** "El mundo está en peligro..."

5. **Conectar nodos:**
   - Click en el círculo de salida del primer nodo
   - Arrastra hasta el círculo de entrada del segundo nodo
   - Aparece una flecha conectándolos

6. **Guardar la conversación:**
   - En la ventana Dialogue Editor: `File > Save As...`
   - Ubicación: `Assets/Dialogues/Cinematics/`
   - Nombre: **"Intro_Cinematic_Dialogue"**
   - Click **Save**

✅ **Resultado**: Conversación `Intro_Cinematic_Dialogue.asset` creada.

---

### Paso 2.2: Crear CinematicConfigSO para la Intro

1. En Project, navega a: `Assets/ScriptableObjects/Cinematics/Intros/`
2. Click derecho → `Create > CDG > Cinematic Configuration`
3. Nombra el asset: **"Level1_Intro_Cinematic"**

4. **Configurar en el Inspector:**
   ```
   ┌─────────────────────────────────────────┐
   │ Cinematic Info                          │
   ├─────────────────────────────────────────┤
   │ Cinematic Name: Level 1 Intro           │
   │                                         │
   │ Playback Type                           │
   │ Cinematic Type: Dialogue                │  ← IMPORTANTE
   │                                         │
   │ Timeline (leave empty)                  │
   │ Timeline Asset: None                    │
   │                                         │
   │ Dialogue                                │
   │ Dialogue Conversation:                  │
   │   [Intro_Cinematic_Dialogue] ← Arrastra│
   │                                         │
   │ Camera                                  │
   │ Lock Camera: ✓                          │
   │                                         │
   │ Skippable                               │
   │ Can Skip: ✓                             │
   │ Skip Key: Space                         │
   │                                         │
   │ Audio                                   │
   │ Background Music: None (por ahora)      │
   └─────────────────────────────────────────┘
   ```

5. **Asignar el diálogo:**
   - Click en el círculo al lado de "Dialogue Conversation"
   - Busca y selecciona: `Intro_Cinematic_Dialogue`
   - O arrastra directamente desde Project

✅ **Resultado**: Cinemática inicial configurada.

---

## 📋 PARTE 3: TUTORIAL (15 minutos)

### Paso 3.1: Crear Wave de Tutorial (Enemigos Fáciles)

1. Navega a: `Assets/ScriptableObjects/Waves/Easy/`
2. Click derecho → `Create > CDG > Wave Configuration`
3. Nombra: **"Wave_Tutorial_Practice"**

4. **Configurar:**
   ```
   ┌─────────────────────────────────────────┐
   │ Wave Info                               │
   ├─────────────────────────────────────────┤
   │ Wave Name: Tutorial Practice            │
   │                                         │
   │ Enemy Configuration                     │
   │ Enemy Prefab: [Tu prefab de Enemy] ←   │
   │ Total Enemies: 3                        │
   │ Max Simultaneous Enemies: 2             │
   │                                         │
   │ Spawn Timing                            │
   │ Initial Delay: 1                        │
   │ Spawn Interval: 3                       │
   │                                         │
   │ Spawn Positions                         │
   │ Use Random Spawn Points: ✓              │
   └─────────────────────────────────────────┘
   ```

5. **Asignar prefab de enemigo:**
   - Click en el círculo de "Enemy Prefab"
   - Selecciona tu prefab `Enemy` de `Assets/Prefabs/`

✅ **Resultado**: Wave de práctica creada.

---

### Paso 3.2: Crear Conversación de Tutorial (Opcional)

**OPCIÓN A: Tutorial con Texto Simple** (más rápido)
- Salta al Paso 3.3 directamente

**OPCIÓN B: Tutorial con DialogueEditor** (más completo)

1. `Window > Dialogue Editor`
2. `File > New Conversation`
3. Crear nodos:
   - **Nodo 1:** "¡Bienvenido al tutorial!"
   - **Nodo 2:** "Usa WASD para moverte"
   - **Nodo 3:** "Presiona J para atacar"
   - **Nodo 4:** "¡Ahora derrota estos enemigos!"

4. Conectar todos los nodos en secuencia
5. `File > Save As...`
   - Ubicación: `Assets/Dialogues/Tutorial/`
   - Nombre: **"Tutorial_Conversation"**

✅ **Resultado**: Conversación de tutorial creada (opcional).

---

### Paso 3.3: Crear TutorialConfigSO

1. Navega a: `Assets/ScriptableObjects/Tutorial/`
2. Click derecho → `Create > CDG > Tutorial Configuration`
3. Nombra: **"GameTutorial"**

4. **Configurar (Opción A - Texto Simple):**
   ```
   ┌─────────────────────────────────────────┐
   │ Tutorial Info                           │
   ├─────────────────────────────────────────┤
   │ Tutorial Title: Tutorial                │
   │                                         │
   │ Tutorial Display Mode                   │
   │ Display Mode: SimpleText                │  ← Simple
   │                                         │
   │ Simple Text Mode                        │
   │ Tutorial Steps: Size 4                  │
   │   Element 0: "¡Bienvenido!"             │
   │   Element 1: "Usa WASD para moverte"    │
   │   Element 2: "Presiona J para atacar"   │
   │   Element 3: "¡Derrota a los enemigos!" │
   │                                         │
   │ Input Display                           │
   │ Show Input Prompts: ✓                   │
   │ Input Prompts: Size 4                   │
   │   Element 0: ""                         │
   │   Element 1: "WASD - Movimiento"        │
   │   Element 2: "J - Ataque"               │
   │   Element 3: ""                         │
   │                                         │
   │ Practice Wave                           │
   │ Practice Wave: [Wave_Tutorial_Practice]│  ← Arrastra
   │                                         │
   │ Tutorial Settings                       │
   │ Player Invulnerable: ✓                  │
   │ Auto Advance: False                     │
   │ Advance Key: Space                      │
   │                                         │
   │ Completion                              │
   │ Completion Cinematic: None (por ahora)  │
   └─────────────────────────────────────────┘
   ```

**O Configurar (Opción B - DialogueEditor):**
   ```
   Display Mode: DialogueEditor
   Tutorial Conversation: [Tutorial_Conversation]  ← Si lo creaste
   Practice Wave: [Wave_Tutorial_Practice]
   Player Invulnerable: ✓
   ```

✅ **Resultado**: Tutorial configurado.

---

### Paso 3.4: Crear Cinemática Post-Tutorial

1. `Window > Dialogue Editor`
2. `File > New Conversation`
3. Crear 2 nodos simples:
   - **Nodo 1:** "¡Bien hecho!"
   - **Nodo 2:** "Ahora comienza la aventura..."

4. Conectar y guardar:
   - Ubicación: `Assets/Dialogues/Cinematics/`
   - Nombre: **"PostTutorial_Dialogue"**

5. Crear CinematicConfigSO:
   - Ubicación: `Assets/ScriptableObjects/Cinematics/Outros/`
   - Nombre: **"Tutorial_Complete_Cinematic"**
   - Cinematic Type: **Dialogue**
   - Dialogue Conversation: **[PostTutorial_Dialogue]**
   - Can Skip: ✓

✅ **Resultado**: Cinemática post-tutorial lista.

---

## 📋 PARTE 4: NIVEL 1 (20 minutos)

### Paso 4.1: Crear Waves para Stage 1

1. Navega a: `Assets/ScriptableObjects/Waves/Easy/`
2. Crear **2 waves básicas**:

**Wave 1:**
   - Nombre: **"Wave_Stage1_Easy1"**
   - Enemy Prefab: [Enemy]
   - Total Enemies: **5**
   - Max Simultaneous: **3**
   - Spawn Interval: **2**

**Wave 2:**
   - Nombre: **"Wave_Stage1_Easy2"**
   - Enemy Prefab: [Enemy]
   - Total Enemies: **6**
   - Max Simultaneous: **3**
   - Spawn Interval: **1.5**

✅ **Resultado**: 2 waves creadas.

---

### Paso 4.2: Crear StageConfigSO para Stage 1

1. Navega a: `Assets/ScriptableObjects/Stages/Level1/`
2. Click derecho → `Create > CDG > Stage Configuration`
3. Nombra: **"Stage1_Config"**

4. **Configurar:**
   ```
   ┌─────────────────────────────────────────┐
   │ Stage Info                              │
   ├─────────────────────────────────────────┤
   │ Stage Name: Stage 1                     │
   │ Stage Number: 1                         │
   │                                         │
   │ Waves                                   │
   │ Waves: Size 2                           │
   │   Element 0: [Wave_Stage1_Easy1]        │
   │   Element 1: [Wave_Stage1_Easy2]        │
   │                                         │
   │ Mini-Boss (Optional)                    │
   │ Mini Boss Config: None (por ahora)      │
   │                                         │
   │ Cinematics (Optional)                   │
   │ Intro Cinematic: None                   │
   │                                         │
   │ Stage Completion                        │
   │ Glory Reward: 50                        │
   │ Score Reward: 500                       │
   └─────────────────────────────────────────┘
   ```

5. **Asignar waves:**
   - Arrastra ambas waves a la lista

✅ **Resultado**: Stage 1 configurado.

---

### Paso 4.3: Crear Stages 2 y 3 (Similar)

**Para ahorrar tiempo, copia Stage1_Config:**

1. En Project, selecciona `Stage1_Config`
2. Ctrl+D para duplicar
3. Renombra a **"Stage2_Config"**
4. En Inspector:
   - Stage Name: **Stage 2**
   - Stage Number: **2**

5. Duplicar nuevamente:
   - Renombra a **"Stage3_Config"**
   - Stage Name: **Stage 3**
   - Stage Number: **3**

✅ **Resultado**: 3 stages listos (todos usan las mismas waves por ahora).

---

## 📋 PARTE 5: BOSS (15 minutos)

### Paso 5.1: Crear Diálogos del Boss

1. `Window > Dialogue Editor`

**Boss Intro:**
   - `File > New Conversation`
   - Crear 2 nodos:
     - **Nodo 1:** "¡Así que has llegado hasta aquí!"
     - **Nodo 2:** "¡Prepárate para la derrota!"
   - Guardar como: `Assets/Dialogues/Cinematics/Boss_Level1_Intro_Dialogue`

**Boss Defeat:**
   - `File > New Conversation`
   - Crear 2 nodos:
     - **Nodo 1:** "¡Imposible...!"
     - **Nodo 2:** "Has ganado... esta vez..."
   - Guardar como: `Assets/Dialogues/Cinematics/Boss_Level1_Defeat_Dialogue`

✅ **Resultado**: Diálogos del boss creados.

---

### Paso 5.2: Crear Cinemáticas del Boss

**Boss Intro Cinematic:**
1. Ubicación: `Assets/ScriptableObjects/Cinematics/Bosses/`
2. `Create > CDG > Cinematic Configuration`
3. Nombre: **"Boss_Level1_Intro_Cinematic"**
4. Configurar:
   - Cinematic Type: **Dialogue**
   - Dialogue Conversation: **[Boss_Level1_Intro_Dialogue]**
   - Lock Camera: ✓
   - Can Skip: ✓

**Boss Defeat Cinematic:**
1. Misma carpeta
2. `Create > CDG > Cinematic Configuration`
3. Nombre: **"Boss_Level1_Defeat_Cinematic"**
4. Configurar:
   - Cinematic Type: **Dialogue**
   - Dialogue Conversation: **[Boss_Level1_Defeat_Dialogue]**

✅ **Resultado**: Cinemáticas del boss listas.

---

### Paso 5.3: Crear BossConfigSO

1. Navega a: `Assets/ScriptableObjects/Bosses/Level1/`
2. `Create > CDG > Boss Configuration`
3. Nombra: **"Boss_Level1_Config"**

4. **Configurar:**
   ```
   ┌─────────────────────────────────────────┐
   │ Boss Info                               │
   ├─────────────────────────────────────────┤
   │ Boss Name: Brutal Boss                  │
   │ Boss Prefab: [Tu prefab de Enemy]  ←    │
   │   (por ahora usa el mismo que Enemy)    │
   │                                         │
   │ Stats                                   │
   │ Max Health: 500                         │
   │ Damage: 30                              │
   │ Defense: 10                             │
   │ Move Speed: 3                           │
   │                                         │
   │ Phases                                  │
   │ Phase Thresholds: Size 3                │
   │   Element 0: 0.75                       │
   │   Element 1: 0.5                        │
   │   Element 2: 0.25                       │
   │                                         │
   │ Phase Names: Size 4                     │
   │   Element 0: "Primera Fase"             │
   │   Element 1: "Segunda Fase"             │
   │   Element 2: "Fase Enraged"             │
   │   Element 3: "Última Resistencia"       │
   │                                         │
   │ Special Attacks                         │
   │ Special Attack Cooldown: 10             │
   │                                         │
   │ Cinematics                              │
   │ Intro Cinematic:                        │
   │   [Boss_Level1_Intro_Cinematic]         │
   │ Defeat Cinematic:                       │
   │   [Boss_Level1_Defeat_Cinematic]        │
   │                                         │
   │ Rewards                                 │
   │ Glory Reward: 200                       │
   │ Score Reward: 2000                      │
   └─────────────────────────────────────────┘
   ```

✅ **Resultado**: Boss configurado.

---

## 📋 PARTE 6: NIVEL COMPLETO (10 minutos)

### Paso 6.1: Crear Cinemática Outro del Nivel

1. `Window > Dialogue Editor`
2. `File > New Conversation`
3. Crear 2 nodos:
   - **Nodo 1:** "¡Victoria!"
   - **Nodo 2:** "Has completado el Nivel 1"

4. Guardar como: `Assets/Dialogues/Cinematics/Level1_Outro_Dialogue`

5. Crear CinematicConfigSO:
   - Ubicación: `Assets/ScriptableObjects/Cinematics/Outros/`
   - Nombre: **"Level1_Outro_Cinematic"**
   - Cinematic Type: **Dialogue**
   - Dialogue Conversation: **[Level1_Outro_Dialogue]**

✅ **Resultado**: Cinemática final lista.

---

### Paso 6.2: Crear LevelConfigSO Principal

🎯 **ESTE ES EL ASSET MÁS IMPORTANTE - CONECTA TODO**

1. Navega a: `Assets/ScriptableObjects/Levels/`
2. `Create > CDG > Level Configuration`
3. Nombra: **"Level1_Config"**

4. **Configurar TODO:**
   ```
   ┌─────────────────────────────────────────┐
   │ Level Info                              │
   ├─────────────────────────────────────────┤
   │ Level Name: Level 1                     │
   │ Level Number: 1                         │
   │                                         │
   │ Cinematics                              │
   │ Intro Cinematic:                        │
   │   [Level1_Intro_Cinematic]         ← 1  │
   │                                         │
   │ Outro Cinematic:                        │
   │   [Level1_Outro_Cinematic]         ← 2  │
   │                                         │
   │ Stages                                  │
   │ Stages: Size 3                          │
   │   Element 0: [Stage1_Config]       ← 3  │
   │   Element 1: [Stage2_Config]       ← 4  │
   │   Element 2: [Stage3_Config]       ← 5  │
   │                                         │
   │ Boss                                    │
   │ Boss Config:                            │
   │   [Boss_Level1_Config]             ← 6  │
   │                                         │
   │ Audio                                   │
   │ Level Music: None (por ahora)           │
   │ Boss Music: None (por ahora)            │
   │                                         │
   │ Rewards                                 │
   │ Glory Reward: 100                       │
   │ Score Reward: 1000                      │
   └─────────────────────────────────────────┘
   ```

5. **Verificar que TODO está conectado:**
   - ✅ Intro Cinematic asignada
   - ✅ 3 Stages asignados
   - ✅ Boss Config asignado
   - ✅ Outro Cinematic asignada

✅ **Resultado**: Nivel completo configurado!

---

## 📋 PARTE 7: CONFIGURAR LA ESCENA (20 minutos)

### Paso 7.1: Crear GameObject LevelFlowManager

1. En Hierarchy, click derecho → `Create Empty`
2. Renombra a: **"LevelFlowManager"**
3. Con el objeto seleccionado, en Inspector:
   - Click en **"Add Component"**
   - Busca: **"LevelFlowManager"**
   - Click para agregarlo

4. **Asignar el nivel:**
   ```
   ┌─────────────────────────────────────────┐
   │ Level Configuration                     │
   │ Level Config: [Level1_Config] ← Arrastra│
   │                                         │
   │ Current State                           │
   │ Current State: Idle                     │
   │                                         │
   │ Debug                                   │
   │ Skip Cinematics: □ (dejar sin marcar)   │
   └─────────────────────────────────────────┘
   ```

✅ **Resultado**: LevelFlowManager en escena.

---

### Paso 7.2: Crear GameObject TutorialManager

1. Hierarchy → `Create Empty`
2. Renombra: **"TutorialManager"**
3. `Add Component` → **"TutorialManager"**

4. **Configurar:**
   ```
   ┌─────────────────────────────────────────┐
   │ Tutorial Configuration                  │
   │ Tutorial Config: [GameTutorial]         │
   │                                         │
   │ UI References                           │
   │ Tutorial UI: None (crear después)       │
   │                                         │
   │ Practice Wave                           │
   │ Practice Wave Manager: None (crear)     │
   └─────────────────────────────────────────┘
   ```

✅ **Resultado**: TutorialManager en escena.

---

### Paso 7.3: Crear WaveManager para Tutorial

1. Hierarchy → `Create Empty`
2. Renombra: **"WaveManager_Tutorial"**
3. `Add Component` → **"WaveManager"**

4. **Configurar:**
   ```
   ┌─────────────────────────────────────────┐
   │ Wave Configuration                      │
   │ Waves: Size 1                           │
   │   Element 0: [Wave_Tutorial_Practice]   │
   │                                         │
   │ Spawn Points                            │
   │ Spawn Points: Size 2                    │
   │   Element 0: [Crear Transform]     →    │
   │   Element 1: [Crear Transform]     →    │
   │ Spawn Radius: 2                         │
   │                                         │
   │ State                                   │
   │ Auto Start: □ (NO marcar)               │
   └─────────────────────────────────────────┘
   ```

5. **Crear Spawn Points:**
   - Hierarchy → `Create Empty` (dentro de WaveManager_Tutorial)
   - Renombra: **"SpawnPoint1"**
   - Position: **(10, 0, 0)** (ajustar según tu escena)
   - Duplicar (Ctrl+D)
   - Renombra: **"SpawnPoint2"**
   - Position: **(-10, 0, 0)**
   - Arrastra ambos a la lista de Spawn Points

6. **Conectar con TutorialManager:**
   - Selecciona `TutorialManager` en Hierarchy
   - Arrastra `WaveManager_Tutorial` al campo "Practice Wave Manager"

✅ **Resultado**: Wave manager conectado.

---

### Paso 7.4: Crear CinematicsManager

1. Hierarchy → `Create Empty`
2. Renombra: **"CinematicsManager"**
3. `Add Component` → **"CinematicsManager"**

4. **Configurar (opcional, usa defaults):**
   ```
   ┌─────────────────────────────────────────┐
   │ Playable Director                       │
   │ Playable Director: None (auto-create)   │
   │                                         │
   │ Cinematic Camera                        │
   │ Cinematic Camera: None (por ahora)      │
   │ Cinematic Camera Object: None           │
   │                                         │
   │ UI                                      │
   │ Skip Prompt UI: None (crear después)    │
   │ Fade Canvas Group: None (crear después) │
   │                                         │
   │ Settings                                │
   │ Fade Duration: 1                        │
   └─────────────────────────────────────────┘
   ```

✅ **Resultado**: CinematicsManager en escena.

---

### Paso 7.5: Verificar GameManager Existente

1. Busca en Hierarchy: **"GameManager"**
2. Si NO existe, crearlo:
   - `Create Empty` → Renombra: **"GameManager"**
   - `Add Component` → Busca tu **"GameManager"** script
3. Verificar que está configurado

✅ **Resultado**: GameManager presente.

---

### Paso 7.6: Crear AudioManager

1. Hierarchy → `Create Empty`
2. Renombra: **"AudioManager"**
3. `Add Component` → **"AudioManager"**
4. (Se autoconfigura)

✅ **Resultado**: AudioManager en escena.

---

### Paso 7.7: Verificar ConversationManager

Tu **DialogueEditor** ya tiene un prefab:

1. Busca en Project: `Assets/DialogueEditor/ConversationManager.prefab`
2. Arrástral a la **Hierarchy**
3. Si ya existe en escena, perfecto

✅ **Resultado**: ConversationManager en escena.

---

## 📋 PARTE 8: TESTEAR (15 minutos)

### Paso 8.1: Guardar Todo

1. `Ctrl+S` (guardar escena)
2. `File > Save Project`

---

### Paso 8.2: Play!

1. Click en el botón **Play** ▶️
2. Observa la consola

**Secuencia esperada:**
```
1. LevelFlowManager: Starting Level 1
2. Playing cinematic: Level 1 Intro
3. → Aparece diálogo "Bienvenido..."
4. Presiona SPACE para avanzar
5. Tutorial comienza
6. → Aparece texto tutorial o diálogo
7. Presiona SPACE para avanzar
8. Aparecen 3 enemigos tutorial
9. (Placeholder - se completa automáticamente)
10. Post-tutorial cinematic
11. Stage 1 comienza...
```

---

### Paso 8.3: Debugging

Si algo no funciona:

**Error: "LevelConfig not assigned"**
- Verifica que `Level1_Config` está asignado en LevelFlowManager

**Error: "ConversationManager not found"**
- Verifica que `ConversationManager` prefab está en escena

**No aparecen enemigos:**
- Verifica que el prefab está asignado en las WaveConfigs
- Verifica que los Spawn Points tienen posiciones correctas

**Nada pasa:**
- Revisa la consola para logs
- Verifica que LevelFlowManager está en la escena

---

## 🎉 RESULTADO FINAL

Si todo funcionó, deberías ver:

✅ Cinemática intro con diálogos
✅ Tutorial (texto o diálogos)
✅ Enemigos de práctica aparecen
✅ Cinemática post-tutorial
✅ Stages comienzan a ejecutarse

---

## 📊 Checklist Final

- [ ] Todas las carpetas creadas
- [ ] Conversaciones de diálogo creadas (5 total)
- [ ] CinematicConfigs creadas (4 total)
- [ ] WaveConfigs creadas (3+ total)
- [ ] StageConfigs creadas (3 total)
- [ ] TutorialConfig creado
- [ ] BossConfig creado
- [ ] Level1_Config creado y TODO conectado
- [ ] LevelFlowManager en escena
- [ ] TutorialManager en escena
- [ ] WaveManager_Tutorial en escena
- [ ] CinematicsManager en escena
- [ ] AudioManager en escena
- [ ] ConversationManager en escena
- [ ] GameManager en escena
- [ ] Spawn points creados
- [ ] Testeo exitoso

---

## 🚀 Próximos Pasos

Una vez que todo funcione:

1. **Mejorar diálogos** - Agregar más texto, portraits
2. **Crear prefab de boss real** - Usar BossController
3. **Agregar más waves** - Diferentes dificultades
4. **Crear mini-bosses** - Para cada stage
5. **Agregar UI visual** - Barras de vida, score
6. **Agregar música** - Audio clips en configs
7. **Balancear stats** - Vida, daño, velocidad

---

**¡Todo listo! Sigue estos pasos y tendrás el flujo completo funcionando! 🎮**
