# 🐉 Tungsaur - Beat 'Em Up 2D Game

Juego de acción 2D tipo beat 'em up desarrollado en Unity.

## 🚀 Empezar Aquí

**Para configurar el juego desde cero, sigue esta guía:**

### 📖 [GUIA_COMPLETA_PASO_A_PASO.md](GUIA_COMPLETA_PASO_A_PASO.md)

Esta guía te lleva paso a paso desde cero hasta tener el juego funcionando con:
- ✅ Sistema de diálogos con DialogueEditor
- ✅ Cinemáticas (intro, outro, boss)
- ✅ Tutorial configurable
- ✅ Sistema de waves de enemigos
- ✅ Boss con fases
- ✅ Arquitectura completa de managers

**Tiempo estimado:** 1-2 horas

---

## 🎮 Arquitectura del Juego

### Flujo del Nivel:
```
Intro Cinematic → Tutorial → Stage 1 → Stage 2 → Stage 3 → Boss Fight → Outro Cinematic
```

### Managers Principales:
- **LevelFlowManager**: Controla el flujo completo del nivel
- **CinematicsManager**: Maneja cinemáticas (Timeline y DialogueEditor)
- **TutorialManager**: Sistema de tutorial guiado
- **WaveManager**: Sistema de spawning de enemigos
- **BossController**: Control de boss con fases
- **UIManager**: UI centralizada
- **AudioManager**: Audio con crossfading

### ScriptableObjects (Data-Driven):
Todo es configurable sin tocar código:
- **LevelConfigSO**: Configuración completa del nivel
- **StageConfigSO**: Configuración de cada stage
- **WaveConfigSO**: Waves de enemigos
- **CinematicConfigSO**: Cinemáticas
- **BossConfigSO**: Stats y fases del boss
- **TutorialConfigSO**: Configuración del tutorial

---

## 📂 Estructura del Proyecto

```
Assets/
├── Scripts/
│   ├── Managers/         # Todos los managers
│   ├── ScriptableObjects/ # Definiciones de ScriptableObjects
│   ├── Characters/       # Player, Enemy, Boss controllers
│   └── UI/              # Componentes de UI
│
├── ScriptableObjects/    # Instancias de configuración
│   ├── Levels/
│   ├── Stages/
│   ├── Waves/
│   ├── Bosses/
│   ├── Cinematics/
│   └── Tutorial/
│
├── Dialogues/           # Prefabs de conversaciones DialogueEditor
│   ├── Cinematics/
│   └── Tutorial/
│
└── Prefabs/
    ├── Enemies/
    ├── Managers/
    └── UI/
```

---

## 🛠️ Tecnologías

- **Unity 2022+**
- **C# .NET**
- **DialogueEditor** (Unity Asset Store)
- **Timeline** (cinemáticas opcionales)
- **ScriptableObjects** (arquitectura data-driven)

---

## 📝 Desarrollo

### Branch Principal:
- `main`: Código estable
- `reestructuracion`: Nueva arquitectura con managers y ScriptableObjects

### Commits Importantes:
- Arquitectura completa de managers
- Integración de DialogueEditor
- Sistema de ScriptableObjects
- Documentación unificada

---

## 🎯 Características

✅ **Sistema de Diálogos**: DialogueEditor integrado para cinemáticas y tutorial
✅ **Data-Driven**: Todo configurable con ScriptableObjects
✅ **Arquitectura Escalable**: Managers singleton con event-driven communication
✅ **Boss con Fases**: Sistema de fases basado en % de vida
✅ **Tutorial Flexible**: Modo texto simple o DialogueEditor
✅ **Wave System**: Spawning avanzado de enemigos

---

## 📄 Licencia

Proyecto educativo / personal

---

## 👥 Créditos

- **DialogueEditor**: Unity Asset Store
- **Arquitectura**: Sistema de managers y ScriptableObjects custom

---

**🎮 ¡A jugar y desarrollar!**
