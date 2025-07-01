using Unity.Collections;

/// <summary>
/// Configuration item for a single wave containing enemy spawn data
/// </summary>
public class WaveConfigItem : IFileStreamObject
{
    public int id;
    public EnemyType enemyType;
    public int num;          // Number of enemies to spawn
    public float gapTime;    // Time interval between spawns

    public int ModelVersion => 1;

    public void ReadOrWrite(IFileStream stream, int version)
    {
        switch (version)
        {
            case 1:
                stream.ReadOrWriteInt(ref id, "wave_id");
                stream.ReadOrWriteEnum<EnemyType>(ref enemyType, "enemy_type");
                stream.ReadOrWriteInt(ref num, "enemy_count");
                stream.ReadOrWriteFloat(ref gapTime, "gap_time");
                break;
            default:
                stream.ReadOrWriteInt(ref id, "wave_id");
                stream.ReadOrWriteEnum<EnemyType>(ref enemyType, "enemy_type");
                stream.ReadOrWriteInt(ref num, "enemy_count");
                stream.ReadOrWriteFloat(ref gapTime, "gap_time");
                break;
        }
    }

    public override string ToString()
    {
        return $"{{wave_id: {id}, enemy_type: {enemyType}, count: {num}, gap_time: {gapTime}}}";
    }
}