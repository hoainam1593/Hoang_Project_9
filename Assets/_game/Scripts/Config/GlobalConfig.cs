/// <summary>
/// Scene names used in the game
/// </summary>
public enum SceneName
{
    SceneMain,
    SceneBattle,
    SceneLoading
}

/// <summary>
/// Overall game states for application flow
/// </summary>
public enum GameState
{
    MainMenu,
    Loading,
    Playing,
    Paused,
    GameOver,
    Victory
}

/// <summary>
/// Gameplay-specific states during battle
/// </summary>
public enum GameplayState
{
    Playing,
    Paused,
    Stopped
}

public static class ResourcesConfig
{
    public const string TurretPrefab = "Assets/_game/AssetResources/Prefab/Turret";
    public const string EnemyPrefab = "Assets/_game/AssetResources/Prefab/Enemy";
    public const string BulletPrefab = "Assets/_game/AssetResources/Prefab";
    public const string MapData = "Assets/_game/AssetResources/MapData";
}

public static class PlayerPrefsConfig
{
    public const string Key_Select_Map = "Key_Select_Map";
}
