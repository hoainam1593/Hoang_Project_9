#if USE_APPLOVIN
using Newtonsoft.Json;
using UnityEngine;

public partial class AdImplementation_applovin : IAdImplementation
{
	public bool IsEu()
	{
		return MaxSdk.GetSdkConfiguration().ConsentFlowUserGeography == MaxSdkBase.ConsentFlowUserGeography.Gdpr;
	}

	public void ShowUMPPopup_setting()
	{
		MaxSdk.CmpService.ShowCmpForExistingUser(error =>
		{
			if (error != null)
			{
				Debug.LogError($"[UMP] show CMP popup fail error={JsonConvert.SerializeObject(error)}");
			}
		});
	}
}
#endif