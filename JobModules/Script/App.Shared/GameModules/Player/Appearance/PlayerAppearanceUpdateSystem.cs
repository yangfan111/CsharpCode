using App.Shared.Components.Player;
using Core.GameModule.Interface;
using Core.Prediction.UserPrediction.Cmd;
using Core.Utils;
using Core.Animation;
using Utils.Appearance;
using Core.Appearance;
using App.Shared.GameModules.Player.CharacterState;
using Core.CharacterState;
using UnityEngine;
using XmlConfig;
using App.Shared.Player;
using App.Shared.GameModules.Player.CharacterBone;
using Core.CharacterBone;
using Core.EntityComponent;

namespace App.Shared.GameModules.Player.Appearance
{
    public class PlayerAppearanceUpdateSystem : IUserCmdExecuteSystem
    {
        private static LoggerAdapter Logger = new LoggerAdapter(typeof(PlayerAppearanceUpdateSystem));

        public void ExecuteUserCmd(IUserCmdOwner owner, IUserCmd cmd)
        {
            var player = owner.OwnerEntity as PlayerEntity;
            CheckPlayerLifeState(player);
            AppearanceUpdate(player);
        }

        private void AppearanceUpdate(PlayerEntity player)
        {
            var appearance = player.appearanceInterface.Appearance;

            CheckPlayerNeedUnActive(player);
            
            if(player.hasGamePlay)
                appearance.CheckP3HaveInit(!player.gamePlay.HasNewRoleIdChangedFlag());
            
            appearance.CreateComponentData(player.latestAppearance, player.predictedAppearance);
            
            appearance.SyncLatestFrom(player.latestAppearance);
            appearance.SyncPredictedFrom(player.predictedAppearance);
            appearance.SyncClientFrom(player.clientAppearance);
            
            appearance.TryRewind();
            appearance.Execute();
            appearance.Update();
            
            appearance.SyncLatestTo(player.latestAppearance);
            appearance.SyncPredictedTo(player.predictedAppearance);
            appearance.SyncClientTo(player.clientAppearance);
        }

        private void CheckPlayerNeedUnActive(PlayerEntity player)
        {
            var appearanceInterface = player.appearanceInterface;
            if (player.gamePlay.ClientVisibility != player.gamePlay.Visibility)
            {
                appearanceInterface.Appearance.SetVisibility(player.gamePlay.Visibility);
                player.gamePlay.ClientVisibility = player.gamePlay.Visibility;
            }
        }

        #region LifeState

        private void CheckPlayerLifeState(PlayerEntity player)
        {
            if (null == player || null == player.playerGameState) return;
            var gameState = player.playerGameState;
            switch (gameState.CurrentPlayerLifeState)
            {
                case PlayerLifeStateEnum.Reborn:
                case PlayerLifeStateEnum.Revive:
                    Reborn(player);
                    break;
                case PlayerLifeStateEnum.Dead:
                    Dead(player);
                    break;
            }
        }

        private void Reborn(PlayerEntity player)
        {
            if (null == player) return;
            var appearance = player.appearanceInterface.Appearance;
            if (null == appearance) return;
            appearance.PlayerReborn();
            
            Logger.InfoFormat("AppearancePlayerReborn");
        }
        
        private void Dead(PlayerEntity player)
        {
            if (null == player) return;
            var appearance = player.appearanceInterface.Appearance;
            if (null == appearance) return;
            appearance.PlayerDead();
            
            Logger.InfoFormat("AppearancePlayerDead");
        }

        #endregion
    }
}
