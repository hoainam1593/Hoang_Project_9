
using Cysharp.Threading.Tasks;
using Newtonsoft.Json;

public class ServerConfigJson
{
    #region core

    public int gameConfigVersion;
    public string gameClientVersion;
    public int addressableVersion;

    private ServerController serverController;

    public ServerConfigJson()
    {
    }
    
    public ServerConfigJson(ServerController serverController)
    {
        this.serverController = serverController;
    }

    #endregion
    
    #region remote

    public async UniTask Download()
    {
        var json = await serverController.GameContent_get(
            $"{serverController.serverEnvironment}/server_config.json");
        if (!string.IsNullOrEmpty(json))
        {
            JsonConvert.PopulateObject(json, this);
        }
    }

    public async UniTask Upload()
    {
        var json = StaticUtils.JsonSerializeToFriendlyText(this);
        await serverController.GameContent_set($"{serverController.serverEnvironment}/server_config.json", json);
    }

    #endregion

    #region local

    private string localFilePath => $"{PlayerModelManager.GetModelFolderPath()}/server_config.json";

    public void Save()
    {
        var json = StaticUtils.JsonSerializeToFriendlyText(this);
        StaticUtils.WriteTextFile(localFilePath, json);
    }

    public void Load()
    {
        if (StaticUtils.CheckFileExist(localFilePath))
        {
            var json = StaticUtils.ReadTextFile(localFilePath);
            JsonConvert.PopulateObject(json, this);
        }
    }

    #endregion
}
