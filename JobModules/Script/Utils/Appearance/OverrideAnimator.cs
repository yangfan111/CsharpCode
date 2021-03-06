﻿using Core.Utils;
using UnityEngine;
using Utils.Appearance.Weapon.WeaponShowPackage;
using Utils.AssetManager;
using Object = UnityEngine.Object;
using Utils.CharacterState;
using Utils.Configuration;
using Utils.Singleton;
using Utils.Utils;

namespace Utils.Appearance
{
    public class OverrideAnimator : ParamBase
    {
        private static readonly LoggerAdapter Logger = new LoggerAdapter(typeof(OverrideAnimator));

        private readonly bool _firstPerson;
        
        private readonly CustomProfileInfo _changeOverrideControllerInfo;
        private readonly CustomProfileInfo _changeOverrideControllerUpdateAnimationInfo;

        private Animator _animator;
        private Sex _sex;
        private bool _unique;

        protected OverrideAnimator(bool firstPerson)
        {
            _changeOverrideControllerInfo = SingletonManager.Get<DurationHelp>().GetCustomProfileInfo("ChangeOverrideController");
            _changeOverrideControllerUpdateAnimationInfo = 
                SingletonManager.Get<DurationHelp>().GetCustomProfileInfo("ChangeOverrideControllerUpdateAnimation");
            
            _firstPerson = firstPerson;
        }
        
        public void SetSex(Sex sex)
        {
            _sex = sex;
        }

        public void SetUnique(bool unique)
        {
            _unique = unique;
        }
        
        public void SetAnimator(Animator animator)
        {
            _animator = animator;
            Change(UniversalConsts.InvalidIntId);
        }

        protected void Change(int weaponId)
        {
            if(_unique) return;

            try
            {
                _changeOverrideControllerInfo.BeginProfileOnlyEnableProfile();
                if (_firstPerson)
                {
                    ChangeFirstPersonAnimation(_sex, weaponId);
                }
                else
                {
                    ChangeThirdPersonAnimation(_sex, weaponId);
                }
            }
            finally
            {
                _changeOverrideControllerInfo.EndProfileOnlyEnableProfile();
            }
        }
        
        public void ChangeTransition(int weaponId)
        {
            if (_firstPerson) return;
            ChangeThirdPersonTransitionAnimation(_sex, weaponId);
        }

        private void ChangeFirstPersonAnimation(Sex sex, int weaponId)
        {
            if (weaponId <= 0)
            {
                var assetInfo = SingletonManager.Get<WeaponAvatarConfigManager>().GetEmptyHandedFirstPersonAnim(sex);
                ProcessAsset(assetInfo);
            }
            else
            {
                var assetAddr = SingletonManager.Get<WeaponAvatarConfigManager>().GetFirstPersonAnimation(weaponId, sex);
                if (!string.IsNullOrEmpty(assetAddr.BundleName) && !string.IsNullOrEmpty(assetAddr.AssetName))
                {
                    ProcessAsset(assetAddr);
                }
            }
        }

        private void ProcessAsset(AssetInfo assetInfo)
        {
            var obj = SingletonManager.Get<WeaponAvatarConfigManager>().GetOrNull(assetInfo);
            if (obj == null)
            {
                Logger.ErrorFormat("{0} is not preloaded!", assetInfo);
            }
            else
            {
                ApplyOverrideController(obj);
            }
        }

        private void ChangeThirdPersonAnimation(Sex sex, int weaponId)
        {
            if (weaponId <= 0)
            {
                var assetInfo = SingletonManager.Get<WeaponAvatarConfigManager>().GetEmptyHandedThirdPersonAnim(sex);
                ProcessAsset(assetInfo);
            }
            else
            {
                var assetAddr = SingletonManager.Get<WeaponAvatarConfigManager>().GetThirdPersonAnimation(weaponId, sex);
                if (!string.IsNullOrEmpty(assetAddr.BundleName) && !string.IsNullOrEmpty(assetAddr.AssetName))
                {
                    ProcessAsset(assetAddr);
                }
            }
        }
        
        private void ChangeThirdPersonTransitionAnimation(Sex sex, int weaponId)
        {
            var assetInfo = SingletonManager.Get<WeaponAvatarConfigManager>().GetThirdPersonAnimationTransition(weaponId, sex);
            if (!string.IsNullOrEmpty(assetInfo.BundleName) && !string.IsNullOrEmpty(assetInfo.AssetName))
            {
                ProcessAsset(assetInfo);
            }
        }

        private void ApplyOverrideController(Object obj)
        {
            var newAnim = obj as AnimatorOverrideController;
            if (newAnim != null && _animator.gameObject.activeSelf)
            {
                //_logger.InfoFormat("change clip from:{0} to:{1}", _animator.runtimeAnimatorController.name, newAnim.name);
                // should be like this, all override controllers share RuntimeAnimatorController, which set by Editor
                try
                {
                    _changeOverrideControllerUpdateAnimationInfo.BeginProfileOnlyEnableProfile();
                    _animator.runtimeAnimatorController = newAnim;
                    // 虽然保留了状态，但是骨骼数据没有更新，之前取消是因为异步，导致UpdateBone在animator.Update(0)之前，会覆盖上半身修正的数据，导致闪了一帧
                    //_animator.Update(0);
                }
                finally
                {
                    _changeOverrideControllerUpdateAnimationInfo.EndProfileOnlyEnableProfile(); 
                }
            }
        }
    }
}
