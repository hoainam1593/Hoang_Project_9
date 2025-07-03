using Amazon.CloudFront.Model;
using System.Collections.Generic;

public class TurretUpgradeConfigItem : BaseConfigItem
{
    public int turretId;
    public List<TurretUpgradeStats> bonusStats = new List<TurretUpgradeStats>();

    public override void ReadOrWrite(IFileStream stream)
    {
        stream.ReadOrWriteInt(ref turretId, "turret_id");
        stream.ReadOrWriteListObj(ref bonusStats, "upgrade_lv");
    }

    public TurretUpgradeStats GetBonusStats(int upgradeLv)
    {
        foreach (var stats in bonusStats)
        {
            if (stats.upgradeLv == upgradeLv)
            {
                return stats;
            }
        }
        return null;
    }

    public override string ToString()
    {
        var str = $"{{turret_id: {turretId}, bonus_stats: [";
        foreach (var stats in bonusStats)
        {
            str += $"{stats}, ";
        }
        str += "]}}";
        return str;
    }
}