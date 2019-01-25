using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


public class VoyagerMenuHelper
{

    public static bool DirectoryCopy(string sourceDirName, string destDirName, bool copySubDirs, string filter = "")
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
  
    public static void FixSlashes(ref string path, char separatorChar, char badChar, bool addTrailingSlash)
    {
        if (string.IsNullOrEmpty(path))
            return;

        path = path.Trim().Replace(badChar, separatorChar).TrimStart('\\');
        // Append a trailing slash to play nicely with Wwise
        if (addTrailingSlash && !path.EndsWith(separatorChar.ToString()))
            path += separatorChar;
    }


}
