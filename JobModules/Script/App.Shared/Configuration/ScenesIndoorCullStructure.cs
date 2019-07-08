using Utils.Configuration;
using ArtPlugins;
using XmlConfig;
using Shared.Scripts.SceneManagement;
using System;

namespace App.Shared.Configuration
{
    public class ScenesIndoorCullStructure : AbstractConfigManager<ScenesIndoorCullStructure>
    {
        public IndoorCullRecords Data { get; private set; }

        public override void ParseConfig(string xml)
        {
            try
            {
                if(Data == null)
                {
                    Data = new IndoorCullRecords();
                }

                IndoorCullRecordsWrapper warpper = XmlConfigParser<IndoorCullRecordsWrapper>.Load(xml);

                Data.recordsList.AddRange(warpper.InDoorCullRecords.recordsList);
            }catch(Exception e)
            {
                UnityEngine.Debug.LogError(e);
            }
           
        }

        public void ListToDict()
        {
            Data.ListToDict();
            Data.recordsList.Clear();
        }

        public void Clear()
        {
            if(Data != null && Data.recordsList != null)
            {
                Data.recordsList.Clear();
            }
        }
    }
}