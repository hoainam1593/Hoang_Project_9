public enum GameEvent
{
    //Camera Events:
    
    //Input Events:
    OnDraggedEnd,
    OnDraggedStart,
    OnDragging,
    OnClick,

    //Manager Events:
    OncurrencyManagerInit,
    OnTurretUpgradeMgrInit,

    //Map
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
    OnEnemyDead,
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
    OnGameRetry,

    //Player Events
    OnPlayerInit,
    OnPlayerDeath,
    OnPlayerHealthChanged,
    OnPlayerHPLost,
    OnPlayerHPGained,

    //Wave Events
    OnWaveManagerInit,
    OnWaveComplete,
    OnWaveStart,
    OnAllWavesComplete,
    OnWaveEnemySpawned,
    OnWaveStateChanged,
}