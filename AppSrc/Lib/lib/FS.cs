using System.Collections;
using System.Collections.Generic;
using System;
using System.IO;

namespace YF.FileUtil
{
    /// <summary>
    // 目标文件数据读写:1.静态读取 2.构造FileStream读取
    ///File.ReadAllText(outPath)
    ///FileStream ...
    // 目标文件/文件夹信息操作: 1.静态读取 2.构造DirectoryInfo数据结构
    /// var dir = new DirectoryInfo(sourceDirName)
    /// var files[] = dir.GetFiles()
    /// ********fileInfo.CopyTo(temppath, true)********
    // 获取路径的文件夹名
    ///Path.GetDirectoryName
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

            var dirs             = dir.GetDirectories();

            // If the destination directory doesn't exist, create it. 
            if (System.IO.Directory.Exists(destDirName) && recoverEntirely)
            {
                System.IO.Directory.Delete(destDirName);

            }
            if (!System.IO.Directory.Exists(destDirName))
                System.IO.Directory.CreateDirectory(destDirName);
            // Get the files in the directory and copy them to the new location.
            var files            = dir.GetFiles();
            foreach (var file in files)
            {

                if (filter       == "" || System.IO.Path.GetExtension(file.Name) == filter)
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


        //Application.persistentDataPath /Audio/Generated/ 
        public static string GetPath_StreamingAsset_CustomFolder(string examplePath)
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
        //Application.persistentDataPath /Audio/Generated/ Windows 
        public static string GetPath_StreamingAsset_CustomFolder_PluginSubFolder()
        {
            var platformName = UnityEditorLib.GetPlatformNameByCompile();
            // Combine base path with platform sub-folder
            var platformBasePath = System.IO.Path.Combine(GetPath_StreamingAsset_CustomFolder("Audio/Generator"), platformName);
            FixSlashes(ref platformBasePath);
            return platformBasePath;
        }
        //Application.persistentDataPath /Audio/Generated/ Windows /Decode/xxxx
        public static string GetPath_StreamingAsset_CustomFolder_PluginSubFolder_FinalFolder(string finalFolderName)
        {
#if (UNITY_ANDROID || PLATFORM_LUMIN || UNITY_IOS || UNITY_SWITCH) && !UNITY_EDITOR
// This is for platforms that only have a specific file location for persistent data.
		return System.IO.Path.Combine(UnityEngine.Application.persistentDataPath,finalFolderName);
#else

            return System.IO.Path.Combine(GetPath_StreamingAsset_CustomFolder_PluginSubFolder(), finalFolderName);
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