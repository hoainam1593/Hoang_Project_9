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
                CloseTurretInfoPopup();
                ShowTurretSelectPopup(viewPos, clickedCell).Forget();
            }
        }
        else if (tile == TileEnum.Turret)
        {
            CloseTurretSelectPopup();
            CloseTurretInfoPopup();
            ShowTurretInfoPopup(viewPos, clickedCell).Forget();
        }
        else
        {
            CloseTurretSelectPopup();
            CloseTurretInfoPopup();
        }
    }

    private PopupTurretSelect popup;
    private PopupTurretInfo turretInfoPopup;

    private async UniTaskVoid ShowTurretSelectPopup(Vector3 viewPos, MapCoordinate clickedCell)
    {
        // Debug.Log("ShowTurretSelectPopup");
        var worldPos = ConvertScreenPosToCenterTileWorldPos(viewPos);
        selectedCell = clickedCell;
        popup = await PopupManager.instance.OpenPopupWorld<PopupTurretSelect>();
        popup.InitView(clickedCell, worldPos, WorldTileSize);
    }

    private async UniTaskVoid ShowTurretInfoPopup(Vector3 viewPos, MapCoordinate clickedCell)
    {
        // Debug.Log("ShowTurretInfoPopup");
        var worldPos = ConvertScreenPosToCenterTileWorldPos(viewPos);
        selectedCell = clickedCell;
        turretInfoPopup = await PopupManager.instance.OpenPopupWorld<PopupTurretInfo>();
        turretInfoPopup.InitView(clickedCell, worldPos, WorldTileSize);
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

    private void CloseTurretInfoPopup()
    {
        if (turretInfoPopup != null)
        {
            // Debug.Log("CloseTurretInfoPopup");
            PopupManager.instance.ClosePopup(turretInfoPopup);
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

    private void OnTurretDespawnComplete(object data)
    {
        var parseData = (TurretInfo)data;
        var coordinate = parseData.mapCoordinate;
        if (mapData.IsInMatrix(coordinate))
        {
            mapData.tiles[coordinate.x][coordinate.y] = (int)TileEnum.Ground;
            Debug.Log($"MapCtrl: Set tile at {coordinate} back to Ground after turret despawn");
        }
    }
}