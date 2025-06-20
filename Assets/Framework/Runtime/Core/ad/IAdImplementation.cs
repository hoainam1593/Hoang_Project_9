
using UnityEngine.Events;

public interface IAdImplementation
{
	#region ad

	void ShowBanner();
	void HideBanner();

	bool ShowInterstitial(UnityAction callbackClose);

	bool ShowRewarded(UnityAction callbackReward, UnityAction callbackClose);

	#endregion

	#region UMP

	bool IsEu();
	void ShowUMPPopup_setting();

	#endregion

	#region log

	string AdNetworkNameForAdjustLogging { get; }
	string AdNetworkNameForFirebaseLogging { get; }

	#endregion
}