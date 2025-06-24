using UnityEngine;

public partial class CameraCtrl
{
    #region Task Converter

    public Vector3 WorldToScreenSize(Vector3 worldTileSize)
    {
        float unitsPerScreenHeight = Camera.main.orthographicSize * 2f;
        float pixelsPerUnit = Screen.height / unitsPerScreenHeight;

        float tileScreenWidth  = worldTileSize.x * pixelsPerUnit;
        float tileScreenHeight = worldTileSize.y * pixelsPerUnit;
        return new  Vector3(tileScreenWidth, tileScreenHeight, 1);
    }

    public Vector3 WorldToScreenPoint(Vector3 worldPoint)
    {
        return mainCam.WorldToScreenPoint(worldPoint);
    }
    
    #endregion Task Converter!!
    
}
