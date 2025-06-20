
using UnityEngine;
#if UNITY_EDITOR || UNITY_IOS
using System.Runtime.InteropServices;
#endif

public static partial class StaticUtils
{
    #region open store app

    public static void OpenStoreApp()
    {
#if UNITY_EDITOR || UNITY_STANDALONE_WIN
        OpenStoreApp_dummy();
#elif UNITY_ANDROID
		OpenStoreApp_android();
#elif UNITY_IOS
		OpenStoreApp_ios();
#endif
    }

    private static void OpenStoreApp_dummy()
    {
    }
    
    private static void OpenStoreApp_android()
    {
        var uriStr = $"market://details?id={Application.identifier}";
        using (var unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
        using (var currentActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity"))
        using (var uriClass = new AndroidJavaClass("android.net.Uri"))
        using (var uri = uriClass.CallStatic<AndroidJavaObject>("parse", uriStr))
        using (var intentClass = new AndroidJavaClass("android.content.Intent"))
        {
            var ACTION_VIEW = intentClass.GetStatic<string>("ACTION_VIEW");
            using (var intent = new AndroidJavaObject("android.content.Intent", ACTION_VIEW, uri))
            {
                currentActivity.Call("startActivity", intent);
            }
        }
    }
    
#if UNITY_EDITOR || UNITY_IOS
    
    [DllImport("__Internal")]
    private static extern void _IosStaticUtils_OpenStorePage(string appId);
    
    private static void OpenStoreApp_ios()
    {
        var appId = GameFrameworkConfig.instance.iosAppId;
        _IosStaticUtils_OpenStorePage(appId);
    }
    
#endif

    #endregion
}
