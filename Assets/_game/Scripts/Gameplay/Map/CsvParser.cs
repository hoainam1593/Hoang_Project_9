using System;
using System.Linq;
using UnityEngine;

public static class CsvParser
{
    public static MapData ToMapData(string text)
    {
        // Debug.Log($"mapData csv: {text}");
        int row = 1;
        int col = 1;
        
        string[] lines = text.Split('\n');

        try
        {
            if (lines.Length > 0)
            {
                var line1 = lines[0];
                string[] headers = line1.Split(',');
                row =  int.Parse(headers[0]);
                col = int.Parse(headers[1]);
                if (row == 0 || col == 0)
                {
                    return null;
                }

                var data = new MapData(row, col);

                if (lines.Length < row + 1)
                {
                    Debug.LogError($"Missing data lines: {row + 1 - lines.Length}");
                    return null;
                }
                
                for (int i = 1; i < row + 1; i++)
                {
                    var line =  lines[i];
                    // Debug.Log($"Line {i}: {line}");
                    var cells = line.Split(',');

                    if (cells.Length < col)
                    {
                        Debug.LogError($"Missing cell at line: {i}");
                        return null;
                    }
                    
                    for (int j = 0; j < col; j++)
                    {
                        data.tiles[i-1][j] = int.Parse(cells[j]);
                        // Debug.Log($"MapData[{i-1}][{j}]: {data.tiles[i-1][j]}");
                    }
                }

                reverseMap(data);
                return data;
            }
            else
            {
                return null;
            }
        }
        catch (Exception e)
        {
            Debug.Log(e.Message);
        }
        return null;
    }

    private static void reverseMap(MapData data)
    {
        data.tiles = data.tiles.Reverse().ToArray();
    }
}