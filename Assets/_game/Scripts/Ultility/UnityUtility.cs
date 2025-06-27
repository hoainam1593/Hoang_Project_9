using UnityEngine;

public static partial class UnityUtility
{
    public static T GetOrAddComponent<T>(this GameObject go) where T : Component
    {
        var comp =  go.GetComponent<T>();
        if (comp != null)
        {
            return comp;
        }
        return go.AddComponent<T>();
    }

    public static T GetOrAddComponent<T>(this Transform transform) where T : Component
    {
        return transform.gameObject.GetOrAddComponent<T>();
    }
        
}