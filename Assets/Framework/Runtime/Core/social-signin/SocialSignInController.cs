
using Cysharp.Threading.Tasks;

public class SocialSignInController : SingletonMonoBehaviour<SocialSignInController>
{
	private ISocialSignIn impl;

	protected override void Awake()
	{
		base.Awake();

#if UNITY_EDITOR || UNITY_STANDALONE
		impl = new SocialSignIn_dummy();
#elif UNITY_ANDROID
		impl = new SocialSignIn_google(gameObject.name, nameof(NativeCallback_loginSuccess), nameof(NativeCallback_loginFail));
#elif UNITY_IOS
		impl = new SocialSignIn_apple();
#endif
	}

	private void Update()
	{
		impl.Update();
	}

	public async UniTask<object> SignIn()
	{
		return await impl.SignIn();
	}

	public async UniTask SignOut()
	{
		await impl.SignOut();
	}

	public void NativeCallback_loginSuccess(string data)
	{
		impl.NativeCallback_loginSuccess(data);
	}

	public void NativeCallback_loginFail(string data)
	{
		impl.NativeCallback_loginFail(data);
	}
}