using UnityEngine;
using Cysharp.Threading.Tasks;
using UnityEngine.Serialization;

public partial class EntityManager
{
    [SerializeField] private ObjectPool turretPool;
    [SerializeField] private ObjectPool enemyPool;
    [SerializeField] private ObjectPool bulletPool;

    private async UniTask<T> SpawnEntityViaPooling<T>(EntityType entityType, string entityName, Vector3 position) where T : EntityBase
    {
        var go = await GetPool(entityType).Spawn(entityName);
        // go.transform.parent = root;
        go.name = entityName;
        go.transform.position = position;
        return go.GetOrAddComponent<T>(); 
    }
    
    private void DespawnEntityToPool(EntityType entityType, GameObject go)
    {
            if (go == null) { return; }

            switch (entityType)
            {
                case EntityType.Turret:
                    turretPool.Despawn(go);
                    break;
                case EntityType.Enemy:
                    enemyPool.Despawn(go);
                    break;
                case EntityType.Bullet:
                    bulletPool.Despawn(go);
                    break;
            }
    }

    private ObjectPool GetPool(EntityType entityType)
    {
        switch (entityType)
        {
            case EntityType.Turret:
                return turretPool;
            case EntityType.Enemy:
                return enemyPool;
            case EntityType.Bullet:
                return bulletPool;
        }

        return null;
    }
}