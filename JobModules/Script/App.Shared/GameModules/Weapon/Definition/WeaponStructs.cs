using Core;
using Core.EntityComponent;
using Core.WeaponLogic.Attachment;

namespace App.Shared
{
    /// <summary>
    /// Defines the <see cref="WeaponSlotExpendStruct" />
    /// </summary>
    public struct WeaponSlotExpendStruct
    {
        public EWeaponSlotType slotType;

        public bool needRemoveCurrent;

        public bool needAutoRestuff;

        public WeaponSlotExpendStruct(EWeaponSlotType s, bool in_needRemoveCurrent, bool in_needAutoRestuff)
        {
            slotType = s;
            needRemoveCurrent = in_needRemoveCurrent;
            needAutoRestuff = in_needAutoRestuff;
        }
    }

    /// <summary>
    /// Defines the <see cref="WeaponPartsRefreshStruct" />
    /// </summary>
    public struct WeaponPartsRefreshStruct
    {
        public WeaponScanStruct weaponInfo;

        public EWeaponSlotType slot;

        public WeaponPartsStruct oldParts;

        public WeaponPartsStruct newParts;

        public bool armInPackage;

        public EntityKey lastWeaponKey;

        public void SetLastKey(EntityKey in_lastWeaponKey)
        {
            lastWeaponKey = in_lastWeaponKey;
        }
    }
}
