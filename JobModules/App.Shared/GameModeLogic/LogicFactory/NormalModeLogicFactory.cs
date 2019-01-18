using App.Shared.GameModeLogic.BagSlotLogic;
using App.Shared.GameModeLogic.PickupLogic;
using App.Shared.GameModeLogic.ReservedBulletLogic;
using App.Shared.GameModeLogic.WeaponActionListener;
using App.Shared.GameModeLogic.WeaponInitLoigc;
using App.Shared.GameModules.Weapon;
using Assets.Utils.Configuration;
using Core.GameModeLogic;
using Core.GameModule.System;
using Utils.Configuration;
using Utils.Singleton;

namespace App.Shared.GameModeLogic.LogicFactory
{
    public class NormalModeLogicFactory : AbstractGameeModeLogicFactory
    {
        private Contexts _contexts;
        private ICommonSessionObjects _commonSessionObjects;

        private int _modeId;

        public NormalModeLogicFactory(Contexts contexts, ICommonSessionObjects commonSessionObjects, int modeId)
        {
            _contexts = contexts;
            _commonSessionObjects = commonSessionObjects;
            _modeId = modeId;
        }

        protected override IBagSlotLogic GetBagSlotLogic()
        {
            var bagSlotLogic = new NormalBagSlotLogic();

            return bagSlotLogic;
        }

        protected override IPickupLogic GetPickupLogic()
        {
            var pickupLogic = new NormalPickupLogic(_contexts.sceneObject,
                _contexts.player,
                _contexts.session.entityFactoryObject.SceneObjectEntityFactory,
                _commonSessionObjects.RuntimeGameConfig,
                SingletonManager.Get<GameModeConfigManager>().GetWepaonStayTime(_modeId));


            return pickupLogic;
        }

        protected override IReservedBulletLogic GetReservedBulletLogic()
        {
            var reservedBulletLogic = new LocalReservedBulletLogic();


            return reservedBulletLogic;
        }

        protected override IWeaponProcessListener GetWeaponActionListener()
        {
            var weaponActionListener = new NormalWeaponActionListener();


            return weaponActionListener;
        }

        protected override IWeaponInitLogic GetWeaponIniLogic()
        {
            var bagLimitTime =
                SingletonManager.Get<GameModeConfigManager>().GetBagLimitTime(_commonSessionObjects.RoomInfo.ModeId);
            var weaponInitLogic = new NormalWeaponInitLogic(bagLimitTime,
                SingletonManager.Get<WeaponConfigManager>(),
                SingletonManager.Get<WeaponPropertyConfigManager>());


            return weaponInitLogic;
        }

        protected override IWeaponSlotsLibrary GetWeaponSlotLibary()
        {
            return WeaponSlotsLibrary.Allocate(Core.EWeaponSlotsGroupType.Group);
        }
    }
}