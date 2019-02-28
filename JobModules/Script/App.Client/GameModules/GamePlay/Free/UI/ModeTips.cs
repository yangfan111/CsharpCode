using System.Collections.Generic;
using Assets.Sources.Free.Data;

namespace Assets.Sources.Free.UI
{
    public class ModeTips
    {
        private static IDictionary<string, IList<string>> tipMap = new Dictionary<string, IList<string>>();
        private static IDictionary<string, string> descMap = new Dictionary<string, string>();

        public static string currentLang = "default";


        public static void SetDesc(string lang, string free, string desc)
        {
            var key = GetKey(lang, free);
            descMap.Add(key, desc);
        }

        public static string GetDesc()
        {
            var key = GetKey(currentLang, GameModelLocator.GetInstance().GameModel.RoomData.FreeType);
            if (descMap.ContainsKey(key))
            {
                return descMap[key];
            }

            return "";
        }

        private static string GetKey(string lang, string free)
        {
            return lang + free;
        }

        public static void AddTip(string lang, string free, IList<string> tips)
        {
            var key = GetKey(lang, free);
            tipMap.Add(key, tips);
        }

        public static IList<string> GetTip()
        {
            var key = GetKey(currentLang, GameModelLocator.GetInstance().GameModel.RoomData.FreeType);
            if (tipMap.ContainsKey(key))
            {
                return tipMap[key];
            }

            return new List<string>();
        }
    }
}
