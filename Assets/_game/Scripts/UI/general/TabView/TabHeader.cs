using UnityEngine;
using UnityEngine.UI;
using R3;
using System;
using System.Collections.Generic;

public class TabHeader : MonoBehaviour
{
    private ReactiveProperty<int> tabIndex;
    private ReactiveProperty<int> oldTabIndex;
    
    private int index;
    private Button button;
    private Action<int> callback;

    [SerializeField] private List<GameObject> _highlightElements;
    [SerializeField] private List<GameObject> _inactiveElements;
    

    private void Awake()
    {
        button = gameObject.GetComponent<Button>();
        
        button.onClick.AddListener(() =>
        {
            callback?.Invoke(index);
        });
    }

    public void Init(int index, Action<int> callback, ReactiveProperty<int> tabIndex, ReactiveProperty<int> oldTabIndex)
    {
        this.index = index;
        this.callback = callback;
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

    private void OnSelect()
    {
        Debug.Log($"OnSelect {index}");
        foreach (var highlightElement in _highlightElements)
        {
            highlightElement.SetActive(true);
        }        
        foreach (var inactive in _inactiveElements)
        {
            inactive.SetActive(false);
        }
    }

    private  void UnSelect()
    {
        Debug.Log($"UnSelect {index}");
        foreach (var highlightElement in _highlightElements)
        {
            highlightElement.SetActive(false);
        }        
        foreach (var inactive in _inactiveElements)
        {
            inactive.SetActive(true);
        }
    }
}
