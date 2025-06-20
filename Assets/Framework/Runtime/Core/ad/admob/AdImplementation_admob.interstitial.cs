
#if USE_ADMOB

using GoogleMobileAds.Api;
using UnityEngine;
using UnityEngine.Events;

public partial class AdImplementation_admob
{
	private InterstitialAd interstitialAd;
	private UnityAction onInterstitialAdClosed;

	private void LoadInterstitial()
	{
		var unitId = GetAdUnitID_interstitial();
		if (string.IsNullOrEmpty(unitId))
		{
			return;
		}

		if (interstitialAd != null)
		{
			interstitialAd.Destroy();
			interstitialAd = null;
		}

		InterstitialAd.Load(unitId, new AdRequest(), (ad, error) =>
		{
			if (error != null || ad == null)
			{
				Debug.LogError($"[admob] load interstitial fail: {error}");
			}
			else
			{
				interstitialAd = ad;

				ad.OnAdFullScreenContentClosed += () =>
				{
					onInterstitialAdClosed?.Invoke();
					LoadInterstitial();
				};
				ad.OnAdFullScreenContentFailed += _ =>
				{
					onInterstitialAdClosed?.Invoke();
					LoadInterstitial();
				};
			}
		});
	}

	public bool ShowInterstitial(UnityAction callbackClose)
	{
		if (interstitialAd != null && interstitialAd.CanShowAd())
		{
			onInterstitialAdClosed = callbackClose;
			interstitialAd.Show();
			
			return true;
		}
		else
		{
			callbackClose?.Invoke();
			return false;
		}
	}
}

#endif