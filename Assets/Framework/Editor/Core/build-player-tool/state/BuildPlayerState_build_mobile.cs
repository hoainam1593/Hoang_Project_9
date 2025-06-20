using System;
using UnityEditor;
using UnityEngine;

public abstract class BuildPlayerState_build_mobile: BuildPlayerState_build
{
	protected BuildEnvironment buildEnvironment = BuildEnvironment.Dev;
	private int buildNumber;

	protected abstract int GetBuildNumber();
	protected abstract void SetBuildNumber(int buildNumber);

	public override void OnBeginState()
	{
		base.OnBeginState();

		buildNumber = GetBuildNumber();
	}

	protected override void OnDrawPlatform()
	{
		base.OnDrawPlatform();

		buildEnvironment = EditorUIElementCreator.CreateDropdownList_enum("choose build environment:",
			BuildEnvironment.Num, buildEnvironment, out bool _);

		if (buildEnvironment == BuildEnvironment.Live)
		{
			EditorGUILayout.BeginHorizontal();
			EditorUIElementCreator.CreateLabelField("build number:");
			EditorGUILayout.LabelField(buildNumber.ToString(), GUI.skin.textField);
			if (EditorUIElementCreator.CreateButton("increase"))
			{
				buildNumber++;
			}
			EditorGUILayout.EndHorizontal();
		}
	}

	protected override BuildPlayerOptions PrepareBuild()
	{
		PlayerSettings.defaultInterfaceOrientation = GameFrameworkConfig.instance.screenOrientation switch
		{
			GameScreenOrientation.Portrait => UIOrientation.Portrait,
			GameScreenOrientation.Landscape => UIOrientation.LandscapeLeft,
			_ => UIOrientation.AutoRotation
		};
		SetBuildNumber(buildNumber);
		return base.PrepareBuild();
	}
}