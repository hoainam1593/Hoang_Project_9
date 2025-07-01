# Game Architecture Refactoring

## T?ng quan thay ??i

?ã ti?n hành refactor ki?n trúc game theo nguyên t?c **Single Responsibility Principle** và **Separation of Concerns**.

## Các file ?ã t?o/ch?nh s?a

### 1. **GameListConfig.cs** (M?i)
- **V? trí:** `Assets/_game/Scripts/GameConfig/GameListConfig.cs`
- **M?c ?ích:** Tách riêng config declaration kh?i GameLauncher
- **Tính n?ng:**
  - Constructor nh?n danh sách configs
  - Default constructor v?i configs m?c ??nh
  - Factory methods: `Create()`, `CreateWithDefaults()`

### 2. **GameManager.cs** (M?i)
- **V? trí:** `Assets/_game/Scripts/GameMgr/GameManager.cs`
- **Trách nhi?m:** Game flow control, state management, system coordination
- **Tính n?ng:**
  - Qu?n lý GameState (MainMenu, Loading, Playing, Paused, GameOver, Victory)
  - Scene management v?i async loading
  - Game systems initialization và cleanup
  - Event handling cho game events
  - Application event forwarding

### 3. **GameLauncher.cs** (Refactored)
- **Trách nhi?m m?i:** Ch? qu?n lý application lifecycle
- **Thay ??i:**
  - Tách GameListConfig ra file riêng
  - S? d?ng constructor pattern cho GameListConfig
  - Forward game control cho GameManager
  - Thêm error handling và logging
  - Application events handling

### 4. **Game.cs** (Enhanced)
- **C?i thi?n:** Pure game logic v?i better state management
- **Thêm:** Documentation và properties ?? check states
- **GameplayState:** Playing, Paused, Stopped

### 5. **GlobalConfig.cs** (Updated)
- **Thêm enums:**
  - `SceneName`: Main, Battle, Loading
  - `GameState`: MainMenu, Loading, Playing, Paused, GameOver, Victory
  - `GameplayState`: Playing, Paused, Stopped

### 6. **GameEvent.cs** (Updated)
- **Thêm events:**
  - OnGameStarted, OnGamePaused, OnGameResumed, OnGameExited
  - OnGameOver, OnVictory
  - OnPlayerDeath, OnWaveComplete

### 7. **HealthBarManager.cs** (Updated)
- **Thêm:** `ClearAll()` method cho cleanup

## Ki?n trúc m?i

### **Phân tách trách nhi?m:**

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

## L?i ích

### 1. **Single Responsibility:**
- M?i class có m?t trách nhi?m rõ ràng
- D? maintain và debug

### 2. **Separation of Concerns:**
- GameListConfig tách riêng
- Game flow tách kh?i application lifecycle
- Clear boundaries between components

### 3. **Scalability:**
- D? thêm features m?i
- Không ?nh h??ng l?n nhau
- Modular architecture

### 4. **Testability:**
- Có th? test t?ng component riêng
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

- Các existing calls ??n GameLauncher.StartGame()/ExitGame() v?n ho?t ??ng
- GameManager handles actual logic internally
- Event system expanded v?i more game events
- HealthBarManager có thêm ClearAll() method

## Future Enhancements

- Scene loading progress callback
- Save/Load game state
- More sophisticated state machine
- Performance metrics tracking
- Memory usage optimization