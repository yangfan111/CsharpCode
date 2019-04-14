using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using com.wd.free.@event;
using App.Server.GameModules.GamePlay.free.player;
using UnityEngine;

namespace App.Shared.FreeFramework.UnitTest
{
    [Serializable]
    public class AnimationTestValue : AbstractTestValue
    {
        public override TestValue GetCaseValue(IEventArgs args)
        {
            TestValue tv = new TestValue();

            FreeData fd = (FreeData)args.GetUnit(UnitTestConstant.Tester);

            //tv.AddField("动作", fd.Player.state.ActionStateId);     
            //tv.AddField("移动", fd.Player.state.MovementStateId);   

            tv.AddField("动作状态标识", fd.Player.state.ActionStateId);       //动作

            tv.AddField("动作转换标识", fd.Player.state.ActionTransitionId);

            tv.AddField("移动状态标识", fd.Player.state.MovementStateId);     //移动

            tv.AddField("移动转换标识", fd.Player.state.MovementTransitionId);

            tv.AddField("保持状态标识", fd.Player.state.KeepStateId);

            tv.AddField("保持转换标识", fd.Player.state.KeepTransitionId);

            tv.AddField("姿势状态标识", fd.Player.state.PostureStateId);

            tv.AddField("姿势转换标识", fd.Player.state.PostureTransitionId);

            tv.AddField("倾斜状态标识", fd.Player.state.LeanStateId);
                         
            tv.AddField("倾斜转换标识", fd.Player.state.LeanTransitionId);

            return tv;
        }
    }
}
