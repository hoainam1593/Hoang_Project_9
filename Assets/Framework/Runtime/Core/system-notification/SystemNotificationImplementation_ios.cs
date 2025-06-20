#if (UNITY_EDITOR || UNITY_IOS) && USE_SYSTEM_NOTIFICATION
using System;
using Unity.Notifications.iOS;

public class SystemNotificationImplementation_ios : ISystemNotificationImplementation
{
	public void SendNotification(string title, string text, TimeSpan trigger)
	{
		var triggerProp = new iOSNotificationTimeIntervalTrigger()
		{
			TimeInterval = trigger,
			Repeats = false,
		};
		var notification = new iOSNotification()
		{
			Title = title,
			Body = text,
			Trigger = triggerProp,
			ForegroundPresentationOption = PresentationOption.Alert | PresentationOption.Sound,
			ShowInForeground = true,
		};
		iOSNotificationCenter.ScheduleNotification(notification);
	}

	public void CancelAllNotifications()
	{
		iOSNotificationCenter.RemoveAllDeliveredNotifications();
		iOSNotificationCenter.RemoveAllScheduledNotifications();
	}
}
#endif