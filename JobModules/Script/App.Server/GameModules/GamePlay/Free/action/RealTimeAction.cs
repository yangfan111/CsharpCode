using com.wd.free.action;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using com.wd.free.@event;
using commons.data.mysql;
using commons.data;
using com.wd.free.ai;

namespace App.Server.GameModules.GamePlay.Free.action
{
    [Serializable]
    public class RealTimeAction : AbstractGameAction
    {
        public override void DoAction(IEventArgs args)
        {
            List<DataRecord> list = MysqlUtil.SelectRecords("select * from realtime_rule where `rule` = '" + args.Rule.FreeType + "'", FreeRuleConfig.MysqlConnection);
            if (list.Count > 0)
            {
                string config = list[0].GetValue("config");
                if (!string.IsNullOrEmpty(config))
                {
                    object obj = FreeRuleConfig.XmlToObject(config);
                    if (obj is IGameAction)
                    {
                        if(obj is OrderAiAction)
                        {
                            ((IGameAction)obj).Reset(args);
                        }
                        else
                        {
                            ((IGameAction)obj).Act(args);
                        }
                    }
                }
            }
        }
    }
}
