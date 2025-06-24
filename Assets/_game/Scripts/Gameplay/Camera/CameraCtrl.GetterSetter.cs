using UnityEngine;

public partial class CameraCtrl
{
    #region Task Converter

    public Vector3 WorldToScreenSize(Vector3 worldTileSize)
    {
        var unitsPerScreenHeight = mainCam.orthographicSize * 2f;
        var pixelsPerUnit = Screen.height / unitsPerScreenHeight;
        var tileScreenWidth  = worldTileSize.x * pixelsPerUnit;
        var tileScreenHeight = worldTileSize.y * pixelsPerUnit;
        return new  Vector3(tileScreenWidth, tileScreenHeight, 1);
    }

    public Vector3 WorldToScreenPoint(Vector3 worldPoint)
    {
        return mainCam.WorldToScreenPoint(worldPoint);
    }
    
    #endregion Task Converter!!
    
}
