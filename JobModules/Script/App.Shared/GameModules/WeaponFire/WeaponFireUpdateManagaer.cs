using Assets.Utils.Configuration;
using Core;
using Core.Free;
using Core.Utils;
using Core.WeaponLogic;
using Utils.Singleton;



namespace App.Shared.GameModules.Weapon.Behavior
{
    /// <summary>
    /// Defines the <see cref="WeaponFireUpdateManagaer" />
    /// </summary>
    public class WeaponFireUpdateManagaer :IWeaponFireUpdateManagaer
    {
        private static readonly LoggerAdapter Logger = new LoggerAdapter(typeof(WeaponFireUpdateManagaer));

        private WeaponFireScriptsProvider fireScriptsCreator;

        //private IWeaponDataConfigManager _weaponDataConfigManager;

        //private IWeaponResourceConfigManager  SingletonManager.Get<WeaponResourceConfigManager>();
        private IFreeArgs _freeArgs;

      //  private CommonWeaponFireUpdate _commonWeaponFireUpdate;

        public WeaponFireUpdateManagaer(//WeaponConfigManagement weaponDataConfigManager,
                                  //IWeaponResourceConfigManager weaponConfigManager,
                                  WeaponFireScriptsProvider fireScriptsCreator,
            IFreeArgs freeArgs)
        {
            this.fireScriptsCreator = fireScriptsCreator;
            //_weaponDataConfigManager = weaponDataConfigManager;
            // SingletonManager.Get<WeaponResourceConfigManager>() = weaponConfigManager;
            _freeArgs = freeArgs;
        }

        public IWeaponFireUpdate GetFireUpdater(int? weaponId)
        {
            var realWeaponId = weaponId;
            if (!weaponId.HasValue)
            {
                realWeaponId = WeaponUtil.EmptyHandId;
            }

            var weaponAllConfig = SingletonManager.Get<WeaponConfigManagement>().FindConfigById(realWeaponId.Value);
            if (weaponAllConfig.S_DefualtBehavior != null)
            {
                var commonWeaponFireUpdate = new CommonWeaponFireUpdate(fireScriptsCreator.GetFireController(weaponAllConfig));
                return commonWeaponFireUpdate;

            }
             if (weaponAllConfig.S_TacticBehvior != null)
            {
                return new TacticWeaponFireUpdate(realWeaponId.Value, _freeArgs);
            }
             if (weaponAllConfig.S_DoubleBehavior != null)
            {
                return new DoubleWeaponFireUpdate(fireScriptsCreator.GetFireController(weaponAllConfig, weaponAllConfig.S_DoubleBehavior.LeftFireLogic),
              fireScriptsCreator.GetFireController(weaponAllConfig, weaponAllConfig.S_DoubleBehavior.RightFireLogic));
            }
            return null;
        }

        public void ClearCache()
        {
            fireScriptsCreator.ClearCache();
        }
    }
}
