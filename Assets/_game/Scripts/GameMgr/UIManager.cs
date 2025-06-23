

using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.Rendering;

public enum UI
{
    TurretSelectorUI,    
}

public class UIManager : SingletonMonoBehaviour<UIManager>
{
    public SerializedDictionary<UI, string> uiNames =  new SerializedDictionary<UI, string>()
    {
        {UI.TurretSelectorUI, "TurretSelectorUI"},
    };
    
    public void ShowPopup(UI ui, Vector3 viewPos)
    {
        
    }
}
