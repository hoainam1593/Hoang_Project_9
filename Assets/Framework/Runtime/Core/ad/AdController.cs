
using UnityEngine;
using UnityEngine.Events;

public class AdController : SingletonMonoBehaviour<AdController>
{
	#region core

	[SerializeField] GameObject coverObj;

	private IAdImplementation impl = null;

	public void Initialize()
	{
		if (impl != null)
		{
			return;
		}

#if UNITY_EDITOR || UNITY_STANDALONE
		impl = new AdImplementation_dummy();
#elif USE_ADMOB
		impl = new AdImplementation_admob();
#elif USE_APPLOVIN
        impl = new AdImplementation_applovin();
#endif
	}

	#endregion

	#region ad

	public void ShowBanner()
	{
		impl.ShowBanner();
	}

	public void HideBanner()
	{
		impl.HideBanner();
	}

	public bool ShowInterstitial()
	{
		coverObj.SetActive(true);
		return impl.ShowInterstitial(callbackClose: () => { coverObj.SetActive(false); });
	}

	public bool ShowRewarded(UnityAction callbackReward)
	{
		coverObj.SetActive(true);
		return impl.ShowRewarded(callbackReward, callbackClose: () => { coverObj.SetActive(false); });
	}

	#endregion

	#region UMP

	public bool IsEu()
	{
		return impl.IsEu();
	}

	public void ShowUMPPopup_setting()
	{
		impl.ShowUMPPopup_setting();
	}

	#endregion

	#region log

	public string AdNetworkNameForAdjustLogging => impl.AdNetworkNameForAdjustLogging;
	public string AdNetworkNameForFirebaseLogging => impl.AdNetworkNameForFirebaseLogging;

	#endregion
}