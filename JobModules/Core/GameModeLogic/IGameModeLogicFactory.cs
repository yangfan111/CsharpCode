namespace Core.GameModeLogic
{
    public interface IGameModeLogicFactory
    {
        IWeaponModeLogic CreateWeaponModeLogic();
    }

    public abstract class AbstractGameeModeLogicFactory : IGameModeLogicFactory
    {
        protected abstract IBagSlotLogic GetBagSlotLogic();

        protected abstract IPickupLogic GetPickupLogic();

        protected abstract IReservedBulletLogic GetReservedBulletLogic();

        protected abstract IWeaponActionListener GetWeaponActionListener();

        protected abstract IWeaponInitLogic GetWeaponIniLogic();

        protected abstract IWeaponSlotController GetWeaponSlotController();

        public IWeaponModeLogic CreateWeaponModeLogic()
        {
            return new ModeLogic(GetWeaponIniLogic(),GetWeaponSlotController(),GetBagSlotLogic(),GetPickupLogic(),GetReservedBulletLogic(),GetWeaponActionListener());
        }
    }
}