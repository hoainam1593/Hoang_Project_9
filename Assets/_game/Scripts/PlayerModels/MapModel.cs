using UnityEngine;
using System.Collections.Generic;

public class MapModel : BasePlayerModel
{
    public override int ModelVersion => 1;

    public int MapCount;
    public List<MapInfo> Maps; // Renamed from Chapters to Maps for consistency

    public override void ReadOrWrite(IFileStream stream, int version)
    {
        stream.ReadOrWriteInt(ref MapCount, "MapCount");
        stream.ReadOrWriteListObj<MapInfo>(ref Maps, "Maps"); // Updated parameter name
    }

    public override void OnModelInitializing()
    {
        var mapConfig = ConfigManager.instance.GetConfig<MapConfig>();
        
        MapCount = mapConfig.listConfigItems.Count;
        Maps = new List<MapInfo>(); // Updated variable name
        
        foreach (var item in mapConfig.listConfigItems)
        {
            Maps.Add(new MapInfo(item.mapId, -1)); // Updated variable name
        }

        Maps[0].Star = 0; // Updated variable name
    }
}
