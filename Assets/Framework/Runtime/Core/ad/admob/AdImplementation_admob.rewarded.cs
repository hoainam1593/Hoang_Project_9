
#if USE_ADMOB

using GoogleMobileAds.Api;
using UnityEngine;
using UnityEngine.Events;

public partial class AdImplementation_admob
{
	private RewardedAd rewardedAd;
	private UnityAction onRewardedAdClosed;

	private void LoadRewarded()
	{
		var unitId = GetAdUnitID_rewardedVideo();
		if (string.IsNullOrEmpty(unitId))
		{
			return;
		}

		if (rewardedAd != null)
		{
			rewardedAd.Destroy();
			rewardedAd = null;
		}

		RewardedAd.Load(unitId, new AdRequest(), (ad, error) =>
		{
			if (error != null || ad == null)
			{
				Debug.LogError($"[admob] load rewarded fail: {error}");
			}
			else
			{
				rewardedAd = ad;

				ad.OnAdFullScreenContentClosed += () =>
				{
					onRewardedAdClosed?.Invoke();
					LoadRewarded();
				};

				ad.OnAdFullScreenContentFailed += _ =>
				{
					onRewardedAdClosed?.Invoke();
					LoadRewarded();
				};
			}
		});
	}

	public bool ShowRewarded(UnityAction callbackReward, UnityAction callbackClose)
	{
		if (rewardedAd != null && rewardedAd.CanShowAd())
		{
			onRewardedAdClosed = callbackClose;
			rewardedAd.Show(_ =>
			{
				callbackReward?.Invoke();
			});

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