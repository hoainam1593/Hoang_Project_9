
using System;
using UnityEngine;

public class UtcTime : SingletonMonoBehaviour<UtcTime>
{
    public DateTime UtcNow
    {
        get
        {
            //check server time is available
            
            //fallback
            return UnbiasedTime.Instance.Now().ToUniversalTime();
        }
    }
    
    private void Start()
    {
        Debug.LogError("[SERVER TIME] need to get time from server, plugin UnbiasedTime is not reliable");
    }
}
