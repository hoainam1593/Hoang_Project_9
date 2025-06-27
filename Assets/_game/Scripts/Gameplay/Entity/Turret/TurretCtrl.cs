using UnityEngine;

public class TurretCtrl : EntityBase
{
    private int id;
    private float attack;
    private float range;
    private float speed;
    private string name;
    private MapCoordinate mapCoordinate;

    private Transform target;
    private EnemyCtrl targetCtrl;
    private int targetUid;
    
    #region EntityBase

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

    protected override void OnSpawnStart()
    {
        base.OnSpawnStart();
        Subscribes();
        gameObject.name = $"{name}[{mapCoordinate}]";
        gameObject.layer = LayerMask.NameToLayer("Player");
    }

    public override void OnDespawn()
    {
        base.OnDespawn();
        UnSubscribes();
    }
    
    #endregion EntityBase
    
    
    #region Subscribes/UnSubscribes Event

    private void Subscribes()
    {
        GameEventMgr.GED.Register(GameEvent.OnEnemyDespawnCompleted, OnEnemyDespawn);
    }

    private void UnSubscribes()
    {
        GameEventMgr.GED.UnRegister(GameEvent.OnEnemyDespawnCompleted, OnEnemyDespawn);
    }

    private void OnEnemyDespawn(object data)
    {
        var enemyUId = (int)data;
        if (enemyUId == targetUid)
        {
            target = null;
            targetUid = -1;
        }
    }
    
    #endregion Subscribes/UnSubscribes!!

    #region Main Stream

    protected override void UpdateEachInterval()
    {
        if (target == null)
        {
            DetectTarget();
            return;
        }

        if (IsTargetOutOfRange())
        {
            ChangeTarget();
        }
    }
    
    #endregion Main Stream!!

    private void DetectTarget()
    {
        var layerMask = LayerMask.GetMask("Enemy");
        var hit = Physics2D.CircleCast(transform.position, range, Vector2.zero, range, layerMask: layerMask);
        if (hit)
        {
            target = hit.transform;
            targetCtrl = target.GetComponent<EnemyCtrl>();
            targetUid = targetCtrl.Uid;
            Debug.Log("target found: " + target.gameObject.name);
        }
    }

    private bool IsTargetOutOfRange()
    {
        return ((target.position - transform.position).sqrMagnitude > range * range);
    }

    private void ChangeTarget()
    {
        target = null;
        targetCtrl = null;
        targetUid = -1;
        DetectTarget();
    }
    
}