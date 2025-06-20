using UnityEngine;
using UnityEngine.UI;

public partial class SplashScreenController : MonoBehaviour
{
    [Header("others")]
    public Text txtVersion;
    public GameObject objServerEnvironment;
    public Image imgBackground;

    private void Start()
    {
        txtVersion.text = $"v{Application.version}";
        StretchBackground();
        
        Start_progress();
    }

    private void Update()
    {
        Update_progress();
    }
    
    private void StretchBackground()
    {
        var rect = imgBackground.GetComponent<RectTransform>();
        var texW = imgBackground.sprite.texture.width;
        var texH = imgBackground.sprite.texture.height;

        var ratioScreen = (float)Screen.width / Screen.height;
        var ratioTex = (float)texW / texH;

        Vector2 sz;

        if (ratioTex > ratioScreen)
        {
            sz = new Vector2(ratioTex * Screen.height, Screen.height);
        }
        else
        {
            sz = new Vector2(Screen.width, Screen.width / ratioTex);
        }

        var scale = imgBackground.transform.lossyScale;
        rect.sizeDelta = new Vector2(sz.x / scale.x, sz.y / scale.y);
    }
}
