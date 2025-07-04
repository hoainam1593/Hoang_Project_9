public class TurretConfig: BaseConfig<TurretConfigItem>
{
    public TurretConfigItem GetItem(int id)
    {
        foreach (var item in listConfigItems)
        {
            if (item.id == id)
            {
                return item;
            }
        }

        return null; 
    }  
    
    public TurretConfigItem GetItem(TurretType type, int turretLevel)
    {
        foreach (var item in listConfigItems)
        {
            if (item.type == type && item.level == turretLevel)
            {
                return item;
            }
        }

        return null;
    }
}