using UnityEngine;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;


public interface IPanel
{
    public bool IsOpen { get; set; }
    public void Init(GameObject go);
    public void Open(object data);
    public void Close();
}

public class PanelBase : MonoBehaviour, IPanel
{
    public bool IsOpen { get; set; }
    public void Init(GameObject go)
    {
    }
    public void Open(object data)
    {
    }

    public void Close()
    {
    }
}