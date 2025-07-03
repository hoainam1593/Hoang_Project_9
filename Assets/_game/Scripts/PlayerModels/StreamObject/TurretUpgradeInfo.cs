using Amazon.S3.Model;
using Unity.Profiling;

public class TurretUpgradeInfo : IFileStreamObject
{
    public int ModelVersion => 1;

    public int upgradeLv; // 0 is locked, 1 -> 5 is the upgrade level
    public float attack;
    public float range;
    public float speed;

    public TurretUpgradeInfo()
    {
        this.upgradeLv = 0; // Locked by default
        this.attack = 0;
        this.range = 0;
        this.speed = 0;
    }

    public void ReadOrWrite(IFileStream stream, int version)
    {
        stream.ReadOrWriteInt(ref upgradeLv, "upgrade_lv");
    }
}