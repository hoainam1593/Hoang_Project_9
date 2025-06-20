
using System;
using System.Collections.Generic;
using System.IO;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class AdminUploadCfgState_main : EditorWindowState
{
    #region UI

    public override void OnDraw()
    {
        if (GUILayout.Button("upload configs"))
        {
            OnClickUpload().Forget();
        }
    }

    protected virtual async UniTask OnClickUpload()
    {
        FSM.PushState(new EditorWindowState_doing());
        try
        {
            await UploadConfig();
            StaticUtilsEditor.DisplayDialog("upload configs success");
        }
        catch (Exception e)
        {
            Debug.LogError(e.ToString());
            StaticUtilsEditor.DisplayDialog("upload configs fail, see console for detail");
        }
        FSM.PopState();
    }

    #endregion
    
    #region upload config

    private async UniTask UploadConfig()
    {
        var serverController = new ServerController();
        serverController.Init(true);

        await UploadCfg_text(serverController);
        await UploadCfg_binary(serverController);
        await UpdateServerConfig(serverController);

        await serverController.GameContent_applySet();
    }

    private async UniTask UploadCfg_text(ServerController serverController)
    {
        var lTasks = new List<UniTask>();
        var lFilePaths = StaticUtils.GetFilesInFolder("../GameConfig", 
            false, null, "csv");
        foreach (var filePath in lFilePaths)
        {
            var fileContent = StaticUtils.ReadTextFile(filePath, true);
            var fileName = Path.GetFileName(filePath);
            var s3Key = $"game_configs_text/{fileName}";
            
            lTasks.Add(serverController.GameContent_set(s3Key, fileContent));
        }
        
        await UniTask.WhenAll(lTasks);
    }

    private async UniTask UploadCfg_binary(ServerController serverController)
    {
        using (var stream = new MemoryStream())
        {
            await using (var writer = new BinaryWriter(stream))
            {
                var configReadWriteManager = new ConfigReadWriteManager(ConfigManager.GetListConfigsInEditor());
                configReadWriteManager.ReadConfig_editor();
                configReadWriteManager.WriteConfigBinary(writer);
                
                writer.Flush();
                var data = stream.ToArray();
                
                await serverController.GameContent_set("game_configs_binary/all_config.bin", data);
            }
        }
    }

    private async UniTask UpdateServerConfig(ServerController serverController)
    {
        var serverCfgJson = new ServerConfigJson(serverController);
        await serverCfgJson.Download();

        serverCfgJson.gameConfigVersion++;

        await serverCfgJson.Upload();
    }

    #endregion
}
