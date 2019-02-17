using App.Shared.Util;
using Assets.Utils.Configuration;
using Core;
using Core.WeaponLogic.Attachment;
using Utils.Configuration;
using Utils.Singleton;
using Utils.Utils;
using WeaponConfigNs;
using XmlConfig;

namespace App.Shared.GameModules.Weapon
{
    /// <summary>
    /// Defines the <see cref="PlayerWeaponController" />
    /// </summary>
    public partial class PlayerWeaponController
    {
        /// <summary>
        /// API:parts
        /// </summary>
        /// <param name="slot"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        public bool SetSlotWeaponPart(EWeaponSlotType slot, int id)
        {
            WeaponEntity entity = GetWeaponAgent(slot).Entity;
            if (entity == null)
            {
                throw new ExWeaponNotFoundException("{0} slot weapon not found", slot);
                return false;
            }
            NewWeaponConfigItem wpConfig = SingletonManager.Get<WeaponConfigManager>().GetConfigById(entity.weaponBasicData.ConfigId);
            WeaponPartsStruct lastParts = entity.weaponBasicData.GetParts();
            int realAttachId = BagUtility.GetRealAttachmentId(id, wpConfig.Id);
            bool match = SingletonManager.Get<WeaponPartsConfigManager>().IsPartMatchWeapon(realAttachId, wpConfig.Id);
            if (!match)
                return false;
            var attachments = WeaponPartsUtil.ModifyParts(
                entity.weaponBasicData.GetParts(),
                SingletonManager.Get<WeaponPartsConfigManager>().GetPartType(realAttachId),
                realAttachId);
            entity.weaponBasicData.ApplyParts(attachments);
            if (slot == HeldSlotType)
                RefreshHeldWeaponAttachment();
            WeaponPartsRefreshStruct refreshData = new WeaponPartsRefreshStruct();
            refreshData.weaponInfo = entity.ToWeaponScan();
            refreshData.slot = slot;
            refreshData.oldParts = lastParts;
            refreshData.newParts = entity.weaponBasicData.GetParts();
            RefreshModelWeaponParts(refreshData);
            return true;
        }

        /// <summary>
        /// API:parts
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public bool SetSlotWeaponPart(int id)
        {
            return SetSlotWeaponPart(HeldSlotType, id);
        }

        /// <summary>
        /// API:parts
        /// </summary>
        /// <param name="slot"></param>
        /// <param name="part"></param>
        public void DeleteSlotWeaponPart(EWeaponSlotType slot, EWeaponPartType part)
        {
            if (slot == EWeaponSlotType.None)
                return;
            WeaponEntity entity = GetWeaponAgent(slot).Entity;
            if (entity == null)
            {
                throw new ExWeaponNotFoundException("{0} slot weapon not found", slot);
                return;
            }

            WeaponPartsStruct lastParts = entity.weaponBasicData.GetParts();
            var parts = WeaponPartsUtil.ModifyParts(
                entity.weaponBasicData.GetParts(), part,
                UniversalConsts.InvalidIntId);
            entity.weaponBasicData.ApplyParts(parts);
            if (slot == HeldSlotType)
                RefreshHeldWeaponAttachment();
            var newParts = WeaponPartsUtil.ModifyParts(lastParts, part, UniversalConsts.InvalidIntId);
            newParts = newParts.ApplyDefaultParts(entity.weaponBasicData.ConfigId);
            WeaponPartsRefreshStruct refreshData = new WeaponPartsRefreshStruct();
            refreshData.weaponInfo = entity.ToWeaponScan();
            refreshData.slot = slot;
            refreshData.oldParts = lastParts;
            refreshData.newParts = newParts;
            RefreshModelWeaponParts(refreshData);
        }
    }
}
