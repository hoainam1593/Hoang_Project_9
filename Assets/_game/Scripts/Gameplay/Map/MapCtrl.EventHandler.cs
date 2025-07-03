
using Cysharp.Threading.Tasks;
using UnityEngine;

public partial class MapCtrl
{
    private MapCoordinate selectedCell = MapCoordinate.oneNegative;
  
    private void OnClickedInMap(object data)
    {
        var viewPos = (Vector3)data;
        // Debug.Log("OnClickedInMap: " + viewPos);
        
        var clickedCell = ConvertScreenPosToMatrixCoordinate(viewPos);
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

    private PopupTurretSelect popup;
    private async UniTaskVoid ShowTurretSelectPopup(Vector3 viewPos, MapCoordinate clickedCell)
    {
        // Debug.Log("ShowTurretSelectPopup");
        var worldPos = ConvertScreenPosToCenterTileWorldPos(viewPos);
        selectedCell = clickedCell;
        popup = await PopupManager.instance.OpenPopupWorld<PopupTurretSelect>();
        popup.InitView(clickedCell, worldPos, WorldTileSize);
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
        var parseData = (TurretInfo)data;
        var coordinate = parseData.mapCoordinate;
        if (mapData.IsInMatrix(coordinate))
        {
            mapData.tiles[coordinate.x][coordinate.y] = (int)TileEnum.Turret;
        }
    }
}