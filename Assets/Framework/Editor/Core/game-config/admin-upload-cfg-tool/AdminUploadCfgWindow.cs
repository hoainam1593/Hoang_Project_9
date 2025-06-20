
using UnityEditor;

public class AdminUploadCfgWindow : EditorWindowStateMachine
{
    [MenuItem("\u2726\u2726TOOLS\u2726\u2726/[admin] upload config")]
    static void OnMenuClicked()
    {
        OpenWindow<AdminUploadCfgWindow>(new AdminUploadCfgState_main());
    }
}
