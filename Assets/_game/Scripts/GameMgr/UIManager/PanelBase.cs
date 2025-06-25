using UnityEngine;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEditor.Callbacks;


public interface IPanel
{
    public bool IsOpen { get; set; }
    public void Init(GameObject go);
    public void Open(object data);
    public void Close();
}

public class PanelBase : MonoBehaviour, IPanel
{
    protected Transform transform;
    protected GameObject gameObject;
    
    public bool IsOpen { get; set; }
    public void Init(GameObject go)
    {
        this.gameObject = go;
        this.transform = go.transform;
    }
    
    public void Open(object data)
    {
        IsOpen = true;
        gameObject.SetActive(true);
        
        OnOpen(data);
    }

    public void Close()
    {
        IsOpen = false;
        gameObject.SetActive(false);
        
        OnClose();
    }

    protected virtual void OnOpen(object data)
    {
        
    }

    protected virtual void OnClose()
    {
        
    }
}