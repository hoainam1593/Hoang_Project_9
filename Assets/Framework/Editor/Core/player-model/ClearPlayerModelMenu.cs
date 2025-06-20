
using UnityEditor;

public class ClearPlayerModelMenu
{
    [MenuItem("\u2726\u2726TOOLS\u2726\u2726/clear player model")]
    private static void OnMenuClicked()
    {
        StaticUtils.DeleteFolder(PlayerModelManager.GetModelFolderPath());
        StaticUtilsEditor.DisplayDialog("cleared player model");
    }
}
