using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
namespace YF
{
    public class EditorLib
    {
        /// <summary>
        /// 需要配置全局变量
        /// ProcessStartInfo
        /// </summary>
        /// <returns></returns>
        public static bool StartPythonProgram(string entryPath,object arg)
        {
            var start = new ProcessStartInfo();
            start.FileName = "python";
            if(arg.GetType()== typeof(string))
            {
              //  string s = (string)arg;
                start.Arguments = (string)arg;
            }
            else if(arg.GetType() == typeof(List<string>))
            {
                string acc = "";
                List<string>  argList = (List<string>)arg;
                foreach(var s in argList)
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
                   //     UnityEditor.EditorUtility.DisplayProgressBar(s_progTitle, progMsg, 1.0f);
                        //UnityEngine.Debug.Log(string.Format(
                        //    "WwiseUnity: SoundBank ID conversion succeeded. Find generated Unity script under {0}.", s_bankDir));
                    }
                    else
                        UnityEngine.Debug.LogError("WwiseUnity: Conversion failed.");

                    //UnityEditor.AssetDatabase.Refresh();
                }
                catch (System.Exception ex)
                {
                   // UnityEditor.AssetDatabase.Refresh();

                    //UnityEditor.EditorUtility.ClearProgressBar();
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
        public static string GetPlatformNameByBuildTarget(UnityEditor.BuildTarget target)
        {
            var platformSubDir = string.Empty;
    //        GetCustomPlatformName(ref platformSubDir, target);
            if (!string.IsNullOrEmpty(platformSubDir))
                return platformSubDir;

            switch (target)
            {
                case UnityEditor.BuildTarget.Android:
                    return "Android";

                case UnityEditor.BuildTarget.iOS:
                case UnityEditor.BuildTarget.tvOS:
                    return "iOS";

                case UnityEditor.BuildTarget.StandaloneLinux:
                case UnityEditor.BuildTarget.StandaloneLinux64:
                case UnityEditor.BuildTarget.StandaloneLinuxUniversal:
                    return "Linux";

#if UNITY_2017_3_OR_NEWER
			case UnityEditor.BuildTarget.StandaloneOSX:
#else
                //case UnityEditor.BuildTarget.StandaloneOSXIntel:
                //case UnityEditor.BuildTarget.StandaloneOSXIntel64:
                //case UnityEditor.BuildTarget.StandaloneOSXUniversal:
#endif
                    return "Mac";

                case (UnityEditor.BuildTarget)39: // UnityEditor.BuildTarget.Lumin
                    return "Lumin";

                case UnityEditor.BuildTarget.PS4:
                    return "PS4";

                case UnityEditor.BuildTarget.PSP2:
                    return "Vita";

                case UnityEditor.BuildTarget.StandaloneWindows:
                case UnityEditor.BuildTarget.StandaloneWindows64:
                case UnityEditor.BuildTarget.WSAPlayer:
                    return "Windows";

                case UnityEditor.BuildTarget.XboxOne:
                    return "XboxOne";

                case UnityEditor.BuildTarget.Switch:
                    return "Switch";
            }

            return target.ToString();
        }
    }
}

//    #region//editor uses---------------------------------------------------------------------------
//    ///dictionary复制，复制当前文件列表==>文件递归复制

//    ///刷新UnityEditor
//    public static void RepaintInspector()
//    {
//        var windows = UnityEngine.Resources.FindObjectsOfTypeAll<UnityEditor.EditorWindow>();
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
//    }
//    public static void SaveSettings(WwiseSettings Settings)
//	{
//		try
//		{
//			var xmlDoc = new System.Xml.XmlDocument();
//			var xmlSerializer = new System.Xml.Serialization.XmlSerializer(Settings.GetType());
//			using (var xmlStream = new System.IO.MemoryStream())
//			{
//				var streamWriter = new System.IO.StreamWriter(xmlStream, System.Text.Encoding.UTF8);
//				xmlSerializer.Serialize(streamWriter, Settings);
//				xmlStream.Position = 0;
//				xmlDoc.Load(xmlStream);
//				xmlDoc.Save(System.IO.Path.Combine(UnityEngine.Application.dataPath, WwiseSettingsFilename));
//			}
//		}
//		catch (System.Exception)
//		{
//		}
//	}

//	// Load the WwiseSettings structure from a serialized XML file
//	public static WwiseSettings LoadSettings(bool ForceLoad = false)
//	{
//		if (s_Instance != null && !ForceLoad)
//			return s_Instance;

//		var Settings = new WwiseSettings();
//		try
//		{
//			if (System.IO.File.Exists(System.IO.Path.Combine(UnityEngine.Application.dataPath, WwiseSettingsFilename)))
//			{
//				var xmlSerializer = new System.Xml.Serialization.XmlSerializer(Settings.GetType());
//				var xmlFileStream = new System.IO.FileStream(UnityEngine.Application.dataPath + "/" + WwiseSettingsFilename,
//					System.IO.FileMode.Open, System.IO.FileAccess.Read);
//				Settings = (WwiseSettings) xmlSerializer.Deserialize(xmlFileStream);
//				xmlFileStream.Close();
//			}
//			else
//			{
//				var projectDir = System.IO.Path.GetDirectoryName(UnityEngine.Application.dataPath);
//				var foundWwiseProjects = System.IO.Directory.GetFiles(projectDir, "*.wproj", System.IO.SearchOption.AllDirectories);

//				if (foundWwiseProjects.Length == 0)
//					Settings.WwiseProjectPath = "";
//				else
//				{
//					Settings.WwiseProjectPath =
//						AkUtilities.MakeRelativePath(UnityEngine.Application.dataPath, foundWwiseProjects[0]);
//				}

//				Settings.SoundbankPath = AkSoundEngineController.s_DefaultBasePath;
//			}

//			s_Instance = Settings;
//		}
//		catch (System.Exception)
//		{
//		}

//		return Settings;
//	}
//}

//    #endregion
