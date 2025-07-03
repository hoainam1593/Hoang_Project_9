using UnityEngine;

public class GameResultConfig : BaseConfig<GameResultConfigItem>
{
    public GameResultConfigItem GetFirst()
    {
        return listConfigItems.Count > 0 ? listConfigItems[0] : null;
    }
}
