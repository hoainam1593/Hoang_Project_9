using System;
using UnityEngine;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;



//Main Application lifeCycle
public partial class GameLauncher : MonoBehaviour
{
    [SerializeField] private MapCtrl mapCtrl;
    private GameObject mapRoot;

    private Game game;
    
    private void Awake()
    {
        mapRoot = mapCtrl.gameObject;
    }

    private void Start()
    {
        LoadConfig().Forget();
        
        //hide map
        mapRoot.SetActive(false);
        
        //show main ui
        UIManager.instance.OpenPanel<MainUIPanel>(UIPanel.MainUIPanel);
    }

    #region Main Stream
    public void StartGame()
    {
        var mapName = "map1";
        mapRoot.SetActive(true);
        // mapCtrl.GenerateMap(mapName);
        
        UIManager.instance.ClosePanel(UIPanel.MainUIPanel);
        UIManager.instance.OpenPanel<BattleUIPanel>(UIPanel.BattleUIPanel);
        
        game.StartGame();   
    }

    public void ExitGame()
    {
        mapRoot.SetActive(false);
        
        UIManager.instance.ClosePanel(UIPanel.BattleUIPanel);
        UIManager.instance.OpenPanel<MainUIPanel>(UIPanel.MainUIPanel);
                
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
}
