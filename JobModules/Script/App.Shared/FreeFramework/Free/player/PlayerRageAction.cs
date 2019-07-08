using App.Server.GameModules.GamePlay.free.player;
using App.Shared.GameModules.Player;
using com.wd.free.action;
using com.wd.free.@event;
using Core.Free;
using Sharpen;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace App.Shared.FreeFramework.Free.player
{
    [Serializable]
    public class PlayerRageAction : AbstractPlayerAction, IRule
    {
        private bool rage;
        public override void DoAction(IEventArgs args)
        {
            PlayerEntity entity = GetPlayerEntity(args);
            if (entity != null && entity.hasStateInterface) {
                if (rage) {
                    entity.stateInterface.State.RageStart();
                } else {
                    entity.stateInterface.State.RageEnd();
                }
            }
        }

        public int GetRuleID()
        {
            return (int)ERuleIds.PlayerRageAction;
        }
    }
}
