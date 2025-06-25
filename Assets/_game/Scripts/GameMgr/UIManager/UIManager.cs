using UnityEngine;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;

public enum UIPanel
{
    MainUIPanel,
    BattleUIPanel,
}

public interface IUIManager
{
    public UniTask<IPanel> OpenPanel<T>(UIPanel uiPanel, object data = null) where T : PanelBase, new();
    public void ClosePanel(UIPanel uiPanel);
}


public class UIManager : SingletonMonoBehaviour<UIManager>, IUIManager
{
    public const string UIPanelPath = "Assets/_game/AssetResources/Prefab/UI/Panel";

    [SerializeField] private Canvas MainCanvas;
    private Transform layerMain;

    protected override void Awake()
    {
        base.Awake();
        layerMain =  MainCanvas.transform;
    }
    
    private readonly Dictionary<UIPanel, PanelBase> panels = new Dictionary<UIPanel, PanelBase>();

    public async UniTask<IPanel> OpenPanel<T>(UIPanel uiPanel, object data = null) where T : PanelBase, new()
    {
        if (panels.ContainsKey(uiPanel))
        {
            if (!panels[uiPanel].IsOpen)
            {
                panels[uiPanel].Open(data);
            }
            
            return panels[uiPanel];
        }
        
        return await CreateNewPanel<T>(uiPanel, layerMain, data);
    }

    private async UniTask<PanelBase> CreateNewPanel<T>(UIPanel uiPanel, Transform parent, object data) where T : PanelBase, new()
    {
        var uiName = typeof(T).Name;
        var uiPrefab = await AssetManager.instance.LoadPrefab(UIPanelPath, uiName);
        var go = GameObject.Instantiate(uiPrefab, Vector3.zero, Quaternion.identity, parent);
        var uiRect = go.GetComponent<RectTransform>();
        uiRect.anchoredPosition = Vector2.zero;
        
        var panelCtrl = go.GetOrAddComponent<T>();
        panelCtrl.Init(go);
        panelCtrl.Open(data);
        panels.Add(uiPanel, panelCtrl);
        return panelCtrl;
    }

    public void ClosePanel(UIPanel uiPanel)
    {
        if (panels.TryGetValue(uiPanel, out var panelCtrl))
        {
            Debug.Log($"ClosePanel {uiPanel} - isOpen: {panelCtrl.IsOpen}");
            if (panelCtrl.IsOpen)
            {
                panelCtrl.Close();
            }
        };
    }
}