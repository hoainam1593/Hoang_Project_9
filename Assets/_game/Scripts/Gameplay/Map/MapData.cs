using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class MapData
{
    public int row;
    public int column;
    public int[][] tiles;

    public MapData(int row, int col)
    {
        this.row = row;
        this.column = col;
        tiles = new int[row][];
        for (int i = 0; i < row; i++)
        {
            tiles[i] = new int[col];
            for (int j = 0; j < col; j++)
            {
                // Debug.Log($"MapData[{i}][{j}]: {tiles[i][j]}");
                tiles[i][j] = 0;
            }
        }
    }

    public bool IsInMatrix(MapCoordinate coordinate)
    {
        var xCheck = 0 < coordinate.x && coordinate.x < this.row;
        var ycheck  = 0 < coordinate.y && coordinate.y < this.column;
        return  xCheck && ycheck;
    }
}
