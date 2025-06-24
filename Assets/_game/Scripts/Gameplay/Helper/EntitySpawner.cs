using Cysharp.Threading.Tasks;
using UnityEngine;

public static partial class EntitySpawner
{
    private static bool isUsingPooling = false;

    public static async UniTask<T> SpawnEntity<T>(string package, string entityName, Vector3 position, Transform root) where T : EntityBase
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
            //TODO:
            return null;
        }
    }

    public static void DespawnEntity(GameObject go)
    {
        if (!isUsingPooling)
        {
            if (go == null) { return; }
            go.gameObject.SetActive(false);
        }
        else
        {
            //TODO:
        }
    }
}