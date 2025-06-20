
using DG.Tweening;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoaderOverlayManager : SingletonMonoBehaviour<LoaderOverlayManager>
{
    public GameObject objOverlay;
    public Image imgOverlay;
    public float fadeTime = 0.5f;

    public static bool canSwitchScene = true;

    public void LoadScene(string sceneName)
    {
        if (!canSwitchScene)
        {
            return;
        }
        
        objOverlay.SetActive(true);
        imgOverlay.color = Color.black;
        SceneManager.LoadScene(sceneName);
    }

    public void EndOverlay()
    {
        if (objOverlay.activeSelf)
        {
            imgOverlay.DOFade(0, fadeTime).OnComplete(() => { objOverlay.SetActive(false); });
        }
    }
}
