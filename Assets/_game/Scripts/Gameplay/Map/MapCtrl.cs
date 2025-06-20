using UnityEngine;

public class MapCtrl : MonoBehaviour
{
    [SerializeField] private TextAsset _mapData;

    private void Awake()
    {
        var mapData = CsvParser.ToMapData(_mapData.text);
        
        // var mapModel = new MapModel(mapData);
        MapGenerator.instance.GenerateMap(mapData);
    }
}
