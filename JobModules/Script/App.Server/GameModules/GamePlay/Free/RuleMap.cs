using App.Shared.Components;
using System.Collections.Generic;

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

                dic.Add(5007, "bigHead");
                dic.Add(2005, "bioMain");
                dic.Add(5006, "bioMain");
                dic.Add(2002, "bomb");
                dic.Add(5005, "bomb");
                dic.Add(1002, "chicken");
                dic.Add(5001, "chicken");
                dic.Add(5003, "chicken");
                dic.Add(5004, "chicken");
                dic.Add(6001, "chicken");
                dic.Add(6009, "chicken");
                dic.Add(6010, "chicken");
                dic.Add(100201, "chicken");
                dic.Add(100202, "chicken");
                dic.Add(100203, "chicken");
                dic.Add(100204, "chicken");
                dic.Add(3005, "GrenadeFight");
                dic.Add(2004, "groupClassic");
                dic.Add(5008, "groupClassic");
                dic.Add(2003, "groupNormal");
                dic.Add(5002, "groupNormal");
                dic.Add(6002, "groupNormal");
                dic.Add(3002, "mftest");
                dic.Add(3004, "PistolFight");
                dic.Add(3003, "SniperFight");
                dic.Add(500002, "testCase");
                dic.Add(500001, "yctest");
                /*List<DataRecord> list = MysqlUtil.SelectRecords("select * from rule_replace", FreeRuleConfig.MysqlConnection);
                foreach (DataRecord dr in list)
                {
                    dic.Add(int.Parse(dr.GetValue("race")), dr.GetValue("free"));
                }*/
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
