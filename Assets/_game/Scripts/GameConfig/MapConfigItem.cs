public class MapConfigItem : BaseConfigItem
{
    public int mapId;
    public string mapName;
    
    public override void ReadOrWrite(IFileStream stream)
    {
        stream.ReadOrWriteInt(ref mapId, "map_id");
        stream.ReadOrWriteString(ref mapName, "map_data");
    }

    public override string ToString()
    {
        return $"{{map_id: {mapId}, map_name: {mapName}}}";
    }
}