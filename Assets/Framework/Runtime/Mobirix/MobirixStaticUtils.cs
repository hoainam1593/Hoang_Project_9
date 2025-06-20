
using UnityEngine;

public static class MobirixStaticUtils
{
    public static void OpenCustomerService()
    {
        var languageCode = Application.systemLanguage switch
        {
            SystemLanguage.Korean => "ko",
            SystemLanguage.Japanese => "ja",
            _ => "en",
        };
        var gameCode = GameFrameworkConfig.instance.gameIdForCustomerCare;
        var url = $"https://help.mobirix.com/{languageCode}/game/?game_idx={gameCode}";
        Application.OpenURL(url);
    }

    public static void OpenRefundPolicy()
    {
        var url = "http://www.mobirix.com/refundkr.html";
        Application.OpenURL(url);
    }

    public static void OpenFacebookFanPage()
    {
        var url = "https://www.facebook.com/mobirixplayen";
        Application.OpenURL(url);
    }

    public static void OpenPlayStorePage()
    {
        var packageName = Application.identifier;
        var url = $"https://play.google.com/store/apps/details?id={packageName}";
        Application.OpenURL(url);
    }

    public static void OpenMobirixTermPage()
    {
        var url = "https://policy.mobirix.com/terms?lang=ko";
        Application.OpenURL(url);
    }

    public static void OpenPrivacyPolicy()
    {
        var packageName = Application.identifier;
        var platform = "";

#if UNITY_ANDROID
		platform = "aos";
#elif UNITY_IOS
		platform = "ios";
#endif

        var url = $"https://policy.mobirix.com/personal?lang=ko&game={packageName}&os={platform}";
        Application.OpenURL(url);
    }
}
