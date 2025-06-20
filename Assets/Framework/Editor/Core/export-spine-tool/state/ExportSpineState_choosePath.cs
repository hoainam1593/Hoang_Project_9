
using UnityEngine;

public class ExportSpineState_choosePath : EditorWindowState
{
    private readonly EditorUIElement_pickFolder inputFolder = new("input folder:");
    private readonly EditorUIElement_pickFolder outputFolder = new("output folder:");

    public override void OnDraw()
    {
        inputFolder.Draw();
        outputFolder.Draw();

        if (GUILayout.Button("export spine"))
        {
            FSM.SwitchState(new ExportSpineState_export(inputFolder.PickedPath, outputFolder.PickedPath));
        }
    }
}
