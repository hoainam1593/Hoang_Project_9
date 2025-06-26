using System;
using UnityEngine;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;

public class GameLauncher : SingletonMonoBehaviour<GameLauncher>
{
    [SerializeField] private MapCtrl mapCtrl;
    private GameObject mapRoot;

    private Game game;
    
    protected override void Awake()
    {
        base.Awake();
        mapRoot = mapCtrl.gameObject;
        game = new Game();
    }

    private void Start()
    {
        LoadConfig().Forget();
        LoadModels();

        //hide map
        mapRoot.SetActive(false);
        
        //show main ui
        UIManager.instance.OpenPanel<MainUIPanel>(UIPanel.MainUIPanel);
    }

    #region Main Stream
    public void StartGame(object data)
    {
        var mapName = (string)data;
        Debug.Log("StartGame > map: " + mapName);
        mapRoot.SetActive(true);
        mapCtrl.GenerateMap(mapName);
        
        var path = mapCtrl.GetMatrixPath();
        Debug.Log("MyPath : " + path);
        
        UIManager.instance.ClosePanel(UIPanel.MainUIPanel);
        UIManager.instance.OpenPanel<BattleUIPanel>(UIPanel.BattleUIPanel).Forget();
        
        game.StartGame();   
    }

    public void ExitGame()
    {
        mapRoot.SetActive(false);
        
        UIManager.instance.ClosePanel(UIPanel.BattleUIPanel);
        UIManager.instance.OpenPanel<MainUIPanel>(UIPanel.MainUIPanel).Forget();
                
        game.ExitGame();
    }
    #endregion
    
    #region LoadConfig

    public class GameListConfig : IListConfigDeclaration
    {
        public List<IBaseConfig> listConfigs =>
            new()
            {
                new MapConfig(),
            };
    }
    
    private async UniTaskVoid LoadConfig()
    {
        GameListConfig gameConfig = new GameListConfig();
        await ConfigManager.instance.LoadAllConfigs(gameConfig);
        Debug.Log("LoadConfig");
        
        
        var mapConfig = ConfigManager.instance.GetConfig<MapConfig>();
        Debug.Log("MapCount: " + mapConfig.listConfigItems.Count);
        foreach (var item in mapConfig.listConfigItems)
        {
            Debug.Log("MapItem: " + item);
        }
    }
    
    #endregion LoadConfig!
    
    #region LoadModel

    public void LoadModels()
    {
        List<BasePlayerModel> lModels = new List<BasePlayerModel>()
        {
            new MapModel(),
        };
        PlayerModelManager.instance.LoadAllModels(lModels, "MapModel");
    }
    
    #endregion LoadModel!
}
