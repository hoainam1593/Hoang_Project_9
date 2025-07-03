using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;
using R3.Triggers;

public partial class EntityManager : SingletonMonoBehaviour<EntityManager>//, IEntityManager
{
    private Dictionary<MapCoordinate, TurretCtrl> turretCtrls = new  Dictionary<MapCoordinate, TurretCtrl>();
    private Dictionary<int, EnemyCtrl> enemyCtrls = new  Dictionary<int, EnemyCtrl>();
    private Dictionary<int, BulletCtrl> bulletCtrls = new  Dictionary<int, BulletCtrl>();
    
    
    [SerializeField] private bool isUsingPooling = false;
    
    
    #region Task - Spawn
    public async UniTaskVoid SpawnTurret(MapCoordinate mapCoordinate, Vector3 pos, int turretId)
    {
        if (turretCtrls.ContainsKey(mapCoordinate))
        {
            return;
        }
        
        var turretConfig = ConfigManager.instance.GetConfig<TurretConfig>().GetItem(turretId);
        var turretName = turretConfig.prefabName;
        var turretInfo = new TurretInfo()
        {
            mapCoordinate = mapCoordinate,
            config = turretConfig,
        };

        GameEventMgr.GED.DispatcherEvent(GameEvent.OnTurretSpawnStart, turretInfo);

        //SpawnNewTurret
        TurretCtrl turretCtrl = await SpawnEntity<TurretCtrl>(EntityType.Turret, ResourcesConfig.TurretPrefab, turretName, pos, turretPool.transform);
        turretCtrl.OnSpawn((0, mapCoordinate));
        turretCtrls.Add(mapCoordinate, turretCtrl);
        
        GameEventMgr.GED.DispatcherEvent(GameEvent.OnTurretSpawnCompleted, turretInfo);
    }
    
        
    public async UniTaskVoid SpawnEnemy(Vector3 pos, int enemyId)
    {
        var enemyName = ConfigManager.instance.GetConfig<EnemyConfig>().GetItem(enemyId).prefabName;
        var enemyType = (EnemyType)ConfigManager.instance.GetConfig<EnemyConfig>().GetItem(enemyId).id;

        EnemyCtrl enemyCtrl = null;
        switch (enemyType)
        {
            case EnemyType.Soldier1:
            case EnemyType.Soldier2:
                enemyCtrl = await SpawnEntity<EnemySoldierCtrl>(EntityType.Enemy, ResourcesConfig.EnemyPrefab, enemyName, pos, enemyPool.transform);
                break;
            case EnemyType.Jeep:
            case EnemyType.Truck:
            case EnemyType.Tank1:
            case EnemyType.Tank2:
            case EnemyType.Tank3:
            case EnemyType.Tank4:
            case EnemyType.Tank5:
                enemyCtrl = await SpawnEntity<EnemyCtrl>(EntityType.Enemy, ResourcesConfig.EnemyPrefab, enemyName, pos, enemyPool.transform);
                break;
        }

        if (enemyCtrl != null)
        {
            enemyCtrl.OnSpawn(enemyId);
            enemyCtrls.Add(enemyCtrl.Uid, enemyCtrl);
            GameEventMgr.GED.DispatcherEvent(GameEvent.OnEnemySpawnCompleted, enemyCtrl.Uid);

            SpawnHealthBar(enemyCtrl);
        }
    }
    
    public async UniTaskVoid SpawnBullet<T>(Vector3 pos, float dmg, EnemyCtrl target, float turretRange = 3f) where T : EntityBase
    {
        if (target == null)
        {
            return;
        }

        BulletCtrl bulletCtrl = await SpawnEntity<BulletCtrl>(EntityType.Bullet, ResourcesConfig.BulletPrefab,
            "Bullet", pos, bulletPool.transform);

        //Debug.Log("SpawnBullet > bulletCtrl: " + bulletCtrl.gameObject.GetInstanceID());

        if (!bulletCtrls.ContainsKey(bulletCtrl.gameObject.GetInstanceID()))
        {
            bulletCtrl.OnSpawn((target, dmg, turretRange));
            bulletCtrls.Add(bulletCtrl.gameObject.GetInstanceID(), bulletCtrl);
        }
    }
    
