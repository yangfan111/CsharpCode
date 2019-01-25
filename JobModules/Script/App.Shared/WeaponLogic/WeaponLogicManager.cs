using App.Shared.WeaponLogic.Common;
using Assets.Utils.Configuration;
using Core.Configuration;
using Core.Utils;
using Core.WeaponLogic;
using WeaponConfigNs;

namespace App.Shared.WeaponLogic
{
    public class WeaponLogicManager : IWeaponLogicManager
    {
        private static readonly LoggerAdapter Logger = new LoggerAdapter(typeof(WeaponLogicManager));
        private IFireLogicCreator _fireLogicCreator;
        private IWeaponDataConfigManager _weaponDataConfigManager;
        private IWeaponConfigManager _weaponConfigManager;

        public WeaponLogicManager(IWeaponDataConfigManager weaponDataConfigManager,
            IWeaponConfigManager weaponConfigManager,
            IFireLogicCreator fireLogicCreator)
        {
            _fireLogicCreator = fireLogicCreator;
            _weaponDataConfigManager = weaponDataConfigManager;
            _weaponConfigManager = weaponConfigManager;
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
                return null;
            }
            var defaultConfig = weaponDataConfig.WeaponLogic as DefaultWeaponLogicConfig;
            if(null != defaultConfig)
            {
                return new DefaultWeaponLogic(_fireLogicCreator.GetFireLogic(weaponConfig, defaultConfig.FireLogic));
            }
            var doubleConfig = weaponDataConfig.WeaponLogic as DoubleWeaponLogicConfig;
            if(null != doubleConfig)
            {
                return new DoubleWeaponLogic(_fireLogicCreator.GetFireLogic(weaponConfig, doubleConfig.LeftFireLogic),
                    _fireLogicCreator.GetFireLogic(weaponConfig, doubleConfig.RightFireLogic));
            }

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
