using System;
using System.IO;
using Core.Utils;
using UnityEngine;

namespace App.Shared
{
    public static class Log4ConfigManager
    {
        private static bool _isInitLog = false;
        private static object _lock = new object();
        public static  void InitLog4net()
        {
            lock (_lock)
            {
                if (!_isInitLog)
                {
                    _isInitLog = true;
#if UNITY_2017
       var configFile = Application.dataPath + "\\Config\\log4net.xml";
        log4net.GlobalContext.Properties["LogDir"] = Application.dataPath;
#else
#if UNITY_EDITOR
                    var configFile = Application.dataPath + "/Config/log4net_56_editor.xml";
#else
                     var configFile = Application.dataPath + "/Config/log4net_56.xml";
#endif
                    var logDir = (Application.dataPath + "/../log/");
                    logDir = logDir.Replace("/", Path.DirectorySeparatorChar + "");
                    logDir = Path.GetFullPath(logDir);
                    log4net.GlobalContext.Properties["LogDir"] = logDir;
#endif
                    LoggerAdapter.Init(configFile);
                }
            }
           
        }
    }
}