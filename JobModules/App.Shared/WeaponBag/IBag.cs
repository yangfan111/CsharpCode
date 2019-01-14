using Core.Bag;
using Core.WeaponLogic.Attachment;
using WeaponConfigNs;
using XmlConfig;

namespace App.Shared.WeaponLogic
{
    public interface IBag
    {
        EWeaponSlotType CurWeaponSlot { get; set; }
        int CurWeaponId { get; }
        bool HasCurWeapon { get; }
        bool HasWeaponInSlot(EWeaponSlotType slot);
        void SetWeaponPart(EWeaponSlotType slot, int part);
        void SetWeaponBullet(EWeaponSlotType slot, int bullet);
        int GetWeaponBullet(EWeaponSlotType slot);
        void ClearAttachments(EWeaponSlotType slot);
        void DeleteAttachments(EWeaponSlotType slot, EWeaponPartType type);
        void AddWeaponToSlot(EWeaponSlotType slot, WeaponInfo weapon);
        void SetWeaponBolted(EWeaponSlotType slot, bool bolted);
        bool GetWeaponBolted(EWeaponSlotType slot);
        void RemoveWeapon(EWeaponSlotType slot);
        int LastWeapon { get; set; }
        EFireMode GetFireMode(EWeaponSlotType slot);
        void SetFireMode(EWeaponSlotType slot, EFireMode mode);
        WeaponInfo GetLastWeapon();
        WeaponInfo GetWeaponInfo(EWeaponSlotType slot);
    }
}
