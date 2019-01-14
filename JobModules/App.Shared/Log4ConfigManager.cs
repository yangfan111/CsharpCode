using System;
using System.IO;
using Core.Utils;
using UnityEngine;

namespace App.Shared
{
    public static class Log4ConfigManager
    {
        public static void InitLog4net()
        {
#if UNITY_2017
       var configFile = Application.dataPath + "\\Config\\log4net.xml";
        log4net.GlobalContext.Properties["LogDir"] = Application.dataPath;
#else
            var configFile = Application.dataPath + "/Config/log4net_56.xml";
            var logDir = (Application.dataPath + "/../log/");
            logDir = logDir.Replace("/", Path.DirectorySeparatorChar+"");
            logDir = Path.GetFullPath(logDir);
            log4net.GlobalContext.Properties["LogDir"] = logDir;
#endif
            LoggerAdapter.Init(configFile);
        }
    }
}