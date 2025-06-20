#if USE_APPLOVIN
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;

public partial class AdImplementation_applovin : IAdImplementation
{
	private UnityAction OnRewardedClose;
	private UnityAction OnRewardedGrant;

	private void LoadRewarded()
	{
		var unitId = ApplovinConfig.instance.adUnit_rewarded;
		if (string.IsNullOrEmpty(unitId))
		{
			return;
		}

		MaxSdkCallbacks.Rewarded.OnAdLoadFailedEvent -= OnRewardedAdLoadFailedEvent;
		MaxSdkCallbacks.Rewarded.OnAdHiddenEvent -= OnRewardedAdHiddenEvent;
		MaxSdkCallbacks.Rewarded.OnAdDisplayFailedEvent -= OnRewardedAdFailedToDisplayEvent;
		MaxSdkCallbacks.Rewarded.OnAdReceivedRewardEvent -= OnRewardedAdReceivedRewardEvent;

		MaxSdkCallbacks.Rewarded.OnAdLoadFailedEvent += OnRewardedAdLoadFailedEvent;
		MaxSdkCallbacks.Rewarded.OnAdHiddenEvent += OnRewardedAdHiddenEvent;
		MaxSdkCallbacks.Rewarded.OnAdDisplayFailedEvent += OnRewardedAdFailedToDisplayEvent;
		MaxSdkCallbacks.Rewarded.OnAdReceivedRewardEvent += OnRewardedAdReceivedRewardEvent;

		MaxSdk.LoadRewardedAd(unitId);
	}

	private void OnRewardedAdLoadFailedEvent(string adUnitId, MaxSdkBase.ErrorInfo errorInfo)
	{
		Debug.LogError($"[applovin] load rewarded fail, errorInfo={errorInfo}");
	}

	private void OnRewardedAdHiddenEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
	{
		OnRewardedClose?.Invoke();
		LoadRewarded();
	}

	private void OnRewardedAdFailedToDisplayEvent(string adUnitId, MaxSdkBase.ErrorInfo errorInfo, MaxSdkBase.AdInfo adInfo)
	{
		OnRewardedClose?.Invoke();
		LoadRewarded();
	}

	private void OnRewardedAdReceivedRewardEvent(string adUnitId, MaxSdk.Reward reward, MaxSdkBase.AdInfo adInfo)
	{
		OnRewardedGrant?.Invoke();
	}

	public void ShowRewarded(UnityAction callback, UnityAction callbackClose)
	{
		var unitId = ApplovinConfig.instance.adUnit_rewarded;
		if (!string.IsNullOrEmpty(unitId) && MaxSdk.IsRewardedAdReady(unitId))
		{
			OnRewardedClose = callbackClose;
			OnRewardedGrant = callback;
			MaxSdk.ShowRewardedAd(unitId);
		}
		else
		{
			PopupYesNoOk.ShowOk("text_dialog_msg_no_ads").Forget();
			callbackClose?.Invoke();
		}
	}
}
#endif