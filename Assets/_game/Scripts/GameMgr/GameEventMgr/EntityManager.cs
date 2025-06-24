using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;

public class EntityManager : SingletonMonoBehaviour<EntityManager>//, IEntityManager
{
    [SerializeField] private Transform turretRoot;
    
    private Dictionary<Vector3Int, TurretCtrl> turretCtrls = new  Dictionary<Vector3Int, TurretCtrl>();

    public async UniTaskVoid SpawnTurret(Vector3Int tilePosition, Vector3 pos, string turretName)
    {
        if (turretCtrls.ContainsKey(tilePosition))
        {
            return;
        }
        
        //SpawnNewTurret
        TurretCtrl turretCtrl = await EntitySpawner.SpawnEntity<TurretCtrl>(ResourcesConfig.TurretPrefab, turretName, pos, turretRoot);
        turretCtrl.OnSpawn();
        turretCtrls.Add(tilePosition, turretCtrl);
    }

    public void DespawnTurret(Vector3Int tilePosition)
    {
        if (!turretCtrls.ContainsKey(tilePosition))
        {
            return;
        }
        
        turretCtrls[tilePosition].OnDespawn();
        EntitySpawner.DespawnEntity(turretCtrls[tilePosition].gameObject);
        turretCtrls.Remove(tilePosition);
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