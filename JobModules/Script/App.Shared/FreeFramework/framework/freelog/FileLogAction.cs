using com.wd.free.action;
using com.wd.free.@event;
using Core.Free;
using Core.Utils;
using System;

namespace App.Shared.FreeFramework.framework.freelog
{
    [Serializable]
    public class FileLogAction : AbstractGameAction, IRule
    {
        static LoggerAdapter logger = new LoggerAdapter("FreeTest");

        public string log;

        public override void DoAction(IEventArgs args)
        {
            //if (FreeLog.IsEnable())
            //{
                logger.InfoFormat("{0} | {1}", DateTime.Now.ToString(), args.GetString(log));
            //}
        }

        public int GetRuleID()
        {
            return (int)ERuleIds.FileLogAction;
        }
    }
}
