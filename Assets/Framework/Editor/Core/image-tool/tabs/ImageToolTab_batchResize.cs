using UnityEditor;
using UnityEngine;

public class ImageToolTab_batchResize:EditorUIElement_tabWindow.TabItemWindow
{
    private bool usePercentage = true;
    private float percentage = 100;
    private int newWidth = 100;
    private int newHeight = 100;
    private DefaultAsset folder;
    
    public override string tabText => "batch resize";
    public override void OnDraw()
    {
        usePercentage = EditorGUILayout.Toggle("Use Percentage", usePercentage);
        if (usePercentage)
        {
            percentage = EditorGUILayout.FloatField("Percentage", percentage);
        }
        else
        {
            newWidth = EditorGUILayout.IntField("New Width:", newWidth);
            newHeight = EditorGUILayout.IntField("New Height:", newHeight);
        }

        folder = (DefaultAsset)EditorGUILayout.ObjectField("Image Folder:", folder, typeof(DefaultAsset), false);
        
        if (GUILayout.Button("resize"))
        {
            OnResizeClicked();
        }
    }

    private void OnResizeClicked()
    {
        var path = AssetDatabase.GetAssetPath(folder);
        path = path.Substring("Assets/".Length);

        var imgFiles = StaticUtils.GetFilesInFolder(path, false, null, "png");
        foreach (var file in imgFiles)
        {
            ResizeImage(file);
        }
        
        AssetDatabase.Refresh();

        StaticUtilsEditor.DisplayDialog("resize complete");
    }

    private void ResizeImage(string imgName)
    {
        var folderPath = AssetDatabase.GetAssetPath(folder);
        var imgPath = $"{folderPath}/{imgName}.png";
        var texture = AssetDatabase.LoadAssetAtPath<Texture2D>(imgPath);
        if (ImageToolWindow.ValidateTextureFormat(texture, false))
        {
            var needRevertIsReadable = ImageToolWindow.SetImageIsReadable(texture, true);
            var newSz = GetNewSize(texture);
            var newTexture = ImageToolWindow.ResizeImage(texture, newSz.x, newSz.y);
            ImageToolWindow.SaveTexture(newTexture, imgPath, false);
            if (needRevertIsReadable)
            {
                ImageToolWindow.SetImageIsReadable(texture, false);
            }
        }
    }

    private Vector2Int GetNewSize(Texture2D texture)
    {
        if (usePercentage)
        {
            var t = percentage / 100f;
            return new Vector2Int((int)(t * texture.width), (int)(t * texture.height));
        }
        else
        {
            return new Vector2Int(newWidth, newHeight);
        }
    }
}
