using System.Collections.Generic;

public class TurretConfigItem : BaseConfigItem
{
    public int id;
    public TurretType type;
    public int level;
    public string prefabName;
    public float attack;
    public float range;
    public float speed;
    public int cost;
    
    public override void ReadOrWrite(IFileStream stream)
    {
        stream.ReadOrWriteInt(ref id, "turret_id");
        stream.ReadOrWriteEnum<TurretType>(ref type, "type");
        stream.ReadOrWriteInt(ref level, "level");
        stream.ReadOrWriteString(ref prefabName, "prefab_name");
        stream.ReadOrWriteFloat(ref attack, "attack");
        stream.ReadOrWriteFloat(ref range, "range");
        stream.ReadOrWriteFloat(ref speed, "speed");
        stream.ReadOrWriteInt(ref cost, "cost");
    }

    public override string ToString()
    {
        return $"{{turret_id: {id}, type: {type}, level: {level}, prefab: {prefabName}}}";
    }    
}