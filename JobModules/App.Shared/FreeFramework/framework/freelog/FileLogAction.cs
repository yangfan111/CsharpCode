using com.wd.free.action;
using Core.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using com.wd.free.@event;

namespace App.Shared.FreeFramework.framework.freelog
{
    [Serializable]
    public class FileLogAction : AbstractGameAction
    {
        static LoggerAdapter logger = new LoggerAdapter("UnitTest");

        public string log;

        public override void DoAction(IEventArgs args)
        {
            if (FreeLog.IsEnable())
            {
                logger.InfoFormat("{0} | {1}", DateTime.Now.ToString(), log);
            }
        }
    }
}
