using Core.GameModule.Interface;
using Core.Prediction.UserPrediction.Cmd;
using Core.Utils;
using Core;


namespace App.Shared.GameModules.Player
{
    public class PlayerWeaponDrawSystem : IUserCmdExecuteSystem
    {
        private static readonly LoggerAdapter Logger = new LoggerAdapter(typeof(PlayerWeaponDrawSystem));
        private Contexts _contexts;
             
        public PlayerWeaponDrawSystem(Contexts contexts)
        {
            _contexts = contexts; 
        }

        public void ExecuteUserCmd(IUserCmdOwner owner, IUserCmd cmd)
        {
            if(!cmd.IsDrawWeapon && !cmd.IsForceUnmountWeapon)
            {
                return;
            }
            var player = owner.OwnerEntity as PlayerEntity;
            var controller = player.WeaponController();

            if (cmd.IsForceUnmountWeapon)
            {
              
                controller.ForceUnArmHeldWeapon();
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
            if (controller.IsHeldSlotEmpty)
            {
                EWeaponSlotType lastSlot = player.WeaponController().PollGetLastSlotType(false);
                if (lastSlot != EWeaponSlotType.None)
                {
                    //player.soundManager.Value.PlayOnce(XmlConfig.EPlayerSoundType.ChangeWeapon);
                    controller.DrawWeapon(lastSlot);
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
                controller.UnArmHeldWeapon(null); 
                 
            }
            if (changeWeaponSucess)
            {
                player.PlayWeaponSound(XmlConfig.EWeaponSoundType.SwitchIn);
            }
        }
    }
}
