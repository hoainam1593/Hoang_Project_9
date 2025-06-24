using System.Collections.Generic;
using UnityEngine;
using System;

public class Dispatcher<T> where T : new()
{
    private Dictionary<T, Action<object>> _callbacks = new Dictionary<T, Action<object>>();

    public void Register(T e, Action<object> callback)
    {
        if (!_callbacks.ContainsKey(e))
        {
            _callbacks.Add(e, callback);
        }
        else
        {
            _callbacks[e] += callback;
        }
    }

    public void UnRegister(T e, Action<object> callback)
    {
        if (!_callbacks.ContainsKey(e))
        {
            return;
        }
        _callbacks[e] -= callback;
    }

    public void DispatcherEvent(T e, object data)
    {
        if (!_callbacks.ContainsKey(e))
        {
            return;
        }
        _callbacks[e]?.Invoke(data);
    }
}