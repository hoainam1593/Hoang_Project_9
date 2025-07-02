using System;
using UnityEngine;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;


public class MapItemData
{
    public int Id;
    public int Star;
    public string Name;

    public MapItemData(int id, int star, string name)
    {
        this.Id = id;
        this.Star = star;
        this.Name = name;
    }
}

public class TabViewMap : MonoBehaviour
{
    [SerializeField] private GameObject mapItemPrefab;
    [SerializeField] private ObjectPool pool;
    [SerializeField] private Transform content;

    private List<MapItemData> itemsData;
    private List<GameObject> items;

    private bool createItemCompleted = true;
    

    private void Awake()
    {
        createItemCompleted = true;
    }

    private void OnEnable()
    {
        ClearAllItems();
        LoadAndInitData();
        CreateItems().Forget();
    }

    private void OnDestroy()
    {
        ClearAllItems();
        pool?.DespawnAll();
    }

    #region CreateView

    private void ClearAllItems()
    {
        if (items == null)
        {
            return;
        }
        for (int i = items.Count-1; i >= 0; i--)
        {
            RemoveItem(items[i]);
        }
        items.Clear();
    }

    private void RemoveItem(GameObject go)
    {
        pool?.Despawn(go);
    }
        

    private async UniTaskVoid CreateItems()
    {
        await UniTask.WaitUntil(() => createItemCompleted);
        
        items = new  List<GameObject>();
        createItemCompleted = false;
        foreach (var data in itemsData)
        {
            var go = await CreateItem(data);
            if (!items.Contains(go))
            {
                items.Add(go);
            }
        }

        createItemCompleted = true;
    }

    private async UniTask<GameObject> CreateItem(MapItemData data)
    {
        // Debug.Log("Creating item > map: " + data.Id);
        //var newItem = GameObject.Instantiate(mapItemPrefab, Vector3.zero, Quaternion.identity, content);
        var newItem = await pool.Spawn("MapItem");
        newItem.transform.SetParent(content);
        newItem.transform.SetAsLastSibling();
        
        var itemCtrl = newItem.GetOrAddComponent<MapItemUICtrl>();
        itemCtrl.InitView(data);
        
        return newItem;
    }
    
    #endregion CreateView!

    #region Load And Init Data
    
    private void LoadAndInitData()
    {
        itemsData = new List<MapItemData>();

        string mapName = null;
        var mapConfig = ConfigManager.instance.GetConfig<MapConfig>();
        var mapModel = PlayerModelManager.instance.GetPlayerModel<MapModel>();
        foreach (var map in mapModel.Maps) // Updated to use Maps instead of Chapters
        {
            mapName = mapConfig.GetMapName(map.Id);
            itemsData.Add(new MapItemData(map.Id, map.Star, mapName));
        }
    }
    
    #endregion Load And Init Data!!
}