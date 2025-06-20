
using System;
using Cysharp.Threading.Tasks;
using R3;
using UnityEngine;

public class TestNotify:MonoBehaviour
{
    private void Start()
    {
        var rx = new ReactiveProperty<SystemLanguage>(SystemLanguage.English);
        
        LocalizationController.instance.LoadLocalizationData(rx).Forget();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            TextNotifyController.instance.NotifyText("setting_notice",
                TextNotifyController.ColorCfg.SolidColor(Color.cyan, Color.blue)).Forget();
        }
        
        if (Input.GetKeyDown(KeyCode.A))
        {
            TextNotifyController.instance.NotifyText("setting_notice",
                TextNotifyController.ColorCfg.GradientColor(Color.magenta, 
                    Color.blue, Color.yellow)).Forget();
        }
    }
}
