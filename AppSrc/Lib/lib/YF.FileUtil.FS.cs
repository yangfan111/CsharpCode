using System.Collections;
using System.Collections.Generic;
using System;
namespace YF.FileUtil
{
    /// <summary>
    // 读取目标文件数据
    ///File.ReadAllText(outPath)
    ///FileStream
    // 获取文件/文件夹信息 静态/构造DirectoryInfo数据结构
    /// var dir = new DirectoryInfo(sourceDirName)
    /// var files[] = dir.GetFiles()
    /// ********fileInfo.CopyTo(temppath, true)********
    // 获取路径的文件夹名
    ///Path.GetDirectoryName
    // 获取文件名:1.通过DirectoryInfo,FileInfo去取 2.静态去取
    /// Directory.GetFiles(dirPath, filters, System.IO.SearchOption.AllDirectories)
    //分平台stream路径分类：
    ///Android/ios/switch/非Editor模式:Application.persistentDataPath + folder
    ///Windows/Editor模式:streamingAsset+对应路径
    //获取标准化的文件路径名
    ///path.Trim:移除空白字段
    ///addTrailingSlash:是否增加结尾"/"char分隔符
    ///TrimStartT('\\') 剔除左边
    ///path.EndsWith -> path+= char 剔除右边

    /// </summary>



    public static class FS
    {
        ///copy文件夹
        ///*******new DirectoryInfo(sourceDirName)
        ///dir.GetDirectories()
        ///     file.Exists(destDirName)
        ///         Directory.CreateDirectory(destDirName)
        ///*******dir.GetFiles()
        ///*******file.CopyTo(temppath, true);
        public static void FileOrDicretoryCopy(string sourceDirName, string destDirName, bool copySubDirs)
        {
            var fileName = System.IO.Path.GetFileName(sourceDirName);
            if(System.IO.Path.HasExtension(fileName))
            {
                System.IO.FileInfo fi = new System.IO.FileInfo(sourceDirName);
                fi.Attributes = System.IO.FileAttributes.Normal;
                fi.CopyTo(destDirName, true);
            }
            else
            {
                DirectoryCopy(sourceDirName, destDirName, copySubDirs);
            }
        }
        public static bool DirectoryCopy(string sourceDir, string destDir, bool copySubDirs)
        {
            return DirectoryCopy(sourceDir, destDir, copySubDirs, "", false);
        }
        public static bool DirectoryCopy(string sourceDir, string destDir, bool copySubDirs, string filter)
        {
            return DirectoryCopy(sourceDir, destDir, copySubDirs, filter, false);
        }

        public static bool DirectoryCopy(string sourceDirName, string destDirName, bool copySubDirs, string filter, bool recoverEntirely)
        {
            var dir = System.IO.Directory.CreateDirectory(sourceDirName);
            dir.Attributes = System.IO.FileAttributes.System;
            if (!dir.Exists)
            {
                Console.WriteLine("WwiseUnity: Source directory doesn't exist");
                return false;
            }

            var dirs = dir.GetDirectories();

            // If the destination directory doesn't exist, create it. 
            if (System.IO.Directory.Exists(destDirName) && recoverEntirely)
            {
                System.IO.Directory.Delete(destDirName);

            }
            if (!System.IO.Directory.Exists(destDirName))
                System.IO.Directory.CreateDirectory(destDirName);
            // Get the files in the directory and copy them to the new location.
            var files = dir.GetFiles();
            foreach (var file in files)
            {

                if (filter == "" || System.IO.Path.GetExtension(file.Name) == filter)
                {
                    var temppath = System.IO.Path.Combine(destDirName, file.Name);
                    file.CopyTo(temppath, true);
                }

            }

            // If copying subdirectories, copy them and their contents to new location. 
            if (copySubDirs)
            {
                foreach (var subdir in dirs)
                {
                    var temppath = System.IO.Path.Combine(destDirName, subdir.Name);
                    DirectoryCopy(subdir.FullName, temppath, copySubDirs);
                }
            }

            return true;
        }

