
using System;
using System.IO;
using Cysharp.Threading.Tasks;
using UnityEngine.Events;

public interface IServer
{
    #region game content

    UniTask<string> GameContent_get(string key);
    UniTask GameContent_download(string key, string path);
    
    UniTask GameContent_set(string key, string value);
    UniTask GameContent_set(string key, byte[] value);

    //using aws S3 + cloud front, cloud front using cache, so when update files in S3,
    //get files from cloud front won't return the latest version.
    //use this to clear cache from cloud front,
    //if don't use s3 + cloud front, this won't do anything.
    UniTask GameContent_applySet();
    
    #endregion

    #region player data

    UniTask PlayerData_get(string userId, string playerModelName, UnityAction<Stream> callback);
    UniTask PlayerData_set(string userId, string playerModelName, Stream modelContent);
    UniTask<bool> PlayerData_exists(string userId, string playerModelName);

    #endregion

    #region validate IAP

    UniTask<bool> ValidateGooglePlayReceipt(string pid, string purchaseToken, bool isSubscriptionProduct);
    UniTask<bool> ValidateAppStoreReceipt(string pid, string payload, bool isSubscriptionProduct);
    
    #endregion

    #region other API

    UniTask<DateTime> GetUTCNow();

    #endregion
}