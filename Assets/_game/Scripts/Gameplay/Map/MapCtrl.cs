using System;
using Cysharp.Threading.Tasks;
using UnityEngine;

public partial class MapCtrl : MonoBehaviour, IDispatcher, IRegister
{
    private MapData mapData;
    private Grid mapGrid;

    private void Awake()
    {
        mapGrid = gameObject.GetComponent<Grid>();
        Subscribes();
    }

    private void OnDestroy()
    {
        UnSubscribes();
    }

    public void GenerateMap(string mapName)
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
        this.RegisterEvent(GameEvent.OnClick, OnClickedInMap);
        this.RegisterEvent(GameEvent.OnTurretSpawnStart, OnTurretSpawnStart);
        this.RegisterEvent(GameEvent.OnTurretSpawnCompleted, OnTurretSpawnCompleted);
    }

    public void UnSubscribes()
    {
        this.UnRegisterEvent(GameEvent.OnClick, OnClickedInMap);
        this.UnRegisterEvent(GameEvent.OnTurretSpawnStart, OnTurretSpawnStart);
        this.UnRegisterEvent(GameEvent.OnTurretSpawnCompleted, OnTurretSpawnCompleted);
    }

    
    #endregion Subscribes Methods!
    
    
    #region Task PassParam to Camera

    private void PassParamsToCamera()
    {
        var mapBottomLeft = GetMapBottomLeft();
        this.DispatcherEvent(GameEvent.OnMapSizeUpdate, new GEventData.MapSizeData(Width, Height, mapBottomLeft));
    }
    #endregion
}
