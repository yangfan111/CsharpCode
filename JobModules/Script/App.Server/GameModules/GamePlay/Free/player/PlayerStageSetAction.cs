using App.Shared.Components.Player;
using com.wd.free.action;
using com.wd.free.@event;
using Core.Free;
using System;

namespace App.Server.GameModules.GamePlay.Free.player
{
    [Serializable]
    public class PlayerStageSetAction : AbstractPlayerAction, IRule
    {
        private int stage;

        public override void DoAction(IEventArgs args)
        {
            PlayerEntity playerEntity = GetPlayerEntity(args);
            if (null != playerEntity)
            {
                playerEntity.stage.Value = (EPlayerLoginStage) stage;
            }
        }

        public int GetRuleID()
        {
            return (int)ERuleIds.PlayerStageSetAction;
        }
    }
}
