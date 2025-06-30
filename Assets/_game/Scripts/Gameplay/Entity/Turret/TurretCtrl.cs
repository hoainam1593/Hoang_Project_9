using System;
using UnityEngine;
using System.Collections.Generic;

public class TurretCtrl : EntityBase
{
    private int id;
    private float attack;
    private float range;
    private float speed;
    private string name;
    private MapCoordinate mapCoordinate;

    // private List<Transform> inRangeEnemies;
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
        
        // inRangeEnemies = new List<Transform>();
    }

    protected override void OnSpawnStart()
    {
        base.OnSpawnStart();
        Subscribes();

        gameObject.name = $"{name}[{mapCoordinate}]";
        gameObject.layer = LayerMask.NameToLayer("Player");
        
        var rig2D = gameObject.AddComponent<Rigidbody2D>();
        rig2D.bodyType = RigidbodyType2D.Kinematic;
        
        var collider2D = gameObject.AddComponent<CircleCollider2D>();
        collider2D.radius = range;
        collider2D.isTrigger = true;
    }

    public override void OnDespawn()
    {
        base.OnDespawn();
        UnSubscribes();
    }
    
    #endregion EntityBase
    
    #region Main Stream

    protected override void UpdateEachInterval()  //const: 0.02
    {
    }
    
    #endregion Main Stream!!
    
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


    #region DetectTarget

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (target != null)
        {
            return;
        }

        if (other.gameObject.layer == LayerMask.NameToLayer("Enemy"))
        {
            TargetFound(other);
        }
    }
    
    private void OnTriggerStay2D(Collider2D other)
    {
        if (target != null)
        {
            return;
        }

        if (other.gameObject.layer == LayerMask.NameToLayer("Enemy"))
        {
            TargetFound(other);
        }
    }
    
    private void OnTriggerExit2D(Collider2D other)
    {
        if (target == null)
        {
            return;
        }

        if (other.gameObject.layer == LayerMask.NameToLayer("Enemy")
            && other.gameObject == target.gameObject)
        {
            TargetOutOfRange();
        }
    }

    private void TargetFound(Collider2D other)
    {
        target = other.transform;
        targetCtrl = target.GetComponent<EnemyCtrl>();
        targetUid = targetCtrl.Uid;
        Debug.Log("DetectTarget: " + target.gameObject.name);
    }

    private void TargetOutOfRange()
    {
        Debug.Log("TargetOutOfRange: " + target.gameObject.name);
        target = null;
        targetCtrl = null;
        targetUid = -1;
    }
    #endregion DetectTarget!!!
    
}