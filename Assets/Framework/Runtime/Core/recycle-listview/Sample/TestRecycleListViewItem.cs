
using TMPro;

public class TestRecycleListViewItem:RecycleScrollViewItem
{
    public TextMeshProUGUI text;
    public override void SetData(object data)
    {
        var dataAsInt = (int)data;
        text.text = dataAsInt.ToString();
    }
}