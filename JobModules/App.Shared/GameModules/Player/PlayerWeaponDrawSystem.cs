using Core.GameModule.Interface;
using Core.Prediction.UserPrediction.Cmd;
using Core.Utils;
<<<<<<< HEAD
using Core;
using App.Shared.GameModules.Weapon;
=======
using Core.Bag;
using App.Shared.WeaponLogic;
>>>>>>> 6213b9d866f8e5766fe02025e06c786a8fc53841
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
<<<<<<< HEAD
            var controller = player.GetController<PlayerWeaponController>();
=======
            var controller = player.GetWeaponController();
>>>>>>> 6213b9d866f8e5766fe02025e06c786a8fc53841

            if (cmd.IsForceUnmountWeapon)
            {
              
<<<<<<< HEAD
                controller.ForceUnmountCurrWeapon();
=======
                controller.ForceUnmountHeldWeapon();
>>>>>>> 6213b9d866f8e5766fe02025e06c786a8fc53841
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
<<<<<<< HEAD
            EWeaponSlotType curSlot = player.GetController<PlayerWeaponController>().CurrSlotType;
            if (curSlot == EWeaponSlotType.None)
            {
                EWeaponSlotType lastSlot = player.GetController<PlayerWeaponController>().PopGetLastWeaponId();
=======
            var curSlot = player.weaponAgent.Content.HeldSlotType;
            if (curSlot == EWeaponSlotType.None)
            {
                var lastSlot = player.weaponAgent.Content.PopGetLastWeaponId();
>>>>>>> 6213b9d866f8e5766fe02025e06c786a8fc53841
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
<<<<<<< HEAD
                controller.UnmountCurrWeapon();
=======
                controller.UnmountHeldWeapon();
>>>>>>> 6213b9d866f8e5766fe02025e06c786a8fc53841
            }
            if (changeWeaponSucess)
                player.weaponLogic.State.OnSwitchWeapon();
   
           
        }
    }
}
