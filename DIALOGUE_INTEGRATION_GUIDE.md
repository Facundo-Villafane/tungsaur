# 💬 Guía de Integración de DialogueEditor

## ✅ Estado Actual

El **DialogueEditor** ya está **completamente integrado** en el nuevo sistema de managers. Puedes usarlo para:

1. ✅ **Cinemáticas** (Intro/Outro de niveles y bosses)
2. ✅ **Tutorial** (Instrucciones guiadas)
3. ✅ **Diálogos en juego** (NPCs, eventos)

---

## 🎬 Usar DialogueEditor en Cinemáticas

### Paso 1: Crear Conversación en DialogueEditor

1. **Crear GameObject con NPCConversation:**
   - En Hierarchy: Click derecho → Create Empty
   - Nombra: `IntroCinematic_Dialogue`
   - Add Component: `NPC Conversation`

2. **Abrir DialogueEditor:**
   - En Unity: `Window > Dialogue Editor`
   - Selecciona el GameObject `IntroCinematic_Dialogue` en Hierarchy

3. **Diseña tu diálogo:**
   - Click derecho en el nodo raíz → `Create Speech`
   - Click izquierdo para colocar → Editar texto: "Welcome!"
   - Click derecho en el nodo creado → `Create Speech`
   - Click izquierdo para colocar → Editar texto: "Let's begin..."

4. **Guardar como prefab:**
   - Arrastra el GameObject desde Hierarchy a `Assets/Dialogues/Cinematics/`
   - Nombre del prefab: `IntroCinematic_Dialogue`

### Paso 2: Crear CinematicConfigSO

1. Click derecho → `Create > CDG > Cinematic Configuration`
2. Configurar:
   ```
   ┌────────────────────────────────────────┐
   │ CinematicConfigSO                      │
   ├────────────────────────────────────────┤
   │ Cinematic Name: "Level 1 Intro"       │
   │                                        │
   │ Cinematic Type: Dialogue  ← IMPORTANTE│
   │                                        │
   │ Timeline Asset: [Vacío]                │
   │                                        │
   │ Dialogue Conversation:                 │
   │   Arrastra el prefab GameObject        │
   │   IntroCinematic_Dialogue aquí  ←      │
   │                                        │
   │ Lock Camera: ✓                         │
   │ Can Skip: ✓                            │
   │ Skip Key: Space                        │
   └────────────────────────────────────────┘
   ```

### Paso 3: Asignar al Nivel

```
LevelConfigSO:
  Intro Cinematic: [Level1_Intro (CinematicConfig)]
```

### ✅ Resultado

Cuando el nivel inicia:
1. Se pausa el juego
2. Se bloquea la cámara
3. Se muestra el diálogo usando `ConversationManager`
4. El jugador puede skipear con Space (si `Can Skip = true`)
5. Al terminar, continúa el flujo del nivel

---

## 📚 Usar DialogueEditor en Tutorial

### Opción A: Modo Simple Text (Sin DialogueEditor)

```
TutorialConfigSO:
  Display Mode: SimpleText

  Tutorial Steps:
    - "Welcome to the game!"
    - "Use WASD to move"
    - "Press J to punch"

  Input Prompts:
    - "WASD - Move"
    - "J - Punch"
```

**Resultado**: UI simple con texto overlay

---

### Opción B: Modo DialogueEditor ✨ (NUEVO)

```
TutorialConfigSO:
  Display Mode: DialogueEditor  ← NUEVO

  Tutorial Conversation:
    [Tu conversación de DialogueEditor]

  Practice Wave:
    [WaveConfig para práctica]
```

#### Ejemplo de Conversación Tutorial:

En DialogueEditor, crea:

```
[Start]
   ↓
[NPC: "Welcome to the game!"]
   ↓
[NPC: "Let me show you how to move"]
   ↓
[NPC: "Use WASD keys to walk around"]
   ↓
[Player Options:]
   • "Got it!"
   • "Can you repeat?"
   ↓
[NPC: "Now let's practice combat!"]
   ↓
[End]
```

**Resultado**: Diálogo completo con portraits, opciones, y lógica condicional

---

## 🎯 Casos de Uso

