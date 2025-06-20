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
                spawnTile(tile, i, j, mapData.row, mapData.column);
            }
        }
    }

    private void spawnTile(TileEnum tileEnum, int row, int column, int matrixRow, int matrixCol)
    {
        var sprite = getSprite(tileEnum);
        var pos = convertToTilePosition(matrixRow, matrixCol, row, column);
        var tile = CreateTile(sprite);
        layerGround.SetTile(pos, tile);
    }
    
    private Sprite getSprite(TileEnum tile)
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
            case TileEnum.Hall:
                return tileHall;
        }   
        return tileGround;
    }

    private Vector3Int convertToTilePosition(int row, int col, int x, int y)
    {
        var pos = new Vector3Int(y, x, 0);
        var halfX = col / 2;
        var halfY = row / 2;
        return pos - new  Vector3Int(halfX, halfY, 0);
    }

    private Tile CreateTile(Sprite sprite)
    {
        Tile newTile = ScriptableObject.CreateInstance<Tile>();
        newTile.sprite = sprite;
        newTile.color = Color.white; // hoặc chỉnh màu tùy ý
        newTile.colliderType = Tile.ColliderType.None;
        return newTile;
    }
}