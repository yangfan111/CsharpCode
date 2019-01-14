using Assets.XmlConfig;
using Core.Bag;
using Entitas;
using WeaponConfigNs;

namespace Core.GameModeLogic
{
    public interface IBagSlotLogic
    {
        EWeaponSlotType GetSlotByIndex(int index);
    }

    public interface IWeaponInitLogic
    {
        void InitDefaultWeapon(Entity playerEntity);
        void InitDefaultWeapon(Entity playerEntity, int index);
        void ResetWeaponWithBagIndex(int index, Entity playerEntity);
        bool IsBagSwithEnabled(Entity playerEntity); 
    }

    public interface IPickupLogic
    {
        void SendPickup(int entityId, int itemId, int category, int count);
        void SendAutoPickupWeapon(int entityId);
        void AutoPickupWeapon(int playerEntityId, int entityId);
        void DoPickup(int playerEntityId, int itemEntityId);
        void Dorp(int playerEntityId, EWeaponSlotType slot);
    }

    public interface IReservedBulletLogic
    {
        int SetReservedBullet(Entity playerEntity, EWeaponSlotType slot, int count);
        int SetReservedBullet(Entity playerEntity, EBulletCaliber caliber, int count);
        int GetReservedBullet(Entity playerEntity, EWeaponSlotType slot);
        int GetReservedBullet(Entity playerEntity, EBulletCaliber caliber);
    }

    public interface IWeaponSlotController
    {
        EWeaponSlotType GetWeaponSlotByIndex(int index);
        bool IsSlotValid(EWeaponSlotType slot);
        EWeaponSlotType[] AvaliableSlots { get; } 
        void InitPlayerWeaponBySlotInfo(Entity playerEntity);
    }
    
    public interface IWeaponActionListener
    {
        void OnPickup(Entity playerEntity, EWeaponSlotType slot);
        void OnCost(Entity playerEntity, EWeaponSlotType slot);
        void OnDrop(Entity playerEntity, EWeaponSlotType slot);
    }
    
     public interface IWeaponModeLogic : IWeaponInitLogic, IWeaponSlotController, IBagSlotLogic, IPickupLogic,
        IReservedBulletLogic, IWeaponActionListener
    {
    }

    public class ModeLogic : IWeaponModeLogic
    {
        private IWeaponInitLogic _weaponInitLogic;
        private IWeaponSlotController _weaponSlotController;
        private IBagSlotLogic _bagSlotLogic;
        private IPickupLogic _pickupLogic;
        private IReservedBulletLogic _reservedBulletLogic;
        private IWeaponActionListener _weaponActionListener;

        public ModeLogic(IWeaponInitLogic weaponInitLogic, IWeaponSlotController weaponSlotController,
            IBagSlotLogic bagSlotLogic, IPickupLogic pickupLogic, IReservedBulletLogic reservedBulletLogic,
            IWeaponActionListener weaponActionListener)
        {
            _weaponInitLogic = weaponInitLogic;
            _weaponSlotController = weaponSlotController;
            _bagSlotLogic = bagSlotLogic;
            _pickupLogic = pickupLogic;
            _reservedBulletLogic = reservedBulletLogic;
            _weaponActionListener = weaponActionListener;
        }

        #region IWeaponInitLogic

        public void InitDefaultWeapon(Entity playerEntity)
        {
            _weaponInitLogic.InitDefaultWeapon(playerEntity);
        }

        public void InitDefaultWeapon(Entity playerEntity, int index)
        {
            _weaponInitLogic.InitDefaultWeapon(playerEntity, index);
        }

        public void ResetWeaponWithBagIndex(int index, Entity playerEntity)
        {
            _weaponInitLogic.ResetWeaponWithBagIndex(index, playerEntity);
        }

        public bool IsBagSwithEnabled(Entity playerEntity)
        {
            return _weaponInitLogic.IsBagSwithEnabled(playerEntity);
        }

        #endregion

        #region IWeaponSlotController

        public EWeaponSlotType GetWeaponSlotByIndex(int index)
        {
            return _weaponSlotController.GetWeaponSlotByIndex(index);
        }

        public bool IsSlotValid(EWeaponSlotType slot)
        {
            return _weaponSlotController.IsSlotValid(slot);
        }

        public EWeaponSlotType[] AvaliableSlots
        {
            get { return _weaponSlotController.AvaliableSlots; }
        }

        public void InitPlayerWeaponBySlotInfo(Entity playerEntity)
        {
            _weaponSlotController.InitPlayerWeaponBySlotInfo(playerEntity);
        }

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

        public void AutoPickupWeapon(int playerEntityId, int entityId)
        {
            _pickupLogic.AutoPickupWeapon(playerEntityId, entityId);
        }

        public void DoPickup(int playerEntityId, int itemEntityId)
        {
            _pickupLogic.DoPickup(playerEntityId, itemEntityId);
        }

        public void Dorp(int playerEntityId, EWeaponSlotType slot)
        {
            _pickupLogic.Dorp(playerEntityId, slot);
        }

        #endregion

        #region IReservedBulletLogic

        public int SetReservedBullet(Entity playerEntity, EWeaponSlotType slot, int count)
        {
            return _reservedBulletLogic.SetReservedBullet(playerEntity, slot, count);
        }

        public int SetReservedBullet(Entity playerEntity, EBulletCaliber caliber, int count)
        {
            return _reservedBulletLogic.SetReservedBullet(playerEntity, caliber, count);
        }

        public int GetReservedBullet(Entity playerEntity, EWeaponSlotType slot)
        {
            return _reservedBulletLogic.GetReservedBullet(playerEntity, slot);
        }

        public int GetReservedBullet(Entity playerEntity, EBulletCaliber caliber)
        {
            return _reservedBulletLogic.GetReservedBullet(playerEntity, caliber);
        }

        #endregion

        #region IWeaponActionListener

        public void OnPickup(Entity playerEntity, EWeaponSlotType slot)
        {
            _weaponActionListener.OnPickup(playerEntity, slot);
        }

        public void OnCost(Entity playerEntity, EWeaponSlotType slot)
        {
            _weaponActionListener.OnCost(playerEntity, slot);
        }

        public void OnDrop(Entity playerEntity, EWeaponSlotType slot)
        {
            _weaponActionListener.OnDrop(playerEntity, slot);
        }

        #endregion
    }

}