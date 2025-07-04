﻿using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Samsung
{
    #region Value Objects

    public enum BuildMode
    {
        Development,
        Production
    }

    public enum TestResult
    {
        Success,
        Failure
    }

    public enum OperationMode
    {
        OPERATION_MODE_TEST_FAILURE,
        OPERATION_MODE_PRODUCTION,
        OPERATION_MODE_TEST
    }

    public enum ItemType
    {
        item,
        subscription,
        all
    }

    public enum ProrationMode
    {
        INSTANT_PRORATED_DATE,
        INSTANT_PRORATED_CHARGE,
        INSTANT_NO_PRORATION,
        DEFERRED
    }

    [System.Serializable]
    public class ErrorVo
    {
        public int errorCode = 1;
        public string errorString = "";
        public string extraString = "";
    }

    [System.Serializable]
    public class ProductInfoList
    {
        public ErrorVo errorInfo;
        public List<ProductVo> results;
    }

    [System.Serializable]
    public class ProductVo
    {
        public string mItemId = "";
        public string mItemName = "";
        public string mItemPrice = "";
        public string mItemPriceString = "";
        public string mCurrencyUnit = "";
        public string mCurrencyCode = "";
        public string mItemDesc = "";
        public string mType = "";
        public string mConsumableYN = "";
        public string mItemImageUrl = "";
        public string mItemDownloadUrl = "";
        public string mReserved1 = "";
        public string mReserved2 = "";
        public string mFreeTrialPeriod = "";
        public string mSubscriptionDurationUnit = "";
        public string mSubscriptionDurationMultiplier = "";
        public string mTieredPrice = "";
        public string mTieredPriceString = "";
        public string mTieredSubscriptionYN = "";
        public string mTieredSubscriptionDurationUnit = "";
        public string mTieredSubscriptionDurationMultiplier = "";
        public string mTieredSubscriptionCount = "";
        public string mShowStartDate = "";
        public string mShowEndDate = "";
    }

    [System.Serializable]
    public class OwnedProductList
    {
        public ErrorVo errorInfo;
        public List<OwnedProductVo> results;
    }

    [System.Serializable]
    public class OwnedProductVo
    {
        public string mItemId = "";
        public string mItemName = "";
        public string mItemPrice = "";
        public string mItemPriceString = "";
        public string mCurrencyUnit = "";
        public string mCurrencyCode = "";
        public string mItemDesc = "";
        public string mType = "";
        public string mConsumableYN = "";
        public string mSubscriptionEndDate = "";
        public string mPaymentId = "";
        public string mPurchaseId = "";
        public string mPurchaseDate = "";
        public string mPassThroughParam = "";
        public string changeSubscriptionPrices = "";
        public SubscriptionPriceChangeVo subscriptionPriceChangeVo;
    }

    [System.Serializable]
    public class InnerSubscriptionPriceChangeVo
    {
        public string appName = "";
        public string itemName = "";
        public string subscriptionPaymentFreqUnit = "";
        public string subscriptionPaymentFreqN = "";
        public string startDate = "";
        public double originalLocalPrice = 0.0;
        public string originalLocalPriceString = "";
        public double newLocalPrice = 0.0;
        public string newLocalPriceString = "";
        public string isConsented = "N";        // "N" or "Y"
        public string priceChangeMode = "0";    // 0: PRICE_INCREASE_USER_AGREEMENT_REQUIRED, 1: PRICE_INCREASE_NO_USER_AGREEMENT_REQUIRED, 2: PRICE_DECREASE
    }

    [System.Serializable]
    public class SubscriptionPriceChangeVo
    {
        public string appName = "";
        public string itemName = "";
        public string subscriptionPaymentFreqUnit = "";
        public string subscriptionPaymentFreqN = "";
        public string startDate = "";
        public double originalLocalPrice = 0.0;
        public string originalLocalPriceString = "";
        public double newLocalPrice = 0.0;
        public string newLocalPriceString = "";
        public bool isConsented = false;
        public PriceChangeMode priceChangeMode;

        public SubscriptionPriceChangeVo(InnerSubscriptionPriceChangeVo innerVo)
        {
            this.appName = innerVo.appName;
            this.itemName = innerVo.itemName;
            this.subscriptionPaymentFreqUnit = innerVo.subscriptionPaymentFreqUnit;
            this.subscriptionPaymentFreqN = innerVo.subscriptionPaymentFreqN;
            this.startDate = innerVo.startDate;
            this.originalLocalPrice = innerVo.originalLocalPrice;
            this.originalLocalPriceString = innerVo.originalLocalPriceString;
            this.newLocalPrice = innerVo.newLocalPrice;
            this.newLocalPriceString = innerVo.newLocalPriceString;
            this.isConsented = innerVo.isConsented.Equals("Y");
            try
            {
                this.priceChangeMode = (PriceChangeMode)Convert.ToInt32(innerVo.priceChangeMode);
                Debug.Log("SubscriptionPriceChangeVo priceChangeMode: " + priceChangeMode);
            } catch (FormatException)
            {
                Debug.Log("FormatException: SubscriptionPriceChangeVo");
                this.priceChangeMode = PriceChangeMode.PRICE_INCREASE_USER_AGREEMENT_REQUIRED;
            }
        }
    }

    public enum PriceChangeMode
    {
        PRICE_INCREASE_USER_AGREEMENT_REQUIRED = 0,
        PRICE_INCREASE_NO_USER_AGREEMENT_REQUIRED = 1,
        PRICE_DECREASE = 2
    }
    
    [System.Serializable]
    public class PurchasedInfo
    {
        public ErrorVo errorInfo;
        public PurchaseVo results;
    }

    [System.Serializable]
    public class PurchaseVo
    {
        public string mItemId = "";
        public string mItemName = "";
        public string mItemPrice = "";
        public string mItemPriceString = "";
        public string mCurrencyUnit = "";
        public string mCurrencyCode = "";
        public string mItemDesc = "";
        public string mType = "";
        public string mConsumableYN = "";
        public string mPaymentId = "";
        public string mPurchaseId = "";
        public string mPurchaseDate = "";
        public string mVerifyUrl = "";
        public string mPassThroughParam = "";
        public string mItemImageUrl = "";
        public string mItemDownloadUrl = "";
        public string mReserved1 = "";
        public string mReserved2 = "";
        public string mOrderId = "";
    }

    [System.Serializable]
    public class ConsumedList
    {
        public ErrorVo errorInfo;
        public List<ConsumeVo> results;
    }

    [System.Serializable]
    public class ConsumeVo
    {
        public string mPurchaseId = "";
        public string mStatusString = "";
        public int mStatusCode = 0;
    }

    [System.Serializable]
    public class PromotionEligibilityList
    {
        public ErrorVo errorInfo;
        public List<PromotionEligibilityVo> results;
    }

    [System.Serializable]
    public class PromotionEligibilityVo
    {
        public string itemId = "";
        public string pricing = "";
    }
    #endregion

    #region UnityEvents

    [System.Serializable]
    public class OnGetOwenedListEvent : UnityEvent<OwnedProductList>
    {
    }

    [System.Serializable]
    public class OnGetProductsEvent : UnityEvent<ProductInfoList>
    {
    }

    [System.Serializable]
    public class OnPaymentEvent : UnityEvent<PurchasedInfo>
    {
    }

    [System.Serializable]
    public class OnConsumeEvent : UnityEvent<ConsumedList>
    {
    }

    [System.Serializable]
    public class OnGetPromotionEligibilityEvent : UnityEvent<PromotionEligibilityList>
    {
    }

    [System.Serializable]
    public class OnChangeSubscriptionPlanEvent : UnityEvent<PurchasedInfo>
    {
    }

    #endregion

    public class SamsungIAP : MonoBehaviour
    {
        private static SamsungIAP _instance;
        private AndroidJavaObject iapInstance = null;

        public bool prodBuild = false;
        public TestResult testPurchaseResult;

        private string savedPassthroughParam = "";

        public System.Action<ProductInfoList> onGetProductsDetailsListener;
        public System.Action<PurchasedInfo> onStartPaymentListener;
        public System.Action<ConsumedList> onConsumePurchasedItemListener;
        public System.Action<OwnedProductList> onGetOwenedListListener;
        public System.Action<PromotionEligibilityList> onGetPromotionEligibilityListener;
        public System.Action<PurchasedInfo> onChangeSubscriptionPlanListener;

        [SerializeField]
        public OnGetProductsEvent onGetProductsDetails;

        [SerializeField]
        public OnPaymentEvent onPayment;

        [SerializeField]
        public OnConsumeEvent onConsume;

        [SerializeField]
        public OnGetOwenedListEvent onGetOwenedList;

        [SerializeField]
        public OnGetPromotionEligibilityEvent onGetPromotionEligibility;

        [SerializeField]
        public OnChangeSubscriptionPlanEvent onChangeSubscriptionPlan;

        public static SamsungIAP Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = GameObject.FindObjectOfType<SamsungIAP>();
                }

                return _instance;
            }
        }

        void Awake()
        {
            if (iapInstance == null)
            {
                Debug.Log("Awake");

                using (AndroidJavaClass cls = new AndroidJavaClass("com.samsung.android.sdk.iap.lib.activity.SamsungIAPFragment"))
                {
                    //Initialize IAP
                    cls.CallStatic("init", gameObject.name);
                    iapInstance = cls.CallStatic<AndroidJavaObject>("getInstance");
                    if (prodBuild)
                    {
                        SetOperationMode(OperationMode.OPERATION_MODE_PRODUCTION);
                    }
                    else
                    {
                        switch (testPurchaseResult)
                        {
                            case TestResult.Success:
                                SetOperationMode(OperationMode.OPERATION_MODE_TEST);
                                break;
                            case TestResult.Failure:
                                SetOperationMode(OperationMode.OPERATION_MODE_TEST_FAILURE);
                                break;
                        }
                    }
                }

                DontDestroyOnLoad(gameObject);
            }
        }

        public void OnError(string msg)
        {
            Debug.Log("Samsung IAP Error: " + msg);
        }

        #region IAP Functions

        public void SetOperationMode(OperationMode mode)
        {
            if (iapInstance != null)
                iapInstance.Call("setOperationMode", mode.ToString());
            else
                OnError("Android Context not inialized correctly.");
        }

        public void GetProductsDetails(string itemIDs, System.Action<ProductInfoList> listener)
        {
            Debug.Log("GetProductsDetails : !");
            onGetProductsDetailsListener = listener;
            if (iapInstance != null)
                iapInstance.Call("getProductDetails", itemIDs);
            else
                OnError("Android Context not inialized correctly.");
        }

        public void GetOwnedList(ItemType itemType, System.Action<OwnedProductList> listener)
        {
            Debug.Log("GetOwnedList : !");
            onGetOwenedListListener = listener;

            if (iapInstance != null)
                iapInstance.Call("getOwnedList", itemType.ToString());
            else
                OnError("Android Context not inialized correctly.");
        }

        public void StartPayment(string itemID, string passThroughParam, System.Action<PurchasedInfo> listener)
        {
            Debug.Log("StartPayment : !");
            savedPassthroughParam = passThroughParam;
            onStartPaymentListener = listener;

            if (iapInstance != null)
                iapInstance.Call("startPayment", itemID, passThroughParam);
            else
                OnError("Android Context not inialized correctly.");
        }

        public void ConsumePurchasedItems(string purchaseIDs, System.Action<ConsumedList> listener)
        {
            Debug.Log("ConsumePurchasedItems : !");
            onConsumePurchasedItemListener = listener;

            if (iapInstance != null)
                iapInstance.Call("consumePurchasedItems", purchaseIDs);
            else
                OnError("Android Context not inialized correctly.");
        }

        public void GetPromotionEligibility(string itemIDs, System.Action<PromotionEligibilityList> listener)
        {
            Debug.Log("GetPromotionEligibility : !");
            onGetPromotionEligibilityListener = listener;

            if (iapInstance != null)
                iapInstance.Call("getPromotionEligibility", itemIDs);
            else
                OnError("Android Context not inialized correctly.");
        }

        public void ChangeSubscriptionPlan(string oldItemID, string newItemID, ProrationMode prorationMode, string passThroughParam, System.Action<PurchasedInfo> listener)
        {
            Debug.Log("ChangeSubscriptionPlan : !");
            savedPassthroughParam = passThroughParam;
            onChangeSubscriptionPlanListener = listener;

            if (iapInstance != null)
                iapInstance.Call("changeSubscriptionPlan", oldItemID, newItemID, prorationMode.ToString(), passThroughParam);
            else
                OnError("Android Context not inialized correctly.");
        }

        #endregion

        #region Callback Functions

        public void OnGetProductsDetails(string resultJSON)
        {
            Debug.Log("OnGetProductsDetails : " + resultJSON);
            ProductInfoList productList = JsonUtility.FromJson<ProductInfoList>(resultJSON);
            Debug.Log("OnGetProductsDetails cnt:" + productList.results.Count);
            for (int i = 0; i < productList.results.Count; ++i)
                Debug.Log("onGetProductsDetails: " + productList.results[i].mItemName);

            if (onGetProductsDetails != null)
            {
                onGetProductsDetails.Invoke(productList);
            }

            if (onGetProductsDetailsListener != null)
                onGetProductsDetailsListener(productList);
        }

        public void OnGetOwnedProducts(string resultJSON)
        {
            Debug.Log("onGetOwnedProducts");
            OwnedProductList ownedList = JsonUtility.FromJson<OwnedProductList>(resultJSON);
            Debug.Log("onGetOwnedProducts cnt:" + ownedList.results.Count);
            for (int i = 0; i < ownedList.results.Count; ++i)
            {
                Debug.Log("onGetOwnedProducts: " + ownedList.results[i].mItemName);
                setSubscriptionPriceChangeVo(ownedList.results[i]);
            }

            if (onGetOwenedList != null)
            {
                onGetOwenedList.Invoke(ownedList);
            }

            if (onGetOwenedListListener != null)
                onGetOwenedListListener(ownedList);
        }

        private void setSubscriptionPriceChangeVo(OwnedProductVo ownedItem)
        {
            // changeSubscriptionPrices이 Json String형태로 전달되므로, 다시 JSONObject로 변환 필요
            InnerSubscriptionPriceChangeVo innerSubscriptionPriceChangeVo = JsonUtility.FromJson<InnerSubscriptionPriceChangeVo>(ownedItem.changeSubscriptionPrices);
            ownedItem.changeSubscriptionPrices = "";

            // InnerSubscriptionPriceChangeVo -> SubscriptionPriceChangeVo
            ownedItem.subscriptionPriceChangeVo = new SubscriptionPriceChangeVo(innerSubscriptionPriceChangeVo);
        }

        public void OnConsumePurchasedItems(string resultJSON)
        {
            Debug.Log("OnConsumePurchasedItems : " + resultJSON);
            ConsumedList consumedList = JsonUtility.FromJson<ConsumedList>(resultJSON);
            Debug.Log("OnConsumePurchasedItems cnt:" + consumedList.results.Count);
            for (int i = 0; i < consumedList.results.Count; ++i)
            {
                Debug.Log("OnConsumePurchasedItems: " + consumedList.results[i].mPurchaseId);
            }

            if (onConsume != null)
            {
                onConsume.Invoke(consumedList);
            }

            if (onConsumePurchasedItemListener != null)
                onConsumePurchasedItemListener(consumedList);
        }

        public void OnPayment(string resultJSON)
        {
            Debug.Log("onPayment: " + resultJSON);
            PurchasedInfo purchasedInfo = JsonUtility.FromJson<PurchasedInfo>(resultJSON);

            if (purchasedInfo != null)
            {
                if (purchasedInfo.results.mPassThroughParam != savedPassthroughParam)
                    Debug.Log("PassThroughParam is different!!!");
            }

            if (purchasedInfo != null)
            {
                if (purchasedInfo.results.mPassThroughParam != savedPassthroughParam)
                    Debug.Log("PassThroughParam is different!!!");
            }

            if (onPayment != null)
            {
                onPayment.Invoke(purchasedInfo);
            }

            if (onStartPaymentListener != null)
                onStartPaymentListener(purchasedInfo);
        }

        public void OnGetPromotionEligibility(string resultJSON)
        {
            Debug.Log("onGetPromotionEligibility : " + resultJSON);
            PromotionEligibilityList promotionEligibilityList = JsonUtility.FromJson<PromotionEligibilityList>(resultJSON);
            Debug.Log("onGetPromotionEligibility cnt:" + promotionEligibilityList.results.Count);
            for (int i = 0; i < promotionEligibilityList.results.Count; ++i)
            {
                Debug.Log("onGetPromotionEligibility: " + promotionEligibilityList.results[i].itemId);
            }

            if (onGetPromotionEligibility != null)
            {
                onGetPromotionEligibility.Invoke(promotionEligibilityList);
            }

            if (onGetPromotionEligibilityListener != null)
                onGetPromotionEligibilityListener(promotionEligibilityList);
        }

        public void OnChangeSubscriptionPlan(string resultJSON)
        {
            Debug.Log("onChangeSubscriptionPlan: " + resultJSON);
            PurchasedInfo purchasedInfo = JsonUtility.FromJson<PurchasedInfo>(resultJSON);

            if (purchasedInfo != null)
            {
                if (purchasedInfo.results.mPassThroughParam != savedPassthroughParam)
                    Debug.Log("PassThroughParam is different!!!");
            }

            if (onChangeSubscriptionPlan != null)
            {
                onChangeSubscriptionPlan.Invoke(purchasedInfo);
            }

            if (onChangeSubscriptionPlanListener != null)
                onChangeSubscriptionPlanListener(purchasedInfo);
        }

        #endregion

        #region Editor

