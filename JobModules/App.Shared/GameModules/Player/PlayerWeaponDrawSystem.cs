using Core.GameModule.Interface;
using Core.Prediction.UserPrediction.Cmd;
using Core.Utils;
using Core;
using App.Shared.GameModules.Weapon;
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
            var controller = player.GetController<PlayerWeaponController>();

            if (cmd.IsForceUnmountWeapon)
            {
              
                controller.ForceUnmountCurrWeapon();
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
            EWeaponSlotType curSlot = player.GetController<PlayerWeaponController>().CurrSlotType;
            if (curSlot == EWeaponSlotType.None)
            {
                EWeaponSlotType lastSlot = player.GetController<PlayerWeaponController>().PopGetLastWeaponId();
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
                controller.UnmountCurrWeapon();
            }
            if (changeWeaponSucess)
                player.weaponLogic.State.OnSwitchWeapon();
   
           
        }
    }
}
