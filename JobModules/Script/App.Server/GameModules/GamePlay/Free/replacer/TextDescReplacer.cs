using com.wd.free.util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using com.wd.free.@event;
using commons.data;
using commons.data.mysql;
using com.cpkf.yyjd.tools.util;

namespace App.Server.GameModules.GamePlay.Free.replacer
{
    public class TextDescReplacer : IFreeReplacer
    {
        private const string TextDesc = "desc:";
        private const string None = "None";

        private static Dictionary<int, string> descDic;

        public bool CanHandle(string exp, IEventArgs args)
        {
            return exp.StartsWith(TextDesc);
        }

        public string Replace(string exp, IEventArgs args)
        {
            IniDesc();

            exp = exp.Substring(TextDesc.Length);

            string[] vs = StringUtil.Split(exp, ",");

            int id = FreeUtil.ReplaceInt(vs[0].Trim(), args);

            if (descDic.ContainsKey(id))
            {
                string text = descDic[id];
                if (vs.Length > 1)
                {
                    for (int i = 1; i < vs.Length; i++)
                    {
                        text = text.Replace("{" + (i - 1) + "}", vs[i].Trim());
                    }
                }

                return text;
            }

            return None;
        }

        private static void IniDesc()
        {
            if (descDic == null)
            {
                descDic = new Dictionary<int, string>();
                List<DataRecord> list = MysqlUtil.SelectRecords("select * from text_lang", FreeRuleConfig.MysqlConnection);
                foreach (DataRecord dr in list)
                {
                    descDic.Add(int.Parse(dr.GetValue("id")), dr.GetValue("text"));
                }
            }
        }

    }
}
