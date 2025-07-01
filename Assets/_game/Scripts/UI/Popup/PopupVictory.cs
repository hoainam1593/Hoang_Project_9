using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;
using Cysharp.Threading.Tasks;

public class PopupVictory : BasePopup
{
    [SerializeField] private TMPro.TextMeshProUGUI textGoldBonus;
    [SerializeField] private List<Image> starImages;
    [SerializeField] private Button buttonExit;
    [SerializeField] private Button buttonNextLevel;


    protected override void Start()
    {
        base.Start();
        OnStart();
    }

    private void OnStart()
    {
        // Debug.Log("OnStart");
        buttonExit.onClick.AddListener(() =>
        {
            GameManager.instance?.ExitGame().Forget();
            ClosePopup();
        });

        buttonNextLevel.onClick.AddListener(() =>
        {
            //GameManager.instance?.NextLevel().Forget();
            ClosePopup();
        });
    }

    public override void OnClosePopup(bool isRunAnim = true)
    {
        base.OnClosePopup(isRunAnim);

        //UnSubscribes Event
        buttonExit.onClick.RemoveAllListeners();
        buttonNextLevel.onClick.RemoveAllListeners();
    }

    public void InitView(int star, int goldBonus)
    {
        textGoldBonus.text = goldBonus.ToString();
        FillStar(star);
    }

    private void FillStar(int star)
    {
        for (int i = 0; i < starImages.Count; i++)
        {
            if (i < star)
            {
                starImages[i].gameObject.SetActive(true);
            }
            else
            {
                starImages[i].gameObject.SetActive(false);
            }
        }
    }
}

