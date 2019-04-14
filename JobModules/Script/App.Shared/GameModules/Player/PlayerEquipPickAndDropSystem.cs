using Core.GameModule.Interface;
using Core.Prediction.UserPrediction.Cmd;
using Core.Utils;
using Core;
using App.Shared.GameModules.Weapon;

namespace App.Shared.GameModules.Player
{
    public class PlayerEquipPickAndDropSystem : IUserCmdExecuteSystem
    {
        private static readonly LoggerAdapter Logger = new LoggerAdapter(typeof(PlayerEquipPickAndDropSystem));
        private IUserCmdGenerator _userCmdGenerator;

        public PlayerEquipPickAndDropSystem(IUserCmdGenerator userCmdGenerator)
        {
            _userCmdGenerator = userCmdGenerator;
        }

        public void ExecuteUserCmd(IUserCmdOwner owner, IUserCmd cmd)
        {
            var player = owner.OwnerEntity as PlayerEntity;
            var controller = player.WeaponController();
            if (IsNotAliveThenIgnoreCmd(player))
            {
                return;
            }
            if (cmd.ManualPickUpEquip > 0 || cmd.AutoPickUpEquip.Count > 0)
            {
                if(cmd.ManualPickUpEquip > 0 && cmd.IsManualPickUp)
                {
                    player.ModeController().DoPickup(player, cmd.ManualPickUpEquip);
                }
                if(cmd.AutoPickUpEquip.Count > 0)
                {
                    player.ModeController().AutoPickupWeapon(player, cmd.AutoPickUpEquip);
                }
            }
            else if (cmd.FilteredInput.IsInput(XmlConfig.EPlayerInput.IsDropWeapon))
            {
                player.ModeController().Drop(player, controller.HeldSlotType,cmd);
            }
            //投掷时会判断是否已经准备，手雷的对象为Playback，不存在预测回滚的问题
            if (controller.AutoThrowing.HasValue && controller.AutoThrowing.Value)
            {
                if(null != _userCmdGenerator)
                {
                    _userCmdGenerator.SetUserCmd((userCmd) => {
                        userCmd.IsThrowing = true;
                    });
                    controller.AutoThrowing = false;
                }
            }
            
        }

        public bool IsNotAliveThenIgnoreCmd(PlayerEntity player)
        {
            return !player.gamePlay.IsLifeState(Components.Player.EPlayerLifeState.Alive);
        }
    }
}
