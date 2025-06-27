using System;
using System.Data.Common;
using Cysharp.Threading.Tasks;
using Cysharp.Threading.Tasks.Triggers;
using R3;
using UnityEngine;
using UnityEngine.UIElements;

public class EnemyCtrl : EntityBase
{
    private const int EnemyKey = 50;
    private const int EnemyLimit = 1000;
    
    private static int _uid = -1;

    private int id;
    private ReactiveProperty<bool> isAlive;
    private ReactiveProperty<float> hp;
    private float speed;

    private MapCoordinate target = MapCoordinate.oneNegative;
    private Vector3 targetPos;
    private Vector3 direction;
    private bool isMoving;
    private const float DefaultForward = 90f;
    
    public int Uid { get; private set; }
    
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
        isAlive = new ReactiveProperty<bool>(true);
        hp = new ReactiveProperty<float>(config.hp);
        speed = config.speed/4;
    }

    protected override void OnInitComplete()
    {
        base.OnInitComplete();
        StartMove();
    }
    
    #endregion Task - Spawn / Despawn!!!
    
    
    #region Task - MainLoop
    
    protected override void OnLateUpdate()
    {
        if (!isAlive.Value)
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
        Debug.Log("UpdateEachInterval");
        if (isAlive.Value)
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
}
