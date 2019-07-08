using com.wd.free.action;
using com.wd.free.@event;
using Core.Free;
using System;

namespace App.Server.GameModules.GamePlay.Free.player
{
    [Serializable]
    public class PlayerVisibilityAction : AbstractPlayerAction, IRule
    {
        private bool visibility;

        public override void DoAction(IEventArgs args)
        {
            PlayerEntity playerEntity = GetPlayerEntity(args);
            if (null != playerEntity)
            {
                playerEntity.gamePlay.Visibility = visibility;
            }
        }

        public int GetRuleID()
        {
            return (int)ERuleIds.PlayerVisibilityAction;
        }
    }
}
