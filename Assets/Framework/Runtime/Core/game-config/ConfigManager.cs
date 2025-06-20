
using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using UnityEngine;

public class ConfigManager : SingletonMonoBehaviour<ConfigManager>
{
	#region core

	private ConfigReadWriteManager configReadWriteManager;

	public List<IBaseConfig> ListConfigs => configReadWriteManager.listConfigs;
	
	public T GetConfig<T>() where T : class, IBaseConfig
	{
		T result = null;
		var typeT = typeof(T);
		foreach (var i in configReadWriteManager.listConfigs)
		{
			if (i.GetType() == typeT)
			{
				if (result == null)
				{
					result = (T)i;
				}
				else
				{
					throw new Exception($"there are more than 1 {typeT.Name} in listConfigs");
				}
			}
		}
		if (result != null)
		{
			return result;
		}
		else
		{
			throw new Exception($"there's no {typeT.Name} in listConfigs");
		}
	}
	
	#endregion
	
	#region load configs

	public async UniTask LoadAllConfigs(ServerConfigJson serverCfg, IListConfigDeclaration listConfigDeclaration)
	{
		configReadWriteManager = new ConfigReadWriteManager(listConfigDeclaration.listConfigs);
		
#if USE_SERVER_GAME_CONFIG && !UNITY_EDITOR
		await DownloadServerGameConfig(serverCfg);
#endif

		await DoLoadAllConfigs();
	}

	public async UniTask LoadAllConfigs(IListConfigDeclaration listConfigDeclaration)
	{
		configReadWriteManager = new ConfigReadWriteManager(listConfigDeclaration.listConfigs);
		await DoLoadAllConfigs();
	}

	private async UniTask DoLoadAllConfigs()
	{
#if UNITY_EDITOR
		await UniTask.CompletedTask;
		configReadWriteManager.ReadConfig_editor();
#elif UNITY_STANDALONE_WIN
		await configReadWriteManager.ReadConfig_standalone();
#else
		await configReadWriteManager.ReadConfig_mobile();
#endif
	}

	#endregion

	#region download config

	private async UniTask DownloadServerGameConfig(ServerConfigJson serverCfg)
	{
		var localSettings = new ServerConfigJson();
		localSettings.Load();

		if (localSettings.gameConfigVersion == serverCfg.gameConfigVersion)
		{
			return;
		}

		Debug.LogError(
			$"local version={localSettings.gameConfigVersion} remote version={serverCfg.gameConfigVersion} => download config");
#if UNITY_STANDALONE_WIN
		await DownloadConfig_standalone();
#else
		await DownloadConfig_mobile();
#endif
	}

	private async UniTask DownloadConfig_standalone()
	{
		var configFolder = $"{Application.dataPath}/../../GameConfig";
		var lTasks = new List<UniTask>();
		foreach (var i in ListConfigs)
		{
			var cfgName = i.GetType().Name;
			var key = $"{ServerController.instance.serverEnvironment}/game_configs_text/{cfgName}.csv";
			lTasks.Add(ServerController.instance.GameContent_download(key, configFolder));
		}
		await UniTask.WhenAll(lTasks);
	}

	private async UniTask DownloadConfig_mobile()
	{
		var configFolder = $"{Application.persistentDataPath}/GameConfig";
		var key = $"{ServerController.instance.serverEnvironment}/game_configs_binary/all_config.bin";
		await ServerController.instance.GameContent_download(key, configFolder);
	}

	#endregion

	#region process config before build

	public static void PrepareConfigForBuild(bool isWindows)
	{
		var configReadWriteManager = new ConfigReadWriteManager(GetListConfigsInEditor());
		configReadWriteManager.ReadConfig_editor();

		var cfgPath = $"{Application.dataPath}/StreamingAssets/GameConfig";
		StaticUtils.DeleteFolder(cfgPath, isAbsolutePath: true);

		if (isWindows)
		{
			configReadWriteManager.WriteConfig_standalone();
		}
		else
		{
			configReadWriteManager.WriteConfig_mobile();
		}
	}
	
	public static List<IBaseConfig> GetListConfigsInEditor()
	{
		var assembly = StaticUtils.GetAssembly(StaticUtils.MainAssemblyName);
		var interfaceTypes = StaticUtils.ListClassImplementingInterface(assembly, typeof(IListConfigDeclaration));

		if (interfaceTypes.Count != 1)
		{
			throw new Exception("need exactly one IListConfigDeclaration implementation");
		}
		
		var listConfigs = (IListConfigDeclaration)Activator.CreateInstance(interfaceTypes[0]);

		return listConfigs.listConfigs;
	}

	#endregion
}