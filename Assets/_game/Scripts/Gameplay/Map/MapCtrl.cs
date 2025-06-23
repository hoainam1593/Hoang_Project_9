using System;
using Cysharp.Threading.Tasks;
using UnityEngine;

public partial class MapCtrl : MonoBehaviour
{
    [SerializeField] private TextAsset _mapData;
    private MapData mapData;
    private Grid mapGrid;

    public struct MatrixCoordinate
    {
        public int x;
        public int y;

        public MatrixCoordinate(int x, int y)
        {
            this.x = x;
            this.y = y;
        }

        public override string ToString()
        {
            return $"({x},{y})";
        }
    }

    private void Awake()
    {
        mapGrid = gameObject.GetComponent<Grid>();
    }

    private void Start()
    {
        mapData = CsvParser.ToMapData(_mapData.text);
        // var mapModel = new MapModel(mapData);
        GenerateMap(mapData);

        PassParamsToCamera(mapData);

        TestSpawnPrefab();
    }
    
    #region Getter / Setter

        public int Row
        {
            get
            {
                return mapData == null ? mapData.row : 0;
            }
        }

        public int Column
        {
            get
            {
                return mapData == null ? mapData.column : 0;
            }
        }
            

        public TileEnum GetTile(Vector3 screenPos)
        {
            var matrixPos = ConvertFromScreenToMatixCoordinate(screenPos);
            Debug.Log("matrixPos: " + matrixPos);
            
            if (IsInMatrix(matrixPos))
            {
                return (TileEnum)mapData.tiles[matrixPos.x][matrixPos.y];
            }
            else
            {
                return TileEnum.None;
            }
        }
        
    #endregion Getter / Setter!

    private async UniTaskVoid TestSpawnPrefab()
    {
        var prefab = await AssetManager.instance.LoadPrefab(GlobalConfig.Resources.TurretPrefab, "Turret_lv1");
        GameObject.Instantiate(prefab);
    }        
    
    #region Task PassParam to Camera

    private void PassParamsToCamera(MapData mapData)
    {
        var width = mapData.column * layerGround.transform.lossyScale.x;
        var height = mapData.row * layerGround.transform.lossyScale.y;
        var mapBottomLeft = GetMapBottomLeft(mapData.row, mapData.column);
        
        CameraCtrl.instance.UpdateMapSize(width, height, mapBottomLeft);
    }

    private Vector3 GetMapBottomLeft(int row, int col)
    {
        //Get centerOfBottomLeftCell
        var halfX = col / 2;
        var halfY = row / 2;
        var bottomLeftCellCoordinate = new Vector3Int(-halfX, -halfY, 0);
        var centerOfBottomLeftCell = layerGround.CellToWorld(bottomLeftCellCoordinate);
        
        //centerOfBottomLeftCell to map BottomLeft corner
        // Vector3 cellSize = mapGrid.cellSize;
        // Vector3 cellScale = layerGround.transform.lossyScale;
        // Vector3 cellAnchor = layerGround.tileAnchor;
        // var bottomLeftCorner = centerOfBottomLeftCell - new Vector3(cellSize.x * cellScale.x * cellAnchor.x, cellSize.y * cellScale.y * cellAnchor.y, 0);
        // return bottomLeftCorner;
        
        return centerOfBottomLeftCell;
    }
    
    #endregion
}
