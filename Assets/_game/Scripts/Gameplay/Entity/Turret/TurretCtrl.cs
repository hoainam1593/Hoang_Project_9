using System;
using UnityEngine;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using R3;
using Cysharp.Threading.Tasks.CompilerServices;

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
    public TurretConfigItem Config { get; private set; }

    [SerializeField] private Transform objectMachine;
    [SerializeField] private List<Transform> gunHead;

    private bool isAttacking = false;
    private Transform target;
    private EnemyCtrl targetCtrl;
    private int targetUid;
    private IDisposable lookTargetDisposable;
    private CircleCollider2D collider2D;

    #region EntityBase

    private void Awake()
    {
        var rig2D = gameObject.AddComponent<Rigidbody2D>();
        rig2D.bodyType = RigidbodyType2D.Kinematic;

        collider2D = gameObject.AddComponent<CircleCollider2D>();
    }

    protected override void InitData(object data)
    {
        var enemyData = ((int id, MapCoordinate mapCoordinate))data;
        id = enemyData.id;
        mapCoordinate = enemyData.mapCoordinate;
        
        Config = ConfigManager.instance.GetConfig<TurretConfig>().GetItem(id);
        name = Config.prefabName;
        attack = Config.attack;
        range = Config.range;
        speed = Config.speed;
        shooterGapTime = 1 / speed;
    }

    protected override void OnSpawnStart()
    {
        base.OnSpawnStart();
        Subscribes();

        gameObject.name = $"{name}[{mapCoordinate}]";
        gameObject.layer = LayerMask.NameToLayer("Player");

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


    #region Getters/Setters

    public TurretInfo GetTurretInfo()
    {
        return new TurretInfo
        {
            mapCoordinate = this.mapCoordinate,
            config = this.Config,
        };
    }

    #endregion Getters/Setters!!!

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
            StopAttack();
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
        StopAttack();
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
        SpawnBullets().Forget();
    }

    private void StopAttack()
    {
        isAttacking = false;
    }


    private async UniTaskVoid SpawnBullets()
    {
        while (isAttacking && target != null)
        {
            for (int i = 0; i < gunHead.Count; i++)
            {
                SpawnBullet(gunHead[i]);
            }

            // Wait for next shot
            await UniTask.WaitForSeconds(shooterGapTime);
        }
    }

    private void SpawnBullet(Transform gunHead)
    {            
        // Spawn bullet thay vì gây damage trực tiếp
        EntityManager.instance.SpawnBullet<BulletCtrl>(
            gunHead.position,
            attack,
            targetCtrl,
            range
        ).Forget();

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

public class TurretInfo
{
    public MapCoordinate mapCoordinate { get; set; }
    public TurretConfigItem config { get; set; }
}