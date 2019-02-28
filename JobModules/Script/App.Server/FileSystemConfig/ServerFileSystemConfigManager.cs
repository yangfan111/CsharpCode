using System.Xml.Linq;
using App.Shared;
using App.Shared.Configuration;
using Core.Utils;
using UnityEngine;
using Utils.Singleton;
using XmlConfig.BootConfig;

namespace App.Server.Scripts.Config
{
    public class ServerFileSystemConfigManager : Singleton<ServerFileSystemConfigManager>
    {
        
        public ServerFileSystemConfigManager()
        {
            Reload();

        }

        public void Reload()
        {
            var path = "/Config/Server/boot_config.xml";
            BootConfig = FileSystemConfigLoader.LoadXml<ServerBootConfig>(path);
        }
        public ServerBootConfig BootConfig { get; private set; }

    }
}