using App.Server.GameModules.GamePlay;
using App.Shared.Components.Player;
using App.Shared.Util;
using Assets.XmlConfig;
using Core.Prediction.UserPrediction.Cmd;
using Core.Utils;
using UnityEngine;

namespace App.Shared.GameModules.Player
{
    public class PlayerSyncStageSystem:AbstractUserCmdExecuteSystem
    {
        private static LoggerAdapter _logger = new LoggerAdapter(typeof(PlayerSyncStageSystem));
        private Contexts contexts;

        public PlayerSyncStageSystem(Contexts contexts)
        {
            this.contexts = contexts;
        }

        protected override bool Filter(PlayerEntity playerEntity)
        {
            return playerEntity.hasStage && playerEntity.stage.Value != EPlayerLoginStage.Running && !playerEntity.isInitialized;
        }

        protected override void ExecuteUserCmd(PlayerEntity playerEntity, IUserCmd cmd)
        {
           
            playerEntity.stage.Value = EPlayerLoginStage.Running;
            playerEntity.isInitialized = true;
            playerEntity.gamePlay.CoverInit = true;
            FreeRuleEventArgs args = (FreeRuleEventArgs)contexts.session.commonSession.FreeArgs;
            IGameRule rule = (IGameRule)args.Rule;
            rule.PlayerEnter(contexts, playerEntity);
         
            _logger.InfoFormat("PlayerEnter :{0}", playerEntity.entityKey);
         
           
        }
    }
}