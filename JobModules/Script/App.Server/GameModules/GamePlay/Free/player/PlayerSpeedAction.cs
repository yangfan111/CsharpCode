using com.wd.free.action;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using com.wd.free.@event;
using App.Server.GameModules.GamePlay.free.player;
using com.wd.free.util;

namespace App.Server.GameModules.GamePlay.Free.player
{
    [Serializable]
    public class PlayerSpeedAction : AbstractPlayerAction
    {
        private string speed;
        private string time;

        public override void DoAction(IEventArgs args)
        {
            FreeData fd = GetPlayer(args);
            if (fd != null)
            {
                fd.Player.stateInterface.State.SetSpeedAffect(FreeUtil.ReplaceFloat(speed, args));
            }
        }
    }
}
