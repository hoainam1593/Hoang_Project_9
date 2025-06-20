
using UnityEngine;

public static class RemoteAssetsPath
{
    //never put this into a MonoBehaviour, because it 
    //will corrupt that MonoBehaviour on iOS.
    
#if UNITY_EDITOR
    public static string remoteAddressableRuntimePath = $"{Application.dataPath}/../RemoteAddressable";
#elif UNITY_STANDALONE
	public static string remoteAddressableRuntimePath = $"{Application.dataPath}/../../RemoteAddressable";
#else
	public static string remoteAddressableRuntimePath = $"{Application.persistentDataPath}/RemoteAddressable";
#endif
}
