using Core.Utils;
using Core.Bag;
using XmlConfig;
using WeaponConfigNs;
using Core.Configuration;
using Core.Enums;
using Utils.Configuration;
using Assets.Utils.Configuration;
using App.Shared.Util;
using Utils.Utils;
using App.Shared.Components.Bag;
using Core.WeaponLogic.Attachment;
using App.Shared.EntityFactory;
using Assets.XmlConfig;
using Utils.Appearance;
using Utils.Singleton;
namespace App.Shared.WeaponLogic
{
    public partial class PlayerWeaponComponentAgent
    {
        internal EFuncResult SetHeldSlotWeaponPart(int id)
        {
            return SetSlotWeaponPart(HeldSlotType, id);
        }

        internal EFuncResult SetSlotWeaponPart(EWeaponSlotType slot, int id)
        {
            WeaponComponent destWeaponComp = slotExtractor(slot);
            NewWeaponConfigItem wpConfig;
            EFuncResult ret = WeaponUtil.VertifyWeaponComponent(destWeaponComp, out wpConfig);
            if (ret != EFuncResult.Success)
                return ret;
            WeaponPartsStruct lastParts = destWeaponComp.GetParts();
            int realAttachId = BagUtility.GetRealAttachmentId(id, wpConfig.Id);
            bool match = SingletonManager.Get<WeaponPartsConfigManager>().IsPartMatchWeapon(realAttachId, wpConfig.Id);
            if (!match)
                return EFuncResult.Failed;
            var attachments = WeaponPartsUtil.ModifyParts(
                destWeaponComp.GetParts(),
                SingletonManager.Get<WeaponPartsConfigManager>().GetPartType(realAttachId),
                realAttachId);
            destWeaponComp.ApplyParts(attachments);
            //=>TODO:
            //if (slot == GetHoldSlot())
            //    RefreshCurrentWeaponAttachmentLogic();
            RefreshWeaponPartsModel(destWeaponComp.Id, slot, lastParts, destWeaponComp.GetParts());
            return EFuncResult.Success;
        }
        internal void DeleteSlotWeaponPart(EWeaponSlotType slot, EWeaponPartType part)
        {
            if (slot == EWeaponSlotType.None)
                return;
            var weaponComp = slotExtractor(slot);
            CommonUtil.WeakAssert(weaponComp != null);

            var lastParts = weaponComp.GetParts();
            var parts = WeaponPartsUtil.ModifyParts(
                weaponComp.GetParts(), part,
                UniversalConsts.InvalidIntId);
            weaponComp.ApplyParts(parts);
            //=>TODO:
            //if (slot == GetHoldSlot())
            //    RefreshCurrentWeaponAttachmentLogic();

            var newParts = WeaponPartsUtil.ModifyParts(lastParts, part, UniversalConsts.InvalidIntId);
            newParts = newParts.ApplyDefaultParts(weaponComp.Id);
            RefreshWeaponPartsModel(weaponComp.Id, slot, lastParts, newParts);
        }
        internal void RefreshWeaponPartsModel(int weaponId, EWeaponSlotType slot, WeaponPartsStruct oldAttachment, WeaponPartsStruct newAttachments)
        {
            //=>TODO:
            // WeaponPartsUtil.RefreshWeaponPartModels(_playerEntity.appearanceInterface.Appearance, weaponId, oldAttachment, newAttachments, slot);
        }

        internal void SetHeldWeaponParts(WeaponPartsStruct attachments)
        {
            //=>TODO:
            // _playerEntity.weaponLogic.Weapon.SetAttachment(attachments);
        }
    }
}