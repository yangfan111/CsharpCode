using UnityEngine;
using System;
using App.Shared.Components.Player;
using Utils.Appearance;
using Utils.Appearance.Weapon;
using Utils.AssetManager;

namespace App.Shared.GameModules.Player.Appearance.WeaponControllerPackage
{
    public class WeaponController : HallWeaponController, ICharacterLoadResource
    {
        #region sync

        public void SyncFromLatestComponent(LatestAppearanceComponent value)
        {
            CopyFromLatestWeaponComponent(value);
        }
        
        public void SyncFromPredictedComponent(PredictedAppearanceComponent value)
        {
            CopyFromPredictedWeaponComponent(value);
        }
        
        public void SyncFromClientComponent(ClientAppearanceComponent value)
        {
            CopyFromClientWeaponComponent(value);
        }

        public void SyncToLatestComponent(LatestAppearanceComponent value)
        {
            CopyToLatestWeaponComponent(value);
        }
        
        public void SyncToPredictedComponent(PredictedAppearanceComponent value)
        {
            CopyToPredictedWeaponComponent(value);
        }
        
        public void SyncToClientComponent(ClientAppearanceComponent value)
        {
            CopyToClientWeaponComponent(value);
        }
        
        #endregion

        public void SetWeaponChangedCallBack(Action<GameObject, GameObject> callBack)
        {
            _weaponChangedCallBack = callBack;
        }

        public void SetCacheChangeAction(Action cacheChangeAction)
        {
            _cacheChangeAction = cacheChangeAction;
        }

        #region Hepler

        private void CopyFromLatestWeaponComponent(LatestAppearanceComponent value)
        {
            if(null == value) return;
            SetLatestWeaponValue(LatestWeaponStateIndex.PrimaryWeaponOne, value.PrimaryWeaponOne);
            SetLatestWeaponValue(LatestWeaponStateIndex.PrimaryWeaponOneMuzzle, value.PrimaryWeaponOneMuzzle);
            SetLatestWeaponValue(LatestWeaponStateIndex.PrimaryWeaponOneLowRail, value.PrimaryWeaponOneLowRail);
            SetLatestWeaponValue(LatestWeaponStateIndex.PrimaryWeaponOneMagazine, value.PrimaryWeaponOneMagazine);
            SetLatestWeaponValue(LatestWeaponStateIndex.PrimaryWeaponOneButtstock, value.PrimaryWeaponOneButtstock);
            SetLatestWeaponValue(LatestWeaponStateIndex.PrimaryWeaponOneScope, value.PrimaryWeaponOneScope);
            SetLatestWeaponValue(LatestWeaponStateIndex.PrimaryWeaponTwo, value.PrimaryWeaponTwo);
            SetLatestWeaponValue(LatestWeaponStateIndex.PrimaryWeaponTwoMuzzle, value.PrimaryWeaponTwoMuzzle);
            SetLatestWeaponValue(LatestWeaponStateIndex.PrimaryWeaponTwoLowRail, value.PrimaryWeaponTwoLowRail);
            SetLatestWeaponValue(LatestWeaponStateIndex.PrimaryWeaponTwoMagazine, value.PrimaryWeaponTwoMagazine);
            SetLatestWeaponValue(LatestWeaponStateIndex.PrimaryWeaponTwoButtstock, value.PrimaryWeaponTwoButtstock);
            SetLatestWeaponValue(LatestWeaponStateIndex.PrimaryWeaponTwoScope, value.PrimaryWeaponTwoScope);
            SetLatestWeaponValue(LatestWeaponStateIndex.SideArm, value.SideArm);
            SetLatestWeaponValue(LatestWeaponStateIndex.SideArmMuzzle, value.SideArmMuzzle);
            SetLatestWeaponValue(LatestWeaponStateIndex.SideArmLowRail, value.SideArmLowRail);
            SetLatestWeaponValue(LatestWeaponStateIndex.SideArmMagazine, value.SideArmMagazine);
            SetLatestWeaponValue(LatestWeaponStateIndex.SideArmButtstock, value.SideArmButtstock);
            SetLatestWeaponValue(LatestWeaponStateIndex.SideArmScope, value.SideArmScope);
            SetLatestWeaponValue(LatestWeaponStateIndex.MeleeWeapon, value.MeleeWeapon);
            SetLatestWeaponValue(LatestWeaponStateIndex.ThrownWeapon, value.ThrownWeapon);
            SetLatestWeaponValue(LatestWeaponStateIndex.TacticWeapon, value.TacticWeapon);
        }
        
        private void CopyFromClientWeaponComponent(ClientAppearanceComponent value)
        {
            if(null == value) return;
            SetClientWeaponValue(ClientWeaponStateIndex.AlternativeWeaponLocator, value.AlternativeWeaponLocator);
            SetClientWeaponValue(ClientWeaponStateIndex.AlternativeP3WeaponLocator, value.AlternativeP3WeaponLocator);
        }

