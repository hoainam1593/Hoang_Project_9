using UnityEngine;
using UnityEngine.UI;
using Cysharp.Threading.Tasks;

public class PopupGameOver : BasePopup
{
    [SerializeField] private Button buttonExit;
    [SerializeField] private Button buttonRetry;


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

        buttonRetry.onClick.AddListener(() =>
        {
            GameManager.instance?.RetryGame().Forget();
            ClosePopup();
        });
    }

    public override void OnClosePopup(bool isRunAnim = true)
    {
        base.OnClosePopup(isRunAnim);

        //UnSubscribes Event
        buttonExit.onClick.RemoveAllListeners();
        buttonRetry.onClick.RemoveAllListeners();
    }
}

