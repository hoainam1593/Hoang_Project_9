using System;
using UnityEditor.Tilemaps;
using UnityEngine;

public partial class MapCtrl : MonoBehaviour
{
    [SerializeField] private TextAsset _mapData;
    private Grid mapGrid;

    private void Awake()
    {
        mapGrid = gameObject.GetComponent<Grid>();
    }

    private void Start()
    {
        var mapData = CsvParser.ToMapData(_mapData.text);
        // var mapModel = new MapModel(mapData);
        GenerateMap(mapData);

        PassParamsToCamera(mapData);
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
