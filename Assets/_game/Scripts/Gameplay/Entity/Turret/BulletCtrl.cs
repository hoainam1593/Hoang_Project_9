using System;
using UnityEngine;

public class BulletCtrl : EntityBase
{
    private EnemyCtrl Target;
    private int targetUid = -1;
    private float dmg;
    private Vector3 direction;
    
    public const float speed = 10f;

    #region EntityBase
    protected override void InitData(object data)
    {
        var bulletData = ((EnemyCtrl target, float dmg))data;
        this.Target = bulletData.target;
        this.targetUid = Target.Uid;
        this.dmg = bulletData.dmg;
    }

    protected override void OnSpawnStart()
    {
        base.OnSpawnStart();
        Subscribes();
    }
    
    public override void OnDespawn()
    {
        base.OnDespawn();
        UnSubscribes();
    }

    protected override void OnUpdate()
    {
        if (Target == null)
        {
            DespawnSelf();
            return;
        }

        // Di chuyển bullet về phía target
        Moving();
    }

    private void Moving()
    {
        direction = (Target.transform.position - transform.position).normalized;
        transform.position += direction * speed * Time.deltaTime;
    }

    #endregion EntityBase
    
    #region Subscribes/UnSubscribes Event

    private void Subscribes()
    {
        GameEventMgr.GED.Register(GameEvent.OnEnemyDespawnCompleted, OnTargetDespawn);
    }

    private void UnSubscribes()
    {
        GameEventMgr.GED.UnRegister(GameEvent.OnEnemyDespawnCompleted, OnTargetDespawn);
    }
    
    private void OnTargetDespawn(object data)
    {
        int uid = (int)data;

        Debug.Log($"[BulletCtrl] OnTargetDespawn > Uid: {uid} vs TargetUid: {targetUid}");

        if (uid == targetUid)
        {
            Target = null;
            DespawnSelf();
        }
    }
    #endregion Subscribes/UnSubscribes Event!!
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Enemy") && Target != null 
            && other.gameObject.GetInstanceID() == Target.gameObject.GetInstanceID())
        {
            AttackTarget();
        }
    }

    private void AttackTarget()
    {
        if (Target == null)
        {
            DespawnSelf();
            return;
        }
        
        Target.TakeDamage(dmg);
        DespawnSelf();
    }

    private void DespawnSelf()
    {
        EntityManager.instance.DespawnBullet(gameObject);
    }
}