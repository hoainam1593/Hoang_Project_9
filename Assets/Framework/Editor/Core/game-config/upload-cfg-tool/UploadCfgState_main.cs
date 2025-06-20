
using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class UploadCfgState_main : EditorWindowState
{
    private ServerEnvironment serverEnvironment;
    
    public override void OnDraw()
    {
        serverEnvironment = EditorUIElementCreator.CreateDropdownList_enum(
            "server: ", ServerEnvironment.count, serverEnvironment, out _);
        
        if (GUILayout.Button("upload configs"))
        {
            OnClickUpload().Forget();
        }
    }

    private async UniTask OnClickUpload()
    {
        FSM.PushState(new EditorWindowState_doing());
        try
        {
            var binaryCfgPath = ConvertToBinaryCfg();
            await UploadCfg(binaryCfgPath);
            
            StaticUtilsEditor.DisplayDialog("upload configs success");
        }
        catch (Exception e)
        {
            Debug.LogError(e.ToString());
            StaticUtilsEditor.DisplayDialog("upload configs fail, see console for detail");
        }
        FSM.PopState();
    }

    private string ConvertToBinaryCfg()
    {
        var cfgPath = StaticUtilsEditor.RandomATempPath("bin");
        StaticUtils.OpenFileForWrite(cfgPath, binWriter =>
        {
            var configReadWriteManager = new ConfigReadWriteManager(ConfigManager.GetListConfigsInEditor());
            configReadWriteManager.ReadConfig_editor();
            configReadWriteManager.WriteConfigBinary(binWriter);
        }, true);
        return cfgPath;
    }

    private async UniTask UploadCfg(string binaryCfgPath)
    {
        var cmd = $"{StaticUtils.GetProjectPath()}/ExternalTools/AwsUploader/CSharpProjUploadAWS.exe";
        var textCfgFolder = $"{StaticUtils.GetProjectPath()}/GameConfig";
        StaticUtilsEditor.RunBatchScript(cmd, new List<string>()
        {
            "config",
            serverEnvironment.ToString(),
            textCfgFolder,
            binaryCfgPath
        });
    }
}
