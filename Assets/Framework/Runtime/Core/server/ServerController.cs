
using System;
using System.IO;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;

public partial class ServerController : SingletonMonoBehaviour<ServerController>
{
    private IServer impl;

    public void Init(bool isAdmin = false)
    {
#if USE_SERVER_AWS
        impl = new ServerAWS(isAdmin);
#else
        impl = null;
#endif
    }

    private ServerEnvironment _serverEnvironment = ServerEnvironment.QA;
    public ServerEnvironment serverEnvironment
    {
        get
        {
            Debug.LogError("[SERVER] currently, only using QA server, need to implement later");
            return _serverEnvironment;
        }
        set => _serverEnvironment = value;
    }
    
    #region game content

    public async UniTask<string> GameContent_get(string key)
    {
        return await impl.GameContent_get(key);
    }
    
    public async UniTask GameContent_download(string key, string path)
    {
        await impl.GameContent_download(key, path);
    }

    public async UniTask GameContent_set(string key, string value)
    {
        await impl.GameContent_set(key, value);
    }

    public async UniTask GameContent_set(string key, byte[] value)
    {
        await impl.GameContent_set(key, value);
    }

    public async UniTask GameContent_applySet()
    {
        await impl.GameContent_applySet();
    }

    #endregion
    
    #region player data

    public async UniTask PlayerData_get(string userId, string playerModelName, UnityAction<Stream> callback)
    {
        await impl.PlayerData_get(userId, playerModelName, callback);
    }

    public async UniTask PlayerData_set(string userId, string playerModelName, Stream modelContent)
    {
        await impl.PlayerData_set(userId, playerModelName, modelContent);
    }

    public async UniTask<bool> PlayerData_exists(string userId, string playerModelName)
    {
        return await impl.PlayerData_exists(userId, playerModelName);
    }

    #endregion
    
    #region validate IAP

    public async UniTask<bool> ValidateGooglePlayReceipt(string pid, string purchaseToken, bool isSubscriptionProduct)
    {
        return await impl.ValidateGooglePlayReceipt(pid, purchaseToken, isSubscriptionProduct);
    }

    public async UniTask<bool> ValidateAppStoreReceipt(string pid, string payload, bool isSubscriptionProduct)
    {
        return await impl.ValidateAppStoreReceipt(pid, payload, isSubscriptionProduct);
    }

    #endregion

    #region other API

    public async UniTask<DateTime> GetUTCNow()
    {
        return await impl.GetUTCNow();
    }

    #endregion
}