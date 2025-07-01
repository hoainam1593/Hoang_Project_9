public enum GameEvent
{
    //Camera Events:
    
    //Input Events:
    OnDraggedEnd,
    OnDraggedStart,
    OnDragging,
    OnClick,
    
    //
    OnMapSizeUpdate,
    
    //Spawn - Despawn Turret
    OnTurretSpawnStart,
    OnTurretSpawnCompleted,
    OnTurretDespawnStart,
    OnTurretDespawnComplete,
    
    //Spawn - Despawn Enemy
    OnEnemySpawnStart,
    OnEnemySpawnCompleted,
    OnEnemyDespawnStart,
    OnEnemyDespawnCompleted,
    //Enemy
    OnEnemyReachHallGate,
    
    //Game State Events
    OnGameStart,
    OnGameStarted,
    OnGameExit,
    OnGameExited,
    OnGamePaused,
    OnGameResumed,
    OnGameOver,
    OnVictory,
    
    //Player Events
    OnPlayerDeath,
    
    //Wave Events
    OnWaveComplete,
}