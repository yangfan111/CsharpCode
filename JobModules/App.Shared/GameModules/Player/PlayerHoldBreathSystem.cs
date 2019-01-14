using App.Shared.GameModules.Camera.Utils;
using Core.CameraControl.NewMotor;
using Core.GameModule.Interface;
using Core.Prediction.UserPrediction.Cmd;
using Core.Utils;
using System;

namespace Assets.App.Shared.GameModules.Player
{
    public class PlayerHoldBreathSystem : IUserCmdExecuteSystem
    {
        private static readonly LoggerAdapter Logger = new LoggerAdapter(typeof(PlayerHoldBreathSystem));
        public void ExecuteUserCmd(IUserCmdOwner owner, IUserCmd cmd)
        {
            var player = owner.OwnerEntity as PlayerEntity;
            if(null == player)
            {
                return;
            }
            if(cmd.IsHoldBreath && player.IsCameraGunSight()) 
            {
                if(null == player)
                {
                    Logger.Error("owner entity is not player entity ");
                    return;
                }
                if(!player.hasAppearanceInterface)
                {
                    Logger.Error("player has no appearance interface ");
                    return;
                }
                if(player.cameraStateNew.ViewNowMode == (int)ECameraViewMode.GunSight)
                {
                    player.oxygenEnergyInterface.Oxygen.InShiftState = true;
                }
            }
            else
            {
                if(!player.hasAppearanceInterface)
                {
                    Logger.Error("player has no appearance interface ");
                    return;
                }
                player.oxygenEnergyInterface.Oxygen.InShiftState = false;
            }
        }
    }
}
