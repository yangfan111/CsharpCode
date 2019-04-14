using Shared.Scripts.MapConfigPoint;
using System;
using UnityEngine;
using Utils.Configuration;
using XmlConfig;

namespace Assets.Core.Configuration
{
    public class MapPositionConfigManager : AbstractConfigManager<MapPositionConfigManager> 
    {
        public override void ParseConfig(string xml)
        {
            try
            {
                MapConfigPoints.current = XmlConfigParser<MapConfigPoints>.Load(xml);
                Debug.LogFormat("load position done");
            }catch(Exception e)
            {
                Debug.LogErrorFormat("load map position failed.{0}", e.Message);
            }
        }
    }
}
