
using System.Collections.Generic;

public class ImageToolStateDefault:EditorWindowState_tab
{
    public ImageToolStateDefault()
    :base(new List<EditorUIElement_tab.TabItem>()
    {
        new ImageToolTab_resize(),
        new ImageToolTab_batchResize(),
        new ImageToolTab_crop()
    })
    {
    }
}
