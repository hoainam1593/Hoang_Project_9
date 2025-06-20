
using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEditor;
using UnityEngine;

public partial class BuildLocalizationState_main : EditorWindowState
{
    private readonly List<EditorUIElement_pickFile> lPickFiles = new();
    private readonly EditorUIElement_pickFolder pickRemoteAddressableProject =
        new("remote addressable project:", "KEY_REMOTE_ADDRESSABLE_PROJECT");
    private bool uploadToo;
    private ServerEnvironment serverEnvironment;
    
    public override void OnBeginState()
    {
        base.OnBeginState();

        var lExtension = new List<string>() { "csv" };
        foreach (var i in GameFrameworkConfig.instance.locFileNames)
        {
            lPickFiles.Add(new EditorUIElement_pickFile(lExtension, i));
        }
    }

    public override void OnDraw()
    {
        foreach (var i in lPickFiles)
        {
            i.Draw();
        }

        uploadToo = EditorGUILayout.Toggle("upload too", uploadToo);
        if (uploadToo)
        {
            serverEnvironment = EditorUIElementCreator.CreateDropdownList_enum(
                "server: ", ServerEnvironment.count, serverEnvironment, out _);
            pickRemoteAddressableProject.Draw();
        }

        if (GUILayout.Button("build localization"))
        {
            OnClickBuildLocalization().Forget();
        }
    }

    private async UniTask OnClickBuildLocalization()
    {
        FSM.SwitchState(new EditorWindowState_doing());
        try
        {
            await UniTask.Delay(100);

            var lPaths = new List<string>();
            foreach (var file in lPickFiles)
            {
                lPaths.Add(file.PickedPath);
            }

            var charSet = BuildLocalizationText(lPaths);
            BuildTextMeshProFont(charSet);

            if (uploadToo)
            {
                UploadRemoteAddressable(serverEnvironment, pickRemoteAddressableProject.PickedPath);
            }

            FSM.SwitchState(new EditorWindowState_done());
        }
        catch (Exception e)
        {
            Debug.LogError(e.ToString());
            StaticUtilsEditor.DisplayDialog("build localization fail, see console for detail");
        }
    }
}
