
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Unity.EditorCoroutines.Editor;
using UnityEngine;

public class ExportSpineState_export : EditorWindowState
{
    #region core

    private readonly string inputFolder;
    private readonly string outputFolder;
    
    private int maxProgress;
    private int currentProgress;
    
    public ExportSpineState_export(string inputFolder, string outputFolder)
    {
        this.inputFolder = inputFolder;
        this.outputFolder = outputFolder;
    }

    public override void OnBeginState()
    {
        base.OnBeginState();
        
        var listSpineFiles = StaticUtils.GetFilesInFolder(inputFolder, true, null, "spine", true);
        maxProgress = listSpineFiles.Count;
        
        EditorCoroutineUtility.StartCoroutineOwnerless(ExportCoroutine(listSpineFiles));
    }
    
    public override void OnDraw()
    {
        var windowSize = FSM.position;
        var text = $"exporting {currentProgress}/{maxProgress}";
        EditorUIElementCreator.CreateLabelField_center(text, windowSize.width, windowSize.height, 60);
    }

    #endregion

    #region export

    private IEnumerator ExportCoroutine(List<string> listSpineFiles)
    {
        foreach (var i in listSpineFiles)
        {
            yield return new WaitForSeconds(0.5f);
            
            ExportSpine(i);
            currentProgress++;
            FSM.Repaint();
        }
        
        StaticUtilsEditor.DisplayDialog("export spine success");
    }
    
    private void ExportSpine(string spineFilePath)
    {
        var spineFileFolder = StandardizePath(Path.GetDirectoryName(spineFilePath));
        var inputPath = StandardizePath(inputFolder);
        
        var outputPath = spineFileFolder.Replace(inputPath, outputFolder);

        var settingsPath =
            $"{StaticUtils.GetFrameworkPath()}/Editor/Core/export-spine-tool/export-template.export.json";
        if (!StaticUtils.CheckFileExist(settingsPath, true))
        {
            throw new Exception($"File does not exist: {settingsPath}");
        }

        StaticUtilsEditor.RunBatchScript("Spine", new List<string>()
        {
            "-i",
            $"\"{spineFilePath}\"",
            "-o",
            $"\"{outputPath}\"",
            "-e",
            $"\"{settingsPath}\"",
        });
    }

    private string StandardizePath(string path)
    {
        if (path.Contains('\\'))
        {
            path = path.Replace('\\', '/');
        }
        return path;
    }

    #endregion
}
