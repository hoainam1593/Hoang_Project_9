
using System.IO;
using UnityEditor.iOS.Xcode;

public class IosPlistFile
{
    private string plistPath;
    private PlistDocument plist;

    public IosPlistFile(string buildPath)
    {
        plistPath = $"{buildPath}/Info.plist";
        plist = new PlistDocument();
        plist.ReadFromString(File.ReadAllText(plistPath));
    }

    public void SetString(string key, string val)
    {
        plist.root.SetString(key, val);
    }

    public void Save()
    {
        File.WriteAllText(plistPath, plist.WriteToString());
    }
}