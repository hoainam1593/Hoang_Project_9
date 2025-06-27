using System;
using Cysharp.Threading.Tasks;
using UnityEngine;

public partial class MapCtrl : SingletonMonoBehaviour<MapCtrl>
{
    private MapData mapData;
    private Grid mapGrid;

    protected override void Awake()
    {
        base.Awake();
        mapGrid = gameObject.GetComponent<Grid>();
        Subscribes();
        
        var mapName = PlayerPrefs.GetString(PlayerPrefsConfig.Key_Select_Map, "map1");
        GenerateMap(mapName);
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        UnSubscribes();
    }

    private void GenerateMap(string mapName)
    {
        layerGround.ClearAllTiles();

        var _mapData = LoadMapDataFile(mapName);
        mapData = CsvParser.ToMapData(_mapData.text);
        // var mapModel = new MapModel(mapData);
        GenerateMap();
        
        PassParamsToCamera();
    }
    
    #region Subscribes Methods:

    public void Subscribes()
    {
        GameEventMgr.GED.Register(GameEvent.OnClick, OnClickedInMap);
        GameEventMgr.GED.Register(GameEvent.OnTurretSpawnStart, OnTurretSpawnStart);
        GameEventMgr.GED.Register(GameEvent.OnTurretSpawnCompleted, OnTurretSpawnCompleted);
    }

    public void UnSubscribes()
    {
        GameEventMgr.GED.UnRegister(GameEvent.OnClick, OnClickedInMap);
        GameEventMgr.GED.UnRegister(GameEvent.OnTurretSpawnStart, OnTurretSpawnStart);
        GameEventMgr.GED.UnRegister(GameEvent.OnTurretSpawnCompleted, OnTurretSpawnCompleted);
    }

    
    #endregion Subscribes Methods!
    
    
    #region Task PassParam to Camera

    private void PassParamsToCamera()
    {
        var mapBottomLeft = GetMapBottomLeft();
        // Debug.Log("Map size: " + Width + " / " + Height);
        GameEventMgr.GED.DispatcherEvent(GameEvent.OnMapSizeUpdate, new GEventData.MapSizeData(Width, Height, mapBottomLeft));
    }
    #endregion
}
