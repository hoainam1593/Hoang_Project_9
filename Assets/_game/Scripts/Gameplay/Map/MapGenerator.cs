using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Tilemaps;

public class MapGenerator : SingletonMonoBehaviour<MapGenerator>
{
    [Header("TileMap")]
    [SerializeField] private Tilemap layerGround;
    [SerializeField] private Tilemap layerObject;
    
    [Header("Resources")]
    // public SerializedDictionary<(TileEnum, TileEnum), Sprite> mapTiles;
    [SerializeField] private Sprite tileGround;
    [SerializeField] private Sprite tileWater;
    [SerializeField] private Sprite tileTree1;
    [SerializeField] private Sprite tileTree2;
    [SerializeField] private Sprite tileRock;
    [SerializeField] private Sprite tilePath;
    [SerializeField] private Sprite tileHall;

    
    public void GenerateMap(MapData mapData)
    {
        for (int i = 0; i < mapData.row ; i++)
        {
            for (int j = 0; j < mapData.column; j++)
            {
                var tile = (TileEnum)mapData.tiles[i][j];
                spawnTile(tile, i, j);
            }
        }
    }

    private void spawnTile(TileEnum tile, int row, int column)
    {
        switch (tile)
        {
            case TileEnum.Ground:
                spawnGround(row, column);
                break;
            case TileEnum.Water:
                spawnWater(row, column);
                break;
            case TileEnum.Tree1:
                spawnTree1(row, column);
                break;
            case TileEnum.Tree2:
                spawnTree2(row, column);
                break;
            case TileEnum.Rock:
                spawnRock(row, column);
                break;
            case TileEnum.Path:
                spawnPath(row, column);
                break;
            case TileEnum.Hall:
                spawnHall(row, column);
                break;
        }
    }

    private void spawnGround(int row, int column)
    {
        var tile = new Tile();
        tile.sprite = tileGround;
        var pos = new Vector3Int(row, column, 0);
        layerGround.SetTile(pos, tile);
    }
    
    private void spawnWater(int row, int column)
    {
        var tile = new Tile();
        tile.sprite = tileWater;
        var pos = new Vector3Int(row, column, 0);
        layerGround.SetTile(pos, tile);
    }
    
    private void spawnTree1(int row, int column)
    {
        var tile = new Tile();
        tile.sprite = tileTree1;
        var pos = new Vector3Int(row, column, 0);
        layerObject.SetTile(pos, tile);
    }
    
    private void spawnTree2(int row, int column)
    {
        var tile = new Tile();
        tile.sprite = tileTree2;
        var pos = new Vector3Int(row, column, 0);
        layerObject.SetTile(pos, tile);
    }  
    
    private void spawnRock(int row, int column)
    {
        var tile = new Tile();
        tile.sprite = tileRock;
        var pos = new Vector3Int(row, column, 0);
        layerObject.SetTile(pos, tile);
    }
    
    private void spawnPath(int row, int column)
    {
        var tile = new Tile();
        tile.sprite = tilePath;
        var pos = new Vector3Int(row, column, 0);
        layerGround.SetTile(pos, tile);
    }
    
    private void spawnHall(int row, int column)
    {
        var tile = new Tile();
        tile.sprite = tileHall;
        var pos = new Vector3Int(row, column, 0);
        layerGround.SetTile(pos, tile);
    }
}