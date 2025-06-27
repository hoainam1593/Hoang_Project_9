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
    public async UniTaskVoid SpawnTurret(MapCoordinate mapCoordinate, Vector3 pos, string turretName)
    {
        if (turretCtrls.ContainsKey(mapCoordinate))
        {
            return;
        }
        
        // GameEventMgr.GED.DispatcherEvent(GameEvent.OnTurretSpawnStart, mapCoordinate);
        
        //SpawnNewTurret
        TurretCtrl turretCtrl = await SpawnEntity<TurretCtrl>(ResourcesConfig.TurretPrefab, turretName, pos, turretRoot);
        turretCtrl.OnSpawn();
        turretCtrls.Add(mapCoordinate, turretCtrl);
        
        GameEventMgr.GED.DispatcherEvent(GameEvent.OnTurretSpawnCompleted, mapCoordinate);
    }
    
        
    public async UniTaskVoid SpawnEnemy(Vector3 pos, int enemyId)
    {
        var enemyName = ConfigManager.instance.GetConfig<EnemyConfig>().GetItem(enemyId).prefabName;
        EnemyCtrl enemyCtrl = await SpawnEntity<EnemyCtrl>(ResourcesConfig.EnemyPrefab, enemyName, pos, enemyRoot);
        enemyCtrl.OnSpawn();
        enemyCtrls.Add(enemyCtrl.Uid, enemyCtrl);
        
        // GameEventMgr.GED.DispatcherEvent(GameEvent.OnEnemySpawnCompleted, enemyCtrl.Uid);
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
            return go.AddComponent<T>(); 
        }
        else
        {
            return await SpawnEntityViaPooling<T>(entityName, position, root);
        }
    }

    
    #endregion Task - Spawn!!!
    
    
    
    #region Despawn
    
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