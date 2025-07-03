using Amazon.S3.Model;
using Unity.Collections;

/// <summary>
/// Configuration item for a single wave containing enemy spawn data
/// </summary>
public class TurretUpgradeStats : IFileStreamObject
{
    public int upgradeLv;
    public int cost;
    public float attack;
    public float range;
    public float speed;

    public int ModelVersion => 1;

    public void ReadOrWrite(IFileStream stream, int version)
    {
        switch (version)
        {
            case 1:
                stream.ReadOrWriteInt(ref upgradeLv, "upgrade_lv");
                stream.ReadOrWriteInt(ref cost, "cost");
                stream.ReadOrWriteFloat(ref attack, "attack_up");
                stream.ReadOrWriteFloat(ref range, "range_up");
                stream.ReadOrWriteFloat(ref speed, "speed_up");
                break;
            default:
                stream.ReadOrWriteInt(ref upgradeLv, "upgrade_lv");
                stream.ReadOrWriteInt(ref cost, "cost");
                stream.ReadOrWriteFloat(ref attack, "attack_up");
                stream.ReadOrWriteFloat(ref range, "range_up");
                stream.ReadOrWriteFloat(ref speed, "speed_up");
                break;
        }
    }

    public override string ToString()
    {
        return $"UpgradeLv: {upgradeLv}, Cost: {cost}, Attack: {attack}, Range: {range}, Speed: {speed}";
    }
}