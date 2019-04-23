using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class LocalAssetsModeEditor : EditorWindow {

    public static string[] allAssetBundleNames;
    private static Vector2 scrollPos;
    public static HashSet<string> alwaysLocalAssetBundleNames;
      
    [MenuItem("Assets/AssetBundles/localAssetsSetting")]
    public static void init() {
        GetWindow<LocalAssetsModeEditor>().Show();
        allAssetBundleNames= AssetDatabase.GetAllAssetBundleNames();
        alwaysLocalAssetBundleNames = new HashSet<string>();
        alwaysLocalAssetBundleNames.Add("tables");
        foreach (var item in allAssetBundleNames) {
            if (item.StartsWith("configuration")) {
                alwaysLocalAssetBundleNames.Add(item);
            }
        }
            foreach (var item in alwaysLocalAssetBundleNames)
        {
            EditorPrefs.SetBool("LCAB_" + item, true);
        }
    }
    private void OnGUI()
    {
        if (allAssetBundleNames == null)
        {
            Close();
            return;
        }
        bool all_sim = EditorGUILayout.Toggle("全部本地模拟", EditorPrefs.GetBool("LCAB_ALL_SIM", false));
        EditorPrefs.SetBool("LCAB_ALL_SIM", all_sim);
        if (all_sim) {
          
            return;
        }
      
        scrollPos = GUILayout.BeginScrollView(scrollPos);
        
        foreach (var item in allAssetBundleNames)
        {
            bool oldValue = EditorPrefs.GetBool("LCAB_" + item, false);

         bool  newValue=  GUILayout.Toggle(oldValue, item);
            if (alwaysLocalAssetBundleNames.Contains(item)) newValue = true;
            if (newValue != oldValue) {
                EditorPrefs.SetBool("LCAB_" + item, newValue);
            }
        }
        GUILayout.EndScrollView();
    }
}
