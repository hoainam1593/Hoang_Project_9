using TMPro;
using UnityEngine;

public partial class TextNotify : MonoBehaviour
{
    public TextMeshProUGUI text;
    public LocalizedText_tmp localizedText;
    public RectTransform rectTransform;

    public void Setup(TextNotifyController.TextCfg textCfg, TextNotifyController.ColorCfg colorCfg,
        TextNotifyController.PositionCfg positionCfg, Camera mainCamera)
    {
        SetupText(textCfg);
        SetupColor(colorCfg);
        SetupPosition(positionCfg, mainCamera);
        Setup_anim();
    }

    private void SetupText(TextNotifyController.TextCfg textCfg)
    {
        localizedText.SetKeyAndParameters(textCfg.key, textCfg.lParams);
    }

    private void SetupColor(TextNotifyController.ColorCfg colorCfg)
    {
        localizedText.ChangeUnderlayColor(colorCfg.outline);
        if (colorCfg is TextNotifyController.SolidColorCfg solidColorCfg)
        {
            text.enableVertexGradient = false;
            text.color = solidColorCfg.color;
        }
        else if (colorCfg is TextNotifyController.GradientColorCfg gradientColorCfg)
        {
            text.enableVertexGradient = true;
            text.color = Color.white;
            text.colorGradient = new VertexGradient(gradientColorCfg.colorUp, gradientColorCfg.colorUp,
                gradientColorCfg.colorDown, gradientColorCfg.colorDown);
        }
    }

    private void SetupPosition(TextNotifyController.PositionCfg positionCfg, Camera mainCamera)
    {
        if (positionCfg is TextNotifyController.CustomPositionCfg customPositionCfg)
        {
            if (customPositionCfg.inWorldSpace)
            {
                rectTransform.position = mainCamera.WorldToScreenPoint(customPositionCfg.position);
            }
            else
            {
                transform.position = customPositionCfg.position;
            }
        }
        else
        {
            rectTransform.anchoredPosition = Vector2.zero;
        }
    }
}
