﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using System;
using System.Linq;

public class AssetBundleTools
{


    #region 批量 前缀 独立设置assetbundleName
    [MenuItem("Voyager/Build AssetBundle/批量assetbundle命名/maps_textures")]
    private static void SetSelectAssetBundleNames_MapsTextures() {
        SetSelectAssetBundleNames("maps_textures/");
    }
    private static void SetSelectAssetBundleNames(string head)
    {
        int count = 0;
        int total = Selection.objects.Length;
        foreach (var asset in Selection.objects)
        {
            if (EditorUtility.DisplayCancelableProgressBar("renameAseetbundle", "renameAseetbundle", (float)count / total)) {
                EditorUtility.ClearProgressBar();
                EditorUtility.DisplayDialog("操作中断", "中断 " + count + " 个设置", "确定");
                return;
            }
            var set = AssetImporter.GetAtPath(AssetDatabase.GetAssetPath(asset));
            if (set == null) continue;
            var newName = (head + asset.name).ToLower();
            if (!set.assetBundleName.Equals(newName))
            {
                count++;
                set.SetAssetBundleNameAndVariant(newName, null);
            }

        }
        EditorUtility.ClearProgressBar();
        EditorUtility.DisplayDialog("操作完成", "完成 " + count + " 个设置", "确定");
    }
    #endregion

    #region 分类打包
    private static string TargetDir = "../../../release2/";
    [MenuItem("Voyager/Build AssetBundle/分类打包 配置文件")]
    private static void BuildBundlesForConfig() {
        setOutput();
        BuildBundlesByAbName("allConfig","configuration", "terrains","tables");
    }
    [MenuItem("Voyager/Build AssetBundle/分类打包 非场景美术文件")]
    private static void BuildBundlesForArt()
    {
        setOutput();
        BuildBundlesByAbName("allArt", "attachment", "character","bag", "client", "common", "console","effect", "equipment","item","number","pickup","server","sound", "vehicles", "weapon", "wear", "uiprefabs","ui/");
    }
    [MenuItem("Voyager/Build AssetBundle/分类打包 大厅文件")]
    private static void BuildBundlesForHall()
    {
        setOutput();
        BuildBundlesByAbName("allHall", "hall", "icon", "template");
    }
    [MenuItem("Voyager/Build AssetBundle/分类打包 中小场景文件")]
    private static void BuildBundlesForMap()
    {
        setOutput();
        BuildBundlesByAbName("allMap", "level", "3dbackground", "map_models", "map_texutres");
    }
    [MenuItem("Voyager/Build AssetBundle/分类打包 吃鸡大场景文件")]
    private static void BuildBundlesForBigMap()
    {
        setOutput();
        BuildBundlesByAbName("allBigMap", "dynamicscene", "maps_textures");
    }

    [MenuItem("Voyager/Build AssetBundle/分类打包 building")]
    private static void BuildBundlesForProp1()
    {
        setOutput();
        BuildBundlesByAbName("allBuilding", "building");
    }
 
    [MenuItem("Voyager/Build AssetBundle/分类打包 SHADER")]
    private static void BuildBundlesForAll()
    {
        setOutput();
        BuildBundlesByAbName("allShader", "shaders");
    }

