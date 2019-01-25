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
            PlayerEntity player = owner.OwnerEntity as PlayerEntity;
            CheckPlayerLifeState(player);
            AppearanceUpdate(player);
        }

        private void AppearanceUpdate(PlayerEntity player)
        {
            var appearance = player.appearanceInterface.Appearance;
            
            appearance.SyncLatestFrom(player.latestAppearance);
            appearance.SyncPredictedFrom(player.predictedAppearance);
            
            appearance.TryRewind();
            appearance.Execute();
            
            appearance.SyncLatestTo(player.latestAppearance);
            appearance.SyncPredictedTo(player.predictedAppearance);
        }

        #region LifeState

        private void CheckPlayerLifeState(PlayerEntity player)
        {
            if (null == player || null == player.gamePlay) return;

            var gamePlay = player.gamePlay;
            if (!gamePlay.HasLifeStateChangedFlag()) return;

            if (gamePlay.IsLifeState(EPlayerLifeState.Alive) &&
                gamePlay.IsLastLifeState(EPlayerLifeState.Dead))
                Reborn(player);

            if (gamePlay.IsLifeState(EPlayerLifeState.Dead))
                Dead(player);
        }

        private void Reborn(PlayerEntity player)
        {
            if (null == player) return;
            var appearance = player.appearanceInterface.Appearance;
            if (null == appearance) return;
            appearance.PlayerReborn();
        }
        
        private void Dead(PlayerEntity player)
        {
            if (null == player) return;
            var appearance = player.appearanceInterface.Appearance;
            if (null == appearance) return;
            appearance.PlayerDead();
        }

        #endregion
    }
}
