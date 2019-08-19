using App.Server.GameModules.GamePlay;
using App.Shared.Components;
using commons.data;
using commons.data.mysql;
using Core.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;
using UnityEngine;

namespace App.Client.GameModules.GamePlay.Free
{
    public class RuleMap
    {
        private static Dictionary<int, string> dic = new Dictionary<int, string>();
        private static LoggerAdapter _logger = new LoggerAdapter(typeof(RuleMap));
        private static string LastRule;

        public static int GetRuleId(string name)
        {
            LastRule = name;

            foreach (int v in dic.Keys)
            {
                if(dic[v] == name)
                {
                    return v;
                }
            }

            return GameRules.SimpleTest;
        }

        public static string GetRuleName(int rule)
        {
            if (dic.ContainsKey(rule))
            {
                return dic[rule];
            }
            else
            {
                return LastRule;
            }
        }

        public static void Initial(bool mysql)
        {
            dic.Clear();
            if (mysql)
            {
                List<DataRecord> list = MysqlUtil.SelectRecords("select * from rule_replace", FreeRuleConfig.MysqlConnection);
                foreach (DataRecord dr in list)
                {
                    dic.Add(int.Parse(dr.GetValue("race")), dr.GetValue("free"));
                }
                return;
            }

            string path = Application.dataPath + "/Config/Server/Rule/rule_replace.xml";
            FileStream fs = null;
            try
            {
                XmlSerializer xs = new XmlSerializer(typeof(RuleMapConfig));
                fs = new FileStream(path, FileMode.Open, FileAccess.Read);
                RuleMapConfig ruleMapConfig = (RuleMapConfig)xs.Deserialize(fs);

                if (null != ruleMapConfig)
                {
                    foreach (RuleMapConfigItem ruleMapConfigItem in ruleMapConfig.items) {
                        dic[ruleMapConfigItem.race] = ruleMapConfigItem.free;
                    }
                }

                fs.Close();
            }
            catch (Exception e)
            {
                if (fs != null)
                    fs.Close();
                _logger.Error("error : " + e.ToString());
            }
        }



        //private static void Initial()
        //{
        //    if (dic == null)
        //    {
        //        dic = new Dictionary<int, string>();

        //        /*dic.Add(5007, "bigHead");
        //        dic.Add(2005, "bioMain");
        //        dic.Add(5006, "bioMain");
        //        dic.Add(2002, "bomb");
        //        dic.Add(5005, "bomb");
        //        dic.Add(1002, "chicken");
        //        dic.Add(5001, "chicken");
        //        dic.Add(5003, "chicken");
        //        dic.Add(5004, "chicken");
        //        dic.Add(6001, "chicken");
        //        dic.Add(6009, "chicken");
        //        dic.Add(6010, "chicken");
        //        dic.Add(100201, "chicken");
        //        dic.Add(100202, "chicken");
        //        dic.Add(100203, "chicken");
        //        dic.Add(100204, "chicken");
        //        dic.Add(3005, "GrenadeFight");
        //        dic.Add(2004, "groupClassic");
        //        dic.Add(5008, "groupClassic");
        //        dic.Add(2003, "groupNormal");
        //        dic.Add(5002, "groupNormal");
        //        dic.Add(6002, "groupNormal");
        //        dic.Add(3002, "mftest");
        //        dic.Add(3004, "PistolFight");
        //        dic.Add(3003, "SniperFight");
        //        dic.Add(500002, "testCase");
        //        dic.Add(500001, "yctest");
        //        dic.Add(2006, "bioAbyss");
        //        dic.Add(5006, "bioAbyss");*/
        //        List<DataRecord> list = MysqlUtil.SelectRecords("select * from rule_replace", FreeRuleConfig.MysqlConnection);
        //        foreach (DataRecord dr in list)
        //        {
        //            dic.Add(int.Parse(dr.GetValue("race")), dr.GetValue("free"));
        //        }
        //    }
        //}

        [XmlRoot("root")]
        public class RuleMapConfig
        {
            public RuleMapConfigItem[] items;
        }

        [XmlType("item")]
        public class RuleMapConfigItem
        {
            public string free;
            [XmlAttribute("free")]
            public string Free
            {
                get { return free; }
                set { free = value; }
            }

            public int race;
            [XmlAttribute("race")]
            public int Race
            {
                get { return race; }
                set { race = value; }
            }

            public int subRace;
            [XmlAttribute("subRace")]
            public int SubRace
            {
                get { return subRace; }
                set { subRace = value; }
            }

            public int scene;
            [XmlAttribute("scene")]
            public int Scene
            {
                get { return scene; }
                set { scene = value; }
            }
        }
    }
    class RuleCondition
    {
        public string name;
        public int rule;
        public int subRule;
        public int scene;

        public RuleCondition(string name, int rule, int subRule, int scene)
        {
            this.name = name;
            this.rule = rule;
            this.subRule = subRule;
            this.scene = scene;
        }
    }
}
