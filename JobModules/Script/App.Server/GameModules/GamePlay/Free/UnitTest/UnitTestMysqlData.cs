using App.Shared.FreeFramework.UnitTest;
using com.wd.free.trigger;
using commons.data;
using commons.data.mysql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace App.Server.GameModules.GamePlay.Free.UnitTest
{
    public class UnitTestMysqlData : IUnitTestData
    {
        private const string Unknown = "Unknow";
        private const string TableUncheck = "unit_test";
        private const string TableCorrect = "unit_test_correct";
        private const string TableWrong = "unit_test_wrong";

        private static Dictionary<CaseKey, TestValue[]> correct;
        private static Dictionary<CaseKey, TestValue[]> wrong;

        private static void Initial()
        {
            if (correct == null)
            {
                correct = new Dictionary<CaseKey, TestValue[]>();
                wrong = new Dictionary<CaseKey, TestValue[]>();

                List<DataRecord> list = MysqlUtil.SelectRecords("select * from " + TableCorrect, FreeRuleConfig.MysqlConnection);
                foreach (DataRecord dr in list)
                {
                    CaseKey ck = new CaseKey();
                    ck.rule = dr.GetValue("rule");
                    ck.suit = dr.GetValue("suit");
                    ck.caseName = dr.GetValue("case");
                    ck.field = dr.GetValue("field");

                    if (!correct.ContainsKey(ck))
                    {
                        correct.Add(ck, TestValue.RecordsFromString(dr.GetValue("value")));
                    }
                }

                list = MysqlUtil.SelectRecords("select * from " + TableWrong, FreeRuleConfig.MysqlConnection);
                foreach (DataRecord dr in list)
                {
                    CaseKey ck = new CaseKey();
                    ck.rule = dr.GetValue("rule");
                    ck.suit = dr.GetValue("suit");
                    ck.caseName = dr.GetValue("case");
                    ck.field = dr.GetValue("field");

                    if (!correct.ContainsKey(ck))
                    {
                        wrong.Add(ck, TestValue.RecordsFromString(dr.GetValue("value")));
                    }
                }
            }
        }

        public void RecordResult(CaseKey key, TestValue[] tvs)
        {
            Initial();

            //if (correct.ContainsKey(key))
            //{
            //    TestValue[] old = correct[key];

            //    if (old.Length == tvs.Length)
            //    {
            //        bool same = true;
            //        for (int i = 0; i < tvs.Length; i++)
            //        {
            //            if (!tvs[i].IsSame(old[i]))
            //            {
            //                same = false;
            //                break;
            //            }
            //        }
            //        if (!same)
            //        {
            //            RecordMultiResult(key, tvs, TableWrong);
            //        }
            //    }
            //    else
            //    {
            //        RecordMultiResult(key, tvs, TableWrong);
            //    }

            //}
            //else if (wrong.ContainsKey(key))
            //{

            //}
            //else
            //{
            RecordMultiResult(key, tvs, TableUncheck);
            //}
        }

        protected static void RecordMultiResult(CaseKey key, TestValue[] tvs, string table)
        {
            DataRecord dr = new DataRecord();

            dr.AddField("rule", key.rule);
            dr.AddField("suit", key.suit);
            dr.AddField("case", key.caseName);
            dr.AddField("field", tvs[0].Name);
            dr.AddField("value", TestValue.ToRecords(tvs));

            Debug.LogFormat("table:{0}, dr:{1}", table, dr);

            MysqlUtil.Add(dr, table, FreeRuleConfig.MysqlConnection);
        }
    }
}
