using Core.GameModule.Interface;
using Core.Prediction.UserPrediction.Cmd;
using Core.Utils;
using Core.Bag;

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
            if(cmd.IsForceUnmountWeapon)
            {
                player.playerAction.Logic.ForceUnmountWeapon();
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
            var curSlot = player.GetBagLogicImp().GetCurrentWeaponSlot();
            if (curSlot == EWeaponSlotType.None)
            {
                var lastSlot = player.GetBagLogicImp().PopLastWeaponSlot();
                if (lastSlot != EWeaponSlotType.None)
                {
                    //player.soundManager.Value.PlayOnce(XmlConfig.EPlayerSoundType.ChangeWeapon);
                    player.playerAction.Logic.DrawWeapon(lastSlot);
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
                player.playerAction.Logic.UnmountWeapon();
            }
            if (changeWeaponSucess)
                player.weaponLogic.State.OnSwitchWeapon();
   
           
        }
    }
}
