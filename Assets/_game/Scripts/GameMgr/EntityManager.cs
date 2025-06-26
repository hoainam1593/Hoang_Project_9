using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;

public class EntityManager : SingletonMonoBehaviour<EntityManager>//, IEntityManager
{
    [SerializeField] private Transform turretRoot;
    
    private Dictionary<MapCoordinate, TurretCtrl> turretCtrls = new  Dictionary<MapCoordinate, TurretCtrl>();

    public async UniTaskVoid SpawnTurret(MapCoordinate mapCoordinate, Vector3 pos, string turretName)
    {
        if (turretCtrls.ContainsKey(mapCoordinate))
        {
            return;
        }
        
        // GameEventMgr.GED.DispatcherEvent(GameEvent.OnTurretSpawnStart, mapCoordinate);
        
        //SpawnNewTurret
        TurretCtrl turretCtrl = await EntitySpawner.SpawnEntity<TurretCtrl>(ResourcesConfig.TurretPrefab, turretName, pos, turretRoot);
        turretCtrl.OnSpawn();
        turretCtrls.Add(mapCoordinate, turretCtrl);
        
        GameEventMgr.GED.DispatcherEvent(GameEvent.OnTurretSpawnCompleted, mapCoordinate);
    }

    public void DespawnTurret(MapCoordinate mapCoordinate)
    {
        if (!turretCtrls.ContainsKey(mapCoordinate))
        {
            return;
        }
        
        turretCtrls[mapCoordinate].OnDespawn();
        EntitySpawner.DespawnEntity(turretCtrls[mapCoordinate].gameObject);
        turretCtrls.Remove(mapCoordinate);
    }

    public void ClearAllTurrets()
    {
        foreach (var turretCtrl in turretCtrls)
        {
            turretCtrl.Value.OnDespawn();
        }
        turretCtrls.Clear();
    }
}