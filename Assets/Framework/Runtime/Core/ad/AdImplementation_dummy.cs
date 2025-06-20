
using UnityEngine.Events;

public class AdImplementation_dummy : IAdImplementation
{
	#region ad

	public void ShowBanner()
	{
	}
	
	public void HideBanner()
	{
	}
	
	public bool ShowInterstitial(UnityAction callbackClose)
	{
		callbackClose?.Invoke();
		return true;
	}

	public bool ShowRewarded(UnityAction callbackReward, UnityAction callbackClose)
	{
		callbackReward?.Invoke();
		callbackClose?.Invoke();
		return true;
	}

	#endregion

	#region UMP

	public bool IsEu()
	{
		return false;
	}
	
	public void ShowUMPPopup_setting()
	{
	}

	#endregion

	#region log

	public string AdNetworkNameForAdjustLogging => "dummy";
	public string AdNetworkNameForFirebaseLogging => "dummy";

	#endregion
}