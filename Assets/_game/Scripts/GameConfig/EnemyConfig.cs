public class EnemyConfig : BaseConfig<EnemyConfigItem>
{
    public EnemyConfigItem GetItem(int id)
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