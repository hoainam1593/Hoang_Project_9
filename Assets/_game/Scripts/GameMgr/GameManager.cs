using System;
using UnityEngine;
using Cysharp.Threading.Tasks;
using UnityEngine.SceneManagement;

/// <summary>
/// Central game manager responsible for game flow control, state management, and system coordination
/// </summary>
public class GameManager : SingletonMonoBehaviour<GameManager>
{
    [Header("Game Manager Settings")]
    [SerializeField] private bool enableDebugLogs = true;
    
    private Game game;
    private GameState currentState;
    
    /// <summary>
    /// Current game state
    /// </summary>
    public GameState CurrentState => currentState;
    
    /// <summary>
    /// Check if the game is currently playing
    /// </summary>
    public bool IsPlaying => currentState == GameState.Playing;
    
    /// <summary>
    /// Check if the game is currently paused
    /// </summary>
    public bool IsPaused => currentState == GameState.Paused;
    
    protected override void Awake()
    {
        base.Awake();
        game = new Game();
        currentState = GameState.MainMenu;
        
        // Subscribe to game events
        SubscribeToGameEvents();
        
        if (enableDebugLogs) Debug.Log("GameManager: Initialized");
    }

    #region Game Flow Control
    /// <summary>
    /// Launch the game to main menu
    /// </summary>
    public async UniTask LaunchGame()
    {
        try
        {
            if (enableDebugLogs) Debug.Log("GameManager: Launching game to main menu");
            
            // Load main menu scene
            await LoadSceneAsync(SceneName.SceneMain);
            currentState = GameState.MainMenu;
            
            if (enableDebugLogs) Debug.Log("GameManager: Successfully launched to main menu");
        }
        catch (Exception ex)
        {
            Debug.LogError($"GameManager: Failed to launch game - {ex.Message}");
        }
    }

    /// <summary>
    /// Start the battle gameplay
    /// </summary>
    public async UniTask StartGame()
    {
        if (currentState != GameState.MainMenu) 
        {
            if (enableDebugLogs) Debug.LogWarning($"GameManager: Cannot start game from state {currentState}");
            return;
        }
        
        try
        {
            if (enableDebugLogs) Debug.Log("GameManager: Starting game");
            currentState = GameState.Loading;
            
            // Load battle scene
            await LoadSceneAsync(SceneName.SceneBattle);
            
            // Initialize game systems
            await InitializeGameSystems();
            
            // Start the game
            game.StartGame();
            currentState = GameState.Playing;
            
            if (enableDebugLogs) Debug.Log("GameManager: Game started successfully");
        }
        catch (Exception ex)
        {
            Debug.LogError($"GameManager: Failed to start game - {ex.Message}");
            currentState = GameState.MainMenu;
        }
    }

    /// <summary>
    /// Exit the current game and return to main menu
    /// </summary>
    public async UniTask ExitGame()
    {
        if (currentState == GameState.MainMenu) 
        {
            if (enableDebugLogs) Debug.LogWarning("GameManager: Already at main menu");
            return;
        }
        
        try
        {
            if (enableDebugLogs) Debug.Log("GameManager: Exiting game");
            currentState = GameState.Loading;
            
            // Stop the game first
            game.ExitGame();
            
            // Cleanup all systems
            await CleanupGameSystems();
            
            // Return to main menu
            await LoadSceneAsync(SceneName.SceneMain);
            
            currentState = GameState.MainMenu;
            
            if (enableDebugLogs) Debug.Log("GameManager: Successfully returned to main menu");
        }
        catch (Exception ex)
        {
            Debug.LogError($"GameManager: Error during game exit - {ex.Message}");
            currentState = GameState.MainMenu;
        }
    }

    /// <summary>
    /// Pause the current game
    /// </summary>
    public void PauseGame()
    {
        if (currentState == GameState.Playing)
        {
            if (enableDebugLogs) Debug.Log("GameManager: Pausing game");
            currentState = GameState.Paused;
            game.PauseGame();
        }
    }

    /// <summary>
    /// Resume the paused game
    /// </summary>
    public void ResumeGame()
    {
        if (currentState == GameState.Paused)
        {
            if (enableDebugLogs) Debug.Log("GameManager: Resuming game");
            currentState = GameState.Playing;
            game.ResumeGame();
        }
    }

    /// <summary>
    /// Handle game over state
    /// </summary>
    public void GameOver()
    {
        if (currentState == GameState.Playing)
        {
            if (enableDebugLogs) Debug.Log("GameManager: Game Over");
            currentState = GameState.GameOver;
            game.ExitGame();
            
            // Dispatch game over event
            GameEventMgr.GED.DispatcherEvent(GameEvent.OnGameOver);
        }
    }

    /// <summary>
    /// Handle victory state
    /// </summary>
    public void Victory()
    {
        if (currentState == GameState.Playing)
        {
            if (enableDebugLogs) Debug.Log("GameManager: Victory!");
            currentState = GameState.Victory;
            game.ExitGame();
            
            // Dispatch victory event
            GameEventMgr.GED.DispatcherEvent(GameEvent.OnVictory);
        }
    }
    #endregion

