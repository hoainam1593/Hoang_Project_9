
#if USE_FIREBASE_AUTHENTICATION

using Cysharp.Threading.Tasks;
using Firebase.Auth;

public partial class FirebaseController
{
	#region sign in

	public async UniTask<string> SignIn()
	{
		var user = FirebaseAuth.DefaultInstance.CurrentUser;
		if (user != null)
		{
			return user.UserId;
		}

		return await DoSignIn();
	}

	private async UniTask<string> DoSignIn()
	{
		var signResult = await SocialSignInController.instance.SignIn();

#if UNITY_EDITOR || UNITY_STANDALONE
		return (string)signResult;
#elif UNITY_ANDROID
		return await DoSignIn_google(signResult);
#elif UNITY_IOS
		return await DoSignIn_apple(signResult);
#endif
	}

	private async UniTask<string> DoSignIn_google(object socialSignInResult)
	{
		var idToken = socialSignInResult as string;
		var auth = FirebaseAuth.DefaultInstance;
		var credential = GoogleAuthProvider.GetCredential(idToken, null);
		var firebaseSignInResult = await auth.SignInWithCredentialAsync(credential);
		return firebaseSignInResult.UserId;
	}

	private async UniTask<string> DoSignIn_apple(object socialSignInResult)
	{
		var signInWithAppleResult = socialSignInResult as SocialSignIn_apple.SignInWithAppleResult;
		var auth = FirebaseAuth.DefaultInstance;
		var credential = OAuthProvider.GetCredential("apple.com",
			signInWithAppleResult.identityToken, signInWithAppleResult.rawNonce,
			signInWithAppleResult.authorizationCode);
		var firebaseSignInResult = await auth.SignInWithCredentialAsync(credential);
		return firebaseSignInResult.UserId;
	}

	#endregion
	
	#region sign out
	
	public async UniTask SignOut()
	{
		FirebaseAuth.DefaultInstance.SignOut();
		await SocialSignInController.instance.SignOut();
	}
	
	#endregion

	#region delete account

	public async UniTask DeleteUser()
	{
		//before do any sensitive operations like delete user,
		//sign in first
		await DoSignIn();
		await DoDeleteUser();
	}

	private async UniTask DoDeleteUser()
	{
		var user = FirebaseAuth.DefaultInstance.CurrentUser;
		if (user != null)
		{
			await user.DeleteAsync();
		}
	}

	#endregion
}

#endif