#if USE_APPLOVIN

using com.adjust.sdk;
using Newtonsoft.Json;
using UnityEngine;

public partial class AdImplementation_applovin : IAdImplementation
{
	public AdImplementation_applovin()
	{
		MaxSdkCallbacks.Banner.OnAdRevenuePaidEvent += OnAdRevenuePaidEvent;
		MaxSdkCallbacks.Interstitial.OnAdRevenuePaidEvent += OnAdRevenuePaidEvent;
		MaxSdkCallbacks.Rewarded.OnAdRevenuePaidEvent += OnAdRevenuePaidEvent;

		MaxSdkCallbacks.OnSdkInitializedEvent += configuration =>
		{
			Debug.Log($"[applovin] done initialize configuration={JsonConvert.SerializeObject(configuration)}");

			LoadInterstitial();
			LoadRewarded();
		};
		MaxSdk.InitializeSdk();
	}

	private void OnAdRevenuePaidEvent(string adUnitId, MaxSdkBase.AdInfo info)
	{
		FirebaseController.instance.LogAdRevenue("AppLovin",
			info.NetworkName, info.AdUnitIdentifier, info.AdFormat, info.Revenue);

		AdjustController.instance.LogAdRevenue(AdjustConfig.AdjustAdRevenueSourceAppLovinMAX,
			info.Revenue, info.NetworkName, info.AdUnitIdentifier, info.Placement);
	}
}

#endif