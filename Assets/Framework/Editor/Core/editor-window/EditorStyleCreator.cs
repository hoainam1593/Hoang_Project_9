
using UnityEngine;

public class EditorStyleCreator
{
    public static GUIStyle StyleGroupBackground(Color color)
    {
        return new GUIStyle()
        {
            normal = new GUIStyleState()
            {
                background = StaticUtils.CreateColorTexture(color)
            }
        };
    }

    public static GUIStyle StyleBackgroundLabel(Color color)
    {
        var newSkin = new GUIStyle(GUI.skin.label);
        newSkin.normal.background = StaticUtils.CreateColorTexture(color);
        return newSkin;
    }

    public static GUIStyle StyleWordwrapLabel()
    {
        return new GUIStyle(GUI.skin.label)
        {
            wordWrap = true,
        };
    }

    public static GUIStyle StyleAlignRightLabel()
    {
        return new GUIStyle(GUI.skin.label)
        {
            alignment = TextAnchor.MiddleRight
        };
    }

    public static GUIStyle StyleAlignRightButton()
    {
        return new GUIStyle(GUI.skin.button)
        {
            alignment = TextAnchor.MiddleRight
        };
    }

    public static GUIStyle StyleCenterLabel(float parentWidth, float parentHeight,
        int fontSz = -1, Color? color = null, Font font = null)
    {
        var style = new GUIStyle(GUI.skin.label)
        {
            alignment = TextAnchor.MiddleCenter,
            fixedWidth = parentWidth,
            fixedHeight = parentHeight,
        };
        if (fontSz > 0)
        {
            style.fontSize = fontSz;
        }
        if (color != null)
        {
            style.normal.textColor = color.Value;
        }
        if (font != null)
        {
            style.font = font;
        }
        return style;
    }
}
