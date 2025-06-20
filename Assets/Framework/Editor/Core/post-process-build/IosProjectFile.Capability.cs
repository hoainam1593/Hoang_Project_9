using UnityEditor.iOS.Xcode;

#if UNITY_IOS
using AppleAuth.Editor;
#endif

public partial class IosProjectFile
{
    private ProjectCapabilityManager projectCapability;

    const string entitlementsFileName = "Unity-iPhone.entitlements";
    
    private void ConstructCapability()
    {
        var targetId = project.GetUnityMainTargetGuid();
        projectCapability = new ProjectCapabilityManager(projectPath, entitlementsFileName, targetName: null, targetId);
    }

    public void AddCapabilityGameCenter()
    {
        projectCapability.AddGameCenter();

        var unityMainTargetId = project.GetUnityMainTargetGuid();
        project.AddCapability(unityMainTargetId, PBXCapabilityType.GameCenter, entitlementsFileName);
    }

    public static void AddCapabilitySignInWithApple(string path)
    {
        var file = new IosProjectFile(path);
        var targetId = file.project.GetUnityMainTargetGuid();
        file.projectCapability = new ProjectCapabilityManager(file.projectPath, "Entitlements.entitlements", targetName: null, targetId);

        targetId = file.project.GetUnityFrameworkTargetGuid();
#if UNITY_IOS
        file.projectCapability.AddSignInWithAppleWithCompatibility();
#endif

        file.SaveCapability();
    }

    private void SaveCapability()
    {
        projectCapability.WriteToFile();
    }
}
