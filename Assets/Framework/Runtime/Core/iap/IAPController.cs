using System;
using System.Collections.Generic;
using R3;
using UnityEngine;
using UnityEngine.Purchasing;

public class IAPController : SingletonMonoBehaviour<IAPController>
{
    #region core

    private IIAPImplementation impl;
    private Dictionary<string, ProductType> dicProduct;

    public IIAPListener listener { get; set; }
    public ReactiveProperty<bool> IsPlayPassRx { get; set; } = new(false);
    public string CurrencyCode { get; set; } = "";
    
    public void Init(Dictionary<string, ProductType> dicProduct, IIAPListener listener)
    {
        this.listener = listener;
        this.dicProduct = dicProduct;
        if (impl == null)
        {
#if SAMSUNG_STORE
            impl = new IAPImplementation_samsung(dicProduct);
            return;
#endif

#if UNITY_EDITOR || UNITY_STANDALONE
            impl = new IAPImplementation_editor();
#elif UNITY_ANDROID
			impl = new IAPImplementation_mobile_android(dicProduct);
#elif UNITY_IOS
			impl = new IAPImplementation_mobile_ios(dicProduct);
#endif
        }
    }

    #endregion
    
    #region iap implementation bridge

    public string GetPrice(string productId)
    {
        try
        {
            return impl.GetPrice(productId);
        }
        catch (Exception e)
        {
            Debug.LogError(e);
            return "Buy";
        }
    }
    
    public void Purchase(string productId)
    {
        try
        {
            impl.Purchase(productId);
        }
        catch (Exception e)
        {
            Debug.LogError(e);
            listener.OnPurchaseFailed();
        }
    }

    public void RestorePurchase()
    {
        impl.RestorePurchase();
    }

    #endregion
}