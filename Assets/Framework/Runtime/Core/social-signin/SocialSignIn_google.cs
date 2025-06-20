
using Cysharp.Threading.Tasks;
using UnityEngine;

public class SocialSignIn_google:ISocialSignIn
{
	private string callbackTargetName;
	private string callbackSuccessFunc;
	private string callbackFailFunc;

	private string loginNativeToken;
	private string loginNativeErrMsg;

	// use new google login will got this issue:
	// https://stackoverflow.com/questions/71325279/missing-featurename-auth-api-credentials-begin-sign-in-version-6
	const bool useLegacyLogin = true;

	public SocialSignIn_google(string callbackTargetName, string callbackSuccessFunc, string callbackFailFunc)
	{
		this.callbackTargetName = callbackTargetName;
		this.callbackSuccessFunc = callbackSuccessFunc;
		this.callbackFailFunc = callbackFailFunc;
	}

	public async UniTask<object> SignIn()
	{
		loginNativeToken = null;
		loginNativeErrMsg = null;

		using (var unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
		using (var currentActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity"))
		using (var classGoogleLogin = new AndroidJavaClass("com.ironygames.unitygooglesignin.GoogleLogin"))
		{
			var webClient = GameFrameworkConfig.instance.webClientId;
			classGoogleLogin.CallStatic("Login", currentActivity, webClient, useLegacyLogin,
				callbackTargetName, callbackSuccessFunc, callbackFailFunc);
		}

		await UniTask.WaitUntil(() =>
			!string.IsNullOrEmpty(loginNativeToken) || !string.IsNullOrEmpty(loginNativeErrMsg));

		if (!string.IsNullOrEmpty(loginNativeToken))
		{
			return loginNativeToken;
		}
		else
		{
			throw new System.Exception($"[GoogleSignIn] {loginNativeErrMsg}");
		}
	}

	public async UniTask SignOut()
	{
		await UniTask.CompletedTask;

		using (var unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
		using (var currentActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity"))
		using (var classGoogleLogin = new AndroidJavaClass("com.ironygames.unitygooglesignin.GoogleLogin"))
		{
			var webClient = GameFrameworkConfig.instance.webClientId;
			classGoogleLogin.CallStatic("Logout", currentActivity, webClient, useLegacyLogin);
		}
	}

	public void Update()
	{
	}

	public void NativeCallback_loginFail(string data)
	{
		loginNativeErrMsg = data;
	}

	public void NativeCallback_loginSuccess(string data)
	{
		loginNativeToken = data;
	}
}