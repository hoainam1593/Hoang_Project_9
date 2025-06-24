using System;
using Cysharp.Threading.Tasks;
using UnityEngine;

public partial class MapCtrl : MonoBehaviour, IDispatcher, IRegister
{
    [SerializeField] private TextAsset _mapData;
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

    private void Start()
    {
        mapData = CsvParser.ToMapData(_mapData.text);
        // var mapModel = new MapModel(mapData);
        GenerateMap(mapData);

        PassParamsToCamera();
    }
    
    #region Subscribes Methods:

    public void Subscribes()
    {
        this.RegisterEvent(GameEvent.OnClick, OnClickedInMap);
    }

    public void UnSubscribes()
    {
        this.UnRegisterEvent(GameEvent.OnClick, OnClickedInMap);
    }

    
    #endregion Subscribes Methods!
    

    private async UniTaskVoid TestSpawnPrefab()
    {
        // var prefab = await AssetManager.instance.LoadPrefab(GlobalConfig.Resources.TurretPrefab, "Turret_lv1");
        // GameObject.Instantiate(prefab);
    }        
    
    #region Task PassParam to Camera

    private void PassParamsToCamera()
    {
        var mapBottomLeft = GetMapBottomLeft();
        this.DispatcherEvent(GameEvent.OnMapSizeUpdate, new GEventData.MapSizeData(Width, Height, mapBottomLeft));
    }
    #endregion
}
