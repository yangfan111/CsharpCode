using com.wd.free.action;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using com.wd.free.@event;
using App.Server.GameModules.GamePlay.free.player;
using App.Shared.Player;
using com.wd.free.util;

namespace App.Server.GameModules.GamePlay.Free.player
{
    [Serializable]
    public class SetPlayerUiStateAction : AbstractPlayerAction
    {
        private bool remove;

        private string state;

        private string time;

        public override void DoAction(IEventArgs args)
        {
            EPlayerUIState uiState = (EPlayerUIState)FreeUtil.ReplaceInt(state, args);
            FreeData p = GetPlayer(args);
            if (p != null)
            {
                if (remove)
                {
                    PlayerStateUtil.RemoveUIState(uiState, p.Player.gamePlay);
                }
                else
                {
                    int realTime = FreeUtil.ReplaceInt(time, args);
                    if(realTime > 0)
                    {
                        p.StateTimer.AddUITime(uiState, realTime);
                    }
                    PlayerStateUtil.AddUIState(uiState, p.Player.gamePlay);
                }
            }
        }
    }
}
