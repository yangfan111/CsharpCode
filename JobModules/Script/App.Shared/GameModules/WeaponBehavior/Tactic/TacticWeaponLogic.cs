using App.Shared.FreeFramework.Free.Weapon;
using Assets.Utils.Configuration;
using com.wd.free.skill;
using Core.Configuration;
using Core.Free;
using Core.Prediction.UserPrediction.Cmd;
using Utils.Singleton;
using WeaponConfigNs;

namespace App.Shared.GameModules.Weapon.Behavior
{
    /// <summary>
    /// Defines the <see cref="TacticWeaponLogic" />
    /// </summary>
    public class TacticWeaponLogic : IWeaponLogic
    {
        private WeaponAllConfigs _weaponConfig;

        private UnitSkill _unitSkill;

        private ISkillArgs _freeArgs;

        public TacticWeaponLogic(int weaponId, IFreeArgs freeArgs)
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

        public void Update(PlayerWeaponController controller, IUserCmd cmd)
        {
            if (SharedConfig.IsServer)
            {
                if (!_unitSkill.IsEmtpy())
                {
                    _freeArgs.GetInput().SetUserCmd(cmd);

                    _freeArgs.TempUse("current", controller.RelatedFreeData);

                    _unitSkill.Frame(_freeArgs);

                    _freeArgs.Resume("current");
                }
            }
        }
    }
}
