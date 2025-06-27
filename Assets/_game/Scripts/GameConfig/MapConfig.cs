
public class MapConfig : BaseConfig<MapConfigItem>
{
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
