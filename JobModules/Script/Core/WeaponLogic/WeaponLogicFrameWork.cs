using Core;
using Core.EntityComponent;
using WeaponConfigNs;

namespace Core
{
    public interface IBagSlotLogic
    {
        EWeaponSlotType GetSlotByIndex(int index);
    }

    public interface IWeaponInitHandler
    {
        void ResetPlayerWeaponData(IPlayerWeaponGetter controller);
        void Recovery(IPlayerWeaponGetter controller, int index);
        void RecoveryBagContainer(int index, IPlayerWeaponGetter controller);

        void TrashPlayerBagState(IPlayerWeaponGetter controller);
        bool CanModeSwitchBag { get; } 
        int ModeId { get; }
    }

    public interface IPickupLogic
    {
        void SendPickup(int entityId, int itemId, int category, int count);
        void SendAutoPickupWeapon(int entityId);
        void AutoPickupWeapon(int controllerId, int entityId);
        void DoPickup(int controllerId, int itemEntityId);
        void Drop(int controllerId, EWeaponSlotType slot);
    }

    public interface IReservedBulletLogic
    {
        int SetReservedBullet(IPlayerWeaponGetter controller, EWeaponSlotType slot, int count);
        int SetReservedBullet(IPlayerWeaponGetter controller, EBulletCaliber caliber, int count);
        int GetReservedBullet(IPlayerWeaponGetter controller, EWeaponSlotType slot);
        int GetReservedBullet(IPlayerWeaponGetter controller, EBulletCaliber caliber);
    }

    public interface IWeaponSlotsLibrary
    {
        EWeaponSlotType GetWeaponSlotByIndex(int index);
        bool IsSlotValid(EWeaponSlotType slot);
        EWeaponSlotType[] AvaliableSlots { get; }
    }

    public interface IWeaponProcessListener
    {
        void OnPickup(IPlayerWeaponGetter controller, EWeaponSlotType slot);
        void OnExpend(IPlayerWeaponGetter controller, EWeaponSlotType slot);
        void OnDrop(IPlayerWeaponGetter controller, EWeaponSlotType slot,EntityKey dropedWeapon);
    }

    public interface IWeaponMode : IWeaponInitHandler, IBagSlotLogic, IPickupLogic,
       IReservedBulletLogic, IWeaponProcessListener, IWeaponSlotsLibrary
    {
    }

    public class WeaponModeImplement: IWeaponMode
    {
        /// <summary>
        /// 武器初始化
        /// </summary>
        private IWeaponInitHandler weaponInitHandler;
        private IWeaponSlotsLibrary _slotLibrary;
        private IBagSlotLogic _bagSlotLogic;
        private IPickupLogic _pickupLogic;
        private IReservedBulletLogic _reservedBulletLogic;
        private IWeaponProcessListener _weaponActionListener;

        public WeaponModeImplement(IWeaponInitHandler weaponInitLogic, IWeaponSlotsLibrary slotLibary,
            IBagSlotLogic bagSlotLogic, IPickupLogic pickupLogic, IReservedBulletLogic reservedBulletLogic,
            IWeaponProcessListener weaponActionListener)
        {
            weaponInitHandler = weaponInitLogic;
            _slotLibrary = slotLibary;
            _bagSlotLogic = bagSlotLogic;
            _pickupLogic = pickupLogic;
            _reservedBulletLogic = reservedBulletLogic;
            _weaponActionListener = weaponActionListener;
        }

        #region AbstractWeaponInitializeHandler

        public void ResetPlayerWeaponData(IPlayerWeaponGetter controller)
        {
            weaponInitHandler.ResetPlayerWeaponData(controller);
        }

        public void Recovery(IPlayerWeaponGetter controller, int index)
        {
            weaponInitHandler.Recovery(controller, index);
        }

        public void RecoveryBagContainer(int index, IPlayerWeaponGetter controller)
        {
            weaponInitHandler.RecoveryBagContainer(index, controller);
        }

        public bool CanModeSwitchBag
        {
            get { return true; }
        }
        public int ModeId { get { return weaponInitHandler.ModeId; } }
        #endregion

        #region IWeaponSlotController

        public EWeaponSlotType GetWeaponSlotByIndex(int index)
        {
            return _slotLibrary.GetWeaponSlotByIndex(index);
        }

        public bool IsSlotValid(EWeaponSlotType slot)
        {
            return _slotLibrary.IsSlotValid(slot);
        }

        public EWeaponSlotType[] AvaliableSlots
        {
            get { return _slotLibrary.AvaliableSlots; }
        }

        bool IWeaponInitHandler.CanModeSwitchBag => throw new System.NotImplementedException();


        #endregion

        #region IBagSlotLogic

        public EWeaponSlotType GetSlotByIndex(int index)
        {
            return _bagSlotLogic.GetSlotByIndex(index);
        }

        #endregion

        #region IPickupLogic

        public void SendPickup(int entityId, int itemId, int category, int count)
        {
            _pickupLogic.SendPickup(entityId, itemId, category, count);
        }

        public void SendAutoPickupWeapon(int entityId)
        {
            _pickupLogic.SendAutoPickupWeapon(entityId);
        }

        public void AutoPickupWeapon(int controllerId, int entityId)
        {
            _pickupLogic.AutoPickupWeapon(controllerId, entityId);
        }

        public void DoPickup(int controllerId, int itemEntityId)
        {
            _pickupLogic.DoPickup(controllerId, itemEntityId);
        }

        public void Drop(int controllerId, EWeaponSlotType slot)
        {
            _pickupLogic.Drop(controllerId, slot);
        }

        #endregion

        #region IReservedBulletLogic

        public int SetReservedBullet(IPlayerWeaponGetter controller, EWeaponSlotType slot, int count)
        {
            return _reservedBulletLogic.SetReservedBullet(controller, slot, count);
        }

        public int SetReservedBullet(IPlayerWeaponGetter controller, EBulletCaliber caliber, int count)
        {
            return _reservedBulletLogic.SetReservedBullet(controller, caliber, count);
        }

        public int GetReservedBullet(IPlayerWeaponGetter controller, EWeaponSlotType slot)
        {
            return _reservedBulletLogic.GetReservedBullet(controller, slot);
        }

        public int GetReservedBullet(IPlayerWeaponGetter controller, EBulletCaliber caliber)
        {
            return _reservedBulletLogic.GetReservedBullet(controller, caliber);
        }

        #endregion

        #region IWeaponActionListener

        public void OnPickup(IPlayerWeaponGetter controller, EWeaponSlotType slot)
        {
            _weaponActionListener.OnPickup(controller, slot);
        }

        public void OnExpend(IPlayerWeaponGetter controller, EWeaponSlotType slot)
        {
            _weaponActionListener.OnExpend(controller, slot);
        }

    
        public void TrashPlayerBagState(IPlayerWeaponGetter controller)
        {
            throw new System.NotImplementedException();
        }

        public void OnDrop(IPlayerWeaponGetter controller, EWeaponSlotType slot, EntityKey dropedWeapon)
        {
            _weaponActionListener.OnDrop(controller, slot, dropedWeapon);
        }



        #endregion
    }

}