        ///获取指定文件夹，指定规则的文件夹列表
        /// System.IO.Directory.GetFiles(dirPath, filters, System.IO.SearchOption.AllDirectories);
        public static string[] GetDirFileNames(string dirPath, string filters)
        {
            var fileNames = System.IO.Directory.GetFiles(dirPath, filters, System.IO.SearchOption.AllDirectories);
            return fileNames;
        }
        ///获取appdata文件夹目标文件名是否存在
        ///File.Exists
        public static bool SeekAppDataDirTargetFile(string fileRelativePath)
        {
            return (System.IO.File.Exists(System.IO.Path.Combine(UnityEngine.Application.dataPath, fileRelativePath)));

        }
        #region//Unity定制
        ///获取app文件夹目标文件名
        ///Path.GetDirectoryName
        public static string[] GetAppProjDirFileNames(string filters)
        {
            var projectDir = System.IO.Path.GetDirectoryName(UnityEngine.Application.dataPath);
            return GetDirFileNames(projectDir, filters);
        }
        public static string AppRootPath { get { return System.IO.Path.GetDirectoryName(UnityEngine.Application.dataPath); } }


        ///***************************step1:获取Application.streamingAssetsPath|""/custom///***************************
        ///**********************************************streamPath******************************************
        ///1，获取指定平台目标路径：【streamPath】
        ///android非编译器平台：stream目录只写根目录
        ///其他平台：正常streamingAssetsPath目录
        public static string GetPath_PlatformStream(string examplePath)
        {
            // Get full path of base path
#if UNITY_ANDROID && !UNITY_EDITOR
 		var fullBasePath = examplePath;
#else
            var fullBasePath = System.IO.Path.Combine(UnityEngine.Application.streamingAssetsPath, examplePath);
#endif

#if UNITY_SWITCH
		if (fullBasePath.StartsWith("/"))
			fullBasePath = fullBasePath.Substring(1);
#endif
            FixSlashes(ref fullBasePath);
            return fullBasePath;
        }
        ///**********************************************step2:获取../platformName******************************************
        ///【streamPath】 + 【custom】+ 【platformName】 
        public static string GetPath_PlatformStream_Custom_PlatormSubfolder()
        {
            var platformName = EditorLib.GetPlatformNameByCompile();
            // Combine base path with platform sub-folder
            var platformBasePath = System.IO.Path.Combine(GetPath_PlatformStream("Audio/Generator"), platformName);
            FixSlashes(ref platformBasePath);
            return platformBasePath;
        }
        ///**********************************************step3:获取../finalFolder******************************************
        ///本地文件最终的读取位置
        ///Android/ios/switch/非Editor模式:Application.persistentDataPath + folder
        ///Windows/Editor模式:streamPath+custom+platormFolder+finalFolderName
        public static string GetStreamFinalPath(string finalFolderName)
        {
#if (UNITY_ANDROID || PLATFORM_LUMIN || UNITY_IOS || UNITY_SWITCH) && !UNITY_EDITOR
// This is for platforms that only have a specific file location for persistent data.
		return System.IO.Path.Combine(UnityEngine.Application.persistentDataPath,finalFolderName);
#else
            return System.IO.Path.Combine(GetPath_PlatformStream_Custom_PlatormSubfolder(), finalFolderName);
#endif
        }
  
        public static void FixSlashes(ref string path)
        {
#if UNITY_WSA
		var separatorChar = '\\';
#else
            var separatorChar = System.IO.Path.DirectorySeparatorChar;
#endif // UNITY_WSA
            var badChar = separatorChar == '\\' ? '/' : '\\';
            FixSlashes(ref path, separatorChar, badChar, true);
        }
        ///获取标准化的文件路径名
        ///path.Trim:移除空白字段
        ///TrimStartT('\\') 剔除左边
        ///path.EndsWith -> path+= char 剔除右边
        ///addTrailingSlash:是否增加结尾"/"char分隔符
        public static void FixSlashes(ref string path, char separatorChar, char badChar, bool addTrailingSlash)
        {
            if (string.IsNullOrEmpty(path))
                return;

            path = path.Trim().Replace(badChar, separatorChar).TrimStart('\\');
            // Append a trailing slash to play nicely with Wwise
            if (addTrailingSlash && !path.EndsWith(separatorChar.ToString()))
                path += separatorChar;
        }

 

        #endregion
    }
}