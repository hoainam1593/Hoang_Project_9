
using System.Collections.Generic;
using UnityEngine;

public class LogViewerState_selectLog:EditorWindowState
{
    private EditorUIElement_pickFile chooseFile =
        new EditorUIElement_pickFile(new List<string>() { "log", "txt" }, "log path: ");

    public override void OnDraw()
    {
        chooseFile.Draw();
        if (GUILayout.Button("view log"))
        {
            FSM.SwitchState(new LogViewerState_viewLog(chooseFile.PickedPath));
        }
    }
}
