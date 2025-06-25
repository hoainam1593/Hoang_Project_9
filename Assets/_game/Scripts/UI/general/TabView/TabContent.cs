using UnityEngine;
using UnityEngine.UI;
using R3;

public class TabContent : MonoBehaviour
{
    [SerializeField] protected Transform content;

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
                OnShow();
            }
        });

        oldTabIndex.Subscribe((x) =>
        {
            if (x == index)
            {
                OnHide();
            }
        });
    }


    public void Show()
    {
        OnShow();
    }

    public void Hide()
    {
        OnHide();
    }

    protected virtual void OnShow()
    {
        // Debug.Log($"OnSelect Tab: {index}");
        content.gameObject.SetActive(true);
    }

    protected virtual void OnHide()
    {
        // Debug.Log($"UnSelect Tab: {index}");
        content.gameObject.SetActive(false);
    }
}
