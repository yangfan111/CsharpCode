using Core.GameModule.Interface;
using Core.Prediction.UserPrediction.Cmd;
using Core.Utils;
using Core.Bag;
using App.Shared.WeaponLogic;
using App.Shared.Util;

namespace App.Shared.GameModules.Player
{
    public class PlayerWeaponDrawSystem : IUserCmdExecuteSystem
    {
        private static readonly LoggerAdapter Logger = new LoggerAdapter(typeof(PlayerWeaponDrawSystem));
        public void ExecuteUserCmd(IUserCmdOwner owner, IUserCmd cmd)
        {
            if(!cmd.IsDrawWeapon && !cmd.IsForceUnmountWeapon)
            {
                return;
            }
            var player = owner.OwnerEntity as PlayerEntity;
            var controller = player.GetWeaponController();

            if (cmd.IsForceUnmountWeapon)
            {
              
                controller.ForceUnmountHeldWeapon();
                return;
            }

            if(null != cmd.FilteredInput && !cmd.FilteredInput.IsInput(XmlConfig.EPlayerInput.IsDrawWeapon))
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
            bool changeWeaponSucess= true;
            var curSlot = player.weaponAgent.Content.HeldSlotType;
            if (curSlot == EWeaponSlotType.None)
            {
                var lastSlot = player.weaponAgent.Content.PopGetLastWeaponId();
                if (lastSlot != EWeaponSlotType.None)
                {
                    //player.soundManager.Value.PlayOnce(XmlConfig.EPlayerSoundType.ChangeWeapon);
                    controller.DrawSlotWeapon(lastSlot);
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
                controller.UnmountHeldWeapon();
            }
            if (changeWeaponSucess)
                player.weaponLogic.State.OnSwitchWeapon();
   
           
        }
    }
}
