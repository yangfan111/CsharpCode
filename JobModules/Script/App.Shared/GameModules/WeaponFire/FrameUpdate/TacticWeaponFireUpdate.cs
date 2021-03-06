﻿using App.Shared.FreeFramework.Free.Weapon;
using Assets.Utils.Configuration;
using com.wd.free.skill;
using Core.Configuration;
using Core.EntityComponent;
using Core.Free;
using Core.Prediction.UserPrediction.Cmd;
using Utils.Singleton;
using WeaponConfigNs;

namespace App.Shared.GameModules.Weapon.Behavior
{
    /// <summary>
    /// Defines the <see cref="TacticWeaponFireUpdate" />
    /// </summary>
    public class TacticWeaponFireUpdate : IWeaponFireUpdate
    {
        private WeaponAllConfigs _weaponConfig;

        private UnitSkill _unitSkill;

        private ISkillArgs _freeArgs;

        public TacticWeaponFireUpdate(int weaponId, IFreeArgs freeArgs)
        {
            _weaponConfig = SingletonManager.Get<WeaponConfigManagement>().FindConfigById(weaponId);

            if (SharedConfig.IsServer)
            {
                _unitSkill = WeaponSkillFactory.GetSkill(weaponId);
                _freeArgs = freeArgs as ISkillArgs;
            }
        }

        public void Reset()
        {
        }

        public void Update(EntityKey owner, IUserCmd cmd, Contexts contexts)
        {
            if (SharedConfig.IsServer)
            {
                if (!_unitSkill.IsEmtpy())
                {
                    _freeArgs.GetInput().SetUserCmd(cmd);

                    _freeArgs.TempUse("current", owner.WeaponController().RelatedFreeData);

                    _unitSkill.Frame(_freeArgs);

                    _freeArgs.Resume("current");
                }
            }
        }
    }
}
