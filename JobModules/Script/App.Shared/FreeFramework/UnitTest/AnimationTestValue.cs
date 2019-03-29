using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using com.wd.free.@event;
using App.Server.GameModules.GamePlay.free.player;

namespace App.Shared.FreeFramework.UnitTest
{
    [Serializable]
    public class AnimationTestValue : AbstractTestValue
    {
        public override TestValue GetCaseValue(IEventArgs args)
        {
            TestValue tv = new TestValue();

            FreeData fd = (FreeData)args.GetUnit(UnitTestConstant.Tester);

            tv.AddField("动作", fd.Player.state.ActionStateId);
            tv.AddField("移动", fd.Player.state.MovementStateId);

            return tv;
        }
    }
}
