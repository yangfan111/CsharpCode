using App.Shared.Configuration;
using XmlConfig.BootConfig;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using Utils.Singleton;

namespace App.Client.Scripts
{

    public class ClientFileSystemConfigManager : Singleton<ClientFileSystemConfigManager>
    {
        public ClientFileSystemConfigManager()
        {
            if (File.Exists(Application.dataPath +"/Config/Client/boot_config.xml"))
                BootConfig = FileSystemConfigLoader.LoadXml<ClientBootConfig>("/Config/Client/boot_config.xml");
#if UNITY_EDITOR
            var configName = "/Config/Client/boot_config2.xml";
            if (File.Exists(Application.dataPath + "/" + configName))
            {
                var LocalConfig = FileSystemConfigLoader.LoadXml<ClientBootConfig>(configName);
                List<SupplementaryResource> cache = new List<SupplementaryResource>(BootConfig.Resource.Supplement);
                cache.AddRange(LocalConfig.Resource.Supplement);

                BootConfig.Resource.Supplement = cache;
            }
#endif

            // 地形采样配置文件
            var terrainSampleConfig = "/Config/Client/terrainsample_config.xml";
            if (File.Exists(Application.dataPath + terrainSampleConfig))
            {
                TerrainSampleConfig = FileSystemConfigLoader.LoadXml<TerrainSampleConfig>(terrainSampleConfig);
            }
        }

        public ClientBootConfig BootConfig { get; set; }

        public TerrainSampleConfig TerrainSampleConfig { get; private set; }
    }
}