using System;
using System.IO;
using System.Xml.Linq;
using Core.Utils;
using UnityEngine;
using XmlConfig;

namespace App.Shared.Configuration
{
   

    public static class FileSystemConfigLoader
    {
        private static LoggerAdapter _logger = new LoggerAdapter(typeof(FileSystemConfigLoader));
        

        private static string GetAbsolutePath(string configName)
	    {
	        var uri = Application.dataPath + "/" + configName;
	        return uri;
        }

        public static string LoadConfigText(string configName)
        {
            return File.ReadAllText(GetAbsolutePath(configName));
        }

        public static T LoadXml<T>(string xmlPath) where T : class
        {
            return XmlConfigParser<T>.Load(LoadConfigText(xmlPath));
        }

    }
}

