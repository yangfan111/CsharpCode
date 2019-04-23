using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

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
        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void MyLog(object str, params object[] args)
        {
            UnityEngine.Debug.LogFormat("{0}{1}</color>", GetColorStr(DebugUtil.DebugColor.Blue), string.Format(str.ToString(), args));
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
        public static void Log(object str, DebugColor color ,params object[] args)
        {
//            DebugInConsole(str, color, args);
            
            //LogInUnity(str, color, args);
        }

        public static void DebugInConsole(object str, DebugColor color, params object[] args)
        {
            Console.WriteLine(str.ToString(), args);
        }
        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void LogInUnity(object str, DebugColor color, params  object[] args)
        {
            UnityEngine.Debug.LogFormat("{0}{1}</color>", GetColorStr(color), string.Format(str.ToString(), args));
        }
        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void LogInUnity(object str,  params object[] args)
        {
            LogInUnity(str, DebugColor.Default, args);
        }
        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void WarningInUnity(object str, DebugColor color, params  object[] args)
        {
            UnityEngine.Debug.LogWarningFormat("{0}{1}</color>", GetColorStr(color), string.Format(str.ToString(), args));
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
    }
}
