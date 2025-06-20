
using Cysharp.Threading.Tasks;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Tilemaps;
using TMPro;

public class AssetManager : SingletonMonoBehaviour<AssetManager>
{
	#region core

	private readonly AssetGroup assetGroup = new();
	
	protected override void OnDestroy()
	{
		base.OnDestroy();

		assetGroup.ReleaseGroup();
	}

	#endregion

	#region download addressable

	//in order for 2 addressable from 2 project match each other, need:
	// - all group bundle naming = no hash
	// - mono script bundle naming = custom naming
	// - copy whole main project to addressable project to make sure no different GUID
	
	public async UniTask DownloadRemoteAddressable(ServerConfigJson serverCfg)
	{
		var localSettings = new ServerConfigJson();
		localSettings.Load();

		if (localSettings.addressableVersion == serverCfg.addressableVersion)
		{
			return;
		}

		Debug.LogError(
			$"local version={localSettings.addressableVersion} remote version={serverCfg.addressableVersion} => download addressable");
		await DoDownloadRemoteAddressable();
	}

	private async UniTask DoDownloadRemoteAddressable()
	{
#if UNITY_STANDALONE_WIN
		var platform = "StandaloneWindows64";
#elif UNITY_ANDROID
		var platform = "Android";
#elif UNITY_IOS
		var platform = "iOS";
#endif
		var remoteKey =
			$"{ServerController.instance.serverEnvironment}/addressable/{platform}/remotegroup_assets_all.bundle";
		var localPath = $"{RemoteAssetsPath.remoteAddressableRuntimePath}/{platform}";
		await ServerController.instance.GameContent_download(remoteKey, localPath);
	}

	#endregion

	#region load assets

	public async UniTask<GameObject> LoadPrefab(AssetReferenceGameObject assetRef)
	{
		return await assetGroup.LoadPrefab(assetRef);
	}

	public async UniTask<GameObject> LoadPrefab(string parentPath, string assetName)
	{
		return await assetGroup.LoadPrefab(parentPath, assetName);
	}

	public async UniTask<Sprite> LoadSprite(string parentPath, string assetName)
	{
		return await assetGroup.LoadSprite(parentPath, assetName);
	}

	public async UniTask<TextAsset> LoadText(string parentPath, string assetName)
	{
		return await assetGroup.LoadText(parentPath, assetName);
	}

	public async UniTask<TileBase> LoadTile(string parentPath, string assetName)
	{
		return await assetGroup.LoadTile(parentPath, assetName);
	}

	public async UniTask<TMP_FontAsset> LoadTmpFont(string parentPath, string assetName)
	{
		return await assetGroup.LoadTmpFont(parentPath, assetName);
	}

	#endregion
}