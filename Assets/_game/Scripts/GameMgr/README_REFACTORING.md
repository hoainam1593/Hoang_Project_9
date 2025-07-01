# Game Architecture Refactoring

## T?ng quan thay ??i

?� ti?n h�nh refactor ki?n tr�c game theo nguy�n t?c **Single Responsibility Principle** v� **Separation of Concerns**.

## C�c file ?� t?o/ch?nh s?a

### 1. **GameListConfig.cs** (M?i)
- **V? tr�:** `Assets/_game/Scripts/GameConfig/GameListConfig.cs`
- **M?c ?�ch:** T�ch ri�ng config declaration kh?i GameLauncher
- **T�nh n?ng:**
  - Constructor nh?n danh s�ch configs
  - Default constructor v?i configs m?c ??nh
  - Factory methods: `Create()`, `CreateWithDefaults()`

### 2. **GameManager.cs** (M?i)
- **V? tr�:** `Assets/_game/Scripts/GameMgr/GameManager.cs`
- **Tr�ch nhi?m:** Game flow control, state management, system coordination
- **T�nh n?ng:**
  - Qu?n l� GameState (MainMenu, Loading, Playing, Paused, GameOver, Victory)
  - Scene management v?i async loading
  - Game systems initialization v� cleanup
  - Event handling cho game events
  - Application event forwarding

### 3. **GameLauncher.cs** (Refactored)
- **Tr�ch nhi?m m?i:** Ch? qu?n l� application lifecycle
- **Thay ??i:**
  - T�ch GameListConfig ra file ri�ng
  - S? d?ng constructor pattern cho GameListConfig
  - Forward game control cho GameManager
  - Th�m error handling v� logging
  - Application events handling

### 4. **Game.cs** (Enhanced)
- **C?i thi?n:** Pure game logic v?i better state management
- **Th�m:** Documentation v� properties ?? check states
- **GameplayState:** Playing, Paused, Stopped

### 5. **GlobalConfig.cs** (Updated)
- **Th�m enums:**
  - `SceneName`: Main, Battle, Loading
  - `GameState`: MainMenu, Loading, Playing, Paused, GameOver, Victory
  - `GameplayState`: Playing, Paused, Stopped

### 6. **GameEvent.cs** (Updated)
- **Th�m events:**
  - OnGameStarted, OnGamePaused, OnGameResumed, OnGameExited
  - OnGameOver, OnVictory
  - OnPlayerDeath, OnWaveComplete

### 7. **HealthBarManager.cs** (Updated)
- **Th�m:** `ClearAll()` method cho cleanup

## Ki?n tr�c m?i

### **Ph�n t�ch tr�ch nhi?m:**

```
GameLauncher (Application Lifecycle)
    ?
GameManager (Game Flow Control)
    ?
Game (Pure Game Logic)
```

### **GameLauncher:**
- Application initialization
- Config loading
- Model loading
- Application events (pause, focus, quit)

### **GameManager:**
- Game state management
- Scene transitions
- System coordination
- Game events handling
- Cleanup operations

### **Game:**
- Gameplay state management
- Time scale control
- Game systems start/stop
- Pure game logic

## L?i �ch

### 1. **Single Responsibility:**
- M?i class c� m?t tr�ch nhi?m r� r�ng
- D? maintain v� debug

### 2. **Separation of Concerns:**
- GameListConfig t�ch ri�ng
- Game flow t�ch kh?i application lifecycle
- Clear boundaries between components

### 3. **Scalability:**
- D? th�m features m?i
- Kh�ng ?nh h??ng l?n nhau
- Modular architecture

### 4. **Testability:**
- C� th? test t?ng component ri�ng
- Mock-friendly design
- Clear dependencies

### 5. **Error Handling:**
- Comprehensive error handling
- Proper logging
- Graceful failure recovery

## S? d?ng

### **Kh?i t?o configs:**
```csharp
// S? d?ng default configs
var gameConfig = new GameListConfig();

// Ho?c custom configs
var configList = new List<IBaseConfig> 
{
    new MapConfig(),
    new EnemyConfig(),
    new TurretConfig(),
};
var gameConfig = new GameListConfig(configList);
```

### **Game flow control:**
```csharp
// Start game
await GameManager.instance.StartGame();

// Exit game
await GameManager.instance.ExitGame();

// Pause/Resume
GameManager.instance.PauseGame();
GameManager.instance.ResumeGame();
```

## Migration Notes

- C�c existing calls ??n GameLauncher.StartGame()/ExitGame() v?n ho?t ??ng
- GameManager handles actual logic internally
- Event system expanded v?i more game events
- HealthBarManager c� th�m ClearAll() method

## Future Enhancements

- Scene loading progress callback
- Save/Load game state
- More sophisticated state machine
- Performance metrics tracking
- Memory usage optimization