using com.wd.free.action;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using com.wd.free.@event;

namespace App.Server.GameModules.GamePlay.Free.player
{
    [Serializable]
    public class PlayerResetAction : AbstractPlayerAction
    {
        public override void DoAction(IEventArgs args)
        {
            PlayerEntity p = GetPlayerEntity(args);
            p.stateInterface.State.PlayerReborn();
            p.appearanceInterface.Appearance.PlayerReborn();
            p.genericActionInterface.GenericAction.PlayerReborn(p);
        }
    }
}
