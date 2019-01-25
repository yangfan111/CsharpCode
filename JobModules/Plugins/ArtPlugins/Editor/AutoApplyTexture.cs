using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class AutoApplyTexture
{
    [MenuItem("Assets/Voyager/Weapons/设定材质参数")]
    public static void DoFunction()
    {
        var selectedAssets = Selection.GetFiltered<Object>(SelectionMode.DeepAssets);
        foreach (var asset in selectedAssets)
        {
            if (asset is Material)
            {
                SetMaterial(asset as Material);
            }
        }

        EditorUtility.DisplayDialog("操作完成", "完成！", "确定");
    }

    private static void SetMaterial(Material mtl)
    {
        var mtlPath = AssetDatabase.GetAssetPath(mtl);
        var mtlName = mtlPath.Substring(mtlPath.LastIndexOf('/') + 1);
        mtlName = mtlName.Substring(0, mtlName.IndexOf('.'));

        var slash = 0;
        for (int i = mtlPath.Length - 1; i >= 0; i--)
        {
            if (mtlPath[i] == '/')
            {
                slash++;
                if (slash == 2)
                {
                    slash = i;
                    break;
                }
            }
        }
        var folder = mtlPath.Substring(0, slash);

        // Diffuse
        var diffuse = AssetDatabase.LoadAssetAtPath<Texture2D>(string.Format("{0}/textures/{1}_D.tga", folder, mtlName));
        if (diffuse == null)
        {
            diffuse = AssetDatabase.LoadAssetAtPath<Texture2D>(string.Format("{0}/Textures/{1}_D.tga", folder, mtlName));
        }
        mtl.SetTexture("_MainTex", diffuse);

        // Normal
        var normal = AssetDatabase.LoadAssetAtPath<Texture2D>(string.Format("{0}/textures/{1}_N.tga", folder, mtlName));
        if (normal == null)
        {
            normal = AssetDatabase.LoadAssetAtPath<Texture2D>(string.Format("{0}/Textures/{1}_N.tga", folder, mtlName));
        }
        mtl.SetTexture("_BumpMap", normal);

        // Metallic
        var metallic = AssetDatabase.LoadAssetAtPath<Texture2D>(string.Format("{0}/textures/{1}_M.tga", folder, mtlName));
        if (metallic == null)
        {
            metallic = AssetDatabase.LoadAssetAtPath<Texture2D>(string.Format("{0}/Textures/{1}_M.tga", folder, mtlName));
        }
        mtl.SetTexture("_MetallicGlossMap", metallic);
        
        // Occlusion
        var occlusion = AssetDatabase.LoadAssetAtPath<Texture2D>(string.Format("{0}/textures/{1}_AO.tga", folder, mtlName));
        if (occlusion == null)
        {
            occlusion = AssetDatabase.LoadAssetAtPath<Texture2D>(string.Format("{0}/Textures/{1}_AO.tga", folder, mtlName));
        }
        mtl.SetTexture("_OcclusionMap", occlusion);

    }
}
