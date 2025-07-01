using System.Collections.Generic;

/// <summary>
/// Configuration for map waves containing multiple waves
/// </summary>
public class MapWaveConfig : BaseConfig<MapWaveConfigItem>
{
    public MapWaveConfigItem GetItem(int mapId)
    {
        foreach (var item in listConfigItems)
        {
            if (item.mapId == mapId)
            {
                return item;
            }
        }

        return null;
    }
}