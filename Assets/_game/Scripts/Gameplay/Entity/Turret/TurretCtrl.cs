using System;
using UnityEngine;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using R3;

public class TurretCtrl : EntityBase
{
    private int id;
    private float attack;
    private float range;
    private float speed; //the number of shooter per second
    private string name;
    private float shooterGapTime;
    private MapCoordinate mapCoordinate;
    private const float DefaultForward = 90f;
    [SerializeField] private Transform objectMachine;

    private bool isAttacking = false;
    private Transform target;
    private EnemyCtrl targetCtrl;
    private int targetUid;
    private IDisposable lookTargetDisposable;
    
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
        shooterGapTime = 1 / speed;
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

    protected override void OnUpdate()
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
            RemoveTarget();
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
            LookTarget();
            StartAttack();
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
            LookTarget();
            StartAttack();
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
            StopAttack();
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
        RemoveTarget();
    }

    private void RemoveTarget()
    {
        target = null;
        targetCtrl = null;
        targetUid = -1;
        lookTargetDisposable.Dispose();
    }
    #endregion DetectTarget!!!
    
    #region Task - Attack

    private void StartAttack()
    {
        isAttacking = true;
        AttackTarget().Forget();
    }

    private void StopAttack()
    {
        isAttacking = false;
    }


    private async UniTaskVoid AttackTarget()
    {
        while (isAttacking && target != null)
        {
            // Spawn bullet thay vì gây damage trực tiếp
            EntityManager.instance.SpawnBullet<BulletCtrl>(
                objectMachine.position,
                attack,
                targetCtrl
            ).Forget();

            // Wait for next shot
            await UniTask.WaitForSeconds(shooterGapTime);
        }
    }

    private void LookTarget()
    {
        if (targetCtrl == null)
        {
            return;
        }

        lookTargetDisposable = targetCtrl.Position.Subscribe((position) => Rotate(position));
    }

    private void Rotate(Vector3 pos)
    {
        var direction = pos - transform.position; 
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - DefaultForward;
        objectMachine.rotation = Quaternion.Euler(0, 0, angle);
    }
    #endregion Task - Attack!

}