    private async UniTask<T> SpawnEntity<T>(EntityType entityType, string package, string entityName, Vector3 position, Transform root) where T : EntityBase
    {
        if (!isUsingPooling)
        {
            GameObject prefab = await AssetManager.instance.LoadPrefab(package, entityName);
            if (prefab == null)
            {
                return null;
            }
            
            var go = UnityEngine.Object.Instantiate(prefab, position, Quaternion.identity, root);
            go.name = entityName;
            go.transform.position = position;
            return go.GetOrAddComponent<T>(); 
        }
        else
        {
            return await SpawnEntityViaPooling<T>(entityType, entityName, position);
        }
    }

    private void SpawnHealthBar(EnemyCtrl enemy)
    {
        HealthBarManager.instance.CreateHealthBar(enemy.Uid, enemy.Position, enemy.MaxHp, enemy.CrrHp).Forget();
    }


    #endregion Task - Spawn!!!
    
    
    
    #region Despawn

    public void ClearAll()
    {
        foreach (var turret in turretCtrls.Values)
        {
            turret.OnDespawn();
            DespawnEntity(EntityType.Turret, turret.gameObject);
        }
        turretCtrls.Clear();

        foreach (var enemy in enemyCtrls.Values)
        {
            enemy.OnDespawn();
            DespawnEntity(EntityType.Enemy, enemy.gameObject);
        }
        enemyCtrls.Clear();

        foreach (var bullet in bulletCtrls.Values)
        {
            bullet.OnDespawn();
            DespawnEntity(EntityType.Bullet, bullet.gameObject);
        }

        // Cleanup all object pools if using pooling
        if (isUsingPooling)
        {
            ClearAllObjectPools();
        }
    }
    
    public void DespawnTurret(MapCoordinate mapCoordinate)
    {
        if (!turretCtrls.ContainsKey(mapCoordinate))
        {
            return;
        }
        
        turretCtrls[mapCoordinate].OnDespawn();
        DespawnEntity(EntityType.Turret, turretCtrls[mapCoordinate].gameObject);
        turretCtrls.Remove(mapCoordinate);
    }

    public void DespawnEnemy(int uid)
    {
        // Debug.Log("DespawnEnemy > uid: " + uid);
        if (!enemyCtrls.ContainsKey(uid))
        {
            return;
        }
        
        // Get enemy info for rewards before despawning
        EnemyCtrl enemyCtrl = enemyCtrls[uid];
        
        // Clean up enemy
        HealthBarManager.instance.DespawnHealthBar(uid);
        enemyCtrl.OnDespawn();
        DespawnEntity(EntityType.Enemy, enemyCtrl.gameObject);
        enemyCtrls.Remove(uid); 

        GameEventMgr.GED.DispatcherEvent(GameEvent.OnEnemyDespawnCompleted, uid);
    }

    public void DespawnBullet(GameObject bullet)
    {
        int bulletId = bullet.GetInstanceID();
        //Debug.Log("DespawnBullet > bulletCtrl: " + bulletId);
        if (!bulletCtrls.ContainsKey(bulletId))
        {
            return;
        }
        bulletCtrls[bulletId].OnDespawn(); 
        DespawnEntity(EntityType.Enemy, bulletCtrls[bulletId].gameObject);
        bulletCtrls.Remove(bulletId);
    }

    private void DespawnEntity(EntityType entityType, GameObject go)
    {
        // Debug.Log("DespawnEntity > uid: " + go.name);
        if (!isUsingPooling)
        {
            if (go == null) { return; }
            go.gameObject.SetActive(false);
        }
        else
        {
            DespawnEntityToPool(entityType, go);
        }
    }

    /// <summary>
    /// Clear all object pools and reset dictionary states
    /// </summary>
    public void ClearAllObjectPools()
    {
        if (turretPool != null)
        {
            turretPool.DespawnAll();
        }

        if (enemyPool != null)
        {
            enemyPool.DespawnAll();
        }

        if (bulletPool != null)
        {
            bulletPool.DespawnAll();
        }

        Debug.Log("[EntityManager] All object pools cleared");
    }

    #endregion Despawn!!!
}