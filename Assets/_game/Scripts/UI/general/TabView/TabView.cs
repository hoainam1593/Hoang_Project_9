using UnityEngine;
using UnityEngine.UI;
using R3;
using System.Collections.Generic;

public class TabView : MonoBehaviour
{
    private int tabCount;
    public const int DefaultTab = 0;
    
    [SerializeField] List<TabHeader> tabHeaders;
    [SerializeField] List<TabContent> tabContents;
    
    private ReactiveProperty<int> tabIndex = new ReactiveProperty<int>(-1);
    private ReactiveProperty<int> oldTabIndex = new ReactiveProperty<int>(-1);
    
    protected virtual void Awake()
    {
        tabCount = Mathf.Min(tabContents.Count, tabHeaders.Count);
        for (int i = 0; i < tabCount; i++)
        {
            tabHeaders[i].Init(i, SwitchTab, tabIndex, oldTabIndex);
            tabContents[i].Init(i, tabIndex, oldTabIndex);
        }
        
        hideAllTabs();
        SwitchTab(DefaultTab);
    }

    public void SwitchTab(int tab)
    {
        if (tab < 0 || tab >= tabCount)
        {
            return;
        }

        if (tab != tabIndex.Value)
        {
            Debug.Log($"Switch tab to: {tab}");
            oldTabIndex.Value = tabIndex.Value;
            tabIndex.Value = tab;
        }
    }

    private void hideAllTabs()
    {
        foreach (var tab in tabContents)
        {
            tab.Hide();
        }
    }
}
