using System;
using System.Collections.Generic;
using System.IO;
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
            }catch(Exception e)
            {
                Debug.LogErrorFormat("load map position failed.{0}", e.Message);
            }
           
        }

    }
}
