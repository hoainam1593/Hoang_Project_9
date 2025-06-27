using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;

public partial class EntityManager : SingletonMonoBehaviour<EntityManager>//, IEntityManager
{
    [SerializeField] private Transform turretRoot;
    [SerializeField] private Transform enemyRoot;
    
    private Dictionary<MapCoordinate, TurretCtrl> turretCtrls = new  Dictionary<MapCoordinate, TurretCtrl>();
    private Dictionary<int, EnemyCtrl> enemyCtrls = new  Dictionary<int, EnemyCtrl>();
    
    [SerializeField] private bool isUsingPooling = false;

    
    #region Task - Spawn
    public async UniTaskVoid SpawnTurret(MapCoordinate mapCoordinate, Vector3 pos, int turretId)
    {
        if (turretCtrls.ContainsKey(mapCoordinate))
        {
            return;
        }
        
        var turretName = ConfigManager.instance.GetConfig<TurretConfig>().GetItem(turretId).prefabName;
        
        //SpawnNewTurret
        TurretCtrl turretCtrl = await SpawnEntity<TurretCtrl>(ResourcesConfig.TurretPrefab, turretName, pos, turretRoot);
        turretCtrl.OnSpawn(0);
        turretCtrls.Add(mapCoordinate, turretCtrl);
        
        GameEventMgr.GED.DispatcherEvent(GameEvent.OnTurretSpawnCompleted, mapCoordinate);
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
                enemyCtrl = await SpawnEntity<EnemySoldierCtrl>(ResourcesConfig.EnemyPrefab, enemyName, pos, enemyRoot);
                break;
            case EnemyType.Jeep:
            case EnemyType.Truck:
            case EnemyType.Tank1:
            case EnemyType.Tank2:
            case EnemyType.Tank3:
            case EnemyType.Tank4:
            case EnemyType.Tank5:
                enemyCtrl = await SpawnEntity<EnemyCtrl>(ResourcesConfig.EnemyPrefab, enemyName, pos, enemyRoot);
                break;
        }

        if (enemyCtrl != null)
        {
            enemyCtrl.OnSpawn(enemyId);
            enemyCtrls.Add(enemyCtrl.Uid, enemyCtrl);
        }
    }
    
    private async UniTask<T> SpawnEntity<T>(string package, string entityName, Vector3 position, Transform root) where T : EntityBase
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
            return await SpawnEntityViaPooling<T>(entityName, position, root);
        }
    }

    
    #endregion Task - Spawn!!!
    
    
    
    #region Despawn

    public void ClearAll()
    {
        foreach (var turret in turretCtrls.Values)
        {
            turret.OnDespawn();
            DespawnEntity(turret.gameObject);
        }
        turretCtrls.Clear();

        foreach (var enemy in enemyCtrls.Values)
        {
            enemy.OnDespawn();
            DespawnEntity(enemy.gameObject);
        }
        enemyCtrls.Clear();
    }
    
    public void DespawnTurret(MapCoordinate mapCoordinate)
    {
        if (!turretCtrls.ContainsKey(mapCoordinate))
        {
            return;
        }
        
        turretCtrls[mapCoordinate].OnDespawn();
        DespawnEntity(turretCtrls[mapCoordinate].gameObject);
        turretCtrls.Remove(mapCoordinate);
    }

    public void DespawnEnemy(int uid)
    {
        // Debug.Log("DespawnEnemy > uid: " + uid);
        if (!enemyCtrls.ContainsKey(uid))
        {
            return;
        }
        
        enemyCtrls[uid].OnDespawn();
        DespawnEntity(enemyCtrls[uid].gameObject);
        enemyCtrls.Remove(uid); 
    }
    
    private void DespawnEntity(GameObject go)
    {
        // Debug.Log("DespawnEntity > uid: " + go.name);
        if (!isUsingPooling)
        {
            if (go == null) { return; }
            go.gameObject.SetActive(false);
        }
        else
        {
            DespawnEntityToPool(go);
        }
    }
    
    #endregion Despawn!!!
}