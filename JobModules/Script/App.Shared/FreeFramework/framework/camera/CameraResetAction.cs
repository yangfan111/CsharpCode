using com.wd.free.action;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using com.wd.free.@event;
using Core.Free;

namespace App.Shared.FreeFramework.framework.camera
{
    [Serializable]
    public class CameraResetAction : AbstractPlayerAction, IRule
    {
        public override void DoAction(IEventArgs args)
        {
            PlayerEntity p = GetPlayerEntity(args);
            p.gamePlay.CameraEntityId = 0;
        }

        public int GetRuleID()
        {
            return (int)ERuleIds.CameraResetAction;
        }
    }
}
