using Assets.Utils.Configuration;
using Utils.Configuration;

namespace Utils.Appearance.Weapon
{
    public class WeaponModelControllerFactory
    {
        //private IWeaponResourceConfigManager  SingletonManager.Get<WeaponResourceConfigManager>();
        //private IWeaponPartsConfigManager _weaponPartsConfigManager;
        //private IWeaponAvatarConfigManager _weaponAvatarConfigManager;
        private IWeaponModelLoadController<object> _weaponModelLoadController;

        public WeaponModelControllerFactory(
            //IWeaponResourceConfigManager weaponConfigManager,
            //IWeaponPartsConfigManager weaponPartsConfigManager,
            //IWeaponAvatarConfigManager weaponAvatarConfigManager,
            IWeaponModelLoadController<object> weaponModelLoadController)
        {
            // SingletonManager.Get<WeaponResourceConfigManager>() = weaponConfigManager;
            //_weaponPartsConfigManager = weaponPartsConfigManager;
            //_weaponAvatarConfigManager = weaponAvatarConfigManager;
            _weaponModelLoadController = weaponModelLoadController;
        }

        public IWeaponModelController CreateWeaponAssemblyController()
        {
            return new WeaponModelController(_weaponModelLoadController);
                // SingletonManager.Get<WeaponResourceConfigManager>(), 
                //_weaponPartsConfigManager,
                //_weaponAvatarConfigManager,
                //_weaponModelLoadController);
        }
    }
}
