using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Rendering;
using UnityEngine.Serialization;
using UnityEngine.Tilemaps;
using Cysharp.Threading.Tasks;

public partial class MapCtrl
{
    [Header("TileMap")]
    [SerializeField] private Tilemap layerGround;
    
    [Header("Resources")]
    // public SerializedDictionary<(TileEnum, TileEnum), Sprite> mapTiles;
    [SerializeField] private Sprite tileGround;
    [SerializeField] private Sprite tileWater;
    [SerializeField] private Sprite tileTree1;
    [SerializeField] private Sprite tileTree2;
    [SerializeField] private Sprite tileRock;
    [SerializeField] private Sprite tilePath;
    [SerializeField] private Sprite tileHall;

    [SerializeField] private List<Sprite> tileUpHalls;
    [SerializeField] private List<Sprite> tileDownHalls;
    [SerializeField] private List<Sprite> tileLeftHalls;
    // [SerializeField] private List<Sprite> tileRightHalls;

    private List<Vector3Int> hallPositions = new List<Vector3Int>();
    private Vector3Int hallGatePos;
    private HallDirection hallDirection;

    public enum HallDirection
    {
        Down = 1,
        Left = 3,
        Right = 5,
        Up = 7,
    }

    private TextAsset LoadMapDataFile(string mapName)
    {
        var fullPath = $"MapData/{mapName}";
        // Debug.Log("LoadMapData: " + fullPath);
        return Resources.Load<TextAsset>($"MapData/{mapName}");
    }
    
    
    private void GenerateMap()
    {
        Debug.Log($"Generating Map > {Row}x{Column}");
        CacheHallPositionAndSpawnOtherTiles(mapData);

        ConvertHallPartsOrder();
        if (hallDirection == HallDirection.Right)
        {
            ConvertLeftSideToRightSide();
        }
        
        SpawnTileHalls();    
    }
    
    
    #region Init and Cache TileOfSet
    
        private Tile tileOfSet = null;

        private Tile TileOfSet
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
        
        private Tile CreateNewTile()
        {
            var newTile = ScriptableObject.CreateInstance<Tile>();
            newTile.color = Color.white; // hoặc chỉnh màu tùy ý
            newTile.colliderType = Tile.ColliderType.None;
            return newTile;
        }
        
    #endregion Init and Cache TileOfSet!
        

    #region Task - Spawn Tiles (Not Hall)
    
        private void CacheHallPositionAndSpawnOtherTiles(MapData mapData)
        {
            hallPositions.Clear();
            Vector3Int pos;
            for (int i = 0; i < mapData.row ; i++)
            {
                for (int j = 0; j < mapData.column; j++)
                {
                    var tile = (TileEnum)mapData.tiles[i][j];
                    pos = MatrixCoordinateToTilePosition(mapData.row, mapData.column, new MapCoordinate(i, j));
                    if (tile == TileEnum.HallGate || tile == TileEnum.Hall)
                    {
                        CacheHallPosition(tile, pos);
                    }
                    else
                    {
                        SpawnTile(tile, pos);    
                    }
                }
            }
        }
        
        private void CacheHallPosition(TileEnum tile, Vector3Int pos)
        {
            hallPositions.Add(pos);
            if (tile == TileEnum.HallGate)
            {
                var hallGateIndex = hallPositions.Count - 1;
                hallDirection = (HallDirection)hallGateIndex;
                hallGatePos = pos;
            }
        }
        
        

        private void SpawnTile(TileEnum tileEnum, Vector3Int pos)
        {
            var sprite = GetSprite(tileEnum);
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
       
    #endregion Task - Spawn Tiles!

        
    #region Task - Spawn Hall Tiles
        
        private void SpawnTileHalls()
        {
            for (int i = 0; i < hallPositions.Count; i++)
            {
                SpawnTileHall(i, hallDirection, hallPositions[i]);    
            }
        }

        private void SpawnTileHall(int partIndex, HallDirection direction, Vector3Int pos)
        {
            
            var sprite = GetHallSprite(partIndex, direction);
            if (sprite == null) return;
            TileOfSet.sprite = sprite;
            layerGround.SetTile(pos, TileOfSet);
            
            //NOTE: Don't have sprite for right side, so I use sprite of left side hall and rename.
            //Therefore, I need to flip it 180 degree.
            if (hallDirection == HallDirection.Right)
            {
                layerGround.RotateTile(pos, 180f);
            }
        }

        private Sprite GetHallSprite(int partIndex, HallDirection direction)
        {
            switch (direction)
            {
                case HallDirection.Up:
                    return tileUpHalls[partIndex];
                case HallDirection.Left:
                    return tileLeftHalls[partIndex];
                case HallDirection.Right:
                    return tileLeftHalls[partIndex];
                case HallDirection.Down:
                    return tileDownHalls[partIndex];
            }

            return null;
        }
        
    #endregion Task - Spawn Hall Tiles!
    
    #region Task - Convert Hall Order
    
        //Use this function because: the yAxis direction in Tilemap is Up. (from Bottom to Top)
        //But sprite order has indexed from Top to Bottom
        private void ConvertHallPartsOrder()
        {
            List<Vector3Int> backup =  new List<Vector3Int>(hallPositions);
            hallPositions[0] = backup[6];
            hallPositions[1] = backup[7];
            hallPositions[2] = backup[8];
            
            hallPositions[6] = backup[0];
            hallPositions[7] = backup[1];
            hallPositions[8] = backup[2];
        }

        private void ConvertLeftSideToRightSide()
        {
            List<Vector3Int> backup =  new List<Vector3Int>(hallPositions);
            
            hallPositions[0] = backup[8];
            hallPositions[1] = backup[7];
            hallPositions[2] = backup[6];
            
            hallPositions[3] = backup[5];
            hallPositions[5] = backup[3];
            
            hallPositions[6] = backup[2];
            hallPositions[7] = backup[1];
            hallPositions[8] = backup[0];
        }
    #endregion Task - Convert Hall Order!
    
}