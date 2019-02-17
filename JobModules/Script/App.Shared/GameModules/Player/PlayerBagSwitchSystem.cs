using App.Shared.GameModules.Weapon;
using Core.GameModule.Interface;
using Core.GameModule.System;
using Core.Prediction.UserPrediction.Cmd;

namespace App.Shared.GameModules.Player
{
    public class PlayerBagSwitchSystem : IUserCmdExecuteSystem
    {
        private ICommonSessionObjects _commonSessionObjects;
        public PlayerBagSwitchSystem(ICommonSessionObjects commonSessionObjects)
        {
            _commonSessionObjects = commonSessionObjects;
        }

        public void ExecuteUserCmd(IUserCmdOwner owner, IUserCmd cmd)
        {
            
            if (cmd.BagIndex > 0)
            {
                var player = owner.OwnerEntity as PlayerEntity;
                var controller = GameModuleManagement.Get<PlayerWeaponController>(owner.OwnerEntityKey.GetHashCode());
                if(!controller.RelatedModelLogic.IsBagSwithEnabled(controller))
                {
                    return;
                }
                var bags = player.playerInfo.WeaponBags;
                var realBagIndex = cmd.BagIndex -1;
                player.modeLogic.ModeLogic.ResetWeaponWithBagIndex(realBagIndex, player);
            }
        }
    }
}
