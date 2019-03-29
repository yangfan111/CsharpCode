using App.Server.GameModules.GamePlay;
using App.Shared.Components;
using commons.data;
using commons.data.mysql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace App.Client.GameModules.GamePlay.Free
{
    public class RuleMap
    {
        private static Dictionary<int, string> dic;

        private static string LastRule;

        public static int GetRuleId(string name)
        {
            Initial();

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
            Initial();

            if (dic.ContainsKey(rule))
            {
                return dic[rule];
            }
            else
            {
                return LastRule;
            }
        }


        private static void Initial()
        {
            if (dic == null)
            {
                dic = new Dictionary<int, string>();

                List<DataRecord> list = MysqlUtil.SelectRecords("select * from rule_replace", FreeRuleConfig.MysqlConnection);
                foreach (DataRecord dr in list)
                {
                    dic.Add(int.Parse(dr.GetValue("race")), dr.GetValue("free"));
                }
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
