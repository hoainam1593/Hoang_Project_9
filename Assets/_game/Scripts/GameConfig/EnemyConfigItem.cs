public class EnemyConfigItem : BaseConfigItem
{
    public int id;
    public string prefabName;
    public float hp;
    public float speed;
    
    public override void ReadOrWrite(IFileStream stream)
    {
        stream.ReadOrWriteInt(ref id, "enemy_id");
        stream.ReadOrWriteString(ref prefabName, "prefab_name");
        stream.ReadOrWriteFloat(ref hp, "hp");
        stream.ReadOrWriteFloat(ref speed, "speed");
    }
}