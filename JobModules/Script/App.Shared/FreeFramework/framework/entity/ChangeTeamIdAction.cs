using com.wd.free.action;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using com.wd.free.@event;
using com.wd.free.util;
using App.Server.GameModules.GamePlay.Free.entity;
using App.Server.GameModules.GamePlay;
using Core.Free;

namespace com.wd.free.entity
{
    [Serializable]
    public class ChangeTeamidAction : AbstractPlayerAction, IRule
    {
        private int team;
        public override void DoAction(IEventArgs args)
        {
            PlayerEntity entity = GetPlayerEntity(args);
            if (entity != null && entity.hasPlayerInfo )
            {
                entity.playerInfo.TeamId = team;
            }
        }

        public int GetRuleID()
        {
            return (int)ERuleIds.ChangeTeamidAction;
        }
    }
}
