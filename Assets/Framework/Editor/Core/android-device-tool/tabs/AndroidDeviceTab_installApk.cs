
using System.Collections.Generic;
using UnityEngine;

public class AndroidDeviceTab_installApk : EditorUIElement_tabWindow.TabItemWindow
{
	private readonly EditorUIElement_pickFile pickFile = new(new List<string>() { "apk", "aab" }, "pick an apk/aab:");

	public override string tabText => "install APK";

	public override void OnDraw()
	{
		pickFile.Draw();
		if (GUILayout.Button("install"))
		{
			var path = pickFile.PickedPath;
			var pathInCmd = pickFile.PickedPathInCmd;
			if (path.EndsWith(".apk"))
			{
				InstallApk(pathInCmd);
			}
			else
			{
				InstallAab(pathInCmd);
			}
			
			StaticUtilsEditor.DisplayDialog("install success");
		}
	}

	private void InstallApk(string path)
	{
		StaticUtilsEditor.RunBatchScript("adb", new List<string>()
		{
			"install",
			path
		});
	}

	private void InstallAab(string path)
	{
		var bundleToolPath = $"{StaticUtils.GetFrameworkPath()}/Editor/Core/Plugins/bundletool-all-1.18.1.jar";
		var outputApks = StaticUtilsEditor.RandomATempPath("apks");
		StaticUtilsEditor.RunBatchScript("java", new List<string>()
		{
			"-jar",
			bundleToolPath,
			"build-apks",
			$"--bundle={path}",
			$"--output={outputApks}",
			"--mode=universal"
		});

		var unzipFolder = StaticUtilsEditor.RandomATempPath();
		StaticUtilsEditor.RunBatchScript("7z", new List<string>()
		{
			"x", 
			outputApks, 
			$"-o{unzipFolder}", 
			"-y"
		});

		InstallApk($"{unzipFolder}/universal.apk");
	}
}