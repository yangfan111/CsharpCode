using App.Server.GameModules.GamePlay;
using App.Shared.Components.Player;
using App.Shared.Util;
using Core.Prediction.UserPrediction.Cmd;

namespace App.Shared.GameModules.Player
{
    public class PlayerSyncStageSystem:AbstractUserCmdExecuteSystem
    {
        private Contexts contexts;

        public PlayerSyncStageSystem(Contexts contexts)
        {
            this.contexts = contexts;
        }

        protected override bool filter(PlayerEntity playerEntity)
        {
            return playerEntity.hasStage && playerEntity.stage.Value != EPlayerLoginStage.Running;
        }

        protected override void ExecuteUserCmd(PlayerEntity playerEntity, IUserCmd cmd)
        {
            playerEntity.stage.Value = EPlayerLoginStage.Running;
            playerEntity.isInitialized = true;
            FreeRuleEventArgs args = (FreeRuleEventArgs)contexts.session.commonSession.FreeArgs;
            IGameRule rule = (IGameRule)args.Rule;
            rule.PlayerEnter(contexts, playerEntity);
           
        }
    }
}