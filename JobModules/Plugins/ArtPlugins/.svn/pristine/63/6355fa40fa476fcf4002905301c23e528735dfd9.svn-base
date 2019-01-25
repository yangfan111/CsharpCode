using UnityEngine;
using System.Collections;
using System.IO;
using System.Runtime.CompilerServices;
using UnityEditor;
using UnityEditor.Callbacks;

public class CopyConfigPostProcessor 
{
    [PostProcessBuild(1000)]
    public static void OnPostprocessBuild(BuildTarget target, string pathToBuiltProject)
    {
        Copy(pathToBuiltProject, "Config","Config");
        Copy(pathToBuiltProject, "GameData","GameData");

    }

    private static void Copy(string pathToBuiltProject, string srcRelativePath, string destRelativePath)
    {
        var srcDir = Application.dataPath;
        var destDir = pathToBuiltProject.Replace(".exe", "_Data");
        var src = Path.Combine(srcDir, srcRelativePath);
        var dest = Path.Combine(destDir, destRelativePath);
        if (Directory.Exists(src) || File.Exists(src))
        {
            FileUtil.CopyFileOrDirectory(src, dest);
            Debug.LogFormat("copy from {0} to {1}", src, dest);
        }
        else
        {
            Debug.LogFormat("{0} not exists", src);
        }
    }
}