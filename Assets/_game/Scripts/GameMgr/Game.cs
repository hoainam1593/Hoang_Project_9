using UnityEngine;

/// <summary>
/// Interface for game control operations
/// </summary>
public interface IGame
{
    public void StartGame();
    public void PauseGame();
    public void ResumeGame();
    public void ExitGame();
}

/// <summary>
/// Core game logic class handling gameplay states and mechanics
/// </summary>
public class Game : IGame
{
    #region Properties
    /// <summary>
    /// Current gameplay state
    /// </summary>
    public GameplayState CurrentState { get; private set; }
    
    /// <summary>
    /// Check if the game has started
    /// </summary>
    public bool IsStarted => CurrentState != GameplayState.Stopped;
    
    /// <summary>
    /// Check if the game is paused
    /// </summary>
    public bool IsPaused => CurrentState == GameplayState.Paused;
    
    /// <summary>
    /// Check if the game is actively playing
    /// </summary>
    public bool IsPlaying => CurrentState == GameplayState.Playing;
    #endregion

    #region Constructor
    /// <summary>
    /// Initialize the game in stopped state
    /// </summary>
    public Game()
    {
        CurrentState = GameplayState.Stopped;
        Debug.Log("Game: Initialized");
    }
    #endregion

    #region IGame Implementation
    /// <summary>
    /// Start the gameplay systems
    /// </summary>
    public void StartGame()
    {
        Debug.Log("Game: Starting gameplay");
        CurrentState = GameplayState.Playing;
        Time.timeScale = 1f;
        
        // Dispatch game started event
        GameEventMgr.GED.DispatcherEvent(GameEvent.OnGameStarted);
        
        // Start game-specific systems
        StartGameSystems();
    }

    /// <summary>
    /// Pause the gameplay
    /// </summary>
    public void PauseGame()
    {
        if (CurrentState == GameplayState.Playing)
        {
            Debug.Log("Game: Pausing gameplay");
            CurrentState = GameplayState.Paused;
            Time.timeScale = 0f;
            
            GameEventMgr.GED.DispatcherEvent(GameEvent.OnGamePaused);
        }
    }

    /// <summary>
    /// Resume the paused gameplay
    /// </summary>
    public void ResumeGame()
    {
        if (CurrentState == GameplayState.Paused)
        {
            Debug.Log("Game: Resuming gameplay");
            CurrentState = GameplayState.Playing;
            Time.timeScale = 1f;
            
            GameEventMgr.GED.DispatcherEvent(GameEvent.OnGameResumed);
        }
    }

    /// <summary>
    /// Stop the gameplay and clean up
    /// </summary>
    public void ExitGame()
    {
        Debug.Log("Game: Stopping gameplay");
        CurrentState = GameplayState.Stopped;
        Time.timeScale = 1f;
        
        // Stop game-specific systems
        StopGameSystems();
        
        GameEventMgr.GED.DispatcherEvent(GameEvent.OnGameExited);
    }
    #endregion

    #region Private Methods
    /// <summary>
    /// Start all game-specific systems
    /// </summary>
    private void StartGameSystems()
    {
        // Start wave management if available
        // WaveManager.instance?.StartWave();
        
        // Reset player data if needed
        // PlayerDataManager.instance?.ResetData();
        
        Debug.Log("Game: Game systems started");
    }

    /// <summary>
    /// Stop all game-specific systems
    /// </summary>
    private void StopGameSystems()
    {
        // Stop any ongoing game processes
        // WaveManager.instance?.StopWave();
        
        Debug.Log("Game: Game systems stopped");
    }
    #endregion
}
