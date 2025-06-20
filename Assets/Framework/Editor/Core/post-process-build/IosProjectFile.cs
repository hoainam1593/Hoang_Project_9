using UnityEditor.iOS.Xcode;

public partial class IosProjectFile
{
    public enum TargetType
    {
        UnityFramework,
        UnityMain,
        UnityMainTest,
    }

    private string projectPath;
    private PBXProject project;
    
    public IosProjectFile(string buildPath)
    {
        projectPath = PBXProject.GetPBXProjectPath(buildPath);
        project = new PBXProject();
        project.ReadFromFile(projectPath);

        ConstructCapability();
    }

    public void AddFramework(TargetType targetType, string frameworkName)
    {
        var targetId = targetType switch
        {
            TargetType.UnityFramework => project.GetUnityFrameworkTargetGuid(),
            TargetType.UnityMain => project.GetUnityMainTargetGuid(),
            TargetType.UnityMainTest => project.GetUnityMainTestTargetGuid(),
            _ => null,
        };

        project.AddFrameworkToProject(targetId, frameworkName, weak: true);
    }

    public void AddLocalization(string localizationId, string localizationFolder)
    {
        var targetId = project.GetUnityMainTargetGuid();
        var guid = project.AddFolderReference(localizationFolder, localizationId);
        project.AddFileToBuild(targetId, guid);
    }

    public void Save()
    {
        SaveCapability();
        project.WriteToFile(projectPath);
    }
}