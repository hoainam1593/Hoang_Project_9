
using System;
using Cysharp.Threading.Tasks;
using R3;
using UnityEngine;

public class TestChangeOutline:MonoBehaviour
{
    public LocalizedText_tmp localizedText;

    public Color color1;
    public Color color2;

    public GameObject textParent;

    private async UniTask Start()
    {
        var rx = new ReactiveProperty<SystemLanguage>(SystemLanguage.English);

        await LocalizationController.instance.LoadLocalizationData(rx);

        textParent.SetActive(true);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            localizedText.ChangeUnderlayColor(color1);
        }
        
        if (Input.GetKeyDown(KeyCode.A))
        {
            localizedText.ChangeUnderlayColor(color2);
        }
    }
}
