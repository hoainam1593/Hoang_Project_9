using UnityEngine;


public partial class MapCtrl
{
       
    #region Getter / Setter

    public int Row => mapData?.row ?? 0;
    public int Column => mapData?.column ?? 0;
    public float Width => mapData.column * layerGround.transform.lossyScale.x;
    public float Height => mapData.row * layerGround.transform.lossyScale.y;
    
    public Vector3 WorldTileSize => Vector3.Scale(CellSizeUnit, CellScale);
    public Vector3 CellSizeUnit => mapGrid.cellSize;
    public Vector3 CellScale => layerGround.transform.lossyScale;
    public Vector3 CellAnchor => layerGround.tileAnchor;

    public TileEnum GetTile(Vector3 screenPos)
    {
        var matrixPos = ConvertScreenPosToMatrixCoordinate(screenPos);
        // Debug.Log("matrixPos: " + matrixPos);
            
        if (IsInMatrix(matrixPos))
        {
            return (TileEnum)mapData.tiles[matrixPos.x][matrixPos.y];
        }

        return TileEnum.None;
    }

    public Vector3 GetMapBottomLeft()
    {
        return GetMapBottomLeft(Row, Column);
    }
    
    private Vector3 GetMapBottomLeft(int row, int col)
    {
        var bottomLeftCellPos = ConvertMatrixCoordinateToTilePos(row, col, new MapCoordinate(0, 0));
        var centerOfBottomLeftCell = layerGround.CellToWorld(bottomLeftCellPos);
        return centerOfBottomLeftCell;
    }
    
    #endregion Getter / Setter!
    
    #region Task - Position Converter
    
    //Convert from ScreenPos

    public Vector3 ConvertScreenPosToCenterTileWorldPos(Vector3 screenPos)
    {
        var tilePos = ConvertScreenPosToTileWorldPos(screenPos);
        tilePos += Vector3.Scale(CellAnchor, WorldTileSize);
        return tilePos;
    }
    
    public Vector3 ConvertScreenPosToTileWorldPos(Vector3 screenPos)
    {
        var tilePos = ConvertScreenPosToTilePos(screenPos);
        return ConvertTilePosToWorldPos(tilePos);
    }
    
    private MapCoordinate ConvertScreenPosToMatrixCoordinate(Vector3 screenPos)
    {
        var cellPos = ConvertScreenPosToTilePos(screenPos);
        return ConvertTilePosToMatrixCoordinate(Row, Column, cellPos);
    }

    private Vector3Int ConvertScreenPosToTilePos(Vector3 screenPos)
    {
        var worldPos = Camera.main.ScreenToWorldPoint(screenPos);
        return layerGround.WorldToCell(worldPos);
    }
    

    //Convert from MapCoordinate
    public Vector3 ConvertMatrixCoordinateToWorldPos(MapCoordinate matrixPos)
    {
        var tilePos = ConvertMatrixCoordinateToTilePos(matrixPos);
        return ConvertTilePosToWorldPos(tilePos);
    }

    public Vector3 ConvertMatrixCoordinateToCenterTileWorldPos(MapCoordinate matrixPos)
    {
        return ConvertMatrixCoordinateToWorldPos(matrixPos) + Vector3.Scale(CellAnchor, WorldTileSize);
    }
    
    public Vector3Int ConvertMatrixCoordinateToTilePos(MapCoordinate mapPos)
    {
        return ConvertMatrixCoordinateToTilePos(Row, Column, mapPos);
    }
        
    private Vector3Int ConvertMatrixCoordinateToTilePos(int row, int col, MapCoordinate mapPos)
    {
        var pos = new Vector3Int(mapPos.y, mapPos.x, 0);
        var halfX = col / 2;
        var halfY = row / 2;
        return pos - new  Vector3Int(halfX, halfY, 0);
    }

    //Convert from TilePos
    public MapCoordinate ConvertTilePosToMatrixCoordinate(Vector3Int tilePosition)
    {
        return ConvertTilePosToMatrixCoordinate(Row, Column, tilePosition);
    }

    private MapCoordinate ConvertTilePosToMatrixCoordinate(int row, int col, Vector3Int tilePosition)
    {
        var halfX = col / 2;
        var halfY = row / 2;
        var convertedPos =  tilePosition + new Vector3Int(halfX, halfY, 0);
        return new MapCoordinate(convertedPos.y, convertedPos.x);
    }
        
    public Vector3 ConvertTilePosToCenterTileWorldPos(Vector3Int tilePosition)
    {
        return layerGround.CellToWorld(tilePosition) + Vector3.Scale(CellAnchor, WorldTileSize);;
    }    
    
    public Vector3 ConvertTilePosToWorldPos(Vector3Int tilePosition)
    {
        return layerGround.CellToWorld(tilePosition);
    }


    private bool IsInMatrix(MapCoordinate mapPos)
    {
        return mapData.IsInMatrix(mapPos);
    }
    
    #endregion Task - Position Converter!!
}