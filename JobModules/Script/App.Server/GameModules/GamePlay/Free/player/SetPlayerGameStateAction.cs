using com.wd.free.action;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using com.wd.free.@event;
using App.Server.GameModules.GamePlay.free.player;
using App.Shared.GameModules.Player;
using App.Shared.Player;
using com.wd.free.util;
using Core.Free;

namespace App.Server.GameModules.GamePlay.Free.player
{
    [Serializable]
    public class SetPlayerGameStateAction : AbstractPlayerAction, IRule
    {
        private bool remove;

        private string state;

        private string time;

        public override void DoAction(IEventArgs args)
        {
            EPlayerGameState uiState = (EPlayerGameState)FreeUtil.ReplaceInt(state, args);
            FreeData p = GetPlayer(args);
            if (p != null)
            {
                if (remove)
                {
                    PlayerStateUtil.RemoveGameState(uiState, p.Player.gamePlay);
                }
                else
                {
                    int realTime = FreeUtil.ReplaceInt(time, args);
                    if (realTime > 0)
                    {
                        p.StateTimer.AddGameStateTime(uiState, realTime + args.Rule.ServerTime);
                    }
                    PlayerStateUtil.AddPlayerState(uiState, p.Player.gamePlay);
                }
            }
        }

        public int GetRuleID()
        {
            return (int)ERuleIds.SetPlayerGameStateAction;
        }
    }
}
