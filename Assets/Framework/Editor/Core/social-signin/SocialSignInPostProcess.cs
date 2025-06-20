
using UnityEditor;
using UnityEditor.Callbacks;

public class SocialSignInPostProcess
{
    [PostProcessBuild]
    public static void OnPostProcessBuild(BuildTarget target, string path)
    {
        if (target != BuildTarget.iOS)
        {
            return;
        }

        var project = new IosProjectFile(path);

#if USE_GAME_CENTER_SIGNIN
        project.AddCapabilityGameCenter();
#endif

        project.Save();

#if USE_SIGNIN_WITH_APPLE
        IosProjectFile.AddCapabilitySignInWithApple(path);
#endif
    }
}
