
using UnityEditor;
using UnityEditor.Build;
using UnityEngine;

public class BuildPlayerState_build_mobile_android : BuildPlayerState_build_mobile
{
	protected override int GetBuildNumber()
	{
		return PlayerSettings.Android.bundleVersionCode;
	}

	protected override void SetBuildNumber(int buildNumber)
	{
		PlayerSettings.Android.bundleVersionCode = buildNumber;
	}

	protected override BuildPlayerOptions PrepareBuild()
	{
		SetupConfig();
		return ConstructBuildOption(base.PrepareBuild());
	}

	private void SetupConfig()
	{
		var cfg = GameFrameworkConfig.instance;
		
		PlayerSettings.Android.keystorePass = cfg.androidKeystorePassword;
		PlayerSettings.Android.keyaliasPass = cfg.androidKeystorePassword;

		PlayerSettings.Android.targetSdkVersion = (AndroidSdkVersions)cfg.androidTargetSDK;

		//enable this will:
		// - freeze on some device
		// - cause black screen on BlueStack
		PlayerSettings.Android.optimizedFramePacing = false;

		if (buildEnvironment == BuildEnvironment.Live)
		{
			PlayerSettings.SetScriptingBackend(NamedBuildTarget.Android, ScriptingImplementation.IL2CPP);
			PlayerSettings.Android.targetArchitectures = AndroidArchitecture.All;
			EditorUserBuildSettings.buildAppBundle = true;
			PlayerSettings.Android.splitApplicationBinary = true;
		}
		else
		{
			PlayerSettings.SetScriptingBackend(NamedBuildTarget.Android, ScriptingImplementation.Mono2x);
			PlayerSettings.Android.targetArchitectures = AndroidArchitecture.ARMv7;
			EditorUserBuildSettings.buildAppBundle = false;
			PlayerSettings.Android.splitApplicationBinary = false;
		}
	}

	private BuildPlayerOptions ConstructBuildOption(BuildPlayerOptions baseOption)
	{
		var parent = "Builds/Android";
		var extension = buildEnvironment == BuildEnvironment.Live ? "aab" : "apk";
		var fileName = $"{Application.identifier}-{Application.version}-{PlayerSettings.Android.bundleVersionCode}.{extension}";

		baseOption.target = BuildTarget.Android;
		baseOption.targetGroup = BuildTargetGroup.Android;
		baseOption.locationPathName = $"{parent}/{fileName}";

		return baseOption;
	}
}