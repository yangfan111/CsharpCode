using com.wd.free.action;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using com.wd.free.@event;

namespace App.Shared.FreeFramework.framework.camera
{
    [Serializable]
    public class CameraResetAction : AbstractPlayerAction
    {
        public override void DoAction(IEventArgs args)
        {
            PlayerEntity p = GetPlayerEntity(args);
            p.gamePlay.CameraEntityId = 0;
        }
    }
}
