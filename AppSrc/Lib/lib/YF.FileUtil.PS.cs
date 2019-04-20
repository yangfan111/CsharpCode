using System.Collections;
using System.Collections.Generic;
using System;

namespace YF.FileUtil
{
    /// 获取某一特定路径字符串的前后层级关系信息 Path静态方法
    /*
     ChangeExtension
    Combine
    GetDirectoryName
    GetExtension
    GetFileName
    GetFileNameWithoutExtension 
    GetFullPath(String)
        Output is based on your current directory, except
        in the last case, where it is based on the root drive
        string path1 = @"mydir";
        string path2 = @"\mydir";
    GetPathRoot(String)
        // GetPathRoot('\mydir\') returns '\'
        // GetPathRoot('myfile.ext') returns ''
        // GetPathRoot('C:\mydir\myfile.ext') returns 'C:\'
    */


    /// <summary>
    /// badPath做路径替换
    /// URI用法：1.配合Path.GetFullPath使用
    ///         Path.GetFullPath(==>(new System.Uri(tmp)).LocalPath)
    ///          2.计算相对路径
    ///             1-fromUri,toUri = new URI(fromPath/toPath)
    ///             2-relativeUri = fromUri.MakeRelativeUri(toUri);
    ///             3-relativePath = System.Uri.UnescapeDataString(relativeUri.ToString())
    /// </summary>
    public static class PS
    {
        public static readonly char wrongSeparatorChar = System.IO.Path.DirectorySeparatorChar == '/' ? '\\' : '/';

        ///-path的规范化：1.badChar.Replace 2.barChar.Replcace.Trim()/TrimStart()
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
        ///3.System.IO.Path.GetFullPath(==>(new System.Uri(tmp)).LocalPath)
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
}