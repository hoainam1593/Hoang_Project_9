public class SystemNotificationController : SingletonMonoBehaviour<SystemNotificationController>
{
    private ISystemNotificationImplementation impl;
    private ISystemNotificationListener listener;
    
    public void Init(ISystemNotificationListener listener)
    {
        this.listener = listener;
        
#if UNITY_EDITOR || UNITY_STANDALONE_WIN || !USE_SYSTEM_NOTIFICATION
        impl = new SystemNotificationImplementation_editor();
#elif UNITY_ANDROID
		impl = new SystemNotificationImplementation_android();
#else
		impl = new SystemNotificationImplementation_ios();
#endif
    }

    private void OnApplicationPause(bool pause)
    {
        if (impl == null)
        {
            return;
        }
        
        if (pause)
        {
            listener.OnSendingAllNotifications();
        }
        else
        {
            impl.CancelAllNotifications();
        }
    }

    private void OnApplicationQuit()
    {
        if (impl == null)
        {
            return;
        }

        listener.OnSendingAllNotifications();
    }
}