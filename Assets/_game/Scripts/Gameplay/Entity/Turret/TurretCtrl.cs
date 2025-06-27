using UnityEngine;
using UnityEngine.AddressableAssets.Initialization;
using UnityEngine.UIElements;

public class TurretCtrl : EntityBase
{
    private int id;
    private float attack;
    private float range;
    private float speed;
    private string name;
    private MapCoordinate mapCoordinate;
    
    public override void OnSpawn(object data)
    {
        base.OnSpawn(data);
        InitData(data);
    }

    protected override void InitData(object data)
    {
        var enemyData = ((int id, MapCoordinate mapCoordinate))data;
        id = enemyData.id;
        mapCoordinate = enemyData.mapCoordinate;
        
        var config = ConfigManager.instance.GetConfig<TurretConfig>().GetItem(id);
        name = config.prefabName;
        attack = config.attack;
        range = config.range;
        speed = config.speed;
    }

    protected override void OnInitStart()
    {
        base.OnInitStart();
        gameObject.name = $"{name}[{mapCoordinate}]";
    }

    #region Main Stream

    protected override void UpdateEachInterval()
    {
        
    }

    #endregion Main Stream!!
}