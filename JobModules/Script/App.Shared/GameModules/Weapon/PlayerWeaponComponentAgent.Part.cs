using Core;
using XmlConfig;
using WeaponConfigNs;
using Core.Enums;
using Utils.Configuration;
using App.Shared.Util;
using Utils.Utils;
using Core.WeaponLogic.Attachment;
using Utils.Singleton;
using Core.Utils;

namespace App.Shared.GameModules.Weapon
{
    public partial class PlayerWeaponComponentAgent
    {

        internal EFuncResult SetSlotWeaponPart(Contexts contexts, EWeaponSlotType slot, int id, System.Action<Contexts> onWeaponAttachmentRefresh, WeaponPartsModelRefresh onModelPartsRefresh)
        {
            var destWeaponComp = slotExtractor(contexts, slot);
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
                onWeaponAttachmentRefresh(contexts);
            WeaponPartsRefreshData refreshData = new WeaponPartsRefreshData();
            refreshData.weaponInfo = destWeaponComp.ToWeaponInfo();
            refreshData.slot = slot;
            refreshData.oldParts = lastParts;
            refreshData.newParts = destWeaponComp.GetParts();
            onModelPartsRefresh(contexts, refreshData);
            return EFuncResult.Success;
        }
        internal void DeleteSlotWeaponPart(Contexts contexts, EWeaponSlotType slot, EWeaponPartType part, System.Action<Contexts> onCurrWeaponAttachmentRefresh, WeaponPartsModelRefresh onPartModelRefresh)
        {
            if (slot == EWeaponSlotType.None)
                return;
            var weaponComp = slotExtractor(contexts, slot);
            AssertUtility.Assert(weaponComp != null);

            WeaponPartsStruct lastParts = weaponComp.GetParts();
            var parts = WeaponPartsUtil.ModifyParts(
                weaponComp.GetParts(), part,
                UniversalConsts.InvalidIntId);
            weaponComp.ApplyParts(parts);
            if (slot == CurrSlotType)
                onCurrWeaponAttachmentRefresh(contexts);
            var newParts = WeaponPartsUtil.ModifyParts(lastParts, part, UniversalConsts.InvalidIntId);
            newParts = newParts.ApplyDefaultParts(weaponComp.WeaponId);
            WeaponPartsRefreshData refreshData = new WeaponPartsRefreshData();
            refreshData.weaponInfo = weaponComp.ToWeaponInfo();
            refreshData.slot = slot;
            refreshData.oldParts = lastParts;
            refreshData.newParts = newParts ;
            onPartModelRefresh(contexts, refreshData);
        }

    }
}