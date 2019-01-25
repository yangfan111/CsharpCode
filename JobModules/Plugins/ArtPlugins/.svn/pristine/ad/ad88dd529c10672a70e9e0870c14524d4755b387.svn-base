using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace Assets.Plugins.ArtPlugins.Editor
{
    public static class CommonEditorUtility
    {
        public static string GetFullPath(string path)
        {
            var dataPath = Application.dataPath;
            var assetsStr = "/Assets";
            var fullPath = dataPath.Substring(0, dataPath.Length - assetsStr.Length) + "/" + path;
            fullPath = fullPath.Replace("/", "\\");
            return fullPath;
        }
    
        public static string GetRelativePath(string fullPath)
        {
            var relativePath = fullPath.Replace(Application.dataPath.Replace("/", "\\"), "Assets");
            relativePath = relativePath.Replace("\\", "/");
            return relativePath;
        }

        public class PrefabInfo
        {
            public GameObject Go;
            public string RelativePath;
        }
        public static List<PrefabInfo> GetAllPrefabInSelectedFolder()
        {
            var id = Selection.activeInstanceID;
            var path = AssetDatabase.GetAssetPath(id);
            Debug.LogFormat("path is {0}", path);
            if(string.IsNullOrEmpty(path))
            {
                EditorUtility.DisplayDialog("Error", "请选择文件夹", "确定");
            }
            return GetAllPrefabsInFolder(path);
        }

        public static List<PrefabInfo> GetAllPrefabsInFolder(string path)
        {
            var prefabList = new List<PrefabInfo>();
            if (EditorUtility.DisplayDialog("Warning", string.Format("将要对文件夹{0}进行操作", path), "确定", "取消"))
            {
                var assets = AssetDatabase.LoadAllAssetsAtPath(path);
                var files = new List<string>();
                GetAllFilePath(CommonEditorUtility.GetFullPath(path), files); 
                Debug.LogFormat("{0} asset in {1}", files.Count, path);
                foreach(var filePath in files)
                {
                    var isPrefab = filePath.EndsWith(".prefab");
                    if(isPrefab)
                    {
                        var relativePath = CommonEditorUtility.GetRelativePath(filePath);
                        var go = AssetDatabase.LoadAssetAtPath<GameObject>(relativePath);
                        if(null != go)
                        {
                            prefabList.Add(new PrefabInfo { Go = go, RelativePath = relativePath});
                        }
                        else
                        {
                            Debug.LogErrorFormat("asset in path {0} is not prefab !", relativePath);
                        }
                    }
                }
            }
            return prefabList;
        }

        public static void GetAllFilePath(string rootDir, List<string> pathList)
        {
            if (!Directory.Exists(rootDir))
            {
                EditorUtility.DisplayDialog("error", string.Format("illegal directory : {0}", rootDir), "get it ");
                return;
            }
            var files = Directory.GetFiles(rootDir);
            foreach(var file in files)
            {
                pathList.Add(file);
            }
            var directories = Directory.GetDirectories(rootDir);
            foreach(var dir in directories)
            {
                GetAllFilePath(dir, pathList);
            }
        }
    }
}
