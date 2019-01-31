using App.Shared.GameModules.Weapon;
using App.Shared.WeaponLogic;
using Core.GameModule.Interface;
using Core.Prediction.UserPrediction.Cmd;
using Core.Utils;

namespace App.Shared.GameModules.Player
{
    public class PlayerAttackSystem : IUserCmdExecuteSystem
    {
        private static LoggerAdapter _logger = new LoggerAdapter(typeof(PlayerAttackSystem));
        private WeaponLogicManager _weaponLogicManager;
        private Contexts _contexts;

        public PlayerAttackSystem(Contexts contexts)
        {
            _contexts = contexts;
            _weaponLogicManager = contexts.session.commonSession.WeaponLogicManager as WeaponLogicManager;
        }

        public void ExecuteUserCmd(IUserCmdOwner owner, IUserCmd cmd)
        {
            PlayerEntity playerEntity = (PlayerEntity)owner.OwnerEntity;
            var weaponId = playerEntity.GetController<PlayerWeaponController>().CurrSlotWeaponId(_contexts);
            var weaponLogic = _weaponLogicManager.GetWeaponLogic(weaponId);
            var weaponEntity = playerEntity.GetWeaponEntityByHandDefault(_contexts);
            if(null != weaponLogic)
            {
                weaponLogic.Update(playerEntity, weaponEntity, cmd);
            }
        }
    }
}