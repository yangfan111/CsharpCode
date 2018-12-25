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
