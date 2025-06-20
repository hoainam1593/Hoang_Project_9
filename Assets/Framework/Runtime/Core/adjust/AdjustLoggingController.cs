#if USE_ADJUST_LOGGING

using AdjustSdk;

public class AdjustLoggingController : SingletonMonoBehaviour<AdjustLoggingController>
{
    protected override void Awake()
    {
        base.Awake();

        var appToken = GameFrameworkConfig.instance.adjustAppToken;
        var cfg = new AdjustConfig(appToken, AdjustEnvironment.Production);
        Adjust.InitSdk(cfg);
    }

    public static void LogIAP(string pid, double revenue)
    {
        var evtToken = GameFrameworkConfig.instance.adjustIAPEventToken;
        var evt = new AdjustEvent(evtToken)
        {
            ProductId = pid
        };

        evt.SetRevenue(revenue, "KRW");
        evt.AddCallbackParameter("pid", pid);
        
        Adjust.TrackEvent(evt);
    }

    public static void LogAd(double revenue)
    {
        var adNetwork = AdController.instance.AdNetworkNameForAdjustLogging;
        var evt = new AdjustAdRevenue(adNetwork);
        
        evt.SetRevenue(revenue, "USD");
        
        Adjust.TrackAdRevenue(evt);
    }
}

#endif