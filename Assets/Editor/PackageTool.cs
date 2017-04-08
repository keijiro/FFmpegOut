using UnityEngine;
using UnityEditor;

public class PackageTool
{
    [MenuItem("Package/Update Package")]
    static void UpdatePackage()
    {
        AssetDatabase.ExportPackage(
            new [] { "Assets/FFmpegOut", "Assets/StreamingAssets" },
            "FFmpegOut.unitypackage", ExportPackageOptions.Recurse
        );
    }
}
