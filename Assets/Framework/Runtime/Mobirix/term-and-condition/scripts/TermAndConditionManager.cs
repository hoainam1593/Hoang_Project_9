using UnityEngine;
using Cysharp.Threading.Tasks;

public static class TermAndConditionManager
{
    public class ClosePopupResult
    {
        public bool toggleNotice;
        public bool toggleNightNotice;
    }
    
    private const bool alwaysShowTerm = false;
    
    public static async UniTask<ClosePopupResult> OpenTerm(SystemLanguage language)
    {
        // check first open
        var key = "CAN_OPEN_TERM";
        var canOpen = PlayerPrefs.GetInt(key, 1);

        if (canOpen == 0 && !alwaysShowTerm)
        {
            return null;
        }

        // check language
        if (language != SystemLanguage.Korean && !alwaysShowTerm)
        {
            PlayerPrefs.SetInt(key, 0);
            return null;
        }

        // open
        var closePopup = false;
        var namePrefab = Screen.width > Screen.height ? "term-horizontal" : "term-vertical";
        var prefab = Resources.Load<GameObject>(namePrefab);
        var pu = Object.Instantiate(prefab).GetComponent<TermAndConditionView>();
        pu.closeEvent = () => { closePopup = true; };

        await UniTask.WaitUntil(() => closePopup);

        PlayerPrefs.SetInt(key, 0);

        return new ClosePopupResult()
        {
            toggleNotice = pu.toggleNotice,
            toggleNightNotice = pu.toggleNightNotice,
        };
    }
}