    #region Game Systems Management
    /// <summary>
    /// Initialize all game systems required for gameplay
    /// </summary>
    private async UniTask InitializeGameSystems()
    {
        if (enableDebugLogs) Debug.Log("GameManager: Initializing game systems");
        
        try
        {
            // Initialize entity manager
            if (EntityManager.instance != null)
            {
                // EntityManager should be ready, no special initialization needed
            }
            
            // Initialize health bar manager
            if (HealthBarManager.instance != null)
            {
                // HealthBarManager should be ready
            }
            
            // Load game data
            await LoadGameData();
            
            if (enableDebugLogs) Debug.Log("GameManager: Game systems initialized");
        }
        catch (Exception ex)
        {
            Debug.LogError($"GameManager: Failed to initialize game systems - {ex.Message}");
            throw;
        }
    }

    /// <summary>
    /// Load game-specific data for the current session
    /// </summary>
    private async UniTask LoadGameData()
    {
        try
        {
            if (enableDebugLogs) Debug.Log("GameManager: Loading game data");
            
            // Load selected map
            var selectedMap = PlayerPrefs.GetInt(PlayerPrefsConfig.Key_Select_Map, 0);
            if (enableDebugLogs) Debug.Log($"GameManager: Loading map {selectedMap}");
            
            // Here you would load map-specific data
            // await MapManager.instance.LoadMap(selectedMap);
            
            // Small delay to simulate loading
            await UniTask.Delay(100);
            
            if (enableDebugLogs) Debug.Log("GameManager: Game data loaded");
        }
        catch (Exception ex)
        {
            Debug.LogError($"GameManager: Failed to load game data - {ex.Message}");
            throw;
        }
    }

    /// <summary>
    /// Clean up all game systems
    /// </summary>
    private async UniTask CleanupGameSystems()
    {
        if (enableDebugLogs) Debug.Log("GameManager: Cleaning up game systems");
        
        try
        {
            // Clear all entities
            EntityManager.instance?.ClearAll();
            
            // Clear health bars
            HealthBarManager.instance?.ClearAll();
            
            // Unload unused resources
            await Resources.UnloadUnusedAssets();
            
            if (enableDebugLogs) Debug.Log("GameManager: Game systems cleaned up");
        }
        catch (Exception ex)
        {
            Debug.LogError($"GameManager: Error during cleanup - {ex.Message}");
        }
    }
    #endregion

    #region Scene Management
    /// <summary>
    /// Load a scene asynchronously
    /// </summary>
    /// <param name="sceneName">Scene to load</param>
    private async UniTask LoadSceneAsync(SceneName sceneName)
    {
        string sceneNameStr = sceneName.ToString();
        if (enableDebugLogs) Debug.Log($"GameManager: Loading scene {sceneNameStr}");
        
        var operation = SceneManager.LoadSceneAsync(sceneNameStr);
        
        while (!operation.isDone)
        {
            await UniTask.Yield();
        }
        
        if (enableDebugLogs) Debug.Log($"GameManager: Scene {sceneNameStr} loaded successfully");
    }
    #endregion

    #region Event Handling
    /// <summary>
    /// Subscribe to relevant game events
    /// </summary>
    private void SubscribeToGameEvents()
    {
        // Subscribe to relevant game events
        GameEventMgr.GED.Register(GameEvent.OnPlayerDeath, OnPlayerDeath);
        GameEventMgr.GED.Register(GameEvent.OnWaveComplete, OnWaveComplete);
        
        if (enableDebugLogs) Debug.Log("GameManager: Subscribed to game events");
    }

    /// <summary>
    /// Unsubscribe from game events
    /// </summary>
    private void UnsubscribeFromGameEvents()
    {
        GameEventMgr.GED.UnRegister(GameEvent.OnPlayerDeath, OnPlayerDeath);
        GameEventMgr.GED.UnRegister(GameEvent.OnWaveComplete, OnWaveComplete);
        
        if (enableDebugLogs) Debug.Log("GameManager: Unsubscribed from game events");
    }

    /// <summary>
    /// Handle player death event
    /// </summary>
    private void OnPlayerDeath(object data)
    {
        if (enableDebugLogs) Debug.Log("GameManager: Player died");
        GameOver();
    }

    /// <summary>
    /// Handle wave complete event
    /// </summary>
    private void OnWaveComplete(object data)
    {
        if (enableDebugLogs) Debug.Log("GameManager: Wave completed");
        // Check if all waves are complete for victory condition
        // This logic should be implemented based on your wave system
    }

    protected override void OnDestroy()
    {
        UnsubscribeFromGameEvents();
        base.OnDestroy();
    }
    #endregion

    #region Application Events
    /// <summary>
    /// Handle application pause
    /// </summary>
    private void OnApplicationPause(bool pauseStatus)
    {
        if (pauseStatus && currentState == GameState.Playing)
        {
            PauseGame();
        }
    }

    /// <summary>
    /// Handle application focus change
    /// </summary>
    private void OnApplicationFocus(bool hasFocus)
    {
        if (!hasFocus && currentState == GameState.Playing)
        {
            PauseGame();
        }
    }
    #endregion
}