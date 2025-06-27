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
    
    
    //Game
    OnGameStart,
    OnGameExit,
}