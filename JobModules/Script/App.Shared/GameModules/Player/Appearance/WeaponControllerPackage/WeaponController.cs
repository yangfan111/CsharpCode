using Core.CharacterState;
using Core.Compare;
using Core.Configuration;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using System;
using App.Shared.Components.Player;
using Core.Utils;
using XmlConfig;
using Utils.Appearance;
using Utils.AssetManager;

namespace App.Shared.GameModules.Player.Appearance.WeaponControllerPackage
{
    public class WeaponController : WeaponControllerBase, ICharacterLoadResource
    {
        public WeaponController()
        {
        }
        
        #region sync

        public void SyncFromLatestComponent(LatestAppearanceComponent value)
        {
            CopyFromLatestWeaponComponent(value);
        }
        
        public void SyncFromPredictedComponent(PredictedAppearanceComponent value)
        {
            CopyFromPredictedWeaponComponent(value);
        }

        public void SyncToLatestComponent(LatestAppearanceComponent value)
        {
            CopyToLatestWeaponComponent(value);
        }
        
        public void SyncToPredictedComponent(PredictedAppearanceComponent value)
        {
            CopyToPredictedWeaponComponent(value);
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

        private void CopyFromPredictedWeaponComponent(PredictedAppearanceComponent value)
        {
            if(null == value) return;
            SetPredictedWeaponValue(PredictedWeaponStateIndex.WeaponInHand, value.WeaponInHand);
            SetPredictedWeaponValue(PredictedWeaponStateIndex.AlternativeWeaponLocator, value.AlternativeWeaponLocator);
            SetPredictedWeaponValue(PredictedWeaponStateIndex.AlternativeP3WeaponLocator, value.AlternativeP3WeaponLocator);
            SetPredictedWeaponValue(PredictedWeaponStateIndex.ReloadState, value.ReloadState);
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
            value.AlternativeWeaponLocator = GetPredictedWeaponValue(PredictedWeaponStateIndex.AlternativeWeaponLocator);
            value.AlternativeP3WeaponLocator = GetPredictedWeaponValue(PredictedWeaponStateIndex.AlternativeP3WeaponLocator);
            value.ReloadState = GetPredictedWeaponValue(PredictedWeaponStateIndex.ReloadState);
        }

        protected override AbstractLoadRequest CreateLoadRequest(AssetInfo assetInfo, ILoadedHandler loadedHanlder)
        {
            return LoadRequestFactory.Create<PlayerEntity>(assetInfo, loadedHanlder.OnLoadSucc);
        }

        #endregion
    }
}
