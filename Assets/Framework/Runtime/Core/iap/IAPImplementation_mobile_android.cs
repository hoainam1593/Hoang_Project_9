
using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Purchasing;

public class IAPImplementation_mobile_android: IAPImplementation_mobile
{
	private IGooglePlayStoreExtensions googlePlayExtension;

	public IAPImplementation_mobile_android(Dictionary<string, ProductType> dicProduct)
		: base(dicProduct)
	{

	}

	protected override void ConfigureBuilder(ConfigurationBuilder builder)
	{
		builder.Configure<IGooglePlayConfiguration>().SetDeferredPurchaseListener(product =>
		{
			//show a popup inform that this purchase is deferred
		});

		var pid = GameFrameworkConfig.instance.playPassPID;
		if (!string.IsNullOrEmpty(pid))
		{
			//add play pass product
			builder.AddProduct(pid, ProductType.Consumable);
		}
	}

	public override void OnInitialized(IStoreController controller, IExtensionProvider extensions)
	{
		base.OnInitialized(controller, extensions);

		googlePlayExtension = extensions.GetExtension<IGooglePlayStoreExtensions>();

		var pid = GameFrameworkConfig.instance.playPassPID;
		if (!string.IsNullOrEmpty(pid))
		{
			//check subscribe play pass
			var product = GetProduct(pid);
			var price = product.metadata.localizedPrice;
			IAPController.instance.IsPlayPassRx.Value = price == 0;

			Debug.Log($"[PlayPass] product {pid} has price={price}");
		}
	}

	protected override bool IsDeferredPurchase(Product product)
	{
		try
		{
			return googlePlayExtension.IsPurchasedProductDeferred(product);
		}
		catch (Exception e)
		{
			UserReportPurchaseException(e, product, "Fail to check purchase is deferred");
			throw e;
		}
	}

	protected override async UniTask<bool> ValidateReceipt(Product product)
	{
		string token;
        try
		{
			var googleReceipt = IAPReceiptGooglePlay.FromJson(product.receipt);
			token = googleReceipt.Payload.json.purchaseToken;

		}
		catch (Exception e)
		{
			UserReportPurchaseException(e, product, "Fail to parse receipt");
			throw e;
		}

		try
		{
			var pid = product.definition.id;
			var isSubscription = product.definition.type == ProductType.Subscription;
			var validateOK = await ServerController.instance.ValidateGooglePlayReceipt(pid, token, isSubscription);
			if (!validateOK)
			{
				throw new Exception("Validate receipt return FAIL");
			}

			return true;
		}
		catch (Exception e)
		{
			UserReportPurchaseException(e, product, "Fail to validate receipt");
			throw e;
		}
	}

	public override void RestorePurchase()
	{
	}
}