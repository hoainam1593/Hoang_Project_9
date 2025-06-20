
#if USE_FIREBASE_ANALYTICS

using System.Collections.Generic;
using Firebase.Analytics;

public partial class FirebaseController
{
	public void LogEvent(string eventName)
	{
		if (!isFirebaseAvailable)
		{
			return;
		}

		listener.OnUpdatingUserProperties();
		FirebaseAnalytics.LogEvent(eventName);
	}

	public void LogEvent(string eventName, params Parameter[] parameters)
	{
		if (!isFirebaseAvailable)
		{
			return;
		}

		listener.OnUpdatingUserProperties();
		FirebaseAnalytics.LogEvent(eventName, parameters);
	}

	public void UpdateProperties(Dictionary<string, string> properties)
	{
		foreach (var i in properties)
		{
			FirebaseAnalytics.SetUserProperty(i.Key, i.Value);
		}
	}
}

#endif