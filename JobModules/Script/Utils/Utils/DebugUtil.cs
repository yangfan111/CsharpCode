using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using UnityEngine;

namespace Core.Utils
{
    public static class DebugUtil
    {
        public static string GetBytesString(byte[] array)
        {
            return GetBytesString(array, array.Length, 0);
        }

        public static string GetBytesString(byte[] array, int length, int offset)
        {
            if (null == array)
            {
                return string.Empty;
            }

            if (length > array.Length || length < 0)
            {
                return string.Empty;
            }

            if (offset >= array.Length)
            {
                return string.Empty;
            }

            var sb = new StringBuilder();
            for (var i = offset; i < length; i++)
            {
                sb.AppendFormat("[{0}]-", array[i]);
            }

            return sb.ToString();
        }


        public static string GetStreamString(MemoryStream stream)
        {
            var sb = new StringBuilder();
            for (var i = 0; i < stream.Length; i++)
            {
                sb.AppendFormat("[{0}]-", stream.GetBuffer()[i]);
            }

            return sb.ToString();
        }


        public enum DebugColor
        {
            Default,
            Red,
            Green,
            Blue,
            Black,
            Grey
        }

        public static string ShootLocalDir =@"F:\voyager\UnityProjects\ShootArchive\";
        public static  string ShootArchivePathC = Path.Combine(ShootLocalDir,"ShootC") ;
        public static  string ShootArchivePathS =Path.Combine(ShootLocalDir,"ShootS") ;
        public static  string ArchieveFilePath;
        private static int    roomId;
        private static bool   shootDirExist;
        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void InitShootArchiveC(string owner, int in_roomId)
        {
            var fileName = string.Format("{0}.log", DateTime.Now.ToString("HH_mm"));
            ArchieveFilePath = Path.Combine(ShootArchivePathC, fileName);
            shootDirExist =  Directory.Exists(ShootLocalDir);
            if (shootDirExist && !Directory.Exists(ShootArchivePathC))
                Directory.CreateDirectory(ShootArchivePathC);
        }
        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void InitShootArchiveS(int in_roomId)
        {
            roomId = in_roomId;
            logger.InfoFormat("***************New Battle Session{0}{1}***************\n", roomId,
                DateTime.Now.ToString("HH:mm:ss"));
            var fileName = string.Format("{0}.log", DateTime.Now.ToString("HH_mm"));
            ArchieveFilePath = Path.Combine(ShootArchivePathS, fileName);
            shootDirExist      = Directory.Exists(ShootLocalDir);
            if (shootDirExist && !Directory.Exists(ShootArchivePathS))
                Directory.CreateDirectory(ShootArchivePathS);
        }

        public static LoggerAdapter logger = new LoggerAdapter("ShootArchive");
        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void AppendShootText(int cmdSeq, string str, params object[] args)
        {
            if (!DebugConfig.AppendShootArchiveText)
                return;
            string seqPre = string.Format("[CmdSeq]:{0},", cmdSeq);
            str = string.Format(str, args);
            AppendLocalText(cmdSeq,seqPre,str);
            //logger.InfoFormat("{0}{1}", seqPre, str);
           
        }
        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        private static void AppendLocalText(int cmdSeq,string seqPre,string str)
        {
            if (shootDirExist)
            {
                File.AppendAllText(ArchieveFilePath, seqPre + str + "\n");
            }
        }
        public static void AppendMatchedShootText(string shootKey, string clientData, string serverData)
        {
            string seqPre = string.Format("Match[{0}],", shootKey);
            logger.InfoFormat("{0}[Svr]{1}", seqPre, serverData);
            logger.InfoFormat("{0}[Cli]{1}\n", seqPre, clientData);
// #if UNITY_EDITOR
//             if (localAppend)
//                 File.AppendAllText(ShootArchivePath, seqPre + str + "\n");
// #endif
        }
        public static void AppendMissShootText(string shootKey, string valuableData,bool isServer)
        {
            string seqPre = string.Format("Miss[{0}],", shootKey);
            if(isServer)
                logger.InfoFormat("{0}[Svr]{1}", seqPre, valuableData);
            else 
                logger.InfoFormat("{0}[Cli]{1}", seqPre, valuableData);
            // #if UNITY_EDITOR
            //             if (localAppend)
            //                 File.AppendAllText(ShootArchivePath, seqPre + str + "\n");
            // #endif
        }
        
        
        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void MyLog(object str, params object[] args)
        {
            UnityEngine.Debug.LogFormat("{0}{1}</color>", GetColorStr(DebugUtil.DebugColor.Blue),
                string.Format(str.ToString(), args));
        }

        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void MyLog(object str, DebugColor color, params object[] args)
        {
            UnityEngine.Debug.LogFormat("{0}{1}</color>", GetColorStr(color), string.Format(str.ToString(), args));
        }

        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void LogWarning(object str, DebugColor color, params object[] args)
        {
            WarningInUnity(str, color, args);
        }

        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void Log(object str, DebugColor color, params object[] args)
        {
//            DebugInConsole(str, color, args);

            //LogInUnity(str, color, args);
        }

        public static void DebugInConsole(object str, DebugColor color, params object[] args)
        {
            Console.WriteLine(str.ToString(), args);
        }

        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void LogInUnity(object str, DebugColor color, params object[] args)
        {
            UnityEngine.Debug.LogFormat("{0}{1}</color>", GetColorStr(color), string.Format(str.ToString(), args));
        }

        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void LogInUnity(object str, params object[] args)
        {
            LogInUnity(str, DebugColor.Default, args);
        }

        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void WarningInUnity(object str, DebugColor color, params object[] args)
        {
            UnityEngine.Debug.LogWarningFormat("{0}{1}</color>", GetColorStr(color),
                string.Format(str.ToString(), args));
        }

        public static string GetColorStr(DebugColor color)
        {
            string colStr = "<color=#000000>";
            switch (color)
            {
                default:
                case DebugColor.Black:
                    colStr = "<color=#000000>";
                    break;
                case DebugColor.Blue:
                    colStr = "<color=#0000ff>";
                    break;
                case DebugColor.Green:
                    colStr = "<color=#00ff00>";
                    break;
                case DebugColor.Grey:
                    colStr = "<color=#888888>";
                    break;
                case DebugColor.Red:
                    colStr = "<color=#ff000>";
                    break;
            }

            return colStr;
        }

        public static bool WeakAssert(bool b, string msg)
        {
            if (!b)
            {
                logger.ErrorFormat("assert faield, cause: {0}\n{1}", msg, (new StackTrace()).ToString());
            }

            return b;
        }

    }
}