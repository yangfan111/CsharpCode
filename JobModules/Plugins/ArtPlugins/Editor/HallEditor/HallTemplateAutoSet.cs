using UnityEditor;
using UnityEngine;

public class HallTemplateAutoSet : AssetPostprocessor
{
    static string templateUrl = "Assets/Assets/template/";
    static string templateAssetBundleName = "tables";
    static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
    {
        foreach (var str in importedAssets)
        {
            if(str.Contains(templateUrl) && str.EndsWith(".xml"))
            {
                AssetImporter importer = AssetImporter.GetAtPath(str);
                if(string.IsNullOrEmpty(importer.assetBundleName))
                {
                    importer.assetBundleName = templateAssetBundleName;
                    Debug.Log(str + "设置assetBundleName为" + templateAssetBundleName);
                }
            }
        }
        //AssetDatabase.SaveAssets();
        //AssetDatabase.Refresh();
    }

}