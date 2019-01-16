
using App.Shared.Components.Bag;
using Core.Bag;

namespace App.Shared.WeaponLogic
{
    public enum Err_WeaponLogicErrCode
    {
        Sucess = 0,
        Err_Default = 1,
        Err_IdNotExisted = 2,
        Err_PlayerDontHaveThrowingComponent = 3,
        Err_PlayerGrenadePullVertifyFailed = 4,
        Err_GrenadeListShowCountZero = 5,
        Err_GrenadeNotFindNextUsableId = 6,
        Err_SameWeaponPosition = 7,
        Err_SlotNone = 8,
        Err_NullComponent = 9,
        Err_SlotInvailed = 10,

    }
    public delegate Err_WeaponLogicErrCode WeaponBag_SlotProcessCallback(WeaponSlotBaseHandler contrller);

    public delegate void WeaponBag_SlotSwitchCallback(EWeaponSlotType slotType, int nextId);
    public delegate void WeaponBag_WeaponSpendCallback(WeaponSpendCallbackData data);
    public delegate WeaponComponent WeaponSlotComponenExtractor(EWeaponSlotType slot);
    public delegate WeaponStateComponent WeaponStateComponentExtractor(bool autoState);
    public delegate GrenadeInventoryDataComponent GrenadeCacheComponentExtractor();
    public struct WeaponSpendCallbackData
    {
        public EWeaponSlotType slotType;
        public bool needRemoveCurrent;
        public bool needAutoRestuff;
        public WeaponSpendCallbackData(EWeaponSlotType s,bool in_needRemoveCurrent, bool in_needAutoRestuff)
        {
            slotType = s;
            needRemoveCurrent = in_needRemoveCurrent;
            needAutoRestuff = in_needAutoRestuff;
        }
    }
}