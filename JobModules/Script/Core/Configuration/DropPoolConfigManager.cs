using System;
using UnityEngine;
using Utils.Configuration;
using XmlConfig;

namespace Assets.Core.Configuration
{
    public class DropPoolConfigManager : AbstractConfigManager<DropPoolConfigManager> 
    {
        public override void ParseConfig(string xml)
        {
            try
            {
                DropPoolConfig.current = XmlConfigParser<DropPoolConfig>.Load(xml);
            }catch(Exception e)
            {
                Debug.LogErrorFormat("load DropPool failed.{0}", e.Message);
            }
        }
    }
}
