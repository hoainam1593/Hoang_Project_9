using UnityEngine;

public partial class MapCtrl : MonoBehaviour
{
    [SerializeField] private TextAsset _mapData;

    private void Awake()
    {
        var mapData = CsvParser.ToMapData(_mapData.text);
        
        // var mapModel = new MapModel(mapData);
        GenerateMap(mapData);
    }
}
