using System;
using UnityEngine;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;

public partial class GameLauncher : SingletonMonoBehaviour<GameLauncher>
{
    private GameObject mapRoot;

    private Game game;
    
    protected override void Awake()
    {
        base.Awake();
        DontDestroyOnLoad(this);
        
        RegisterSceneLoaded();
        game = new Game();
    }

    private async UniTaskVoid Start()
    {
        LoadConfig().Forget();
        LoadModels();

        await LoadScene(SceneName.Main);
    }

    #region Main Stream
    public async UniTaskVoid StartGame()
    {
        await LoadScene(SceneName.Battle);
        
        game.StartGame();   
    }

    public async UniTaskVoid ExitGame()
    {
        await LoadScene(SceneName.Main);
                
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