### 1️⃣ Cinemática de Intro (Nivel 1)

```
DialogueEditor: "Level1_Intro_Dialogue"
  - Mostrar villano
  - Explicar misión
  - Motivación del héroe

↓ Asignar a

CinematicConfigSO: "Level1_Intro"
  Type: Dialogue
  Conversation: [Level1_Intro_Dialogue]

↓ Asignar a

LevelConfigSO: "Level1Config"
  Intro Cinematic: [Level1_Intro]
```

---

### 2️⃣ Boss Intro Cinematic

```
DialogueEditor: "Boss_Intro_Dialogue"
  [Boss: "You dare challenge me?!"]
  [Player: "I'm ready!"]
  [Boss: "Then face your doom!"]

↓ Asignar a

CinematicConfigSO: "BossIntro"
  Type: Dialogue
  Conversation: [Boss_Intro_Dialogue]

↓ Asignar a

BossConfigSO: "Boss_Level1_Config"
  Intro Cinematic: [BossIntro]
```

**Resultado**: Cuando el boss aparece, se reproduce el diálogo automáticamente.

---

### 3️⃣ Tutorial con DialogueEditor

```
DialogueEditor: "Tutorial_Conversation"
  [Trainer: "Welcome!"]
  [Trainer: "Let me teach you combat"]

  [Conditional: Has_Learned_Punch?]
    YES → [Next lesson]
    NO  → [Repeat punch tutorial]

  [Trainer: "Great job! Now try this..."]

↓ Asignar a

TutorialConfigSO: "GameTutorial"
  Display Mode: DialogueEditor
  Tutorial Conversation: [Tutorial_Conversation]
  Player Invulnerable: ✓
```

**Ventaja**: Puedes usar lógica condicional del DialogueEditor (parameters, conditions).

---

## 🔧 Arquitectura Técnica

### Flujo de Cinemáticas:

```
LevelFlowManager
    ↓
CinematicsManager.PlayCinematic(config)
    ↓
¿Tipo de cinemática?
    ├─ Timeline → PlayableDirector
    └─ Dialogue → ConversationManager ← Tu sistema
```

### Código (Ya implementado):

```csharp
// En CinematicsManager.cs
private IEnumerator PlayDialogueCinematic(CinematicConfigSO cinematicConfig)
{
    // Obtener NPCConversation del GameObject prefab
    NPCConversation conversation = cinematicConfig.dialogueConversation.GetComponent<NPCConversation>();

    // Usa tu ConversationManager existente
    ConversationManager.Instance.StartConversation(conversation);

    // Espera a que termine
    while (ConversationManager.Instance.IsConversationActive)
    {
        yield return null;
    }
}
```

---

## 📦 Sistema ConversationManager (Tu código existente)

Tu `ConversationManager` ya tiene estas features:

- ✅ Texto con scroll animado
- ✅ Portraits de personajes
- ✅ Opciones de diálogo
- ✅ Lógica condicional (parameters)
- ✅ Audio por diálogo
- ✅ Eventos de callbacks

**Todo esto funciona directamente en el nuevo sistema** 🎉

---

## 🎨 Ventajas de Usar DialogueEditor

### vs Cinemáticas con Timeline:

| Aspecto | Timeline | DialogueEditor |
|---------|----------|----------------|
| **Complejidad** | Alta (animaciones, cámara) | Baja (solo diálogos) |
| **Setup Time** | Lento | Rápido |
| **Branching** | Difícil | Fácil (built-in) |
| **Conditions** | Manual scripting | Visual editor |
| **Portraits** | Manual setup | Built-in |
| **Ideal para** | Cutscenes complejas | Conversaciones |

### Cuándo Usar Cada Uno:

**DialogueEditor** ✅
- Conversaciones entre personajes
- Tutorial con instrucciones
- Boss intros/outros simples
- NPCs en el juego

**Timeline** ✅
- Cinemáticas con movimiento de cámara
- Secuencias de acción
- Sincronización de animaciones complejas
- Efectos visuales y audio coordinados

---

## 📋 Checklist de Integración

### Para Cinemáticas:

