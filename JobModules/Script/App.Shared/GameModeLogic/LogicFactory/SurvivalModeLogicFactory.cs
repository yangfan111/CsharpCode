using App.Shared.GameModules.Weapon;
using App.Shared.GameModeLogic.BagSlotLogic;
using App.Shared.GameModeLogic.PickupLogic;
using App.Shared.GameModeLogic.ReservedBulletLogic;
using App.Shared.GameModeLogic.WeaponActionListener;
using App.Shared.GameModeLogic.WeaponInitLoigc;
using Assets.Utils.Configuration;
using Core;
using Core.GameModule.System;
using Utils.Singleton;

namespace App.Shared.GameModeLogic.LogicFactory
{
    public class SurvivalModeLogicFactory : AbstractGameeModeLogicFactory
    {
        private Contexts _contexts;
        private ICommonSessionObjects _commonSessionObjects;


        public SurvivalModeLogicFactory(Contexts contexts, ICommonSessionObjects commonSessionObjects)
        {
            UnityEngine.Debug.Log("survival mode ");
            _contexts = contexts;
            _commonSessionObjects = commonSessionObjects;
        }

        protected override IBagSlotLogic GetBagSlotLogic()
        {
            var _bagSlotLogic = new DoublePrimeWeaponBagSlotLogic();

            return _bagSlotLogic;
        }

        protected override IPickupLogic GetPickupLogic()
        {
            var _pickupLogic = new SurvivalPickupLogic(_contexts, _contexts.session.entityFactoryObject.SceneObjectEntityFactory, _commonSessionObjects.RuntimeGameConfig);

            return _pickupLogic;
        }

        protected override IReservedBulletLogic GetReservedBulletLogic()
        {
            var _reservedBulletLogic = new SharedReservedBulletLogic(_contexts);

            return _reservedBulletLogic;
        }

        protected override IWeaponProcessListener GetWeaponActionListener()
        {
            var _weaponActionListener = new DummyWeaponActionListener();

            return _weaponActionListener;
        }

        protected override IWeaponInitHandler InitializeHandler()
        {
            var _weaponInitLogic = new DummyWeaponInitializeHandler();

            return _weaponInitLogic;
        }

        protected override IWeaponSlotsLibrary GetWeaponSlotLibary()
        {
            return WeaponSlotsLibrary.Allocate(Core.EWeaponSlotsGroupType.Default);
        }
    }
}