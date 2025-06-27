using System.Collections.Generic;
using UnityEngine;

public partial class MapCtrl
{
    TilemapPath<Vector3Int> tilemapPath = null;
    MatrixPath<MapCoordinate> matrixPath = null;

    public MatrixPath<MapCoordinate> GetMatrixPath()
    {        
        if (matrixPath != null)
        {
            return matrixPath;
        }        
        matrixPath = GetMatrixPathFromMap();
        return matrixPath;
    }

    public TilemapPath<Vector3Int> GetTilemapPath()
    {

        if (tilemapPath != null)
        {
            return tilemapPath;
        }

        if (matrixPath != null)
        {
            tilemapPath = CovertMatrixPathToTilePath(matrixPath);
            return tilemapPath;
        }
        
        matrixPath = GetMatrixPathFromMap();
        tilemapPath = CovertMatrixPathToTilePath(matrixPath);
        return tilemapPath;
    }
    
    private TilemapPath<Vector3Int> CovertMatrixPathToTilePath(MatrixPath<MapCoordinate> matrixPath)
    {
        TilemapPath<Vector3Int> path = new TilemapPath<Vector3Int>();
        foreach (var point in matrixPath.Points)
        {
            path.Add(ConvertMatrixCoordinateToTilePos(point));
        }

        return path;
    }
    
    #region Task - GetPath from TileMap

    private int[] xDiff = new[] { -1, 1, 0, 0 };
    private int[] yDiff = new[] { 0, 0, -1, 1 };
    private bool[][] isVisited;
    
    private MatrixPath<MapCoordinate> GetMatrixPathFromMap()
    {
        isVisited = new bool[Row][];
        for (int i = 0; i < Row; i++)
        {
            isVisited[i] = new bool[Column];
        }
        
        List<MapCoordinate> paths = new List<MapCoordinate>();

        var mapPoint = ConvertTilePosToMatrixCoordinate(hallGatePos);
        paths.Add(mapPoint);
        
        MapCoordinate nextPoint;
        while (GetPoint(mapPoint, out nextPoint))
        {
            paths.Add(nextPoint);
            mapPoint = nextPoint;
        }
        
        paths.Reverse();
        return new MatrixPath<MapCoordinate>(paths);
    }

    private bool GetPoint(MapCoordinate crrPoint, out MapCoordinate nextPoint)
    {
        // Debug.Log("CrrPoint: " + crrPoint);
        isVisited[crrPoint.x][crrPoint.y] = true;
        nextPoint = new MapCoordinate(crrPoint);
        for (int i = 0; i < 4; i++)
        {
            nextPoint = crrPoint + new MapCoordinate(xDiff[i], yDiff[i]);
            // Debug.Log("NextPoint: " + nextPoint);
            if (mapData.IsInMatrix(nextPoint) && !isVisited[nextPoint.x][nextPoint.y])
            {
                // Debug.Log("ChecK point " + nextPoint + " : " + (TileEnum)mapData.tiles[nextPoint.x][nextPoint.y]);
                if (mapData.tiles[nextPoint.x][nextPoint.y] == (int)TileEnum.Path)
                {
                    isVisited[nextPoint.x][nextPoint.y] = true;
                    return true;
                }
            }
        }

        return false;
    }
    #endregion Task - GetPath from TileMap
}