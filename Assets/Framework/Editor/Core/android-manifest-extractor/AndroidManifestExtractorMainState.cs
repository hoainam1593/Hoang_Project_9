using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class AndroidManifestExtractorMainState : EditorWindowState
{
	private EditorUIElement_pickFile pickFile = new EditorUIElement_pickFile(new List<string>() { "apk" }, "pick an apk:");

	public override void OnDraw()
	{
		pickFile.Draw();
		if (GUILayout.Button("extract AndroidManifest.xml"))
		{
			var toolPath = $"{Application.dataPath}/_framework/android-manifest-extractor/Editor/apktool_2.9.3.jar";
			var outPath = StaticUtilsEditor.RandomATempPath();
			StaticUtilsEditor.RunBatchScript("java", new List<string>()
			{
				"-jar", toolPath, "d", "-s", "-o", outPath, pickFile.PickedPath,
			});

			Process.Start($"{outPath}/AndroidManifest.xml");

			StaticUtilsEditor.DisplayDialog("extract success");
		}
	}
}