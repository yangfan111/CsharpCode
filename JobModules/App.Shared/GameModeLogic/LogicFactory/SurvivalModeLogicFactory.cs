using App.Shared.WeaponLogic;
using App.Shared.GameModeLogic.BagSlotLogic;
using App.Shared.GameModeLogic.PickupLogic;
using App.Shared.GameModeLogic.ReservedBulletLogic;
using App.Shared.GameModeLogic.WeaponActionListener;
using App.Shared.GameModeLogic.WeaponInitLoigc;
using Assets.Utils.Configuration;
using Core.GameModeLogic;
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
            var _pickupLogic = new SurvivalPickupLogic(_contexts.player, _contexts.sceneObject, _contexts.session.entityFactoryObject.SceneObjectEntityFactory, _commonSessionObjects.RuntimeGameConfig);

            return _pickupLogic;
        }

        protected override IReservedBulletLogic GetReservedBulletLogic()
        {
            var _reservedBulletLogic = new SharedReservedBulletLogic(SingletonManager.Get<WeaponConfigManager>());

            return _reservedBulletLogic;
        }

        protected override IWeaponActionListener GetWeaponActionListener()
        {
            var _weaponActionListener = new DummyWeaponActionListener();

            return _weaponActionListener;
        }

        protected override IWeaponInitLogic GetWeaponIniLogic()
        {
            var _weaponInitLogic = new DummyWeaponInitLogic();

            return _weaponInitLogic;
        }

        protected override IWeaponSlotController GetWeaponSlotController()
        {
            var _weaponSlotController = new DefaultWeaponSlotController();

            return _weaponSlotController;
        }
    }
}