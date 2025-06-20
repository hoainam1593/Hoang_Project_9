#if USE_APPLOVIN

using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;

public partial class AdImplementation_applovin : IAdImplementation
{
	private UnityAction OnInterstitialClose;

	private void LoadInterstitial()
	{
		var unitId = ApplovinConfig.instance.adUnit_interstitial;
		if (string.IsNullOrEmpty(unitId))
		{
			return;
		}

		MaxSdkCallbacks.Interstitial.OnAdLoadFailedEvent -= OnInterstitialLoadFailedEvent;
		MaxSdkCallbacks.Interstitial.OnAdHiddenEvent -= OnInterstitialHiddenEvent;
		MaxSdkCallbacks.Interstitial.OnAdDisplayFailedEvent -= OnInterstitialAdFailedToDisplayEvent;

		MaxSdkCallbacks.Interstitial.OnAdLoadFailedEvent += OnInterstitialLoadFailedEvent;
		MaxSdkCallbacks.Interstitial.OnAdHiddenEvent += OnInterstitialHiddenEvent;
		MaxSdkCallbacks.Interstitial.OnAdDisplayFailedEvent += OnInterstitialAdFailedToDisplayEvent;

		MaxSdk.LoadInterstitial(unitId);
	}

	private void OnInterstitialLoadFailedEvent(string adUnitId, MaxSdkBase.ErrorInfo errorInfo)
	{
		Debug.LogError($"[applovin] load interstitial fail, errorInfo={errorInfo}");
	}

	private void OnInterstitialHiddenEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
	{
		OnInterstitialClose?.Invoke();
		LoadInterstitial();
	}

	private void OnInterstitialAdFailedToDisplayEvent(string adUnitId, MaxSdkBase.ErrorInfo errorInfo, MaxSdkBase.AdInfo adInfo)
	{
		OnInterstitialClose?.Invoke();
		LoadInterstitial();
	}

	public void ShowInterstitial(UnityAction callbackClose)
	{
		var unitId = ApplovinConfig.instance.adUnit_interstitial;
		if (!string.IsNullOrEmpty(unitId) && MaxSdk.IsInterstitialReady(unitId))
		{
			OnInterstitialClose = callbackClose;
			MaxSdk.ShowInterstitial(unitId);
		}
		else
		{
			PopupYesNoOk.ShowOk("text_dialog_msg_no_ads").Forget();
			callbackClose?.Invoke();
		}
	}
}

#endif