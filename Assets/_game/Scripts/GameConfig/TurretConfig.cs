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
}