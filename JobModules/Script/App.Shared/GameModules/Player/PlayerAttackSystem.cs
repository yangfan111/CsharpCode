using App.Shared.GameModules.Weapon;
using App.Shared.GameModules.Weapon.Behavior;
using Core.GameModule.Interface;
using Core.Prediction.UserPrediction.Cmd;
using Core.Utils;

namespace App.Shared.GameModules.Player
{
    public class PlayerAttackSystem : IUserCmdExecuteSystem
    {
        private static LoggerAdapter _logger = new LoggerAdapter(typeof(PlayerAttackSystem));
        private WeaponFireUpdateManagaer _weaponFireUpdateManagaer;
        private Contexts _contexts;

        public PlayerAttackSystem(Contexts contexts)
        {
            _contexts = contexts;
            _weaponFireUpdateManagaer = contexts.session.commonSession.WeaponFireUpdateManager as WeaponFireUpdateManagaer;
        }

        public void ExecuteUserCmd(IUserCmdOwner owner, IUserCmd cmd)
        {
            var controller =  GameModuleManagement.Get<PlayerWeaponController>(owner.OwnerEntityKey.EntityId);
            var weaponId = controller.HeldWeaponAgent.ConfigId;
            if (weaponId < 1) return;
            var fireUpdater = _weaponFireUpdateManagaer.GetFireUpdater(weaponId);
            if(null != fireUpdater)
            {
                fireUpdater.Update(controller, cmd);
            }
        }
    }
}