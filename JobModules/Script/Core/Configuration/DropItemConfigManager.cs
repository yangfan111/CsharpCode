using System;
using UnityEngine;
using Utils.Configuration;
using XmlConfig;

namespace Assets.Core.Configuration
{
    public class DropItemConfigManager : AbstractConfigManager<DropItemConfigManager> 
    {
        public override void ParseConfig(string xml)
        {
            try
            {
                DropItemConfig.current = XmlConfigParser<DropItemConfig>.Load(xml);
                Logger.Info("load DropItem succeeded.");
            }catch(Exception e)
            {
                Debug.LogErrorFormat("load DropItem failed.{0}", e.Message);
            }
        }
    }
}
