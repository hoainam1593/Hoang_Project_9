using UnityEngine;
using UnityEngine.UI;
using R3;

public class TabContent : MonoBehaviour
{
    [SerializeField] private Transform content;

    private int index;
    private ReactiveProperty<int> tabIndex;
    private ReactiveProperty<int> oldTabIndex;

    public void Init(int index, ReactiveProperty<int> tabIndex, ReactiveProperty<int> oldTabIndex)
    {
        this.index = index;
        this.tabIndex = tabIndex;
        this.oldTabIndex = oldTabIndex;

        tabIndex.Subscribe((x) =>
        {
            if (x == index)
            {
                OnSelect();
            }
        });

        oldTabIndex.Subscribe((x) =>
        {
            if (x == index)
            {
                UnSelect();
            }
        });
    }

    public void Hide()
    {
        content.gameObject.SetActive(false);
    }

    private void OnSelect()
    {
        Debug.Log($"OnSelect Tab: {index}");
        content.gameObject.SetActive(true);
    }

    private  void UnSelect()
    {
        Debug.Log($"UnSelect Tab: {index}");
        content.gameObject.SetActive(false);
    }
}
