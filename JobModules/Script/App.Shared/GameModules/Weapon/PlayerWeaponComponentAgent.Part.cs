using Core;
using XmlConfig;
using WeaponConfigNs;
using Core.Enums;
using Utils.Configuration;
using App.Shared.Util;
using Utils.Utils;
using App.Shared.Components.Bag;
using Core.WeaponLogic.Attachment;
using Utils.Singleton;
namespace App.Shared.GameModules.Weapon
{
    public partial class PlayerWeaponComponentAgent
    {

        internal EFuncResult SetSlotWeaponPart(EWeaponSlotType slot, int id, System.Action onWeaponAttachmentRefresh, WeaponPartsModelRefresh onModelPartsRefresh)
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
            if (slot == CurrSlotType)
                onWeaponAttachmentRefresh();
            onModelPartsRefresh(destWeaponComp.ToWeaponInfo(), slot, lastParts, destWeaponComp.GetParts(),false);
            return EFuncResult.Success;
        }
        internal void DeleteSlotWeaponPart(EWeaponSlotType slot, EWeaponPartType part, System.Action onCurrWeaponAttachmentRefresh, WeaponPartsModelRefresh onPartModelRefresh)
        {
            if (slot == EWeaponSlotType.None)
                return;
            var weaponComp = slotExtractor(slot);
            CommonUtil.WeakAssert(weaponComp != null);

            WeaponPartsStruct lastParts = weaponComp.GetParts();
            var parts = WeaponPartsUtil.ModifyParts(
                weaponComp.GetParts(), part,
                UniversalConsts.InvalidIntId);
            weaponComp.ApplyParts(parts);
            if (slot == CurrSlotType)
                onCurrWeaponAttachmentRefresh();
            var newParts = WeaponPartsUtil.ModifyParts(lastParts, part, UniversalConsts.InvalidIntId);
            newParts = newParts.ApplyDefaultParts(weaponComp.Id);
            onPartModelRefresh(weaponComp.ToWeaponInfo(), slot, lastParts, newParts,false);
        }

    }
}