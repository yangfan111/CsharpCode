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

        protected abstract IWeaponProcessListener GetWeaponActionListener();

        protected abstract IWeaponInitLogic GetWeaponIniLogic();

       protected abstract IWeaponSlotsLibrary GetWeaponSlotLibary();

        

        public IWeaponModeLogic CreateWeaponModeLogic()
        {
            return new ModeLogic(GetWeaponIniLogic(), GetWeaponSlotLibary(), GetBagSlotLogic(),GetPickupLogic(),GetReservedBulletLogic(),GetWeaponActionListener());
        }
    }
}