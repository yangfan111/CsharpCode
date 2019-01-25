using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class PicClean
{
    [MenuItem("Tools/UI/清理大厅Prefabs中没用到的图片")]
    static void UIExport()
    {
        Dictionary<string, int> usedPic = new Dictionary<string, int>();
        string[] ids = AssetDatabase.FindAssets("t:Prefab", new string[] { "Assets/ArtSubmit/Hall/Prefabs", "Assets/ArtSubmit/Ui" });
        for (int i = 0; i < ids.Length; i++)
        {
            string[] paths = AssetDatabase.GetDependencies(new string[] { AssetDatabase.GUIDToAssetPath(ids[i])});
            foreach (string path in paths)
            {
                if (path.EndsWith(".png") || path.EndsWith(".jpg") || path.EndsWith(".psd"))
                {
                    if (!usedPic.ContainsKey(path))
                    {
                        usedPic.Add(path, 1);
                    }
                    else
                    {
                        usedPic[path]++;
                    }
                }
            }
        }
        List<string> allPic = new List<string>();

        //大厅图片文件夹//
        string[] hallPicPaths = Directory.GetFiles("Assets/ArtSubmit/Hall/Modules", "*.*", SearchOption.AllDirectories).Where(s => s.EndsWith(".png") || s.EndsWith(".jpg") || s.EndsWith(".psd")).ToArray();
        allPic.AddRange(hallPicPaths);

        //暂时不加入战斗图片文件夹//
        //string[] battlePicPaths = Directory.GetFiles("Assets/ArtSubmit/Ui/UiRes", "*.*", SearchOption.AllDirectories).Where(s => s.EndsWith(".png") || s.EndsWith(".jpg") || s.EndsWith(".psd")).ToArray();
        //allPic.AddRange(battlePicPaths);

        List<string> usedPathList = new List<string>();
        List<string> unusedPathList = new List<string>();
        foreach (string path in allPic)
        {
            string newPath = path.Replace(@"\", "/");
            if (usedPic.ContainsKey(newPath))
            {
                //统计多次使用的图片方便后期做图集规划//
                if (usedPic[newPath] >= 3)
                {
                    usedPathList.Add(newPath);
                }
            }
            else
            {
                var importer = AssetImporter.GetAtPath(newPath);
                if(string.IsNullOrEmpty(importer.assetBundleName))
                {
                    unusedPathList.Add(newPath);
                }
            }
        }
        AssetDatabase.Refresh();

        foreach(var path in usedPathList)
        {
            Debug.Log("图片: " + path + " 使用次数" + usedPic[path]);
        }

        foreach (var path in unusedPathList)
        {
            Debug.Log("删除未使用图片: " + path);
            File.Delete(path);
        }

        Debug.Log("清理完毕！");

    }
}
