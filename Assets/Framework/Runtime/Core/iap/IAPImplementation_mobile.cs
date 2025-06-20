
using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Purchasing;
using UnityEngine.Purchasing.Extension;

public abstract class IAPImplementation_mobile : IIAPImplementation, IDetailedStoreListener
{
    #region core

    private IStoreController storeController;
    private List<string> subscriptionProducts = new();
    private List<string> consumableProducts = new();

	public IAPImplementation_mobile(Dictionary<string, ProductType> dicProduct)
    {
        var builder = ConfigurationBuilder.Instance(StandardPurchasingModule.Instance());
        ConfigureBuilder(builder);

        foreach (var i in dicProduct)
        {
            builder.AddProduct(i.Key, i.Value);
            if (i.Value == ProductType.Subscription)
            {
                subscriptionProducts.Add(i.Key);
            }
            else if (i.Value == ProductType.Consumable)
            {
                consumableProducts.Add(i.Key);
            }
        }

        UnityPurchasing.Initialize(this, builder);
    }

    protected abstract void ConfigureBuilder(ConfigurationBuilder builder);
    protected abstract bool IsDeferredPurchase(Product product);

    #endregion

    #region IDetailedStoreListener init

    public virtual void OnInitialized(IStoreController controller, IExtensionProvider extensions)
    {
        storeController = controller;
        QuerySubscriptionInfo();

        //get currency code
        var product = GetProduct(consumableProducts[0]);
        IAPController.instance.CurrencyCode = product.metadata.isoCurrencyCode;
    }

    public void OnInitializeFailed(InitializationFailureReason error)
    {
        Debug.LogError($"[IAP] initialize fail error={error}");
    }

    public void OnInitializeFailed(InitializationFailureReason error, string message)
    {
        Debug.LogError($"[IAP] initialize fail error={error} message={message}");
    }

    #endregion

    #region IDetailedStoreListener purchase

    public void OnPurchaseFailed(Product product, PurchaseFailureDescription failureDescription)
    {
        var fullMsg = $"[IAP] purchase {product.definition.id} fail error={failureDescription.reason}";
        if (!string.IsNullOrEmpty(failureDescription.message))
        {
            fullMsg += $" message={failureDescription.message}";
        }
        Debug.LogError(fullMsg);
        IAPController.instance.listener.OnPurchaseFailed();
    }

    public void OnPurchaseFailed(Product product, PurchaseFailureReason failureReason)
    {
        OnPurchaseFailed(product, new PurchaseFailureDescription(product.definition.id, failureReason, null));
    }

    public PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs purchaseEvent)
    {
        var product = purchaseEvent.purchasedProduct;
        Debug.Log($"[IAP] purchase success product={product.definition.id}");
        if (!IsDeferredPurchase(product))
        {
            ProcessPurchaseAsync(product).Forget();
        }
        return PurchaseProcessingResult.Pending;
    }

    private async UniTask ProcessPurchaseAsync(Product product)
    {
        var validateResult = await ValidateReceipt(product);
        if (validateResult)
        {
            var productId = product.definition.id;

            try
            {
                IAPController.instance.listener.OnRewardPlayer(productId);
            }
            catch (Exception e)
            {
                UserReportPurchaseException(e, product, "Fail to reward player");
                throw e;
            }

            try
            {
                if (subscriptionProducts.Contains(productId))
                {
                    QuerySubscriptionInfo();
                }
            }
            catch (Exception e)
            {
                UserReportPurchaseException(e, product, "Fail to get subscription info");
                throw e;
            }
        }
        else
        {
            Debug.LogError($"[IAP] validate receipt failed, check receipt below");
            Debug.LogError(product.receipt);
            IAPController.instance.listener.OnPurchaseFailed();
        }
        storeController.ConfirmPendingPurchase(product);
    }

    protected abstract UniTask<bool> ValidateReceipt(Product product);

    protected void UserReportPurchaseException(Exception e, Product product, string summary)
    {
#if USE_USER_REPORT
        UserReportManager.instance.SendReport(new UserReportManager.ReportInfo()
        {
            description = $"transaction id={product.transactionID}",
            dimensions = new Dictionary<string, string>()
            {
                { "version", Application.version }
            },
            summary = summary,
            attachment = new List<UserReportManager.AttachmentInfo>()
            {
                new UserReportManager.AttachmentInfo()
                {
                    title="receipt.json",
                    content=product.hasReceipt?product.receipt:$"{product.definition.id} has no receipt"
                },
                new UserReportManager.AttachmentInfo()
                {
                    title="exception.txt",
                    content=e.ToString(),
                }
            },
        });
#endif
    }

    #endregion

    #region IIAPImplementation

    protected Product GetProduct(string productId)
    {
        if (storeController == null)
        {
            throw new Exception("[IAP] storeController is null");
        }

        var product = storeController.products.WithID(productId);
        if (product == null || !product.availableToPurchase)
        {
            throw new Exception($"[IAP] product {productId} is unavailable");
        }

        return product;
    }

    public string GetPrice(string productId)
    {
        var product = GetProduct(productId);
        return $"{product.metadata.isoCurrencyCode} {product.metadata.localizedPrice:N0}";
    }

    public void Purchase(string productId)
    {
        var product = GetProduct(productId);
        storeController.InitiatePurchase(product);
    }

    public abstract void RestorePurchase();

    #endregion

    #region subscription

    private void QuerySubscriptionInfo()
    {
		var dicSubscriptionInfo = new Dictionary<string, DateTime>();
        foreach (var i in subscriptionProducts)
        {
			dicSubscriptionInfo.Add(i, GetSubscriptionExpire(i));
        }
        IAPController.instance.listener.OnQuerySubscriptionInfo(dicSubscriptionInfo);
    }

    private DateTime GetSubscriptionExpire(string id)
    {
        var product = GetProduct(id);
        if (product.receipt == null)
        {
            return new DateTime();
        }
        var mng = new SubscriptionManager(product, intro_json: null);
        var info = mng.getSubscriptionInfo();

        return info.getExpireDate();
    }

	#endregion
}