        private void CopyFromPredictedWeaponComponent(PredictedAppearanceComponent value)
        {
            if(null == value) return;
            SetPredictedWeaponValue(PredictedWeaponStateIndex.WeaponInHand, value.WeaponInHand);
            SetPredictedWeaponValue(PredictedWeaponStateIndex.ReloadState, value.ReloadState);
            SetPredictedWeaponValue(PredictedWeaponStateIndex.OverrideControllerState, value.OverrideControllerState);
        }

        private void CopyToLatestWeaponComponent(LatestAppearanceComponent value)
        {
            if(null == value) return;
            value.PrimaryWeaponOne = GetLatestWeaponValue(LatestWeaponStateIndex.PrimaryWeaponOne);
            value.PrimaryWeaponOneMuzzle = GetLatestWeaponValue(LatestWeaponStateIndex.PrimaryWeaponOneMuzzle);
            value.PrimaryWeaponOneLowRail = GetLatestWeaponValue(LatestWeaponStateIndex.PrimaryWeaponOneLowRail);
            value.PrimaryWeaponOneMagazine = GetLatestWeaponValue(LatestWeaponStateIndex.PrimaryWeaponOneMagazine);
            value.PrimaryWeaponOneButtstock = GetLatestWeaponValue(LatestWeaponStateIndex.PrimaryWeaponOneButtstock);
            value.PrimaryWeaponOneScope = GetLatestWeaponValue(LatestWeaponStateIndex.PrimaryWeaponOneScope);
            value.PrimaryWeaponTwo = GetLatestWeaponValue(LatestWeaponStateIndex.PrimaryWeaponTwo);
            value.PrimaryWeaponTwoMuzzle = GetLatestWeaponValue(LatestWeaponStateIndex.PrimaryWeaponTwoMuzzle);
            value.PrimaryWeaponTwoLowRail = GetLatestWeaponValue(LatestWeaponStateIndex.PrimaryWeaponTwoLowRail);
            value.PrimaryWeaponTwoMagazine = GetLatestWeaponValue(LatestWeaponStateIndex.PrimaryWeaponTwoMagazine);
            value.PrimaryWeaponTwoButtstock = GetLatestWeaponValue(LatestWeaponStateIndex.PrimaryWeaponTwoButtstock);
            value.PrimaryWeaponTwoScope = GetLatestWeaponValue(LatestWeaponStateIndex.PrimaryWeaponTwoScope);
            value.SideArm = GetLatestWeaponValue(LatestWeaponStateIndex.SideArm);
            value.SideArmMuzzle = GetLatestWeaponValue(LatestWeaponStateIndex.SideArmMuzzle);
            value.SideArmLowRail = GetLatestWeaponValue(LatestWeaponStateIndex.SideArmLowRail);
            value.SideArmMagazine = GetLatestWeaponValue(LatestWeaponStateIndex.SideArmMagazine);
            value.SideArmButtstock = GetLatestWeaponValue(LatestWeaponStateIndex.SideArmButtstock);
            value.SideArmScope = GetLatestWeaponValue(LatestWeaponStateIndex.SideArmScope);
            value.MeleeWeapon = GetLatestWeaponValue(LatestWeaponStateIndex.MeleeWeapon);
            value.ThrownWeapon = GetLatestWeaponValue(LatestWeaponStateIndex.ThrownWeapon);
            value.TacticWeapon = GetLatestWeaponValue(LatestWeaponStateIndex.TacticWeapon);
        }

        private void CopyToPredictedWeaponComponent(PredictedAppearanceComponent value)
        {
            if(null == value) return;
            value.WeaponInHand = GetPredictedWeaponValue(PredictedWeaponStateIndex.WeaponInHand);
            value.ReloadState = GetPredictedWeaponValue(PredictedWeaponStateIndex.ReloadState);
            value.OverrideControllerState = GetPredictedWeaponValue(PredictedWeaponStateIndex.OverrideControllerState);
        }
        
        private void CopyToClientWeaponComponent(ClientAppearanceComponent value)
        {
            if(null == value) return;
            value.AlternativeWeaponLocator = GetClientWeaponValue(ClientWeaponStateIndex.AlternativeWeaponLocator);
            value.AlternativeP3WeaponLocator = GetClientWeaponValue(ClientWeaponStateIndex.AlternativeP3WeaponLocator);
        }

        protected override AbstractLoadRequest CreateLoadRequest(AssetInfo assetInfo, ILoadedHandler loadedHanlder)
        {
            return LoadRequestFactory.Create<PlayerEntity>(assetInfo, loadedHanlder.OnLoadSucc);
        }

        #endregion
    }
}
