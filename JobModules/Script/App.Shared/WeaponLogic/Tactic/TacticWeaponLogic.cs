﻿using App.Server.GameModules.GamePlay.free.player;
using App.Shared.FreeFramework.Free.Weapon;
using com.wd.free.skill;
using Core.Configuration;
using Core.Free;
using Core.Prediction.UserPrediction.Cmd;
using Core.WeaponLogic;
using Utils.Singleton;
using WeaponConfigNs;

namespace App.Shared.GameModules.Weapon.Tactic
{
    public class TacticWeaponLogic : IWeaponLogic
    {
        private WeaponConfig _weaponConfig;
        private UnitSkill _unitSkill;
        private ISkillArgs _freeArgs;
        public TacticWeaponLogic(int weaponId, IFreeArgs freeArgs)
        {
            _weaponConfig = SingletonManager.Get<WeaponDataConfigManager>().GetConfigById(weaponId);

            if (SharedConfig.IsServer)
            {
                _unitSkill = WeaponSkillFactory.GetSkill(weaponId);
                _freeArgs = freeArgs as ISkillArgs;
            }
        }

        public void Reset()
        {
        }

        public void Update(PlayerEntity playerEntity, WeaponEntity weaponEntity, IUserCmd cmd)
        {
            if (SharedConfig.IsServer)
            {
                if (!_unitSkill.IsEmtpy())
                {
                    _freeArgs.GetInput().SetUserCmd(cmd);

                    _freeArgs.TempUse("current", (FreeData)playerEntity.freeData.FreeData);

                    _unitSkill.Frame(_freeArgs);

                    _freeArgs.Resume("current");
                }
            }
        }
    }
}
