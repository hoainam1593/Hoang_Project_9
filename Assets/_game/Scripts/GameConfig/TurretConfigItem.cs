using System.Collections.Generic;

public class TurretConfigItem : BaseConfigItem
{
    public int id;
    public int type;
    public int level;
    public string prefabName;
    public float attack;
    public float range;
    public float speed;
    public string upgradeStats;
    
    public override void ReadOrWrite(IFileStream stream)
    {
        stream.ReadOrWriteInt(ref id, "turret_id");
        stream.ReadOrWriteInt(ref type, "type");
        stream.ReadOrWriteInt(ref level, "level");
        stream.ReadOrWriteString(ref prefabName, "prefab_name");
        stream.ReadOrWriteFloat(ref attack, "attack");
        stream.ReadOrWriteFloat(ref range, "range");
        stream.ReadOrWriteFloat(ref speed, "speed");
        stream.ReadOrWriteString(ref upgradeStats, "upgrade_stats_percent");
    }

    public override string ToString()
    {
        return $"{{turret_id: {id}, type: {type}, level: {level}, prefab: {prefabName}}}";
    }    
}