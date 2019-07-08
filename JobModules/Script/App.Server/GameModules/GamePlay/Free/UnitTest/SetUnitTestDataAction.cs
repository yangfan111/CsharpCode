using com.wd.free.action;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using com.wd.free.@event;
using App.Server.GameModules.GamePlay.Free.UnitTest;
using Core.Free;

namespace Assets.App.Server.GameModules.GamePlay.Free.UnitTest
{
    [Serializable]
    public class SetUnitTestDataAction : AbstractGameAction, IRule
    {
        public override void DoAction(IEventArgs args)
        {
            args.FreeContext.TestCase.data = new UnitTestMysqlData();
        }

        public int GetRuleID()
        {
            return (int)ERuleIds.SetUnitTestDataAction;
        }
    }
}
