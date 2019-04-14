using System;
using UnityEngine;
using Utils.Configuration;
using XmlConfig;

namespace Assets.Core.Configuration
{
    public class DropAreaConfigManager : AbstractConfigManager<DropAreaConfigManager> 
    {
        public override void ParseConfig(string xml)
        {
            try
            {
                DropAreaConfig.current = XmlConfigParser<DropAreaConfig>.Load(xml);
            }catch(Exception e)
            {
                Debug.LogErrorFormat("load DropArea failed.{0}", e.Message);
            }
        }
    }
}
