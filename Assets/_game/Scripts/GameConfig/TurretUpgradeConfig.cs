public class TurretUpgradeConfig: BaseConfig<TurretUpgradeConfigItem>
{
    public TurretUpgradeConfigItem GetItem(int turretId)
    {
        foreach (var item in listConfigItems)
        {
            if (item.turretId == turretId)
            {
                return item;
            }
        }

        return null;
    }
}