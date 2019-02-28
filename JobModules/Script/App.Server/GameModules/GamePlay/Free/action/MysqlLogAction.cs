using com.wd.free.action;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using com.wd.free.@event;
using commons.data.mysql;
using commons.data;
using Sharpen;
using com.wd.free.util;

namespace App.Server.GameModules.GamePlay.Free.action
{
    [Serializable]
    public class MysqlLogAction : AbstractGameAction
    {
        public string key;
        public string message;

        public override void DoAction(IEventArgs args)
        {
            DataRecord dr = new DataRecord();

            dr.AddField("time", DateTime.Now.Ticks.ToString());
            dr.AddField("key", key);
            dr.AddField("message", FreeUtil.ReplaceVar(message, args));
            MysqlUtil.Add(dr, "simple_log", FreeRuleConfig.MysqlConnection);
        }
    }
}
