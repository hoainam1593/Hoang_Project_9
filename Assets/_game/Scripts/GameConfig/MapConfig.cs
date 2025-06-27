
public class MapConfig : BaseConfig<MapConfigItem>
{
    public MapConfigItem GetMapItem(int Id)
    {
        foreach (var item in listConfigItems)
        {
            if (item.mapId == Id)
            {
                return item;
            }
        }

        return null; 
    }
    
    public string GetMapName(int Id)
    {
        foreach (var item in listConfigItems)
        {
            if (item.mapId == Id)
            {
                return item.mapName;
            }
        }

        return "";
    }
}
