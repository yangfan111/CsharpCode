using Utils.Configuration;
using ArtPlugins;
using XmlConfig;
using Shared.Scripts.SceneManagement;
using System;

namespace App.Shared.Configuration
{
    public class InDoorItemStructure : AbstractConfigManager<InDoorItemStructure>
    {
        public IndoorCullRecordsWrapper Data { get; private set; }

        public override void ParseConfig(string xml)
        {
            try
            {
                Data = XmlConfigParser<IndoorCullRecordsWrapper>.Load(xml);
            }
            catch (Exception e)
            {
                UnityEngine.Debug.LogError(e);
            }

        }
    }
}