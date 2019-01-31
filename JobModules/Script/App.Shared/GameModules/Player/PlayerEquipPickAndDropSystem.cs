using Core.GameModule.Interface;
using Core.Prediction.UserPrediction.Cmd;
using Core.Utils;
using Core;

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
            if(IsNotAliveThenIgnoreCmd(player))
            {
                return;
            }
            if (cmd.PickUpEquip > 0)
            {
                if(cmd.IsManualPickUp)
                {
                    player.modeLogic.ModeLogic.DoPickup(player.entityKey.Value.EntityId, cmd.PickUpEquip);
                }
                else
                {
                    player.modeLogic.ModeLogic.AutoPickupWeapon(player.entityKey.Value.EntityId, cmd.PickUpEquip);
                }
            }
            //投掷时会判断是否已经准备，手雷的对象为Playback，不存在预测回滚的问题
            if (player.hasWeaponAutoState && player.weaponAutoState.AutoThrowing)
            {
                if(null != _userCmdGenerator)
                {
                    _userCmdGenerator.SetUserCmd((userCmd) => {
                        userCmd.IsThrowing = true;
                    });
                    player.weaponAutoState.AutoThrowing = false;
                }
            }
            if(cmd.FilteredInput.IsInput(XmlConfig.EPlayerInput.IsDropWeapon))
            { 
                var slot = (EWeaponSlotType)player.bagState.CurSlot;
                player.modeLogic.ModeLogic.Dorp(player.entityKey.Value.EntityId, slot);
            }
        }

        public bool IsNotAliveThenIgnoreCmd(PlayerEntity player)
        {
            return !player.gamePlay.IsLifeState(Components.Player.EPlayerLifeState.Alive);
        }
    }
}
