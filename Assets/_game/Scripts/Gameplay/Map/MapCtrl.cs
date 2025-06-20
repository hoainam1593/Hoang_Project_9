using UnityEngine;

public class MapCtrl : MonoBehaviour
{
    [SerializeField] private TextAsset _mapData;

    private void Awake()
    {
        var mapData = JsonUtility.FromJson<MapData>(_mapData.text);
        // var mapModel = new MapModel(mapData);
        MapGenerator.instance.GenerateMap(mapData);
    }
}
