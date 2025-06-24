using UnityEngine;


public partial class MapCtrl
{
       
    #region Getter / Setter

    public int Row => mapData?.row ?? 0;
    public int Column => mapData?.column ?? 0;
    public float Width => mapData.column * layerGround.transform.lossyScale.x;
    public float Height => mapData.row * layerGround.transform.lossyScale.y;
    
    
    private Vector3 CellSizeUnit => mapGrid.cellSize;
    public Vector3 CellScale => layerGround.transform.lossyScale;
    public Vector3 CellAnchor => layerGround.tileAnchor;
    public Vector3 CellSize => Vector3.Scale(CellSizeUnit, CellScale);

    public TileEnum GetTile(Vector3 screenPos)
    {
        var matrixPos = ScreenToMatixCoordinate(screenPos);
        Debug.Log("matrixPos: " + matrixPos);
            
        if (IsInMatrix(matrixPos))
        {
            return (TileEnum)mapData.tiles[matrixPos.x][matrixPos.y];
        }
        else
        {
            return TileEnum.None;
        }
    }

    public Vector3 GetWorldPosOfTile(Vector3 screenPos)
    {
        var tilePos = ScreenPointToTilePosition(screenPos);
        return TilePositionToWorldPosition(tilePos);
    }
    #endregion Getter / Setter!
}