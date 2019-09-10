using App.Shared.Components.Player;
using Core.GameModule.Interface;
using Core.Prediction.UserPrediction.Cmd;
using Core.Utils;

namespace App.Shared.GameModules.Player
{
    public class PlayerEquipPickAndDropSystem : IUserCmdExecuteSystem
    {
        private static readonly LoggerAdapter Logger = new LoggerAdapter(typeof(PlayerEquipPickAndDropSystem));

        public PlayerEquipPickAndDropSystem(){}

        public void ExecuteUserCmd(IPlayerUserCmdGetter getter, IUserCmd cmd)
        {
            var player = getter.OwnerEntity as PlayerEntity;
            var controller = player.WeaponController();
            if (!player.gamePlay.IsLifeState(EPlayerLifeState.Alive) || !player.gamePlay.CanAutoPick())
                return;
            
            if(cmd.ManualPickUpEquip > 0 && cmd.IsManualPickUp)
                player.ModeController().DoPickup(player, cmd.ManualPickUpEquip);

            if(cmd.AutoPickUpEquip != null && cmd.AutoPickUpEquip.Count > 0 && cmd.RenderTime - player.position.ServerTime > 500)
                player.ModeController().AutoPickupWeapon(player, cmd.AutoPickUpEquip);

            if (cmd.FilteredInput.IsInput(XmlConfig.EPlayerInput.IsDropWeapon))
                player.ModeController().Drop(player, controller.HeldSlotType,cmd);
        }
    }
}
