

using UnityEditor.Rendering;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using Cysharp.Threading.Tasks;
using UnityEngine.Serialization;

public enum UI
{
    TurretSelectUI,    
}

public enum Layer
{
    Popup,
}

public class UIManager : SingletonMonoBehaviour<UIManager>
{
    private const float TargetScreenWidth = 1920f;
    private const float TargetScreenHeight = 1080f;
    private const string UIPrefabPath = "Assets/_game/AssetResources/Prefab/UI";
    private Vector2 popupPivot;
        
    [SerializeField] private Canvas layerPopup;
    private RectTransform layerPopupRect;

    private SerializedDictionary<UI, string> uiNames = new SerializedDictionary<UI, string>()
    {
        { UI.TurretSelectUI, "TurretSelectUI" },
    };

    private Dictionary<UI, GameObject> activePopups = new Dictionary<UI, GameObject>();
    private Dictionary<UI, GameObject> inActivePopups = new Dictionary<UI, GameObject>();

    protected override void Awake()
    {
        base.Awake();
        layerPopupRect = layerPopup.GetComponent<RectTransform>();
        popupPivot = layerPopupRect.pivot;
    }

    public async UniTaskVoid ShowPopup(UI ui, Vector3 screenPoint)
    {
        if (activePopups.ContainsKey(ui)) return;
        if (inActivePopups.ContainsKey(ui))
        {
            ActivePopups(ui, screenPoint);
        }

        CreateNewPopup(ui, screenPoint).Forget();
    }

    private void ActivePopups(UI ui, Vector3 screenPoint)
    {
        if (!inActivePopups.ContainsKey(ui)) return;
        
        var uiPopup = inActivePopups[ui];
        inActivePopups.Remove(ui);
        activePopups.Add(ui, uiPopup);
        
        SetupPopup(uiPopup, screenPoint);
        uiPopup.SetActive(true);
    }

    private void SetupPopup(GameObject uiInstance, Vector3 screenPoint)
    {
        var uiRect = uiInstance.GetComponent<RectTransform>();
        // uiInstance.transform.localPosition = ConvertToLocalPoint(Layer.Popup, screenPoint);
        uiRect.anchoredPosition = screenPoint - new Vector3(TargetScreenWidth * popupPivot.x, TargetScreenHeight * popupPivot.y, 0);
        uiRect.anchoredPosition += 
        //Note: Temporary to hard set ui size.
        uiRect.sizeDelta = new Vector2(256, 256);
    }

    private async UniTaskVoid CreateNewPopup(UI ui, Vector3 screenPoint)
    {
        var uiName = GetUIPrefabName(ui);
        if (uiName == null) return;

        GameObject prefab = await LoadUIPrefab(uiName);
        if (object.ReferenceEquals(null, prefab)) return;
        var uiPopup = GameObject.Instantiate(prefab, Vector3.zero, Quaternion.identity, layerPopup.transform);
        SetupPopup(uiPopup, screenPoint);
    }
    
    private string GetUIPrefabName(UI ui)
    {
        if (!uiNames.ContainsKey(ui))
        {
            return null;
        }
        return uiNames[ui];
    }

    private async UniTask<GameObject> LoadUIPrefab(string uiName)
    {
        var prefab = await AssetManager.instance.LoadPrefab(UIPrefabPath, uiName);
        if (object.ReferenceEquals(null, prefab)) return null;
        return prefab;
    }
}