- [x] DialogueEditor instalado
- [x] ConversationManager en escena
- [x] CinematicsManager integrado
- [ ] Crear conversaciones en DialogueEditor
- [ ] Crear CinematicConfigSO con tipo "Dialogue"
- [ ] Asignar a LevelConfig/BossConfig

### Para Tutorial:

- [x] TutorialManager actualizado
- [x] Soporte para DialogueEditor agregado
- [ ] Crear conversación de tutorial
- [ ] Crear TutorialConfigSO con modo "DialogueEditor"
- [ ] Asignar conversación

---

## 🎓 Ejemplo Completo: Level 1

### Estructura de Assets:

```
Assets/
├── Dialogues/
│   ├── Level1_Intro.prefab          (GameObject con NPCConversation)
│   ├── Tutorial_Dialogue.prefab     (GameObject con NPCConversation)
│   ├── Boss_Intro.prefab            (GameObject con NPCConversation)
│   └── Boss_Defeat.prefab           (GameObject con NPCConversation)
│
└── Configs/
    ├── Cinematics/
    │   ├── Level1_Intro_Cinematic.asset    (CinematicConfigSO)
    │   ├── Tutorial_Cinematic.asset        (CinematicConfigSO)
    │   ├── Boss_Intro_Cinematic.asset      (CinematicConfigSO)
    │   └── Boss_Defeat_Cinematic.asset     (CinematicConfigSO)
    │
    ├── Tutorial/
    │   └── Tutorial_Config.asset           (TutorialConfigSO)
    │
    ├── Boss/
    │   └── Boss_Level1_Config.asset        (BossConfigSO)
    │
    └── Levels/
        └── Level1_Config.asset             (LevelConfigSO)
```

### Conexiones:

```
Level1_Intro.prefab (GameObject con NPCConversation)
    ↓
Level1_Intro_Cinematic.asset (CinematicConfigSO)
    Type: Dialogue
    Conversation: [Level1_Intro prefab]  ← Arrastra el prefab GameObject
    ↓
Level1_Config.asset (LevelConfigSO)
    Intro Cinematic: [Level1_Intro_Cinematic]
```

---

## 🐛 Troubleshooting

### "Dialogue no se muestra"
✅ Verifica que `ConversationManager` está en la escena
✅ Verifica que la conversación está asignada en el config
✅ Revisa que `Cinematic Type = Dialogue`

### "Tutorial no muestra diálogos"
✅ Verifica que `Display Mode = DialogueEditor`
✅ Verifica que `Tutorial Conversation` está asignada
✅ Verifica que `ConversationManager` está en la escena

### "No puedo skipear el diálogo"
✅ Verifica que `Can Skip = true` en el CinematicConfig
✅ El skip funciona con la tecla configurada (default: Space)

---

## 🚀 Próximos Pasos

1. **Crear conversaciones** en DialogueEditor
2. **Configurar CinematicConfigs** con tus diálogos
3. **Testear** el flujo completo del nivel
4. **Agregar portraits** y audio a los diálogos
5. **Usar conditions** para branching avanzado

---

## 📊 Resumen

| Sistema | Usa DialogueEditor | Cómo |
|---------|-------------------|------|
| **Cinemáticas** | ✅ | CinematicConfigSO → Type: Dialogue |
| **Tutorial** | ✅ | TutorialConfigSO → Mode: DialogueEditor |
| **Boss Intro** | ✅ | BossConfigSO → Intro Cinematic |
| **Boss Defeat** | ✅ | BossConfigSO → Defeat Cinematic |
| **NPCs** | ✅ | Usar ConversationManager directamente |

**Todo está conectado y listo para usar!** 🎉

---

## 💡 Tips

1. **Reusar conversaciones**: Puedes usar la misma conversación en múltiples cinemáticas
2. **Parameters**: Usa parameters del DialogueEditor para hacer diálogos dinámicos
3. **Audio**: Agrega clips de audio a cada nodo para voiceover
4. **Portraits**: Asigna sprites de personajes para avatares
5. **Conditions**: Usa conditions para diálogos que cambian según progreso

---

**¡Tu DialogueEditor ahora está 100% integrado con toda la arquitectura! 🎮**
