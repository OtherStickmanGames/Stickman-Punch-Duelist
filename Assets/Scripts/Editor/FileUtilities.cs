using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public class FileUtilities
{
    [MenuItem("Utility/Files/BuildAreasBundle")]
    public static void BuildAreasBundle()
    {
        Debug.Log(Application.streamingAssetsPath);
        //string editorDir = Application.dataPath + "/RoadsBundle/Standalone";
        //Directory.CreateDirectory(editorDir);
        //BuildPipeline.BuildAssetBundles(editorDir, BuildAssetBundleOptions.None, BuildTarget.StandaloneWindows64);
        BuildPipeline.BuildAssetBundles(Application.streamingAssetsPath, BuildAssetBundleOptions.ChunkBasedCompression, BuildTarget.Android);
        AssetDatabase.Refresh();
    }

    [MenuItem("Utility/Files/Test")]
    public static void Test()
    {
        var assets = AssetDatabase.FindAssets("_", new[] { "Assets/Resources/Map/Generated/Areas" });

        foreach (var assetPath in assets)
        {
            GUID.TryParse(assetPath, out GUID guid);

            var path = AssetDatabase.GUIDToAssetPath(guid);
            var asset = AssetDatabase.LoadMainAssetAtPath(path);
            AssetDatabase.SetLabels(asset, new[] { "Vegetation" });
            AssetDatabase.Refresh();
            var labels = AssetDatabase.GetLabels(guid);   
        }
    }


    [MenuItem("Utility/Files/SetBundleName")]
    public static void SetBundleName()
    {
        var assetsGUIDs = AssetDatabase.FindAssets("_", new[] { "Assets/Resources/Map/Generated/Areas" });

        foreach (var assetGUID in assetsGUIDs)
        {
            var path = AssetDatabase.GUIDToAssetPath(assetGUID);
            var nameParts = path.Replace(".asset", "").Split('_');

            AssetImporter.GetAtPath(path).SetAssetBundleNameAndVariant(nameParts[1], "");

            AssetDatabase.Refresh();
        }
    }
}
