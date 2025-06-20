using R3;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class TermAndConditionView : MonoBehaviour
{
    [Header("toggle")] public Toggle toggleTerm;
    public Toggle togglePersonalInfo;
    public Toggle toggleNotice;
    public Toggle toggleNightNotice;
    public Toggle toggleAgreeAll;

    [Header("button")] public Button btnLearnMoreTerm;
    public Button btnLearnMorePersonalInfo;
    public Button btnStartGame;
    
    public UnityAction closeEvent;

    protected void Start()
    {
        SetupToggle();
        SetupButton();
    }

    private void SetupToggle()
    {
        toggleAgreeAll.isOn = false;

        toggleAgreeAll.OnValueChangedAsObservable()
            .Subscribe(agreeAll =>
            {
                toggleTerm.isOn = agreeAll;
                togglePersonalInfo.isOn = agreeAll;
                toggleNotice.isOn = agreeAll;
                toggleNightNotice.isOn = agreeAll;
            });

        toggleNotice.OnValueChangedAsObservable()
            .Subscribe(isOn =>
            {
                toggleNightNotice.interactable = isOn;
                if (!isOn)
                {
                    toggleNightNotice.isOn = false;
                }
            });

        toggleNotice.onValueChanged.AddListener(val =>
        {
            ConfirmNoticeManager.OpenConfirmNotice(val, false);
        });

        toggleNightNotice.onValueChanged.AddListener(val =>
        {
            ConfirmNoticeManager.OpenConfirmNotice(val, true);
        });
    }

    private void SetupButton()
    {
        toggleTerm.OnValueChangedAsObservable()
            .CombineLatest(togglePersonalInfo.OnValueChangedAsObservable(),
                (agreeTerm, agreeInfo) => agreeTerm && agreeInfo)
            .SubscribeToInteractable(btnStartGame);

        btnLearnMoreTerm.OnClickAsObservable()
            .Subscribe(_ => { MobirixStaticUtils.OpenMobirixTermPage(); });

        btnLearnMorePersonalInfo.OnClickAsObservable()
            .Subscribe(_ => { MobirixStaticUtils.OpenPrivacyPolicy(); });

        btnStartGame.OnClickAsObservable()
            .Subscribe(_ =>
            {
                closeEvent?.Invoke();
                Destroy(gameObject);
            });
    }

    private void Update()
    {
        if (StaticUtils.IsPressBackKey())
        {
            StaticUtils.ExitGame();
        }
    }
}
