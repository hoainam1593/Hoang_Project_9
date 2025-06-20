
using UnityEngine;

public enum GameScreenOrientation
{
    Portrait,
    Landscape,
}

public partial class GameFrameworkConfig
{
    [Header("build player")] 
    public int androidTargetSDK = 34;
    public GameScreenOrientation screenOrientation;
    public string androidKeystorePassword = "Hopper#123";
    
    private static readonly Vector2Int landscapeWindowSize = new(1802, 942);
    private static readonly Vector2Int portraitWindowSize = new(520, 968);
    public Vector2Int buildWindowSize
    {
        get
        {
            return screenOrientation switch
            {
                GameScreenOrientation.Portrait => portraitWindowSize,
                GameScreenOrientation.Landscape => landscapeWindowSize,
                _ => Vector2Int.zero,
            };
        }
    }
}
