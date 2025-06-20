
using System;
using System.Collections.Generic;
using UnityEngine;

public partial class BuildLocalizationState_main
{
    private void UploadRemoteAddressable(ServerEnvironment serverEnvironment, string projPath)
    {
        CopyAssets(projPath);
        BuildAssets(projPath);
        UploadAssets(serverEnvironment, projPath);
    }

    private void CopyAssets(string projPath)
    {
        var srcPath = $"{Application.dataPath}/_game/localization-data";
        var targetPath = $"{projPath}/Assets/_game/localization-data";
        StaticUtils.CopyFolder(srcPath, targetPath, true);
    }

    private void BuildAssets(string projPath)
    {
        var lPlatforms = new List<string>()
        {
            "StandaloneWindows64", "Android", "iOS"
        };
        
        var shFile = $"{projPath}/build_addressable.sh";
        var unityExe = $"\"{AppDomain.CurrentDomain.BaseDirectory}/Unity.exe\"";

        foreach (var i in lPlatforms)
        {
            StaticUtilsEditor.RunShellScript(shFile, new List<string>()
            {
                unityExe,
                projPath,
                i
            });
        }
    }

    private void UploadAssets(ServerEnvironment serverEnvironment, string projPath)
    {
        var cmd = $"{StaticUtils.GetProjectPath()}/ExternalTools/AwsUploader/CSharpProjUploadAWS.exe";
        StaticUtilsEditor.RunBatchScript(cmd, new List<string>()
        {
            "addressable",
            serverEnvironment.ToString(),
            $"{projPath}/ServerData",
        });
    }
}
