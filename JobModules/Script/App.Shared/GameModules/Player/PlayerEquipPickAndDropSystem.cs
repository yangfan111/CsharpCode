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
            //TODO 暂时没有考虑回滚，后续需对回滚的情况做处理
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
                var slot = (EWeaponSlotType)player.weaponState.CurrentWeaponSlot;
                player.modeLogic.ModeLogic.Dorp(player.entityKey.Value.EntityId, slot);
            }
        }

        public bool IsNotAliveThenIgnoreCmd(PlayerEntity player)
        {
            return !player.gamePlay.IsLifeState(Components.Player.EPlayerLifeState.Alive);
        }
    }
}
