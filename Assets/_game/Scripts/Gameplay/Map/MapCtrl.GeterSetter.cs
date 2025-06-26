using UnityEngine;


public partial class MapCtrl
{
       
    #region Getter / Setter

    public int Row => mapData?.row ?? 0;
    public int Column => mapData?.column ?? 0;
    public float Width => mapData.column * layerGround.transform.lossyScale.x;
    public float Height => mapData.row * layerGround.transform.lossyScale.y;
    
    public Vector3 WorldTileSize => Vector3.Scale(CellSizeUnit, CellScale);
    private Vector3 CellSizeUnit => mapGrid.cellSize;
    public Vector3 CellScale => layerGround.transform.lossyScale;
    public Vector3 CellAnchor => layerGround.tileAnchor;

    public TileEnum GetTile(Vector3 screenPos)
    {
        var matrixPos = ScreenToMatixCoordinate(screenPos);
        // Debug.Log("matrixPos: " + matrixPos);
            
        if (IsInMatrix(matrixPos))
        {
            return (TileEnum)mapData.tiles[matrixPos.x][matrixPos.y];
        }

        return TileEnum.None;
    }

    public Vector3 GetWorldPosOfTile(Vector3 screenPos)
    {
        var tilePos = ScreenPointToTilePosition(screenPos);
        return TilePositionToWorldPosition(tilePos);
    }

    public Vector3 GetMapBottomLeft()
    {
        return GetMapBottomLeft(Row, Column);
    }
    
    private Vector3 GetMapBottomLeft(int row, int col)
    {
        var bottomLeftCellPos = MatrixCoordinateToTilePosition(row, col, new MapCoordinate(0, 0));
        var centerOfBottomLeftCell = layerGround.CellToWorld(bottomLeftCellPos);
        return centerOfBottomLeftCell;
    }
    
    #endregion Getter / Setter!
    
    #region Task - Position Converter

    public Vector3Int MatrixCoordinateToTilePosition(MapCoordinate mapPos)
    {
        return MatrixCoordinateToTilePosition(Row, Column, mapPos);
    }
        
    private Vector3Int MatrixCoordinateToTilePosition(int row, int col, MapCoordinate mapPos)
    {
        var pos = new Vector3Int(mapPos.y, mapPos.x, 0);
        var halfX = col / 2;
        var halfY = row / 2;
        return pos - new  Vector3Int(halfX, halfY, 0);
    }

    public MapCoordinate TilePositionToMatrixCoordinate(Vector3Int tilePosition)
    {
        return TilePositionToMatrixCoordinate(Row, Column, tilePosition);
    }

    private MapCoordinate TilePositionToMatrixCoordinate(int row, int col, Vector3Int tilePosition)
    {
        var halfX = col / 2;
        var halfY = row / 2;
        var convertedPos =  tilePosition + new Vector3Int(halfX, halfY, 0);
        // Debug.Log("ClickedCell: " + convertedPos);
        return new MapCoordinate(convertedPos.y, convertedPos.x);
    }

    private Vector3Int ScreenPointToTilePosition(Vector3 screenPos)
    {
        var worldPos = Camera.main.ScreenToWorldPoint(screenPos);
        return layerGround.WorldToCell(worldPos);
    }
        
    private Vector3 TilePositionToWorldPosition(Vector3Int tilePosition)
    {
        return layerGround.CellToWorld(tilePosition);
    }

    private MapCoordinate ScreenToMatixCoordinate(Vector3 screenPos)
    {
        var cellPos = ScreenPointToTilePosition(screenPos);
        return TilePositionToMatrixCoordinate(Row, Column, cellPos);
    }

    private bool IsInMatrix(MapCoordinate mapPos)
    {
        return (0 <= mapPos.x && mapPos.x < Row) && (0 <= mapPos.y && mapPos.y < Column);
    }
    
    #endregion Task - Position Converter!!
}