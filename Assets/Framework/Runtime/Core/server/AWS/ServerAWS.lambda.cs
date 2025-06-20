
#if USE_SERVER_AWS

using System;
using Cysharp.Threading.Tasks;

public partial class ServerAWS
{
    public async UniTask<bool> ValidateGooglePlayReceipt(string pid, string purchaseToken, bool isSubscriptionProduct)
    {
        await UniTask.CompletedTask;

        throw new Exception("need implement ValidateGooglePlayReceipt");
    }

    public async UniTask<bool> ValidateAppStoreReceipt(string pid, string payload, bool isSubscriptionProduct)
    {
        await UniTask.CompletedTask;

        throw new Exception("need implement ValidateAppStoreReceipt");
    }

    public async UniTask<DateTime> GetUTCNow()
    {
        await UniTask.CompletedTask;

        throw new Exception("need implement GetUTCNow");
    }
}

#endif