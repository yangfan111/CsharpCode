namespace Core
{
    public interface IGameModeLogicFactory
    {
        IWeaponMode CreateWeaponModeLogic();
    }

    public abstract class AbstractGameeModeLogicFactory : IGameModeLogicFactory
    {
        protected abstract IBagSlotLogic GetBagSlotLogic();

        protected abstract IPickupLogic GetPickupLogic();

        protected abstract IReservedBulletLogic GetReservedBulletLogic();

        protected abstract IWeaponProcessListener GetWeaponActionListener();

        protected abstract IWeaponInitHandler InitializeHandler();

       protected abstract IWeaponSlotsLibrary GetWeaponSlotLibary();

        

        public IWeaponMode CreateWeaponModeLogic()
        {
            return new WeaponModeImplement(InitializeHandler(), GetWeaponSlotLibary(), GetBagSlotLogic(),GetPickupLogic(),GetReservedBulletLogic(),GetWeaponActionListener());
        }
    }
}