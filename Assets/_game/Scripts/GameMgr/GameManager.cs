using UnityEngine;

public class GameManager : SingletonMonoBehaviour<GameManager>
{
    [SerializeField] private MapCtrl mapCtrl;
    [SerializeField] private CameraCtrl cameraCtrl;
    
#if UNITY_EDITOR
    public void OnClick(Vector3 viewPos)
    {
        var tile = mapCtrl.GetTile(viewPos);
        Debug.Log("OnClick Tile: " + tile);
        
        if (tile == TileEnum.Ground)
        {
            ShowTurretSelectorUI(viewPos);
        }
    }

    private void ShowTurretSelectorUI(Vector3 viewPos)
    {        
        var tilePosition = mapCtrl.GetWorldPosOfTile(viewPos);
        var screenPoint = cameraCtrl.WorldToScreenPoint(tilePosition);
        var tileSize = mapCtrl.WorldTileSize;
        var screenTileSize = cameraCtrl.WorldToScreenSize(tileSize);
        screenPoint += (screenTileSize * 0.5f);
        UIManager.instance.ShowPopup(UI.TurretSelectUI, screenPoint);
    }
    
#endif
    
    
}
