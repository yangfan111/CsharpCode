using Core;
using Core.Utils;
using Utils.Configuration;
using Utils.Singleton;
using Utils.Utils;
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
        /// <param name="survivalCfgId"></param>
        /// <returns></returns>
        public bool SetWeaponPart(EWeaponSlotType slot, int survivalCfgId)
        {
            var agent = GetWeaponAgent(slot);
            if (!agent.IsValid()) return false;
            int matchedPartId = WeaponPartUtil.GetWeaponFstMatchedPartId(survivalCfgId, agent.ResConfig.Id);
            return SetWeaponPartByPartId(slot,matchedPartId);
        }

        public bool SetWeaponPartByPartId(EWeaponSlotType slot, int matchedPartId)
        {
            var agent = GetWeaponAgent(slot);
            if (!agent.IsValid()) return false;
            WeaponPartsStruct lastParts = agent.PartsScan;
            bool match = SingletonManager.Get<WeaponPartsConfigManager>().IsPartMatchWeapon(matchedPartId, agent.ResConfig.Id);
            if (!match) return false;
            var attachments = WeaponPartUtil.ModifyPartItem(lastParts,
                SingletonManager.Get<WeaponPartsConfigManager>().GetPartType(matchedPartId), matchedPartId);
            agent.BaseComponent.ApplyParts(attachments);
            if (slot == HeldSlotType)
                RefreshHeldWeaponAttachment();
            WeaponPartsRefreshStruct refreshData = new WeaponPartsRefreshStruct();
            refreshData.weaponInfo = agent.ComponentScan;
            refreshData.slot = slot;
            refreshData.oldParts = lastParts;
            refreshData.newParts = agent.PartsScan;
            RefreshModelWeaponParts(refreshData);
            DebugUtil.MyLog("Set Weapon part:" + refreshData, DebugUtil.DebugColor.Green);
            return true;
        }

        /// <summary>
        /// API:parts
        /// </summary>
        /// <param name ="id"></param>
        /// <returns></returns>
        public bool SetWeaponPart(int survivalCfgId)
        {
            return SetWeaponPart(HeldSlotType, survivalCfgId);
        }

        public bool SetWeaponPartByPartId(int survivalCfgId)
        {
            return SetWeaponPartByPartId(HeldSlotType, survivalCfgId);
        }

        /// <summary>
        /// API:parts
        /// </summary>
        /// <param name ="slot"></param>
        /// <param name ="partType"></param>
        public void DeleteWeaponPart(EWeaponSlotType slot, EWeaponPartType partType)
        {
            var agent = GetWeaponAgent(slot);
            if (!agent.IsValid()) return;
            WeaponPartsStruct lastParts = agent.PartsScan;
            var newParts = WeaponPartUtil.ModifyPartItem(lastParts, partType,0);
            agent.BaseComponent.ApplyParts(newParts);
            if (slot == HeldSlotType)
                RefreshHeldWeaponAttachment();
            WeaponPartsRefreshStruct refreshData = new WeaponPartsRefreshStruct();
            refreshData.weaponInfo = agent.ComponentScan;
            refreshData.slot = slot;
            refreshData.oldParts = lastParts;
            refreshData.newParts = newParts;
            RefreshModelWeaponParts(refreshData);
            DebugUtil.MyLog("Delete Weapon part:"+refreshData,DebugUtil.DebugColor.Green);
        }
    }
}
