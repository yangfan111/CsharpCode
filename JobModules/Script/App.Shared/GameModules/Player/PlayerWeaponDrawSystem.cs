using App.Shared.Util;
using Core.Prediction.UserPrediction.Cmd;
using Core.Utils;
using Core;


namespace App.Shared.GameModules.Player
{
    public class PlayerWeaponDrawSystem : AbstractUserCmdExecuteSystem
    {
        private static readonly LoggerAdapter Logger = new LoggerAdapter(typeof(PlayerWeaponDrawSystem));
             

        protected override bool Filter(PlayerEntity playerEntity)
        {
            return true;
        }

        protected override bool FilterCmd(IUserCmd cmd)
        {
            return cmd.IsDrawWeapon || cmd.IsForceUnmountWeapon;
        }

        protected override void ExecuteUserCmd(PlayerEntity playerEntity, IUserCmd cmd)
        {
            var controller = playerEntity.WeaponController();

            if (cmd.IsForceUnmountWeapon)
            {
              
                controller.UnArmWeapon(false);
                return;
            }

            if(!cmd.FilteredInput.IsInput(XmlConfig.EPlayerInput.IsDrawWeapon))
            {
                return;
            }
            else
            {
                if(null == cmd.FilteredInput)
                {
                    Logger.Error("FilteredInput in cmd should never be null !");
                }
            }
            bool changeWeaponSucess = true;
            if (controller.IsHeldSlotEmpty)
            {
                EWeaponSlotType lastSlot = controller.PollGetLastSlotType(false);
                if (lastSlot != EWeaponSlotType.None)
                {
                    //player.soundManager.Value.PlayOnce(XmlConfig.EPlayerSoundType.ChangeWeapon);
                    controller.ArmWeapon(lastSlot,true);
                }
                else
                {
                    changeWeaponSucess = false;
                    if (Logger.IsInfoEnabled)
                    {
                        Logger.Info("last weapon slot is none");
                    }
                }
            }
            else
            {
                //   player.soundManager.Value.PlayOnce(XmlConfig.EPlayerSoundType.ChangeWeapon);
                controller.UnArmWeapon(true); 
                 
            }
        }

     
    }
}
