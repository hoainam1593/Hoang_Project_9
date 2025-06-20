
using System;
using System.Collections.Generic;

public interface IIAPListener
{
    void OnPurchaseFailed();
    void OnRewardPlayer(string productId);
    void OnQuerySubscriptionInfo(Dictionary<string, DateTime> dic);
}
