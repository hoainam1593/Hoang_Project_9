
using Cysharp.Threading.Tasks;
using UnityEngine;

public partial class MapCtrl
{
    private MapCoordinate selectedCell = MapCoordinate.oneNegative;
  
    private void OnClickedInMap(object data)
    {
        var viewPos = (Vector3)data;
        Debug.Log("OnClickedInMap: " + viewPos);
        
        var clickedCell = ScreenToMatixCoordinate(viewPos);
        var tile = GetTile(viewPos);
        Debug.Log($"Clicked Tile {clickedCell}: {tile}");
        
        if (tile == TileEnum.Ground)
        {
            if (clickedCell != selectedCell)
            {
                CloseTurretSelectPopup();
                ShowTurretSelectPopup(viewPos, clickedCell).Forget();
            }
        }
        else
        {
            CloseTurretSelectPopup();
        }
    }

    private TurretSelectPopup popup;
    private async UniTaskVoid ShowTurretSelectPopup(Vector3 viewPos, MapCoordinate clickedCell)
    {
        // Debug.Log("ShowTurretSelectPopup");
        var tilePosition = GetWorldPosOfTile(viewPos);
        var uiPos = tilePosition + CellAnchor;
        selectedCell = clickedCell;
        popup = await PopupManager.instance.OpenPopupWorld<TurretSelectPopup>();
        popup.InitView(clickedCell, uiPos, WorldTileSize);
    }

    private void CloseTurretSelectPopup()
    {
        if (popup != null)
        {
            // Debug.Log("CloseTurretSelectPopup");
            PopupManager.instance.ClosePopup(popup);
        }
        selectedCell = MapCoordinate.oneNegative;
    }

    private void OnTurretSpawnStart(object data)
    {
        selectedCell = MapCoordinate.oneNegative;
    }
    
    private void OnTurretSpawnCompleted(object data)
    {
        var coordinate = (MapCoordinate)data;
        if (mapData.IsInMatrix(coordinate))
        {
            mapData.tiles[coordinate.x][coordinate.y] = (int)TileEnum.Turret;
        }
    }
}