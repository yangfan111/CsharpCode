using App.Shared.GameModules.Weapon.Tactic;
using App.Shared.WeaponLogic.Common;
using Assets.Utils.Configuration;
using Core.Configuration;
using Core.Free;
using Core.Utils;
using Core.WeaponLogic;
using WeaponConfigNs;

namespace App.Shared.WeaponLogic
{
    public class WeaponLogicManager : IWeaponLogicManager
    {
        private static readonly LoggerAdapter Logger = new LoggerAdapter(typeof(WeaponLogicManager));
        private IFireLogicProvider _fireLogicCreator;
        private IWeaponDataConfigManager _weaponDataConfigManager;
        private IWeaponConfigManager _weaponConfigManager;
        private IFreeArgs _freeArgs;
        private DefaultWeaponLogic _defaultWeaponLogic;

        public WeaponLogicManager(IWeaponDataConfigManager weaponDataConfigManager,
            IWeaponConfigManager weaponConfigManager,
            IFireLogicProvider fireLogicCreator,
            IFreeArgs freeArgs)
        {
            _fireLogicCreator = fireLogicCreator;
            _weaponDataConfigManager = weaponDataConfigManager;
            _weaponConfigManager = weaponConfigManager;
            _defaultWeaponLogic = new DefaultWeaponLogic();
            _freeArgs = freeArgs;
        }

        public IWeaponLogic GetWeaponLogic(int? weaponId)
        {
            var realWeaponId = weaponId;
            if(!weaponId.HasValue)
            {
                realWeaponId = GetHandId();
            }
            if(!realWeaponId.HasValue)
            {
                return null;
            }
            var weaponDataConfig = _weaponDataConfigManager.GetConfigById(realWeaponId.Value);
            var weaponConfig = _weaponConfigManager.GetConfigById(realWeaponId.Value);
            if (null == weaponConfig || null == weaponConfig)
            {
                realWeaponId = GetHandId();
            }
            if(!realWeaponId.HasValue)
            {
                Logger.Error("weaponId is illegal and no hand in config");
                return null;
            }
            var defaultConfig = weaponDataConfig.WeaponLogic as DefaultWeaponLogicConfig;
            if(null != defaultConfig)
            {
                _defaultWeaponLogic.SetFireLogic(_fireLogicCreator.GetFireLogic(weaponConfig, defaultConfig.FireLogic));
                return _defaultWeaponLogic;
            }
            var doubleConfig = weaponDataConfig.WeaponLogic as DoubleWeaponLogicConfig;
            if(null != doubleConfig)
            {
                return new DoubleWeaponLogic(_fireLogicCreator.GetFireLogic(weaponConfig, doubleConfig.LeftFireLogic),
                    _fireLogicCreator.GetFireLogic(weaponConfig, doubleConfig.RightFireLogic));
            }
            var tacticConfig = weaponDataConfig.WeaponLogic as TacticWeaponLogicConfig;
            if(null != tacticConfig)
            {
                return new TacticWeaponLogic(realWeaponId.Value, _freeArgs);
            }
            Logger.ErrorFormat("illegal weaponLogic for id {0}", realWeaponId);
            return null;
        }

        private int? GetHandId( )
        {
            return _weaponConfigManager.EmptyHandId.Value;
        }

        public void ClearCache()
        {
            _fireLogicCreator.ClearCache();
        }
    }
}
