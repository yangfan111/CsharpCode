using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Core.Utils;
using UnityEngine;
using Utils.AssetManager;
using Object = UnityEngine.Object;
using Utils.CharacterState;
using Utils.Configuration;
using Utils.Singleton;

namespace Utils.Appearance
{
    class OverrideAnimator
    {
        private static LoggerAdapter _logger = new LoggerAdapter(typeof(OverrideAnimator));

        private readonly Animator _animator;
        private bool _firstPerson;

        public OverrideAnimator(Animator animator, bool firstPerson)
        {
            _animator = animator;
            _firstPerson = firstPerson;
        }

        public void Change(Sex sex, bool unique, int weaponId)
        {
            if(unique) return;
            
            if (_firstPerson)
            {
                ChangeFirstPersonAnimation(sex, weaponId);
            }
            else
            {
                ChangeThirdPersonAnimation(sex, weaponId);
            }
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
                _logger.ErrorFormat("{0} is not preloaded!", assetInfo);
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

        private void ApplyOverrideController(Object obj)
        {
            AnimatorOverrideController newAnim = obj as AnimatorOverrideController;
            if (newAnim != null && _animator.gameObject.activeSelf)
            {
                //_logger.InfoFormat("change clip from:{0} to:{1}", _animator.runtimeAnimatorController.name, newAnim.name);
                // should be like this, all override controllers share RuntimeAnimatorController, which set by Editor
                _animator.runtimeAnimatorController = newAnim;
                // 虽然保留了状态，但是骨骼数据没有更新，之前取消是因为异步，导致UpdateBone在animator.Update(0)之前，会覆盖上半身修正的数据，导致闪了一帧
                _animator.Update(0);
            }
        }
    }
}
