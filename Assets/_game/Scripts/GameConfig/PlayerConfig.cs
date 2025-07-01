using UnityEngine;

public class PlayerConfig : BaseConfig<PlayerConfigItem>
{
    public PlayerConfigItem GetFirst()
    {
        if (listConfigItems == null || listConfigItems.Count == 0)
        {
            Debug.LogError("PlayerConfig: No items found.");
            return null;
        }
        
        return listConfigItems[0];
    }
}
