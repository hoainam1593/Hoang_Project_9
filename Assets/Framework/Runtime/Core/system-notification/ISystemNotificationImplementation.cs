
using System;

public interface ISystemNotificationImplementation
{
	void SendNotification(string title, string text, TimeSpan trigger);
	void CancelAllNotifications();
}