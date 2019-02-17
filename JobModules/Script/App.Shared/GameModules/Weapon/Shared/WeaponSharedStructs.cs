using Core;
using Core.EntityComponent;
using Core.WeaponLogic.Attachment;

namespace App.Shared
{
    //public enum Err_WeaponLogicErrCode
    //{
    //    Sucess = 0,
    //    Err_Default = 1,
    //    Err_IdNotExisted = 2,
    //    Err_PlayerDontHaveThrowingComponent = 3,
    //    Err_PlayerGrenadePullVertifyFailed = 4,
    //    Err_GrenadeListShowCountZero = 5,
    //    Err_GrenadeNotFindNextUsableId = 6,
    //    Err_SameWeaponPosition = 7,
    //    Err_SlotNone = 8,
    //    Err_NullComponent = 9,
    //    Err_SlotInvailed = 10,

    //}
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

        public bool needRefreshWeaponLogic;

        public EntityKey lastWeaponKey;

        public void SetRefreshLogic(EntityKey in_lastWeaponKey)
        {
            lastWeaponKey = in_lastWeaponKey;
            needRefreshWeaponLogic = true;
        }
    }
}
