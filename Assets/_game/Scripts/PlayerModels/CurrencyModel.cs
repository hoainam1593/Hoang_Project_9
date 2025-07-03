using UnityEngine;

/// <summary>
/// Player model to hold main player data - simplified to only gold for upgrades.
/// </summary>
public class CurrencyModel : BasePlayerModel
{
    public override int ModelVersion => 1;

    // Main currency for upgrades
    public int Gold;

    public override void ReadOrWrite(IFileStream stream, int version)
    {
        // Only persist gold
        stream.ReadOrWriteInt(ref Gold, "Gold");
    }

    public override void OnModelInitializing()
    {
        // Initialize with default starting gold
        Gold = 1000; // Starting gold for upgrades
        
        if (Debug.isDebugBuild)
        {
            Debug.Log("PlayerModel: Initialized with default gold: " + Gold);
        }
    }
}