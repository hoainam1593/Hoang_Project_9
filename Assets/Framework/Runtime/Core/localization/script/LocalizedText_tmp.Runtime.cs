
using R3;
using UnityEngine;

public partial class LocalizedText_tmp
{
    private bool isInitializedFont;
    
    private void StartRuntime()
    {
        LocalizationController.instance.languageRx
            .Subscribe(_ =>
            {
                tmp.font = LocalizationController.instance.fontAsset;
                LocalizationController.instance.SetupLocalizedText(this);
                LocalizationController.instance.SetUnderlayColorForTMP(tmp, underlayColor);

                isInitializedFont = true;
            })
            .AddTo(this);
    }

    public void ChangeUnderlayColor(Color color)
    {
        underlayColor = color;
        if (isInitializedFont)
        {
            //re-use font have error here,
            //need to check later
            tmp.fontMaterial.SetColor("_UnderlayColor", underlayColor);
        }
    }
}
