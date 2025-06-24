using UnityEngine;

public class EntityBase : MonoBehaviour, IEntity
{
    public void OnSpawn()
    {
        
    }

    public void OnDespawn()
    {
        
    }
}

// public class EntityBase// : IEntity
// {
//     private Transform transform;
//     private GameObject gameObject;
//
//     private bool isActive = false;
//     public bool IsActive => isActive;
//     
//     public void Init(GameObject go)
//     {
//         if (isActive)
//         {
//             return;
//         }
//         
//         this.gameObject = go;
//         this.transform = go.transform;
//         isActive = true;
//     }
//     
//     public void OnSpawn()
//     {
//         if (isActive) return;
//         isActive = true;
//         gameObject.SetActive(true);
//     }
//
//     public void OnDeSpawn()
//     {
//         if (!isActive) return;
//         isActive = false;
//         gameObject.SetActive(false);
//     }
// }