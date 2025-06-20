
using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Purchasing;

public class IAPImplementation_mobile_ios : IAPImplementation_mobile
{
	private IAppleExtensions appleExtension;

	public IAPImplementation_mobile_ios(Dictionary<string, ProductType> dicProduct)
		: base(dicProduct)
	{

	}

	protected override void ConfigureBuilder(ConfigurationBuilder builder)
	{
	}

	public override void OnInitialized(IStoreController controller, IExtensionProvider extensions)
	{
		base.OnInitialized(controller, extensions);

		appleExtension = extensions.GetExtension<IAppleExtensions>();
		appleExtension.RegisterPurchaseDeferredListener(product =>
		{
			//show a popup inform that this purchase is deferred
		});
	}

	protected override bool IsDeferredPurchase(Product product)
	{
		return false;
	}

	protected override async UniTask<bool> ValidateReceipt(Product product)
	{
		var pid = product.definition.id;
		var receipt = JsonConvert.DeserializeObject<IAPReceiptAppStore>(product.receipt);
		var isSubscription = product.definition.type == ProductType.Subscription;
		var validateOk = await ServerController.instance.ValidateAppStoreReceipt(pid, receipt.Payload, isSubscription);

		return validateOk;
	}

	public override void RestorePurchase()
	{
		appleExtension.RestoreTransactions((result, msg) =>
		{
			Debug.Log($"[IAP] restore iap with result={result} msg={msg}");
		});
	}
}