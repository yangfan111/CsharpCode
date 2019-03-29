using App.Shared.Components.Player;
using Core.GameModule.Interface;
using Core.Prediction.UserPrediction.Cmd;

namespace App.Shared.GameModules.Player.CharacterBone
{
    public class ServerCharacterBoneUpdateSystem: IUserCmdExecuteSystem
    {
        public void ExecuteUserCmd(IUserCmdOwner owner, IUserCmd cmd)
        {
            var player = owner.OwnerEntity as PlayerEntity;
            if (null != player && (player.gamePlay.IsLifeState(EPlayerLifeState.Dead)   || player.gamePlay.IsLastLifeState(EPlayerLifeState.Dead)))
            {
                return;
            }

            SyncSightComponent(player);
        }
        
        private void SyncSightComponent(PlayerEntity player)
        {
            var toComponent = player.firstPersonAppearance;
            var fromComponent = player.firstPersonAppearanceUpdate;
            toComponent.SightVerticalShift = fromComponent.SightVerticalShift;
            toComponent.SightHorizontalShift = fromComponent.SightHorizontalShift;
            toComponent.SightVerticalShiftRange = fromComponent.SightVerticalShiftRange;
            toComponent.SightHorizontalShiftDirection = fromComponent.SightHorizontalShiftDirection;
            toComponent.SightVerticalShiftDirection = fromComponent.SightVerticalShiftDirection;
            toComponent.SightRemainVerticalPeriodTime = fromComponent.SightRemainVerticalPeriodTime;
            toComponent.RandomSeed = fromComponent.RandomSeed;
        }
    }
}