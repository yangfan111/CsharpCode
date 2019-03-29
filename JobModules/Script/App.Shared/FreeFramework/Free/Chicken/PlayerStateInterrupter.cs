using App.Server.GameModules.GamePlay.free.player;
using App.Shared.Player;
using com.wd.free.skill;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace App.Shared.FreeFramework.Free.Chicken
{
    public class PlayerStateInterrupter : ISkillInterrupter
    {
        public bool IsInterrupted(ISkillArgs args)
        {
            FreeData fd = (FreeData)args.GetUnit("current");
            if(fd != null)
            {
                return PlayerStateUtil.HasPlayerState(EPlayerGameState.InterruptItem, fd.Player.gamePlay);
            }

            return false;
        }
    }
}
