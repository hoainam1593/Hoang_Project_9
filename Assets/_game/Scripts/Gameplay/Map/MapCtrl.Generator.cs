using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Tilemaps;

public partial class MapCtrl
{
    [Header("TileMap")]
    [SerializeField] private Tilemap layerGround;

    private Tile tileOfSet = null;

    public Tile TileOfSet
    {
        get 
        {
            if (tileOfSet == null)
            {
                tileOfSet = CreateNewTile();
            }
            return tileOfSet;
        }
    }
    
    [Header("Resources")]
    // public SerializedDictionary<(TileEnum, TileEnum), Sprite> mapTiles;
    [SerializeField] private Sprite tileGround;
    [SerializeField] private Sprite tileWater;
    [SerializeField] private Sprite tileTree1;
    [SerializeField] private Sprite tileTree2;
    [SerializeField] private Sprite tileRock;
    [SerializeField] private Sprite tilePath;
    [SerializeField] private Sprite tileHall;

    [SerializeField] private List<Sprite> tileHalls;

    private Vector3Int hallGatePos;
    
    public void GenerateMap(MapData mapData)
    {
        for (int i = 0; i < mapData.row ; i++)
        {
            for (int j = 0; j < mapData.column; j++)
            {
                var tile = (TileEnum)mapData.tiles[i][j];
                
                if (tile == TileEnum.HallGate || tile == TileEnum.Hall)
                {
                    SpawnTileHalls(tile, i, j, mapData.row, mapData.column);    
                }
                else
                {
                    SpawnTile(tile, i, j, mapData.row, mapData.column);    
                }
            }
        }
    }

    private void SpawnTileHalls(TileEnum tileEnum, int row, int column, int matrixRow, int matrixCol)
    {
        
    }

    private void SpawnTile(TileEnum tileEnum, int row, int column, int matrixRow, int matrixCol)
    {
        var sprite = GetSprite(tileEnum);
        var pos = ConvertToTilePosition(matrixRow, matrixCol, row, column);
        TileOfSet.sprite = sprite;
        layerGround.SetTile(pos, TileOfSet);
    }
    
    private Sprite GetSprite(TileEnum tile)
    {
        switch (tile)
        {
            case TileEnum.Ground:
                return tileGround;
            case TileEnum.Water:
                return tileWater;
            case TileEnum.Tree1:
                return tileTree1;
            case TileEnum.Tree2:
                return tileTree2;
            case TileEnum.Rock:
                return tileRock;
            case TileEnum.Path:
                return tilePath;
            default:
               return null;
        }  
        return null; 
    }

    private Vector3Int ConvertToTilePosition(int row, int col, int x, int y)
    {
        var pos = new Vector3Int(y, x, 0);
        var halfX = col / 2;
        var halfY = row / 2;
        return pos - new  Vector3Int(halfX, halfY, 0);
    }

    private Tile CreateNewTile()
    {
        var newTile = ScriptableObject.CreateInstance<Tile>();
        newTile.color = Color.white; // hoặc chỉnh màu tùy ý
        newTile.colliderType = Tile.ColliderType.None;
        return newTile;
    }
}