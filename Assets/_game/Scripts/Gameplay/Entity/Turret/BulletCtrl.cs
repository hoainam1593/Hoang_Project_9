using System;
using UnityEngine;

public class BulletCtrl : EntityBase
{
    private EnemyCtrl Target;
    private int targetUid = -1;
    private float dmg;
    private Vector3 direction;
    
    // Auto-despawn properties
    private float spawnTime;
    private Vector3 spawnPosition;
    private float turretRange;
    private const float BULLET_LIFETIME = 5f; // 5 seconds lifetime
    
    public const float speed = 10f;

    // Debug logging toggle (you can remove this if not needed)
    private bool enableDebugLogs = false;

    #region EntityBase
    protected override void InitData(object data)
    {
        var bulletData = ((EnemyCtrl target, float dmg, float turretRange))data;
        this.Target = bulletData.target;
        this.targetUid = Target.Uid;
        this.dmg = bulletData.dmg;
        this.turretRange = bulletData.turretRange;
    }

    protected override void OnSpawnStart()
    {
        base.OnSpawnStart();
        Subscribes();
        
        // Initialize auto-despawn tracking
        spawnTime = Time.time;
        spawnPosition = transform.position;
        
        if (enableDebugLogs)
        {
            Debug.Log($"[BulletCtrl] Spawned at {spawnPosition} with turret range {turretRange}");
        }
    }
    
    public override void OnDespawn()
    {
        base.OnDespawn();
        UnSubscribes();
    }

    protected override void OnUpdate()
    {
        // Check for auto-despawn conditions
        if (ShouldDespawnByLifetime())
        {
            if (enableDebugLogs)
            {
                Debug.Log($"[BulletCtrl] Despawning due to lifetime exceeded: {Time.time - spawnTime}s");
            }
            DespawnSelf();
            return;
        }
        
        if (ShouldDespawnByRange())
        {
            if (enableDebugLogs)
            {
                float distance = Vector3.Distance(transform.position, spawnPosition);
                Debug.Log($"[BulletCtrl] Despawning due to range exceeded: {distance} > {turretRange}");
            }
            DespawnSelf();
            return;
        }
        
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

    /// <summary>
    /// Check if bullet should despawn due to exceeding lifetime (5 seconds)
    /// </summary>
    private bool ShouldDespawnByLifetime()
    {
        return Time.time - spawnTime >= BULLET_LIFETIME;
    }

    /// <summary>
    /// Check if bullet should despawn due to exceeding turret range
    /// </summary>
    private bool ShouldDespawnByRange()
    {
        float distanceFromSpawn = Vector3.Distance(transform.position, spawnPosition);
        return distanceFromSpawn >= turretRange;
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