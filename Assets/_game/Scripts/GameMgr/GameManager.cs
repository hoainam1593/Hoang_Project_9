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
            
            // Start wave system
            await StartWaveSystem();
            
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
            
            // Close all popups before exiting
            PopupManager.instance?.CloseAllPopup();
            
            // Stop the game first
            game.ExitGame();
            
            // Stop wave system
            WaveManager.instance?.StopWaveSystem();
            
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
    public async void GameOver()
    {
        if (currentState == GameState.Playing)
        {
            if (enableDebugLogs) Debug.Log("GameManager: Game Over");
            currentState = GameState.GameOver;
            game.ExitGame();
            
            // Stop wave system
            WaveManager.instance?.StopWaveSystem();
            
            // Show game over popup
            await ShowGameOverPopup();
            
            // Dispatch game over event
            GameEventMgr.GED.DispatcherEvent(GameEvent.OnGameOver);
        }
    }

    /// <summary>
    /// Handle victory state
    /// </summary>
    public async void Victory()
    {
        if (currentState == GameState.Playing)
        {
            if (enableDebugLogs) Debug.Log("GameManager: Victory!");
            currentState = GameState.Victory;
            game.ExitGame();
            
            // Stop wave system
            WaveManager.instance?.StopWaveSystem();
            
            // Generate random rewards
            int fakeGoldBonus = UnityEngine.Random.Range(1000, 2501); // Random between 1000-2500
            int fakeStar = UnityEngine.Random.Range(1, 4); // Random between 1-3
            
            if (enableDebugLogs) Debug.Log($"GameManager: Victory rewards - Gold: {fakeGoldBonus}, Stars: {fakeStar}");
            
            // Update map progress and unlock next level
            UpdateMapProgress(fakeStar);
            
            // Show victory popup with rewards
            await ShowVictoryPopup(fakeGoldBonus, fakeStar);
            
            // Dispatch victory event
            GameEventMgr.GED.DispatcherEvent(GameEvent.OnVictory);
        }
    }

    /// <summary>
    /// Update map progress and unlock next level if conditions are met
    /// </summary>
    private void UpdateMapProgress(int stars)
    {
        try
        {
            var currentMapId = PlayerPrefs.GetInt(PlayerPrefsConfig.Key_Select_Map_Id, 0);
            var mapModel = PlayerModelManager.instance.GetPlayerModel<MapModel>();
            var mapConfig = ConfigManager.instance.GetConfig<MapConfig>();
            
            // Update current map's star rating if it's better than before
            var currentChapter = mapModel.Chapters.Find(chapter => chapter.Id == currentMapId);
            if (currentChapter != null && (currentChapter.Star == -1 || stars > currentChapter.Star))
            {
                currentChapter.Star = stars;
                if (enableDebugLogs) Debug.Log($"GameManager: Updated map {currentMapId} star rating to {stars}");
            }
            
            // Unlock next level if current map is completed with at least 1 star
            if (stars > 0)
            {
                int nextMapId = currentMapId + 1;
                var nextChapter = mapModel.Chapters.Find(chapter => chapter.Id == nextMapId);
                
                // Check if next map exists in config and is currently locked
                if (nextChapter != null && nextChapter.Star == -1)
                {
                    var nextMapItem = mapConfig.GetMapItem(nextMapId);
                    if (nextMapItem != null)
                    {
                        nextChapter.Star = 0; // Unlock with 0 stars (playable but not completed)
                        if (enableDebugLogs) Debug.Log($"GameManager: Unlocked next level - Map {nextMapId}");
                    }
                }
            }
            
            // Save the updated model
            PlayerModelManager.instance.SaveModel(mapModel);
            
            if (enableDebugLogs) Debug.Log("GameManager: Map progress updated and saved");
        }
        catch (Exception ex)
        {
            Debug.LogError($"GameManager: Failed to update map progress - {ex.Message}");
        }
    }

    /// <summary>
    /// Retry the current game (replay the same map)
    /// </summary>
    /// <remarks>
    /// When RetryGame is called, the game doesn't reload the scene (SceneBattle),
    /// so MonoBehaviour components don't get their Start() method called again.
    /// This means UI components like PlayerInfoCtrl that initialize their values
    /// in Start() won't reset automatically. To fix this, we dispatch an OnGameRetry
    /// event that UI components can listen to and use to reset their state.
    /// </remarks>
    public async UniTask RetryGame()
    {
        if (currentState != GameState.GameOver && currentState != GameState.Victory && currentState != GameState.MainMenu)
        {
            if (enableDebugLogs) Debug.LogWarning($"GameManager: Cannot retry game from state {currentState}");
            return;
        }
        
        try
        {
            if (enableDebugLogs) Debug.Log("GameManager: Retrying current game");
            
            // Close all popups first
            PopupManager.instance?.CloseAllPopup();
            
            // If we're currently in game, clean up first
            if (currentState != GameState.MainMenu)
            {
                currentState = GameState.Loading;
                
                // Stop current game
                game.ExitGame();
                
                // Stop wave system
                WaveManager.instance?.StopWaveSystem();
                
                // Cleanup systems
                await CleanupGameSystems();
                
                // Reset state to MainMenu after cleanup so StartGame() can proceed
                currentState = GameState.MainMenu;
            }
            
            // Dispatch retry event to reset UI components that don't get reinitialized
            // This is crucial for components like PlayerInfoCtrl that set their initial
            // values in Start() but need to be reset when retrying without scene reload
            GameEventMgr.GED.DispatcherEvent(GameEvent.OnGameRetry);
            
            // The selected map should remain the same (stored in PlayerPrefs)
            // Start the game again with the same map
            await StartGame();
            
            if (enableDebugLogs) Debug.Log("GameManager: Game retried successfully");
        }
        catch (Exception ex)
        {
            Debug.LogError($"GameManager: Failed to retry game - {ex.Message}");
            currentState = GameState.MainMenu;
        }
    }

    /// <summary>
    /// Play the next level if it's unlocked
    /// </summary>
    public async UniTask NextLevel()
    {
        if (currentState != GameState.Victory && currentState != GameState.MainMenu)
        {
            if (enableDebugLogs) Debug.LogWarning($"GameManager: Cannot go to next level from state {currentState}");
            return;
        }
        
        try
        {
            var currentMapId = PlayerPrefs.GetInt(PlayerPrefsConfig.Key_Select_Map_Id, 0);
            int nextMapId = currentMapId + 1;
            
            var mapModel = PlayerModelManager.instance.GetPlayerModel<MapModel>();
            var mapConfig = ConfigManager.instance.GetConfig<MapConfig>();
            
            // Check if next map exists and is unlocked
            var nextChapter = mapModel.Chapters.Find(chapter => chapter.Id == nextMapId);
            var nextMapItem = mapConfig.GetMapItem(nextMapId);
            
            if (nextMapItem == null)
            {
                if (enableDebugLogs) Debug.LogWarning($"GameManager: Next map {nextMapId} does not exist in config");
                return;
            }
            
            if (nextChapter == null || nextChapter.Star == -1)
            {
                if (enableDebugLogs) Debug.LogWarning($"GameManager: Next map {nextMapId} is not unlocked yet");
                return;
            }
            
            if (enableDebugLogs) Debug.Log($"GameManager: Starting next level - Map {nextMapId}");
            
            // Close all popups first
            PopupManager.instance?.CloseAllPopup();
            
            // Update selected map in PlayerPrefs
            PlayerPrefs.SetInt(PlayerPrefsConfig.Key_Select_Map_Id, nextMapId);
            PlayerPrefs.SetString(PlayerPrefsConfig.Key_Select_Map_Name, nextMapItem.mapName);
            
            // If we're currently in game, clean up first
            if (currentState != GameState.MainMenu)
            {
                currentState = GameState.Loading;
                
                // Stop current game
                game.ExitGame();
                
                // Stop wave system
                WaveManager.instance?.StopWaveSystem();
                
                // Cleanup systems
                await CleanupGameSystems();
                
                // Reset state to MainMenu after cleanup so StartGame() can proceed
                currentState = GameState.MainMenu;
            }
            
            // Start the game with the new map
            await StartGame();
            
            if (enableDebugLogs) Debug.Log($"GameManager: Successfully started next level - Map {nextMapId}");
        }
        catch (Exception ex)
        {
            Debug.LogError($"GameManager: Failed to start next level - {ex.Message}");
            currentState = GameState.MainMenu;
        }
    }

    /// <summary>
    /// Show victory popup with random rewards
    /// </summary>
    private async UniTask ShowVictoryPopup(int goldBonus, int stars)
    {
        try
        {
            if (enableDebugLogs) Debug.Log("GameManager: Showing victory popup");
            
            // Check if current level is the max level
            bool isMaxLevel = IsCurrentLevelMaxLevel();
            
            var popup = await PopupManager.instance.OpenPopup<PopupVictory>();
            popup.InitView(stars, goldBonus, isMaxLevel); // Pass max level information
            
            if (enableDebugLogs) Debug.Log($"GameManager: Victory popup displayed (Max Level: {isMaxLevel})");
        }
        catch (Exception ex)
        {
            Debug.LogError($"GameManager: Failed to show victory popup - {ex.Message}");
        }
    }

    /// <summary>
    /// Check if the current level is the maximum level available in the game
    /// </summary>
    /// <returns>True if current level is the max level, false otherwise</returns>
    private bool IsCurrentLevelMaxLevel()
    {
        try
        {
            var currentMapId = PlayerPrefs.GetInt(PlayerPrefsConfig.Key_Select_Map_Id, 0);
            var mapConfig = ConfigManager.instance.GetConfig<MapConfig>();
            
            // Find the highest map ID in the config
            int maxMapId = -1;
            foreach (var mapItem in mapConfig.listConfigItems)
            {
                if (mapItem.mapId > maxMapId)
                {
                    maxMapId = mapItem.mapId;
                }
            }
            
            // Check if current map is the max map
            bool isMaxLevel = currentMapId >= maxMapId;
            
            if (enableDebugLogs) Debug.Log($"GameManager: Current map {currentMapId}, Max map {maxMapId}, IsMaxLevel: {isMaxLevel}");
            
            return isMaxLevel;
        }
        catch (Exception ex)
        {
            Debug.LogError($"GameManager: Error checking max level - {ex.Message}");
            return false; // Default to false if there's an error
        }
    }

    /// <summary>
    /// Show game over popup
    /// </summary>
    private async UniTask ShowGameOverPopup()
    {
        try
        {
            if (enableDebugLogs) Debug.Log("GameManager: Showing game over popup");
            
            var popup = await PopupManager.instance.OpenPopup<PopupGameOver>();
            // PopupGameOver doesn't need initialization parameters
            
            if (enableDebugLogs) Debug.Log("GameManager: Game over popup displayed");
        }
        catch (Exception ex)
        {
            Debug.LogError($"GameManager: Failed to show game over popup - {ex.Message}");
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
            
            // Initialize wave manager
            if (WaveManager.instance != null)
            {
                // WaveManager will be initialized when starting waves
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
            var selectedMap = PlayerPrefs.GetInt(PlayerPrefsConfig.Key_Select_Map_Id, 0);
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
    /// Start the wave system for the current game session
    /// </summary>
    private async UniTask StartWaveSystem()
    {
        try
        {
            if (enableDebugLogs) Debug.Log("GameManager: Starting wave system");
            
            // Get selected map (for now, use map ID)
            var selectedMap = PlayerPrefs.GetInt(PlayerPrefsConfig.Key_Select_Map_Id, 0);
            
            // Initialize wave manager with the selected map
            WaveManager.instance.InitializeMap(selectedMap);

            // Set spawn position (you might want to get this from map data)
            var spawnEnemyPosition = PathFinding.instance.GetStartWorldPos();
            WaveManager.instance.SetSpawnPosition(spawnEnemyPosition);
            
            // Start the wave system
            WaveManager.instance.StartWaveSystem().Forget();
            
            if (enableDebugLogs) Debug.Log("GameManager: Wave system started");
        }
        catch (Exception ex)
        {
            Debug.LogError($"GameManager: Failed to start wave system - {ex.Message}");
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
            // Stop wave system
            WaveManager.instance?.StopWaveSystem();
            
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

        if (enableDebugLogs) Debug.Log("GameManager: Subscribed to game events");
    }

    /// <summary>
    /// Unsubscribe from game events
    /// </summary>
    private void UnsubscribeFromGameEvents()
    {
        GameEventMgr.GED.UnRegister(GameEvent.OnPlayerDeath, OnPlayerDeath);
        
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