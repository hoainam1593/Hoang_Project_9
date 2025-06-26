using System;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Cysharp.Threading.Tasks;


public class MapItemData
{
    public int Id;
    public int Star;
    public bool IsActive;
    public string Name;

    public MapItemData(int id, int star, bool isActive, string name)
    {
        this.Id = id;
        this.Star = star;
        this.IsActive = isActive;
        this.Name = name;
    }
}

public class TabViewMap : MonoBehaviour
{    
    [SerializeField] private ObjectPool pool;
    [SerializeField] private Transform content;

    private Dictionary<int, MapItemData> itemsData;
    private Dictionary<int, Transform> items;

    private bool createItemCompleted = true;
    

    private void Awake()
    {
        createItemCompleted = true;
        items = new  Dictionary<int, Transform>();
        
        LoadAndInitData();
        // CreateItems();
    }

    private void OnEnable()
    {
        ClearAllItems();
        LoadAndInitData();
        CreateItems();
        // BalanceItems();
    }

    #region CreateView

    private async UniTaskVoid BalanceItems()
    {
        await UniTask.WaitUntil(() => createItemCompleted);

        createItemCompleted = false;
        
        if (itemsData == null || items == null)
        {
            return; 
        }
        
        if (itemsData.Count == items.Count)
        {
            return;
        }
        

        if (items.Count < itemsData.Count)
        {
            var dataList = itemsData.Values.ToArray();
            //Spawn more
            for (int i = items.Count; i < itemsData.Count; i++)
            {
                CreateItem(dataList[i]).Forget();
            }
        }
        else
        {
            var itemList =  items.Values.ToArray();
            // Remove
            for (int i = itemsData.Count; i < items.Count; i++)
            {
                RemoveItem(itemList[i].gameObject);
            }
        }

        createItemCompleted = true;
    }

    private void ClearAllItems()
    {
        foreach (var item in items)
        {
            RemoveItem(item.Value.gameObject);
        }
        items.Clear();
    }

    private void RemoveItem(GameObject go)
    {
        pool.Despawn(go);
        // GameObject.Destroy(go);
    }
        

    private async UniTaskVoid CreateItems()
    {
        await UniTask.WaitUntil(() => createItemCompleted);
        
        createItemCompleted = false;
        foreach (var data in itemsData.Values)
        {
            if (!items.ContainsKey(data.Id))
            {
                var go = await CreateItem(data);
                if (!items.ContainsKey(data.Id))
                {
                    items.Add(data.Id, go.transform);
                }
            }
        }

        createItemCompleted = true;
    }

    private async UniTask<GameObject> CreateItem(MapItemData data)
    {
        Debug.Log("Creating item > map: " + data.Id);
        // var newItem = GameObject.Instantiate(mapItemPrefab, Vector3.zero, Quaternion.identity, content);
        var newItem = await pool.Spawn("MapItem");
        newItem.transform.SetParent(content);
        newItem.transform.SetAsLastSibling();
        
        var itemCtrl = newItem.GetOrAddComponent<MapItemCtrl>();
        itemCtrl.InitView(data);
        
        return newItem;
    }
    
    #endregion CreateView!

    #region Load And Init Data
    
    private void LoadAndInitData()
    {
        itemsData = new Dictionary<int, MapItemData>();
        var mapConfig = ConfigManager.instance.GetConfig<MapConfig>();
        
        foreach (var item in mapConfig.listConfigItems)
        {
            itemsData.Add(item.mapId, new MapItemData(item.mapId, 0, false, item.mapName));
        }
        
        var mapModel = PlayerModelManager.instance.GetPlayerModel<MapModel>();
        foreach (var chapter in mapModel.Chapters)
        {
            itemsData[chapter.Id].Star = chapter.Star;
            itemsData[chapter.Id].IsActive = chapter.IsActive;
        }
    }
    
    #endregion Load And Init Data!!
}