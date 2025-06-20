
#if USE_SERVER_AWS

using UnityEditor;

public class LoginAWSWindow : EditorWindowStateMachine
{
    [MenuItem("\u2726\u2726TOOLS\u2726\u2726/login AWS")]
    static void OnMenuClicked()
    {
        OpenWindow<LoginAWSWindow>(new LoginAWSState_main());
    }
}

#endif