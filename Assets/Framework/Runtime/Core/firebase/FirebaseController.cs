
#if USE_FIREBASE_ANALYTICS || USE_FIREBASE_AUTHENTICATION

using Firebase;
using Firebase.Extensions;
using UnityEngine;

public partial class FirebaseController : SingletonMonoBehaviour<FirebaseController>
{
	private bool isFirebaseAvailable = false;
	private IFirebaseListener listener;

	public void Init(IFirebaseListener listener)
	{
		this.listener = listener;

		if (isFirebaseAvailable)
		{
			return;
		}
		
		FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task =>
		{
			if (task.Result == DependencyStatus.Available)
			{
				Debug.Log($"[firebase] appId={FirebaseApp.DefaultInstance.Options.AppId}");

				isFirebaseAvailable = true;
			}
			else
			{
				Debug.LogError($"[firebase] resolve dependency fail, status={task.Result}");
			}
		});
	}
}

#endif