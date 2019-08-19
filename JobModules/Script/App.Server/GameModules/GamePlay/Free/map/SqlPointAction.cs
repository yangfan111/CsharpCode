using Sharpen;
using com.wd.free.@event;
using com.wd.free.action;
using com.wd.free.exception;
using com.wd.free.map.position;
using com.wd.free.para;
using com.wd.free.unit;
using Core.Free;
using System.Collections.Generic;
using commons.data;
using commons.data.mysql;
using App.Server.GameModules.GamePlay;
using Core.Utils;

namespace gameplay.gamerule.free.map
{
    [System.Serializable]
    public class SqlPointAction : AbstractGameAction, IRule
    {
        static readonly LoggerAdapter Logger = new LoggerAdapter(typeof(SqlPointAction));

        private const string SQL = "select* from test_position where map_id='{0}' AND position_type = '{1}'";

        private IGameAction action;

        private int map;

        private int type;

        public override void DoAction(IEventArgs args)
        {
            List<DataRecord> records = GetRecords();
            foreach (DataRecord record in records) {
                args.GetDefault().GetParameters().TempUse(new FloatPara("x",float.Parse(record.GetValue("x"))));
                args.GetDefault().GetParameters().TempUse(new FloatPara("y", float.Parse(record.GetValue("y"))));
                args.GetDefault().GetParameters().TempUse(new FloatPara("z", float.Parse(record.GetValue("z"))));
                args.GetDefault().GetParameters().TempUse(new FloatPara("rotation", float.Parse(record.GetValue("rotation"))));
                try
                {
                    action.Act(args);
                }
                catch
                {
                    Logger.DebugFormat("SqlPointAction has no action !");
                }
                args.GetDefault().GetParameters().Resume("x");
                args.GetDefault().GetParameters().Resume("y");
                args.GetDefault().GetParameters().Resume("z");
                args.GetDefault().GetParameters().Resume("rotation");
            }
        }

        private List<DataRecord> GetRecords()
        {
            List<DataRecord> records = MysqlUtil.SelectRecords(string.Format(SQL,map, type), FreeRuleConfig.MysqlConnection);
            return records;
        }

        public int GetRuleID()
        {
            return (int)ERuleIds.SqlPointAction;
        }
    }
}
