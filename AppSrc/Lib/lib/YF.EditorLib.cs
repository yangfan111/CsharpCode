using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEditor;
namespace YF
{
    public class EditorLib
    {
        /// <summary>
        /// 开始python应用
        /// 需要配置全局变量
        /// ProcessStartInfo
        ///  start.FileName = "python";
        ///  start.Arguments = (string)arg
        ///  start.UseShellExecute = false;
        /// start.RedirectStandardOutput = true;;
        /// ar process = System.Diagnostics.Process.Start(start))
        ///   process.WaitForExit();
        ///   (process.ExitCode == 0)
        /// </summary>
        /// <returns></returns>
        public static bool StartPythonProgram(string entryPath, object arg)
        {
            var start = new ProcessStartInfo();
            start.FileName = "python";
            if (arg.GetType() == typeof(string))
            {
                //  string s = (string)arg;
                start.Arguments = (string)arg;
            }
            else if (arg.GetType() == typeof(List<string>))
            {
                string acc = "";
                List<string> argList = (List<string>)arg;
                foreach (var s in argList)
                {
                    acc += s;
                    acc += ",";
                }
                acc.TrimEnd(',');
                start.Arguments = acc;
            }
            start.UseShellExecute = false;
            start.RedirectStandardOutput = true;
            //添加bar
            using (var process = System.Diagnostics.Process.Start(start))
            {
                process.WaitForExit();
                try
                {
                    //ExitCode throws InvalidOperationException if the process is hanging
                    if (process.ExitCode == 0)
                    {
                        return true;
                        //     EditorUtility.DisplayProgressBar(s_progTitle, progMsg, 1.0f);
                        //UnityEngine.Debug.Log(string.Format(
                        //    "WwiseUnity: SoundBank ID conversion succeeded. Find generated Unity script under {0}.", s_bankDir));
                    }
                    else
                        UnityEngine.Debug.LogError("WwiseUnity: Conversion failed.");

                    //AssetDatabase.Refresh();
                }
                catch (System.Exception ex)
                {
                    // AssetDatabase.Refresh();

                    //EditorUtility.ClearProgressBar();
                    //UnityEngine.Debug.LogError(string.Format(
                    //"WwiseUnity: SoundBank ID conversion process failed with exception: {}. Check detailed logs under the folder: Assets/Wwise/Logs.",
                    //    ex));
                }
                //移除bar
                return false;
            }
        }
        ///**********************************************platformName******************************************
        ///UNITY_STANDALONE_WIN,
        ///UNITY_ANDROID
        ///UNITY_IOS
        ///UNITY_EDITOR_WIN
        public static string GetPlatformNameByCompile()
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
        /// <summary>
        /// 根据打包平台获取对应总平台名
        /// </summary>
        /// <param name="target"></param>
        /// <returns></returns>
        public static string GetPlatformNameByBuildTarget(BuildTarget target)
        {
            var platformSubDir = string.Empty;
            //        GetCustomPlatformName(ref platformSubDir, target);
            if (!string.IsNullOrEmpty(platformSubDir))
                return platformSubDir;

            switch (target)
            {
                case BuildTarget.Android:
                    return "Android";

                case BuildTarget.iOS:
                case BuildTarget.tvOS:
                    return "iOS";

                case BuildTarget.StandaloneLinux:
                case BuildTarget.StandaloneLinux64:
                case BuildTarget.StandaloneLinuxUniversal:
                    return "Linux";

#if UNITY_2017_3_OR_NEWER
			case BuildTarget.StandaloneOSX:
#else
                    //case BuildTarget.StandaloneOSXIntel:
                    //case BuildTarget.StandaloneOSXIntel64:
                    //case BuildTarget.StandaloneOSXUniversal:
#endif
                    return "Mac";

                case (BuildTarget)39: // BuildTarget.Lumin
                    return "Lumin";

                case BuildTarget.PS4:
                    return "PS4";

                case BuildTarget.PSP2:
                    return "Vita";

                case BuildTarget.StandaloneWindows:
                case BuildTarget.StandaloneWindows64:
                case BuildTarget.WSAPlayer:
                    return "Windows";

                case BuildTarget.XboxOne:
                    return "XboxOne";

                case BuildTarget.Switch:
                    return "Switch";
            }

            return target.ToString();
        }
    }
    #region//scene 
    public class SceneLib
    {
        /// <summary>
        /// editor下代码创建场景
        /// UnityEditor.SceneManagement ==> EditorSceneManager
        ///                             ==> OtherThings
        /// 
        /// Engine下代码访问场景
        /// UnityEngine.SceneManagement
        /// SceneManager.GetActiveScene();
        /// </summary>
        /// <returns> UnityEngine.SceneManagement.Scene</returns>
        public static UnityEngine.SceneManagement.Scene GetCurrentScene()
        {
            return UnityEngine.SceneManagement.SceneManager.GetActiveScene();
        }
        /// <summary>
        /// 编译器环境下保存场景
        /// </summary>
        /// <param name="destSceneName">目标拷贝场景名</param>
        /// <param name="saveAsCopy">作为备份使用</param>
        /// <returns></returns>
        public static bool SaveCurrentScene(string destSceneName, bool saveAsCopy = false)
        {
            bool ret;
            if (destSceneName == null)
                ret = UnityEditor.SceneManagement.EditorSceneManager.SaveScene(GetCurrentScene());
            else
                ret = UnityEditor.SceneManagement.EditorSceneManager.SaveScene(GetCurrentScene(), destSceneName, saveAsCopy);
            return ret;
        }


    }
    #endregion
}

//    #region//editor uses---------------------------------------------------------------------------
//    ///dictionary复制，复制当前文件列表==>文件递归复制

//    ///刷新UnityEditor
//    public static void RepaintInspector()
//    {
//        var windows = UnityEngine.Resources.FindObjectsOfTypeAll<EditorWindow>();
//        foreach (var win in windows)
//            if (win.titleContent.text == "Inspector")
//                win.Repaint();
//    }
//    //runtime执行命令行 cmd:命令行文件，args：参数
//    public static string ExecuteCommandLine(string command, string arguments)
//    {
//        var process = new System.Diagnostics.Process();
//        process.StartInfo.FileName = command;
//        process.StartInfo.UseShellExecute = false;
//        process.StartInfo.RedirectStandardOutput = true;
//        process.StartInfo.CreateNoWindow = true;
//        process.StartInfo.Arguments = arguments;
//        process.Start();

//        // Synchronously read the standard output of the spawned process. 
//        var reader = process.StandardOutput;
//        var output = reader.ReadToEnd();

//        // Waiting for the process to exit directly in the UI thread. Similar cases are working that way too.

//        // TODO: Is it better to provide a timeout avoid any issues of forever blocking the UI thread? If so, what is
//        // a relevant timeout value for soundbank generation?
//        process.WaitForExit();
//        process.Close();

//        return output;
