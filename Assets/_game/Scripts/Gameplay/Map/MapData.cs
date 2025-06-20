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
                tiles[i][j] = 0;
            }
        }
    }
}
