
using TMPro;

public class TestListItem: RecycleScrollViewItem
{
    public TextMeshProUGUI txtIndex;
    
    public override void SetData(object data)
    {
        var intData = (int)data;
        txtIndex.text = intData.ToString();
    }
}
