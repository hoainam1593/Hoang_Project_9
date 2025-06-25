using UnityEngine;
using UnityEngine.UI;

public class MapItemCtrl : MonoBehaviour
{
    [SerializeField] private Button button;
    [SerializeField] private TMPro.TextMeshProUGUI number;
    
    string mapName;
    
    public void InitView(MapItemData itemData)
    {
        number.text = itemData.id.ToString();
        mapName = itemData.name;
        
        button.onClick.AddListener(() =>
        {
            GameLauncher.instance.StartGame(mapName);
        });
    }
}
