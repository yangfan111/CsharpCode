using com.cpkf.yyjd.tools.util;
using com.wd.free.action;
using com.wd.free.para.exp;
using com.wd.free.skill;
using commmons.util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace App.Server.GameModules.GamePlay.Free
{
    public class FreeGameContext
    {
        private static Dictionary<string, byte[]> cache;
        private static Dictionary<string, object> objCache;

        public static void Initial()
        {
            if (cache == null)
            {
                cache = new Dictionary<string, byte[]>();
                objCache = new Dictionary<string, object>();

                string prefix = "Server/GameConfig/";
                string[] files = FileUtils.getAllFiles(Application.dataPath + "/GameData/Server/GameConfig");
                foreach (string file in files)
                {
                    if (file.EndsWith(".xml"))
                    {
                        string temp = file.Replace("\\", "/");
                        string name = temp.Substring(temp.IndexOf(prefix) + prefix.Length).Replace(".xml", "");
                        Debug.LogFormat("name:{0}, file{1}", name, temp);
                        try
                        {
                            CacheGameObject(name);
                        }
                        catch (Exception e)
                        {
                            Debug.LogErrorFormat("failed to initial game config {0},at {1}\n{2}", name, file, e.StackTrace);
                        }
                    }
                }
            }
        }

        // 获得游戏动作的单例，一般用于单帧动作
        public static IGameAction GetGameAction(string name)
        {
            return (IGameAction)GetGameObjectSingleton(name);
        }

        // 获得游戏动作的新的实例，一般用于多帧动作
        public static IGameAction NewGameAction(string name)
        {
            return (IGameAction)GetGameObject(name);
        }

        // 获得游戏条件的单例
        public static IParaCondition GetGameCondition(string name)
        {
            return (IParaCondition)GetGameObjectSingleton(name);
        }

        // 获得游戏技能新的实例，技能由于有CD时间等，一般需要新的实例
        public static ISkill NewSkill(string name)
        {
            return (ISkill)GetGameObject(name);
        }

        private static void CacheGameObject(string name)
        {
            if (!objCache.ContainsKey(name))
            {
                object action = FreeRuleConfig.XmlToObject(FreeRuleConfig.GetXmlContent(name));
                objCache.Add(name, action);
                cache.Add(name, SerializeUtil.ObjectToByte(action));
            }
        }

        private static object GetGameObjectSingleton(string name)
        {
            CacheGameObject(name);

            return objCache[name];
        }

        private static object GetGameObject(string name)
        {
            CacheGameObject(name);

            return SerializeUtil.ByteToObject(cache[name]);
        }

    }
}
