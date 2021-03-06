﻿using App.Shared.Components.Player;
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

        public void ExecuteUserCmd(IPlayerUserCmdGetter getter, IUserCmd cmd)
        {
            var controller = getter.OwnerEntityKey.WeaponController();
            var weaponId = controller.HeldWeaponAgent.ConfigId;
            var jobAttribute = controller.JobAttribute;
            if (weaponId < 1 || Filter(getter, cmd)) return;
            if (jobAttribute == (int)EJobAttribute.EJob_Variant ||
                jobAttribute == (int)EJobAttribute.EJob_Matrix) {
                weaponId = WeaponUtil.MeleeVariant;
            }

            var fireUpdater = _weaponFireUpdateManagaer.GetFireUpdater(weaponId);
            if(null != fireUpdater)
            {
                fireUpdater.Update(getter.OwnerEntityKey, cmd, _contexts);
            }
        }

        protected bool Filter(IPlayerUserCmdGetter getter, IUserCmd cmd)
        {
            var controller = getter.OwnerEntityKey.WeaponController();
            return (cmd.IsLeftAttack && controller.NotMove);
        }
    }
}