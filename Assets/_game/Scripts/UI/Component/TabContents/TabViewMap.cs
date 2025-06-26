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

public class TabViewMap : TabContent
{    
    [SerializeField] private GameObject mapItemPrefab;

    private Dictionary<int, MapItemData> mapItems = new Dictionary<int, MapItemData>();
    
    #region Tab Content
    
    protected override void OnShow()
    {
        base.OnShow();

        LoadAndInitData();

        // Debug.Log("Try print map Model: " + mapModel.Chapters.Count.ToString());

        CreateItems();
    }
    

    protected override void OnHide()
    {
        base.OnHide();
    }
    
    #endregion Tab Content!

    #region CreateView
    
    

    private void CreateItems()
    {
        foreach (var item in mapItems)
        {
            CreateItem(item.Value);
        }
    }

    private void CreateItem(MapItemData itemData)
    {
        var newItem = GameObject.Instantiate(mapItemPrefab, Vector3.zero, Quaternion.identity, content);
        var itemCtrl = newItem.GetOrAddComponent<MapItemCtrl>();
        itemCtrl.InitView(itemData);
    }
    
    #endregion CreateView!

    #region Load And Init Data
    
    private void LoadAndInitData()
    {
        var mapConfig = ConfigManager.instance.GetConfig<MapConfig>();
        foreach (var item in mapConfig.listConfigItems)
        {
            mapItems.Add(item.mapId, new MapItemData(item.mapId, 0, false, item.mapName));
        }
        
        var mapModel = PlayerModelManager.instance.GetPlayerModel<MapModel>();
        foreach (var chapter in mapModel.Chapters)
        {
            mapItems[chapter.Id].star = chapter.Star;
            mapItems[chapter.Id].isActive = chapter.IsActive;
        }
    }
    
    #endregion Load And Init Data!!
}