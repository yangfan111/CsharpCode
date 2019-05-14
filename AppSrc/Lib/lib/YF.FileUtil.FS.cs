using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine.Windows;
using Directory = System.IO.Directory;

namespace YF.FileUtil
{
  
    /// <summary>
    /// 文件浅拷贝：找到sourceDir的所有FileInfo，然后执行~.copyTo(DestDirPath+fileName)
    /// 文件深拷贝: 找到sourceDir的所有DirectInfo，再次执行文件浅拷贝
    /// 获取文件信息 1.通过DirectoryInfo,FileInfo去取 2.静态去取
    ///->获取指定文件夹，指定规则的文件信息列表
    /// new System.IO.DirectoryInfo(sourceDirName).GetFiles(dirPath)
    ///->获取指定文件夹，指定规则的文件名列表
    /// System.IO.Directory.GetFiles(dirPath, filters, System.IO.SearchOption.AllDirectories);
    /// 获取某一特定路径字符串的前后层级关系信息 Path静态方法
    /// </summary>
    
  
    
    /// <summary>
    ///二进制流的存放路径：1+3
    /// Unity Streaming 路径/ 自定义Root文件夹(fst) /平台名 (scd)/ 自定义数据文件夹(final)
    /// </summary>


    
    /// <summary>
    ///获取标准化的文件路径名
    /// 替换badChar
    ///1.path.Trim:移除空白字段
    ///2.TrimStartT('\\') 剔除左边
    ///3.path+char添加右边
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
            if (System.IO.Path.HasExtension(fileName))
            {
                System.IO.FileInfo fi = new System.IO.FileInfo(sourceDirName);
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
        /// <summary>
        ///文件浅拷贝：找到sourceDir的所有FileInfo，然后执行~.copyTo(DestDirPath+fileName)
        ///文件深拷贝: 找到sourceDir的所有DirectInfo，再次执行文件浅拷贝
        /// </summary>
      
        public static bool DirectoryCopy(string sourceDirName, string destDirName, bool copySubDirs, string filter,
            bool recoverEntirely)
        {
            // Get the subdirectories for the specified directory.
            var dir = new System.IO.DirectoryInfo(sourceDirName);

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

        #region//Unity定制标准化文件夹名

        ///获取app文件夹目标文件名
        ///Path.GetDirectoryName
        public static string[] GeAppRootPathFileNames(string filters)
        {
            var projectDir = System.IO.Path.GetDirectoryName(UnityEngine.Application.dataPath);
            return GetDirFileNames(projectDir, filters);
        }

        public static string AppRootPath
        {
            get { return System.IO.Path.GetDirectoryName(UnityEngine.Application.dataPath); }
        }

        private static string FstRootDir = "Example";

        /// <summary>
        ///二进制流的存放路径：1+3
        /// Unity Streaming 路径/ 自定义Root文件夹(fst) /平台名 (scd)/ 自定义数据文件夹(final)
        /// </summary>
        /// <returns></returns>
        private static string GetStreamingPath()
        {
#if (UNITY_ANDROID || PLATFORM_LUMIN || UNITY_IOS || UNITY_SWITCH) && !UNITY_EDITOR
            return Application.persistentDataPath;
#else
            return UnityEngine.Application.streamingAssetsPath;
#endif
        }

        public static string GetStreamingPath_FstRoot(string examplePath)
        {
            string fullPath;
            var fontPath = GetStreamingPath();
            if (string.IsNullOrEmpty(fontPath))
                fullPath = examplePath;
            else
                fullPath = System.IO.Path.Combine(fontPath, examplePath);

            FixSlashes(ref fullPath);
            return fullPath;
        }

        public static string GetStreamingPath_ScdRoot(string fontPath)
        {
            var platformName = EditorLib.GetPlatformNameByCompile();
            // Combine base path with platform sub-folder
            var platformBasePath = System.IO.Path.Combine(fontPath, platformName);
            FixSlashes(ref platformBasePath);
            return platformBasePath;
        }

        public static string GetStreamingPath_Final(string finalDirName)
        {
            var font = GetStreamingPath_FstRoot(FstRootDir);
            var scd = GetStreamingPath_ScdRoot(font);
            var platformBasePath = System.IO.Path.Combine(scd, finalDirName);
            FixSlashes(ref platformBasePath);
            return platformBasePath;
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
        /// 替换badChar
        ///1.path.Trim:移除空白字段
        ///2.TrimStartT('\\') 剔除左边
        ///3.addTrailingSlash:是否增加结尾"/"char分隔符
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