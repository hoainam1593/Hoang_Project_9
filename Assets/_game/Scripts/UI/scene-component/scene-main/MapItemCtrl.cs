using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine.Serialization;

public class MapItemCtrl : RecycleScrollViewItem
{
    [SerializeField] private Button button;
    [SerializeField] private TMPro.TextMeshProUGUI number;
    [SerializeField] private Image unlockIcon;
    [SerializeField] private GameObject starsRoot;
    [SerializeField] private List<Image> starsIcon;
    
    string mapName;

    public override void SetData(object data)
    {
        MapItemData itemData = (MapItemData)data;
        
        number.text = itemData.Id.ToString();
        mapName = itemData.Name;
        
        button.onClick.AddListener(() =>
        {
            PlayerPrefs.SetString(PlayerPrefsConfig.Key_Select_Map, mapName);
            GameLauncher.instance.StartGame().Forget();
        });

        SetStar(itemData.Star);
        SetUnlock(itemData.IsActive);
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
