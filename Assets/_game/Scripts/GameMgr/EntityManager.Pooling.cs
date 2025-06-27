using UnityEngine;
using Cysharp.Threading.Tasks;

public partial class EntityManager
{
    [SerializeField] private ObjectPool pool;

    private async UniTask<T> SpawnEntityViaPooling<T>(string entityName, Vector3 position, Transform root) where T : EntityBase
    {
        var go = await pool.Spawn(entityName);
        go.transform.parent = root;
        go.name = entityName;
        go.transform.position = position;
        return go.GetOrAddComponent<T>(); 
    }
    
    private void DespawnEntityToPool(GameObject go)
    {
            if (go == null) { return; }
            pool.Despawn(go);

    }
}