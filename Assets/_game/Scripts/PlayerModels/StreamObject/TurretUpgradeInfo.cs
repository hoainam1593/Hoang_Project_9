public class TurretUpgradeInfo : IFileStreamObject
{
    public int ModelVersion => 1;

    public int upgradeLv; // -1 is locked, 0 is unlocked but inactive, >0 is active upgrade level
    public float attack;
    public float range;
    public float speed;

    public TurretUpgradeInfo()
    {
        this.upgradeLv = -1; // Locked by default
        this.attack = 0;
        this.range = 0;
        this.speed = 0;
    }

    public void ReadOrWrite(IFileStream stream, int version)
    {
        stream.ReadOrWriteInt(ref upgradeLv, "upgrade_lv");
    }
}