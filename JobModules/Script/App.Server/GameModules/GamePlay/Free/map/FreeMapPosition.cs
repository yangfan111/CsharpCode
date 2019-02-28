using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;
using XmlConfig;

namespace App.Server.GameModules.GamePlay.Free.map
{
    public class FreeMapPosition
    {
        private static Dictionary<int, MapConfigPoints> cache = new Dictionary<int, MapConfigPoints>();

        public static MapConfigPoints GetPositions(int map)
        {
            if (!cache.ContainsKey(map))
            {
                string file = Application.dataPath + "/GameData/Server/Map/" + map + ".xml";
                if (File.Exists(file))
                {
                    string xml = GetFileContent(file, Encoding.UTF8);
                    MapConfigPoints ps = XmlConfigParser<MapConfigPoints>.Load(xml);

                    cache.Add(map, ps);
                }
                else
                {
                    cache.Add(map, MapConfigPoints.current);
                }
            }

            return cache[map];
        }

        private static string GetFileContent(string filename, Encoding enconding)
        {
            StreamReader din = new StreamReader(File.Open(filename, FileMode.Open, FileAccess.Read, FileShare.ReadWrite), enconding);
            StringBuilder st = new StringBuilder();
            string str;

            while ((str = din.ReadLine()) != null)
            {
                st.Append(str);
                st.Append("\n");
            }

            din.Close();

            return st.ToString();
        }
    }
}
