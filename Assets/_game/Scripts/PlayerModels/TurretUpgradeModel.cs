using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;

public class TurretUpgradeModel : BasePlayerModel
{
    public override int ModelVersion => 1;

    public Dictionary<int, TurretUpgradeInfo> upgradeInfos; //<turretId, TurretUpgradeInfo>

    public override void ReadOrWrite(IFileStream stream, int version)
    {
        stream.ReadOrWriteDicIntObj<TurretUpgradeInfo>(ref upgradeInfos, "upgradeInfos"); // Updated parameter name
    }

    public override void OnModelInitializing()
    {
        var turretUpgradeConfig = ConfigManager.instance.GetConfig<TurretUpgradeConfig>();

        upgradeInfos = new Dictionary<int, TurretUpgradeInfo>();
        foreach (var config in turretUpgradeConfig.listConfigItems)
        {
            //Debug.Log($"TurretUpgradeModel: Initializing upgrade info for turretId {config}");
            upgradeInfos.Add(config.turretId, new TurretUpgradeInfo());
        }

        upgradeInfos[0].upgradeLv = 1;
    }


    #region Processing Data Internal

    public TurretUpgradeInfo GetItem(int turretId)
    {
        if (upgradeInfos.TryGetValue(turretId, out var info))
        {
            return info;
        }
        return null;
    }
    #endregion Processing Data Internal!!!
}
