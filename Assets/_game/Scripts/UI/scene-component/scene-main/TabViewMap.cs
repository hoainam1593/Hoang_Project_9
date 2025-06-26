using System;
using UnityEngine;
using System.Collections.Generic;


public class MapItemData
{
    public int id;
    public int star;
    public bool isActive;
    public string name;

    public MapItemData(int id, int star, bool isActive, string name)
    {
        this.id = id;
        this.star = star;
        this.isActive = isActive;
        this.name = name;
    }
}

public class TabViewMap : MonoBehaviour
{    
    [SerializeField] private GameObject mapItemPrefab;
    [SerializeField] private Transform content;

    private Dictionary<int, MapItemData> itemDatas;
    private Dictionary<int, Transform> items;
    

    private void Awake()
    {
        itemDatas = new Dictionary<int, MapItemData>();
        items = new  Dictionary<int, Transform>();
        
        LoadAndInitData();
    }

    private void OnEnable()
    {
        ClearAllItems();
        CreateItems();
    }

    #region CreateView

    private void ClearAllItems()
    {
        foreach (var item in items)
        {
            GameObject.Destroy(item.Value.gameObject);
        }
        items.Clear();
    }
        

    private void CreateItems()
    {
        foreach (var item in itemDatas)
        {
            CreateItem(item.Value);
        }
    }

    private void CreateItem(MapItemData itemData)
    {
        var newItem = GameObject.Instantiate(mapItemPrefab, Vector3.zero, Quaternion.identity, content);
        var itemCtrl = newItem.GetOrAddComponent<MapItemCtrl>();
        itemCtrl.InitView(itemData);
        items.Add(itemData.id, itemCtrl.transform);
    }
    
    #endregion CreateView!

    #region Load And Init Data
    
    private void LoadAndInitData()
    {
        var mapConfig = ConfigManager.instance.GetConfig<MapConfig>();
        
        foreach (var item in mapConfig.listConfigItems)
        {
            itemDatas.Add(item.mapId, new MapItemData(item.mapId, 0, false, item.mapName));
        }
        
        var mapModel = PlayerModelManager.instance.GetPlayerModel<MapModel>();
        foreach (var chapter in mapModel.Chapters)
        {
            itemDatas[chapter.Id].star = chapter.Star;
            itemDatas[chapter.Id].isActive = chapter.IsActive;
        }
    }
    
    #endregion Load And Init Data!!
}