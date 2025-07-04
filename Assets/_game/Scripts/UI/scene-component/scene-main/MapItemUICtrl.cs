using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MapItemUICtrl : MonoBehaviour
{
    [SerializeField] private Button button;
    [SerializeField] private TMPro.TextMeshProUGUI number;
    [SerializeField] private Image unlockIcon;
    [SerializeField] private GameObject starsRoot;
    [SerializeField] private List<Image> starsIcon;

    string mapName;

    public void InitView(MapItemData itemData)
    {
        number.text = itemData.Id.ToString();
        mapName = itemData.Name;

        button.onClick.AddListener(() =>
        {
            PlayerPrefs.SetString(PlayerPrefsConfig.Key_Select_Map_Name, mapName);
            PlayerPrefs.SetInt(PlayerPrefsConfig.Key_Select_Map_Id, itemData.Id);
            GameLauncher.instance.StartGame().Forget();
        });

        SetStar(itemData.Star);
        SetUnlock(itemData.Star >= 0);
    }

    private void SetStar(int number)
    {
        if (number < 0)
        {
            return;
        }

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
