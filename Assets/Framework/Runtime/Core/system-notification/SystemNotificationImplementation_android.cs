#if (UNITY_EDITOR || UNITY_ANDROID) && USE_SYSTEM_NOTIFICATION
using System;
using Unity.Notifications.Android;
using UnityEngine.Android;

public class SystemNotificationImplementation_android : ISystemNotificationImplementation
{
	const string channelId = "default_channel";
	const string permission = "android.permission.POST_NOTIFICATIONS";

	public SystemNotificationImplementation_android()
	{
		var channel = new AndroidNotificationChannel(channelId, "default name", "default description", Importance.Default);
		AndroidNotificationCenter.RegisterNotificationChannel(channel);

		if (!Permission.HasUserAuthorizedPermission(permission))
		{
			Permission.RequestUserPermission(permission);
		}
	}

	public void SendNotification(string title, string text, TimeSpan trigger)
	{
		var notification = new AndroidNotification()
		{
			Title = title,
			Text = text,
			FireTime = DateTime.Now.Add(trigger),
			SmallIcon = "icon_0",
			Style = NotificationStyle.BigTextStyle,
		};
		AndroidNotificationCenter.SendNotification(notification, channelId);
	}

	public void CancelAllNotifications()
	{
		AndroidNotificationCenter.CancelAllNotifications();
	}
}
#endif