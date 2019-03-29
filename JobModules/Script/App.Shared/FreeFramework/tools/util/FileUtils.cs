using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Collections;

namespace commmons.util
{
    public class FileUtils
    {
        public static void SaveToFile(string content, string path)
        {
            FileStream fs = new FileStream(path, FileMode.Create);
            byte[] data = new UTF8Encoding().GetBytes(content);
            fs.Write(data, 0, data.Length);
            fs.Flush();
            fs.Close();
        }

        public static void DeleteFile(string path)
        {
            FileInfo file = new FileInfo(path);
            if (file.Exists)
            {
                file.Delete();
            }
        }

        public static byte[] GetFileStream(string filename)
        {
            FileStream fs = File.Open(filename, FileMode.Open, FileAccess.Read);
            BinaryReader din = new BinaryReader(fs, Encoding.UTF8);

            byte[] bs = din.ReadBytes((int)fs.Length);

            din.Close();

            return bs;
        }

        public static void copy(string srcdir, string desdir)
        {
            if (srcdir.EndsWith("/"))
            {
                srcdir = srcdir.Substring(0, srcdir.Length - 1);
            }
            if (desdir.EndsWith("/"))
            {
                desdir = desdir.Substring(0, srcdir.Length - 1);
            }

            string[] filenames = Directory.GetFileSystemEntries(srcdir);

            foreach (string f in filenames)// 遍历所有的文件和目录
            {
                string file = f.Replace("\\", "/");
                if (Directory.Exists(file))// 先当作目录处理如果存在这个目录就递归Copy该目录下面的文件
                {
                    string currentdir = desdir + "/" + file.Substring(file.LastIndexOf("/") + 1);
                    if (!Directory.Exists(currentdir))
                    {
                        Directory.CreateDirectory(currentdir);
                    }
                    copy(file, currentdir);
                }
                else
                {
                    string srcfileName = file.Substring(file.LastIndexOf("/") + 1);

                    srcfileName = desdir + "/" + srcfileName;

                    if (!Directory.Exists(desdir))
                    {
                        Directory.CreateDirectory(desdir);
                    }

                    File.Copy(file, srcfileName, true);
                }
            }
        }

        public static string GetFileContent(string filename)
        {
            return GetFileContent(filename, Encoding.UTF8);
        }

        public static string[] getAllFiles(string path)
        {
            Queue<FileSystemInfo> list = new Queue<FileSystemInfo>();
            List<String> files = new List<String>();
            DirectoryInfo dir = new DirectoryInfo(path);
            if (!dir.Exists)
            {
                dir.Create();
            }
            FileSystemInfo[] file = dir.GetFileSystemInfos();
            if (file == null)
            {
                return files.ToArray();
            }
            for (int i = 0; i < file.Length; i++)
            {
                if (file[i] is FileInfo)
                {
                    files.Add(file[i].FullName);
                }
                else
                {
                    list.Enqueue(file[i]);
                }
            }
            FileSystemInfo tmp;
            while (list.Count > 0)
            {
                tmp = list.Dequeue();
                if (tmp is DirectoryInfo)
                {
                    file = ((DirectoryInfo)tmp).GetFileSystemInfos();
                    if (file == null)
                    {
                        continue;
                    }
                    for (int i = 0; i < file.Length; i++)
                    {
                        if (file[i] is FileInfo)
                        {
                            files.Add(file[i].FullName);
                        }
                        else
                        {
                            list.Enqueue(file[i]);
                        }
                    }
                }
                else
                {
                    files.Add(tmp.FullName);
                }
            }

            return files.ToArray();
        }

        public static string GetFileContent(string filename, Encoding enconding)
        {
            StreamReader din = new StreamReader(File.Open(filename, FileMode.Open, FileAccess.Read, FileShare.ReadWrite), enconding);
            StringBuilder st = new StringBuilder();
            string str;

            while ((str = din.ReadLine()) != null)
            {
                st.Append(str);
                st.Append("\n");
            }

            din.Close();

            return st.ToString();
        }
    }
}