#if UNITY_EDITOR
        [CustomEditor(typeof(SamsungIAP))]
        public class SamsungIAPManagerEditor : Editor
        {
            public override void OnInspectorGUI()
            {
                var ui = target as SamsungIAP;

                EditorGUILayout.HelpBox("" +
                    "Samsung IAP have two modes\n" +
                    "\n" +
                    "TestMode:\n" +
                    "You can test your application without real IAP transaction.\n" +
                    "\n" +
                    "ProductionMode:\n" +
                    "Turn on production mode when submit your game to the store", MessageType.Info);

                ui.prodBuild = GUILayout.Toggle(ui.prodBuild, "Production Build");

                if (!ui.prodBuild)
                {
                    EditorGUILayout.LabelField("Choose Result for Test Purcahse");
                    ui.testPurchaseResult = (TestResult)EditorGUILayout.EnumPopup(ui.testPurchaseResult);
                }

                EditorGUILayout.Separator();

                EditorGUILayout.HelpBox("Choose Callback functions for Samsung IAP", MessageType.Info);

                EditorGUILayout.PropertyField(this.serializedObject.FindProperty("onGetProductsDetails"));
                EditorGUILayout.PropertyField(this.serializedObject.FindProperty("onPayment"));
                EditorGUILayout.PropertyField(this.serializedObject.FindProperty("onConsume"));
                EditorGUILayout.PropertyField(this.serializedObject.FindProperty("onGetOwenedList"));

                EditorGUILayout.Separator();
            }
        }
#endif

        #endregion
    }
}
