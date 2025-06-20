using System;
using UnityEngine;
using UnityEngine.UI;
using R3;

public class ConfirmNoticeView : MonoBehaviour
{
    public LocalizedText_legacy txtMsg;
    public Button btnOk;
    
    public void Init(bool isAgree, bool isNoticeAtNight)
    {
        var key = isAgree
            ? isNoticeAtNight ? "msg_night_notice_agree" : "msg_notice_agree"
            : isNoticeAtNight
                ? "msg_night_notice_disagree"
                : "msg_notice_disagree";
        var time = DateTime.Now;
        var param = new object[]
        {
            time.Year, time.Month, time.Day,
            new LocalizedTextParameter(GameFrameworkConfig.instance.appNameLocalizedKey)
        };

        txtMsg.SetKeyAndParameters(key, param);
    }

    private void Start()
    {
        btnOk.OnClickAsObservable()
            .Subscribe(_ => { Close(); });
    }

    private void Update()
    {
        if (StaticUtils.IsPressBackKey())
        {
            Close();
        }
    }

    private void Close()
    {
        Destroy(gameObject);
    }
}
