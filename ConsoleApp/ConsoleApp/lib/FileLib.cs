using System.Collections;
using System.Collections.Generic;
using System;
namespace YF.FileUtil
{
    public static class PT
    {
        public static readonly char wrongSeparatorChar = System.IO.Path.DirectorySeparatorChar == '/' ? '\\' : '/';

        ///Path.Replcace
        public static string NormalizePath(string in_path)
        {
            if (string.IsNullOrEmpty(in_path))
                return "";
            //var wrongSeparatorChar = System.IO.Path.DirectorySeparatorChar == '/' ? '\\' : '/';
            return in_path.Replace(wrongSeparatorChar, System.IO.Path.DirectorySeparatorChar);

        }
        ///System.IO.Path.GetFullPath(Uri.LocalPath)
        ///str.Replace
        public static string GetFullNormalizedPath(string str)
        {
            str = System.IO.Path.GetFullPath(new System.Uri(str).LocalPath);
            return str.Replace(wrongSeparatorChar, System.IO.Path.DirectorySeparatorChar);
        }
        ///获取文件完整路径(基础，相对)
        ///1.Path1,Path2 = str.Replace()
        ///2.var tmp =Path.Combine(Path1,Path2)
        ///3.System.IO.Path.GetFullPath((new System.Uri(tmp)).LocalPath)
        public static string GetFullPath(string BasePath, string RelativePath)
        {
            string tmpString;
            if (string.IsNullOrEmpty(BasePath))
                return "";

            var wrongSeparatorChar = System.IO.Path.DirectorySeparatorChar == '/' ? '\\' : '/';

            if (string.IsNullOrEmpty(RelativePath))
                return BasePath.Replace(wrongSeparatorChar, System.IO.Path.DirectorySeparatorChar);

            if (System.IO.Path.GetPathRoot(RelativePath) != "")
                return RelativePath.Replace(wrongSeparatorChar, System.IO.Path.DirectorySeparatorChar);

            tmpString = System.IO.Path.Combine(BasePath, RelativePath);
            tmpString = System.IO.Path.GetFullPath(new System.Uri(tmpString).LocalPath);

            return tmpString.Replace(wrongSeparatorChar, System.IO.Path.DirectorySeparatorChar);
        }
        ///获取相对路径
        ///1.fromUri,toUri = new URI(fromPath/toPath)
        ///2.relativeUri = fromUri.MakeRelativeUri(toUri);
        ///3.relativePath = System.Uri.UnescapeDataString(relativeUri.ToString())
        public static string MakeRelativePath(string fromPath, string toPath)
        {

            fromPath += "/fake_depth";
            try
            {
                if (string.IsNullOrEmpty(fromPath))
                    return toPath;

                if (string.IsNullOrEmpty(toPath))
                    return "";

                var fromUri = new System.Uri(fromPath);
                var toUri = new System.Uri(toPath);

                if (fromUri.Scheme != toUri.Scheme)
                    return toPath;

                var relativeUri = fromUri.MakeRelativeUri(toUri);
                //UnescapeDataString:转换为非转义字符串形式
                var relativePath = System.Uri.UnescapeDataString(relativeUri.ToString());

                return relativePath;
            }
            catch
            {
                return toPath;
            }
        }
    }



    public static class FL
    {
        ///copy文件夹
        ///new DirectoryInfo(sourceDirName)
        ///dir.GetDirectories()
        ///file.Exists(destDirName)
            /// Directory.CreateDirectory(destDirName)
        ///dir.GetFiles()
        ///file.CopyTo(temppath, true);
        public static bool DirectoryCopy(string sourceDirName, string destDirName, bool copySubDirs)
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
            if (!System.IO.Directory.Exists(destDirName))
                System.IO.Directory.CreateDirectory(destDirName);

            // Get the files in the directory and copy them to the new location.
            var files = dir.GetFiles();
            foreach (var file in files)
            {
                var temppath = System.IO.Path.Combine(destDirName, file.Name);
                file.CopyTo(temppath, true);
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
        ///Path.GetDirFileNames
        public static string[] GetAppProjDirFileNames(string filters)
        {
            var projectDir = System.IO.Path.GetDirectoryName(UnityEngine.Application.dataPath);
            return GetDirFileNames(projectDir, filters);
        }
        public static string AppRootPath { get { return System.IO.Path.GetDirectoryName(UnityEngine.Application.dataPath); } }

        ///获取当前平台名:
        ///UNITY_STANDALONE_WIN,
        ///UNITY_ANDROID
        ///UNITY_IOS
        ///UNITY_EDITOR_WIN
        public static string GetUnityPlatformName()
        {
            string platformSubDir = string.Empty;
#if UNITY_EDITOR_WIN || UNITY_STANDALONE_WIN || UNITY_WSA
		platformSubDir = "Windows";
#elif UNITY_EDITOR_OSX || UNITY_STANDALONE_OSX
		platformSubDir = "Mac";
#elif UNITY_STANDALONE_LINUX
		platformSubDir = "Linux";
#elif UNITY_XBOXONE
		platformSubDir = "XBoxOne";
#elif UNITY_IOS || UNITY_TVOS
		platformSubDir = "iOS";
#elif UNITY_ANDROID
		platformSubDir = "Android";
#elif PLATFORM_LUMIN
		platformSubDir = "Lumin";
#elif UNITY_PS4
		platformSubDir = "PS4";
#elif UNITY_WP_8_1
		platformSubDir = "WindowsPhone";
#elif UNITY_SWITCH
		platformSubDir = "Switch";
#elif UNITY_PSP2
#if AK_ARCH_VITA_SW || !AK_ARCH_VITA_HW
		platformSubDir = "VitaSW";
#else
		platformSubDir = "VitaHW";
#endif
#else
            platformSubDir = "Undefined platform sub-folder";
#endif
            return platformSubDir;
        }
        ///本地文件最终的读取位置
        ///Android/ios/switch:Application.persistentDataPath + folder
        ///Windows:Application.streamingAssetsPath + folder

        public static string GetFinalPathName(string finalFolderName)
        {
#if (UNITY_ANDROID || PLATFORM_LUMIN || UNITY_IOS || UNITY_SWITCH) && !UNITY_EDITOR
// This is for platforms that only have a specific file location for persistent data.
		return System.IO.Path.Combine(UnityEngine.Application.persistentDataPath,finalFolderName);
#else
            return System.IO.Path.Combine(GetPlatformBasePath(), finalFolderName);
#endif
        }
        public static string GetPlatformBasePath()
        {
            var platformName = GetUnityPlatformName();
            // Combine base path with platform sub-folder
            var platformBasePath = System.IO.Path.Combine(GetFullSoundBankPath(), platformName);
            FixSlashes(ref platformBasePath);
            return platformBasePath;
        }
        public static string GetFullSoundBankPath()
        {
            // Get full path of base path
            string examplePath = "Audio/Generator";
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
        public static void FixSlashes(ref string path, char separatorChar, char badChar, bool addTrailingSlash)
        {
            if (string.IsNullOrEmpty(path))
                return;

            path = path.Trim().Replace(badChar, separatorChar).TrimStart('\\');
            // Append a trailing slash to play nicely with Wwise
            if (addTrailingSlash && !path.EndsWith(separatorChar.ToString()))
                path += separatorChar;
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

        #endregion
    }
}