    [MenuItem("Voyager/Build AssetBundle/分类打包 临时选定的资源")]
    private static void BuildBundlesForTemp()
    {
        setOutput();
       
        BuildBundlesByAbName("allTemp", AssetDatabase.GetImplicitAssetBundleName(AssetDatabase.GetAssetPath(Selection.activeObject)));
    }
    private static void setOutput() {
        
        var args = System.Environment.GetCommandLineArgs();
        if (args.Length >= 2)
        {
            TargetDir = args[1];
          // Debug.Log("setOutput:" + args[1]);
        }
        

    }
    private static void BuildBundlesByAbName(string manifestName, params string[] groups)
    {
         
        HashSet<string> buildAbNames = new HashSet<string>();
        foreach (var item in AssetDatabase.GetAllAssetBundleNames())
        {
         
            if (groups.Any(x => item.StartsWith(x))) {
                Debug.Log("built item: " + item);
                buildAbNames.Add(item);
            }

        }

        
    
        List<AssetBundleBuild> assetBundleBuilds = new List<AssetBundleBuild>();
 
      
        foreach (var o in buildAbNames)
        {
  
            AssetBundleBuild build = new AssetBundleBuild();
            build.assetBundleName = o;
            build.assetNames = AssetDatabase.GetAssetPathsFromAssetBundle(build.assetBundleName);
            assetBundleBuilds.Add(build);
        }
        // AssetBundles.Utility.AssetBundlesOutputPath = "f";
        // Choose the output path according to the build target.
        var pfName = VoyagerMenu.GetPlatformName();
        string outputPath = Path.GetFullPath(Path.Combine(TargetDir + "AssetBundles",pfName));
        if (!Directory.Exists(outputPath))
            Directory.CreateDirectory(outputPath);
        var manifest = BuildPipeline.BuildAssetBundles(outputPath, assetBundleBuilds.ToArray(), BuildAssetBundleOptions.ChunkBasedCompression, BuildTarget.StandaloneWindows64);
        if (manifest == null)
            throw new Exception("Build Asset Bundle fail, for manifest is null");
        //Path.GetFullPath(Path.Combine(outputPath + "AssetBundles", pfName));
       
        FileUtil.ReplaceFile(Path.Combine(outputPath, pfName), Path.Combine(outputPath, manifestName));
        FileUtil.ReplaceFile(Path.Combine(outputPath, pfName+ ".manifest"), Path.Combine(outputPath, manifestName+ ".manifest"));
        FileUtil.DeleteFileOrDirectory(Path.Combine(outputPath, pfName));
        FileUtil.DeleteFileOrDirectory(Path.Combine(outputPath, pfName+".manifest"));
        Debug.Log("Info: " + "Build Assets at " + outputPath + ", result: " + manifest.GetAllAssetBundles().Length + ". OK");
        EditorUtility.DisplayDialog("Info", "Build Assets at " + outputPath + ", result" + manifest.GetAllAssetBundles().Length, "OK");
    }
    #endregion

    #region 根据excel等 分类数据 设置assetbundleName
    [MenuItem("Voyager/Build AssetBundle/批量assetbundle命名/从剪贴板数据设置")]
    private static void SetSelectAssetBundleNames_ClipDatas()
    {
        var lines = GUIUtility.systemCopyBuffer.Split("\n".ToArray());
        var path = lines[0].Split("\t".ToArray())[0];
        var abNameHead= lines[0].Split("\t".ToArray())[2].TrimEnd('\r').ToLower();
        Debug.Log(path);
        string abName = null;
        int countExecute = 0, countUpdated = 0, countError = 0;
        int total = lines.Length - 1;
        for (int i = 1; i < lines.Length; i++)
        {
            if (EditorUtility.DisplayCancelableProgressBar("renameAseetbundle", "renameAseetbundle", (float)i / total))
            {
                EditorUtility.ClearProgressBar();
                EditorUtility.DisplayDialog("操作中断", "取消剩余的设置?", "确定");
                return;
            }
            var line = lines[i].Split("\t".ToArray());
            if (line.Length < 2 || line[0] == "") continue;
            var prefabName = line[0];
            line[2] = line[2].TrimEnd('\r');
            if (line[2] != "") abName =abNameHead+"/"+ line[2].ToLower();
            countExecute++;
            // Debug.Log(prefabName+"->"+ abName);
            SetSelectAssetBundleNames(path, prefabName, abName, ref countError, ref countUpdated);
        }
        EditorUtility.ClearProgressBar();
      EditorUtility.DisplayDialog("操作完成", "完成 执行:"+ countExecute+",修改:"+countUpdated+",错误:" + countError + ";错误信息看日志", "确定");

    }
        private static void SetSelectAssetBundleNames(string path,string prefabName,string abName,ref int countError,ref int countUpdated)
        {
            
         
          
                var set = AssetImporter.GetAtPath(path+"/"+ prefabName+".prefab");
        if (set == null)
        {
            Debug.LogError(path + "/" + prefabName + ".prefab" + " not found!");
            countError++;
            return;
        }
                var newName = abName.ToLower();
                if (!set.assetBundleName.Equals(newName))
                {
            countUpdated++;
                    set.SetAssetBundleNameAndVariant(newName, null);
                }

            
       
        

    }

    #endregion

}
