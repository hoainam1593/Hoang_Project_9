using R3;
using UnityEngine;

public class EnemyCtrl : EntityBase, IDamagable
{
    private const int EnemyKey = 50;
    private const int EnemyLimit = 1000;
    private const float FixedSpeed = 5f;
    
    private static int _uid = -1;

    private int id;
    public int Uid { get; private set; }
    public bool IsDead { get; private set; }
    public float MaxHp { get; private set; }
    public ReactiveProperty<float> CrrHp { get; private set; }
    public ReactiveProperty<Vector3> Position { get; private set; }
    private float speed;  //Unit per second
    private string name;

    private MapCoordinate target = MapCoordinate.oneNegative;
    private Vector3 targetPos;
    private Vector3 direction;
    private bool isMoving;
    private const float DefaultForward = 90f;
    
    private static int GenerateUid()
    {
        _uid++;
        return EnemyKey * EnemyLimit + _uid;
    }

    private void OnDrawGizmos()
    {
        if (isMoving)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(targetPos, 0.1f);
        }
    }

    #region Task - Spawn / Despawn
    
    protected override void InitData(object data)
    {
        Uid = GenerateUid();
        
        if (data == null)
        {
            return;
        }

        id = (int)data;
        var config = ConfigManager.instance.GetConfig<EnemyConfig>().GetItem(id);
        IsDead = true;
        MaxHp = config.hp;
        CrrHp = new ReactiveProperty<float>(config.hp);
        Position = new ReactiveProperty<Vector3>(transform.position);
        speed = config.speed / FixedSpeed;
        name = config.prefabName;
    }

    protected override void OnSpawnStart()
    {
        base.OnSpawnStart();
        gameObject.name = $"{name}[{Uid}]";
        gameObject.layer = LayerMask.NameToLayer("Enemy");

        CreateHealthBar();
    }

    protected override void OnSpawnComplete()
    {
        base.OnSpawnComplete();
        StartMove();
    }
    
    #endregion Task - Spawn / Despawn!!!
    
    #region Task - Create HealthBar

    private void CreateHealthBar()
    {
        //New HealthBarCtrl
        //Load prefab & SpawnPrefab
        //OnMoving => HealthBarCtrl.UpdatePosition();
        //IChangeablePositionUI
    }
    
    #endregion Task - Create HealthBar!
    
    #region Task - MainLoop
    
    protected override void OnLateUpdate()
    {
        if (!IsDead)
        {
            return;
        }
        
        if (isMoving)
        {
            Moving();
        }
    }
        

    protected override void UpdateEachInterval()
    {
        if (IsDead)
        {
            UpdateTarget();
        }
    }

    #endregion Task - MainLoop!!!
    
    #region Task - UpdateTarget
    private void UpdateTarget()
    {
        if (IsReachTarget())
        {
            if (IsEndOfPath())
            {
                EndMove();
            }
            else
            {
                GetNewTarget();
                Rotate();
            }
        }
    }

    private void GetFirstTarget()
    {
        target = PathFinding.instance.GetStartPoint();
        targetPos = PathFinding.instance.GetWorldPos(target);
        // Debug.Log("NewPos: " +  target);
        direction =  targetPos - transform.position;
        direction.z = 0;
    }

    private bool IsReachTarget()
    {
        var distance = targetPos - transform.position;
        return (Vector3.SqrMagnitude(distance) < Mathf.Epsilon);
    }

    private bool IsEndOfPath()
    {
        return PathFinding.instance.IsEnd(target);
    }

    private void GetNewTarget()
    {
        target = PathFinding.instance.GetNextPoint(target);
        targetPos = PathFinding.instance.GetWorldPos(target);
        direction =  targetPos - transform.position;
        direction.z = 0;
    }
    
    #endregion Task - UpdateTarget!!!
        
    #region Task - Move & Rotate
    
    private void StartMove()
    {
        GetFirstTarget();
        GetNewTarget();
        Rotate();
        isMoving = true;
    }

    private void Moving()
    {
        var newPos = Vector3.MoveTowards(transform.position, targetPos, speed * Time.deltaTime);
        transform.position = newPos;
        Position.Value = newPos;
    }

    private void EndMove()
    {
        isMoving = false;
        EntityManager.instance.DespawnEnemy(Uid);
        GameEventMgr.GED.DispatcherEvent(GameEvent.OnEnemyReachHallGate, Uid);
    }
    
    private void Rotate()
    {
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - DefaultForward;
        transform.rotation = Quaternion.Euler(0, 0, angle);
    }
    
    #endregion Task - Move & Rotate!!!
    
    #region Task - IDamageable

    public void TakeDamage(float dmg)
    {
        float crrHp = CrrHp.Value - dmg;
        if (crrHp <= 0)
        {
            CrrHp.Value = 0;
            IsDead = false;
            OnDead();
        }
        else
        {
            CrrHp.Value = crrHp;
        }
    }

    private void OnDead()
    {
        Debug.Log($"[EnemyCtrl] OnDead > Uid: {Uid}, Id: {id}");
        EntityManager.instance.DespawnEnemy(this.Uid);
    }
    #endregion Task - IDamageable!!!
}
