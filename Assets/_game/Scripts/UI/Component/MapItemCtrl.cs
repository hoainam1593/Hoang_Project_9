using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine.Serialization;

public class MapItemCtrl : MonoBehaviour
{
    [SerializeField] private Button button;
    [SerializeField] private TMPro.TextMeshProUGUI number;
    [SerializeField] private Image unlockIcon;
    [SerializeField] private GameObject starsRoot;
    [SerializeField] private List<Image> starsIcon;
    
    string mapName;
    
    public void InitView(MapItemData itemData)
    {
        number.text = itemData.id.ToString();
        mapName = itemData.name;
        
        button.onClick.AddListener(() =>
        {
            GameLauncher.instance.StartGame(mapName);
        });

        SetStar(itemData.star);
        SetUnlock(itemData.isActive);
    }

    private void SetStar(int number)
    {
        for (int i = 0; i < number; i++)
        {
            starsIcon[i].enabled = true;
        }

        for (int i = number; i < starsIcon.Count; i++)
        {
            starsIcon[i].enabled = false;
        }
    }

    private void SetUnlock(bool isUnlock)
    {
        unlockIcon.enabled = !isUnlock;
        button.enabled = isUnlock;
        starsRoot.SetActive(isUnlock);
    }
}
