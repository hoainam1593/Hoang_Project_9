
using UnityEditor;

public class BuildPlayerWindow : EditorWindowStateMachine
{
	[MenuItem("\u2726\u2726TOOLS\u2726\u2726/\u2726\u2726RELEASE VERSION\u2726\u2726/build player")]
	static void OnMenuClicked()
	{
		OpenWindow<BuildPlayerWindow>(new BuildPlayerState_version());
	}
}