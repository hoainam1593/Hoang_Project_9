using System;
using UnityEngine;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;

public partial class GameLauncher : SingletonMonoBehaviour<GameLauncher>
{
    [Header("Debug")]
    [SerializeField] private bool enableDebugLogs = true;
    
    protected override void Awake()
    {
        base.Awake();
        DontDestroyOnLoad(this);
    }

    private async UniTaskVoid Start()
    {
        await InitializeApplication();
    }

    #region Application Lifecycle
    private async UniTask InitializeApplication()
    {
        try
        {
            if (enableDebugLogs) Debug.Log("GameLauncher: Initializing application");
            
            // Ensure GameManager is available before proceeding
            if (GameManager.instance == null)
            {
                Debug.LogError("GameLauncher: GameManager is not available! Please ensure it's setup correctly.");
                return;
            }
            
            // Load configurations first
            await LoadConfigurations();
            
            // Load player models
            LoadPlayerModels();
            
            // Initialize GameManager and launch game
            await GameManager.instance.LaunchGame();
            
            if (enableDebugLogs) Debug.Log("GameLauncher: Application initialized successfully");
        }
        catch (Exception ex)
        {
            Debug.LogError($"GameLauncher: Failed to initialize application - {ex.Message}");
        }
    }

    public async UniTaskVoid StartGame()
    {
        if (enableDebugLogs) Debug.Log("GameLauncher: Request to start game");
        
        if (GameManager.instance == null)
        {
            Debug.LogError("GameLauncher: GameManager is not available!");
            return;
        }
        
        await GameManager.instance.StartGame();
    }

    public async UniTaskVoid ExitGame()
    {
        if (enableDebugLogs) Debug.Log("GameLauncher: Request to exit game");
        
        if (GameManager.instance == null)
        {
            Debug.LogError("GameLauncher: GameManager is not available!");
            return;
        }
        
        await GameManager.instance.ExitGame();
    }
    #endregion
    
    #region Configuration Loading
    private async UniTask LoadConfigurations()
    {
        try
        {
            if (enableDebugLogs) Debug.Log("GameLauncher: Loading configurations");
            
            // Create config list with default game configs
            var configList = new List<IBaseConfig>
            {
                new MapConfig(),
                new EnemyConfig(),
                new TurretConfig(),
            };
            
            // Create GameListConfig with the config list
            var gameConfig = new GameListConfig(configList);
            
            // Load all configs
            await ConfigManager.instance.LoadAllConfigs(gameConfig);
            
            if (enableDebugLogs)
            {
                var mapConfig = ConfigManager.instance.GetConfig<MapConfig>();
                Debug.Log($"GameLauncher: Loaded {mapConfig.listConfigItems.Count} map configurations");
            }
            
            if (enableDebugLogs) Debug.Log("GameLauncher: Configurations loaded successfully");
        }
        catch (Exception ex)
        {
            Debug.LogError($"GameLauncher: Failed to load configurations - {ex.Message}");
            throw;
        }
    }
    #endregion

    #region Model Loading
    private void LoadPlayerModels()
    {
        try
        {
            if (enableDebugLogs) Debug.Log("GameLauncher: Loading player models");
            
            List<BasePlayerModel> models = new List<BasePlayerModel>()
            {
                new MapModel(),
            };
            
            PlayerModelManager.instance.LoadAllModels(models, "MapModel");
            
            if (enableDebugLogs) Debug.Log("GameLauncher: Player models loaded successfully");
        }
        catch (Exception ex)
        {
            Debug.LogError($"GameLauncher: Failed to load player models - {ex.Message}");
        }
    }
    #endregion

    #region Application Events
    private void OnApplicationPause(bool pauseStatus)
    {
        if (enableDebugLogs) Debug.Log($"GameLauncher: Application pause state changed to {pauseStatus}");
        
        // Forward to GameManager for handling
        if (GameManager.instance != null)
        {
            if (pauseStatus)
                GameManager.instance.PauseGame();
            else
                GameManager.instance.ResumeGame();
        }
    }

    private void OnApplicationFocus(bool hasFocus)
    {
        if (enableDebugLogs) Debug.Log($"GameLauncher: Application focus state changed to {hasFocus}");
        
        // Forward to GameManager for handling
        if (GameManager.instance != null)
        {
            if (!hasFocus)
                GameManager.instance.PauseGame();
            else
                GameManager.instance.ResumeGame();
        }
    }

    private void OnApplicationQuit()
    {
        if (enableDebugLogs) Debug.Log("GameLauncher: Application is quitting");
        
        // Cleanup if needed
        if (GameManager.instance != null)
        {
            GameManager.instance.ExitGame().Forget();
        }
    }
    #endregion
}
