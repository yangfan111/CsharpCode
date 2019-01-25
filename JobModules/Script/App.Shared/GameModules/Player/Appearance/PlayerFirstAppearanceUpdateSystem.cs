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

namespace App.Shared.GameModules.Player.Appearance
{
    public class PlayerFirstAppearanceUpdateSystem : IUserCmdExecuteSystem
    {
        private static LoggerAdapter Logger = new LoggerAdapter(typeof(PlayerFirstAppearanceUpdateSystem));

        public void ExecuteUserCmd(IUserCmdOwner owner, IUserCmd cmd)
        {
            PlayerEntity player = owner.OwnerEntity as PlayerEntity;
            if (player.gamePlay.IsLifeState(EPlayerLifeState.Dead) || player.gamePlay.IsLastLifeState(EPlayerLifeState.Dead))
            {
                return;
            }
            FirstPersonAppearanceUpdate(player);
        }

        // 只涉及第一人称
        private void FirstPersonAppearanceUpdate(PlayerEntity player)
        {
            var appearanceP1 = player.appearanceInterface.FirstPersonAppearance;
            appearanceP1.Update();
        }
    }
}
