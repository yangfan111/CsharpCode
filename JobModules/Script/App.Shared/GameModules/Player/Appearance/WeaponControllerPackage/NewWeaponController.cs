using System;
using App.Shared.Components.Player;
using Core.CharacterState.Action;
using UnityEngine;
using Utils.Appearance;
using Utils.Appearance.Weapon.WeaponShowPackage;
using Utils.AssetManager;
using XmlConfig;

namespace App.Shared.GameModules.Player.Appearance.WeaponControllerPackage
{
    public class NewWeaponController : NewWeaponControllerBase
    {
        public void SetupChangeWeaponAnimation(ICharacterAction action)
        {
            if(null == action) return;
            WeaponAnimation.MountWeapon = action.Select;
            WeaponAnimation.UnMountWeapon = action.Holster;
            WeaponAnimation.ChangeWeapon = action.SwitchWeapon;
            WeaponAnimation.InterruptPlayerAnimation = action.InterruptAction;
            WeaponAnimation.InterruptChangeWeaponAnimation = action.InterruptSwitchWeapon;
        }

        public void SetCacheChangeAction(Action cacheChangeAction)
        {
            CacheChangeAction = cacheChangeAction;
        }
        
        public void SyncFromLatestComponent(LatestAppearanceComponent value)
        {
            CopyFromWeaponIdsComponent(value);
            CopyFromAttachmentIdsComponent(value);
        }
        
        private void CopyFromWeaponIdsComponent(LatestAppearanceComponent value)
        {
            if(null == value) return;
            SetWeaponIdByType(WeaponInPackage.PrimaryWeaponOne, value.PrimaryWeaponOne);
            SetWeaponIdByType(WeaponInPackage.PrimaryWeaponTwo, value.PrimaryWeaponTwo);
            SetWeaponIdByType(WeaponInPackage.SideArm, value.SideArm);
            SetWeaponIdByType(WeaponInPackage.MeleeWeapon, value.MeleeWeapon);
            SetWeaponIdByType(WeaponInPackage.ThrownWeapon, value.ThrownWeapon);
            SetWeaponIdByType(WeaponInPackage.TacticWeapon, value.TacticWeapon);
        }

        private void CopyFromAttachmentIdsComponent(LatestAppearanceComponent value)
        {
            if(null == value) return;
            SetAttachmentIdByType(WeaponInPackage.PrimaryWeaponOne, WeaponPartLocation.Muzzle, value.PrimaryWeaponOneMuzzle);
            SetAttachmentIdByType(WeaponInPackage.PrimaryWeaponOne, WeaponPartLocation.LowRail, value.PrimaryWeaponOneLowRail);
            SetAttachmentIdByType(WeaponInPackage.PrimaryWeaponOne, WeaponPartLocation.Magazine, value.PrimaryWeaponOneMagazine);
            SetAttachmentIdByType(WeaponInPackage.PrimaryWeaponOne, WeaponPartLocation.Buttstock, value.PrimaryWeaponOneButtstock);
            SetAttachmentIdByType(WeaponInPackage.PrimaryWeaponOne, WeaponPartLocation.Scope, value.PrimaryWeaponOneScope);
            
            SetAttachmentIdByType(WeaponInPackage.PrimaryWeaponTwo, WeaponPartLocation.Muzzle, value.PrimaryWeaponTwoMuzzle);
            SetAttachmentIdByType(WeaponInPackage.PrimaryWeaponTwo, WeaponPartLocation.LowRail, value.PrimaryWeaponTwoLowRail);
            SetAttachmentIdByType(WeaponInPackage.PrimaryWeaponTwo, WeaponPartLocation.Magazine, value.PrimaryWeaponTwoMagazine);
            SetAttachmentIdByType(WeaponInPackage.PrimaryWeaponTwo, WeaponPartLocation.Buttstock, value.PrimaryWeaponTwoButtstock);
            SetAttachmentIdByType(WeaponInPackage.PrimaryWeaponTwo, WeaponPartLocation.Scope, value.PrimaryWeaponTwoScope);
            
            SetAttachmentIdByType(WeaponInPackage.SideArm, WeaponPartLocation.Muzzle, value.SideArmMuzzle);
            SetAttachmentIdByType(WeaponInPackage.SideArm, WeaponPartLocation.LowRail, value.SideArmLowRail);
            SetAttachmentIdByType(WeaponInPackage.SideArm, WeaponPartLocation.Magazine, value.SideArmMagazine);
            SetAttachmentIdByType(WeaponInPackage.SideArm, WeaponPartLocation.Buttstock, value.SideArmButtstock);
            SetAttachmentIdByType(WeaponInPackage.SideArm, WeaponPartLocation.Scope, value.SideArmScope);
        }

        public void SyncFromPredictedComponent(PredictedAppearanceComponent value)
        {
            CopyFromHandWeaponTypeComponent(value);
        }

        private void CopyFromHandWeaponTypeComponent(PredictedAppearanceComponent value)
        {
            if(null == value) return;
            SetHandWeaponType((WeaponInPackage) value.WeaponInHand);
            SetChangeOverrideControllerType((OverrideControllerState) value.OverrideControllerState);
        }
        
        protected override AbstractLoadRequest CreateLoadRequest(AssetInfo assetInfo, ILoadedHandler loadedHandler)
        {
            return LoadRequestFactory.Create<PlayerEntity>(assetInfo, loadedHandler.OnLoadSuccess);
        }
    }
}
