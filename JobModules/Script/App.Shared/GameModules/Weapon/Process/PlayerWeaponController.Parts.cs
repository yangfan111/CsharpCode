using Assets.Utils.Configuration;
using Core;
using Core.Utils;
using Utils.Configuration;
using Utils.Singleton;
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
        public void SetTestPart(int partId)
        {
            SetWeaponPartByPartId(HeldSlotType, 70002);
        }
        public bool SetWeaponPartByPartId(EWeaponSlotType slot, int matchedPartId)
        {
            var agent = GetWeaponAgent(slot);
            if (!agent.CanApplyPart) return false;
            WeaponPartsStruct lastParts = agent.PartsScan;
            bool match = SingletonManager.Get<WeaponConfigManagement>().FindConfigById(agent.ConfigId).IsPartMatchWeapon(matchedPartId);
            if (!match) return false;
            var attachments = WeaponPartUtil.ModifyPartItem(lastParts,
                SingletonManager.Get<WeaponPartsConfigManager>().GetPartType(matchedPartId), matchedPartId);
            agent.BaseComponent.ApplyParts(attachments);
            if (slot == HeldSlotType &&  processHelper.FilterRefreshWeapon())
                ApperanceRefreshABreath(HeldWeaponAgent.BreathFactor);
            WeaponPartsRefreshStruct refreshData = new WeaponPartsRefreshStruct();
            refreshData.weaponInfo = agent.ComponentScan;
            refreshData.slot = slot;
            refreshData.oldParts = lastParts;
            refreshData.newParts = agent.PartsScan;
            RefreshWeaponModelAndParts(refreshData);
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
            var defaultParts = agent.WeaponConfigAssy.DefaultParts;
            int newPart = 0; //移除配件后装备默认配件
            switch (partType)
            {
                case EWeaponPartType.LowerRail:
                    newPart = defaultParts.LowerRail;
                    break;
                case EWeaponPartType.UpperRail:
                    newPart = defaultParts.UpperRail;
                    break;
                case EWeaponPartType.Muzzle:
                    newPart = defaultParts.Muzzle;
                    break;
                case EWeaponPartType.Magazine:
                    newPart = defaultParts.Magazine;
                    break;
                case EWeaponPartType.Stock:
                    newPart = defaultParts.Stock;
                    break;
                case EWeaponPartType.SideRail:
                    newPart = defaultParts.SideRail;
                    break;
                case EWeaponPartType.Bore:
                    newPart = defaultParts.Bore;
                    break;
                case EWeaponPartType.Interlock:
                    newPart = defaultParts.Interlock;
                    break;
                case EWeaponPartType.Feed:
                    newPart = defaultParts.Feed;
                    break;
                case EWeaponPartType.Brake:
                    newPart = defaultParts.Brake;
                    break;
                case EWeaponPartType.Trigger:
                    newPart = defaultParts.Trigger;
                    break;
                default:
                    break;
            }
            if (newPart <= 0) newPart = 0;
            var newParts = WeaponPartUtil.ModifyPartItem(lastParts, partType, newPart);
            agent.BaseComponent.ApplyParts(newParts);
            if (slot == HeldSlotType)
            {
                if (processHelper.FilterRefreshWeapon()) 
                    ApperanceRefreshABreath(HeldWeaponAgent.BreathFactor);
            }
            WeaponPartsRefreshStruct refreshData = new WeaponPartsRefreshStruct();
            refreshData.weaponInfo = agent.ComponentScan;
            refreshData.slot = slot;
            refreshData.oldParts = lastParts;
            refreshData.newParts = newParts;
            RefreshWeaponModelAndParts(refreshData);
            DebugUtil.MyLog("Delete Weapon part:"+refreshData,DebugUtil.DebugColor.Green);
        }
    }
}
