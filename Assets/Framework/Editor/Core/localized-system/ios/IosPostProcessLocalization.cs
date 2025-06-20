using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine;

public static class IosPostProcessLocalization
{
	[PostProcessBuild]
	public static void OnPostProcessBuild(BuildTarget target, string path)
	{
		if (target != BuildTarget.iOS)
		{
			return;
		}
		
		var project = new IosProjectFile(path);
		var languageInfo = new LanguageInfoConfig();
		foreach (var i in languageInfo.languageInfoItems)
		{
			var folder = $"{i.Value.iosIsoCode}.lproj";
            var folderPath = $"{Application.dataPath}/_game/localized-system-data/IosLocalization/{folder}";
			project.AddLocalization(folder, folderPath);
		}
		project.Save();
	}
}