﻿using App.Server.GameModules.GamePlay.free.player;
using App.Shared.Player;
using com.wd.free.action;
using com.wd.free.@event;
using com.wd.free.util;
using System;
using App.Shared.GameModules.Player;
using Core.Free;

namespace App.Server.GameModules.GamePlay.Free.player
{
    [Serializable]
    class SetPlayerCastStateAction : AbstractPlayerAction, IRule
    {
        private bool remove;

        private string state;

        public override void DoAction(IEventArgs args)
        {
            var castState = (EPlayerCastState)FreeUtil.ReplaceInt(state, args);
            FreeData p = GetPlayer(args);
            if (p != null)
            {
                if (remove)
                {
                    PlayerStateUtil.RemoveCastState(castState, p.Player.gamePlay);
                }
                else
                {
                    PlayerStateUtil.AddCastState(castState, p.Player.gamePlay);
                }
            }
        }

        public int GetRuleID()
        {
            return (int)ERuleIds.SetPlayerCastStateAction;
        }
    }
}
