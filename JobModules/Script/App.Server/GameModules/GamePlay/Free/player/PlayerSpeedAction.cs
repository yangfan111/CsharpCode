using com.wd.free.action;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using com.wd.free.@event;
using App.Server.GameModules.GamePlay.free.player;
using Assets.App.Server.GameModules.GamePlay.Free;
using com.wd.free.util;
using Core.Free;
using Free.framework;

namespace App.Server.GameModules.GamePlay.Free.player
{
    [Serializable]
    public class PlayerSpeedAction : AbstractPlayerAction
    {
        private string speed;
        private string jump;

        public override void DoAction(IEventArgs args)
        {
            FreeData fd = GetPlayer(args);
            if (fd != null)
            {
                fd.Player.stateInterface.State.SetSpeedAffect(FreeUtil.ReplaceFloat(speed, args));
                SimpleProto sp = FreePool.Allocate();
                sp.Key = FreeMessageConstant.PlayerMoveSpeedSet;
                sp.Fs.Add(FreeUtil.ReplaceFloat(speed, args));
                FreeMessageSender.SendMessage(fd.Player, sp);

                fd.Player.stateInterface.State.SetJumpAffect(FreeUtil.ReplaceFloat(jump, args));
                SimpleProto ps = FreePool.Allocate();
                ps.Key = FreeMessageConstant.PlayerJumpSpeedSet;
                ps.Fs.Add(FreeUtil.ReplaceFloat(jump, args));
                FreeMessageSender.SendMessage(fd.Player, ps);
            }
        }
    }
}
