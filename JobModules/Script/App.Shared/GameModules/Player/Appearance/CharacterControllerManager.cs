using System.Collections.Generic;
using App.Shared.GameModules.HitBox;
using Core.Appearance;
using Core.CharacterController;
using Core.Utils;
using UnityEngine;
using Utils.Appearance.Effects;
using Utils.Configuration;
using Utils.Singleton;

namespace App.Shared.GameModules.Player.Appearance
{
    public class CharacterControllerManager : ICharacterControllerAppearance
    {
        private static readonly LoggerAdapter Logger = new LoggerAdapter(typeof(CharacterController));
        
        private const string BlinkEyeScriptName = "BlinkEye";
        private readonly List<Collider> _colliderList = new List<Collider>();
        
        private readonly CustomProfileInfo _subSetLayerInfo;
        private readonly CustomProfileInfo _subCloseEyeInfo;
        private readonly CustomProfileInfo _subOpenEyeInfo;
        
        private GameObject _characterRoot;
        private ICharacterControllerContext _controller;
        private GameObject _thirdModel;
        
        public CharacterControllerManager()
        {
            _subSetLayerInfo = SingletonManager.Get<DurationHelp>().GetCustomProfileInfo("CharacterControllerManagerSetLayer");
            _subCloseEyeInfo = SingletonManager.Get<DurationHelp>().GetCustomProfileInfo("CharacterControllerManagerCloseEye");
            _subOpenEyeInfo = SingletonManager.Get<DurationHelp>().GetCustomProfileInfo("CharacterControllerManagerOpenEye");
        }
        
        public void SetCharacterRoot(GameObject characterRoot)
        {
            _characterRoot = characterRoot;
            if(null == _characterRoot) return;
            _colliderList.Clear();
            _characterRoot.GetComponentsInChildren(_colliderList);
        }

        public void SetThirdModel(GameObject model)
        {
            _thirdModel = model;
        }
        
        public void SetCharacterController(ICharacterControllerContext controller)
        {
            _controller = controller;
        }
        
        public void PlayerDead(bool isSelf = true)
        {
            SetLayer(_thirdModel, UnityLayerManager.GetLayerIndex(EUnityLayerName.NoCollisionWithEntity));
            CloseEye();
            _controller.enabled = false;
            Logger.InfoFormat("CharacterLog-- PlayerDead :{0}", _characterRoot);
        }

        public void PlayerReborn()
        {
            SetLayer(_thirdModel, UnityLayerManager.GetLayerIndex(EUnityLayerName.Player));
            OpenEye();
            _controller.enabled = true;
            Logger.InfoFormat("PlayerReborn: {0}", _characterRoot);
        }

        private void OpenEye()
        {
            try
            {
                _subOpenEyeInfo.BeginProfileOnlyEnableProfile();
                
                var script = EffectUtility.GetEffect(_characterRoot,BlinkEyeScriptName);
                if (script == null) return;
                script.SetParam("Enable", true);
            }
            finally
            {
                _subOpenEyeInfo.EndProfileOnlyEnableProfile();
            }
        }

        private void CloseEye()
        {
            try
            {
                _subCloseEyeInfo.BeginProfileOnlyEnableProfile();
                
                var script = EffectUtility.GetEffect(_characterRoot, BlinkEyeScriptName);
                if (script == null) return;
                script.SetParam("Enable", false);
            }
            finally
            {
                _subCloseEyeInfo.EndProfileOnlyEnableProfile();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="height"></param>
        /// <param name="baseOnFoot">true 脚底高度不变， false 头顶高度不变</param>
        public void SetCharacterControllerHeight(float height, bool updateCapsule, float standHeight, bool baseOnFoot)
        {
            var characterController = _characterRoot.GetComponent<CharacterController>();
            if (characterController != null)
            {
                //Logger.InfoFormat("change height from:{0} to:{1}, baseOnFoot:{2}", characterController.height, height, baseOnFoot);

                characterController.height = height;
                if (baseOnFoot)
                {
                    var newCenter = characterController.center;
                    newCenter.y = height * 0.5f;
                    characterController.center = newCenter;
                }
                else
                {
                    var newCenter = characterController.center;
                    newCenter.y = standHeight - 0.5f * height;
                    characterController.center = newCenter;
                }
            }
            
            if (updateCapsule)
            {
                var capsule = _characterRoot.GetComponent<CapsuleCollider>();
                if (capsule != null)
                {
                    capsule.height = height;
                    if (baseOnFoot)
                    {
                        var newCenter = capsule.center;
                        newCenter.y = height * 0.5f;
                        capsule.center = newCenter;
                    }
                    else
                    {
                        var newCenter = capsule.center;
                        newCenter.y = standHeight - 0.5f * height;
                        capsule.center = newCenter;
                    }
                }
            }
        }

        public float GetCharacterControllerHeight
        {
            get
            {
                var characterController = _characterRoot.GetComponent<CharacterController>();
                if (characterController != null)
                {
                    return characterController.height;
                }

                return SingletonManager.Get<CharacterInfoManager>().GetDefaultInfo().StandHeight;
            }
        }

        public void SetCharacterControllerCenter(Vector3 value, bool updateCapsule)
        {
            var characterController = _characterRoot.GetComponent<CharacterController>();
            if (characterController != null)
            {
                characterController.center = value;
            }
            
            if (updateCapsule)
            {
                var capsule = _characterRoot.GetComponent<CapsuleCollider>();
                if (capsule != null)
                {
                    capsule.center = value;
                }
            }
        }

        public Vector3 GetCharacterControllerCenter
        {
            get
            {
                var characterController = _characterRoot.GetComponent<CharacterController>();

                /*if (characterController != null)*/
                if (!ReferenceEquals(characterController, null))
                {
                    return characterController.center;
                }

                return new Vector3(0,
                    0.5f * SingletonManager.Get<CharacterInfoManager>().GetDefaultInfo().StandHeight, 0);
            }
        }

        public void SetCharacterControllerRadius(float value, bool updateCapsule)
        {
            var characterController = _characterRoot.GetComponent<CharacterController>();
            if (characterController != null)
            {
                characterController.radius = value;
            }

            if (updateCapsule)
            {
                var capsule = _characterRoot.GetComponent<CapsuleCollider>();
                if (capsule != null)
                {
                    capsule.radius = value;
                }
            }
        }

        public float GetCharacterControllerRadius
        {
            get
            {
                var characterController = _characterRoot.GetComponent<CharacterController>();
                if (characterController != null)
                {
                    return characterController.radius;
                }

                return SingletonManager.Get<CharacterInfoManager>().GetDefaultInfo().StandRadius;
            }
        }
        
        private void SetLayer(GameObject hitboxRoot, int layer)
        {
            try
            {
                _subSetLayerInfo.BeginProfileOnlyEnableProfile();
                var hitBoxTransforms = HitBoxComponentUtility.GetHitBoxTransforms(hitboxRoot);
                Logger.InfoFormat("Hitbox_{0} Num:{1}", hitboxRoot, hitBoxTransforms.Count);
                
                foreach (var v in _colliderList)
                {
                    if(null == v) continue;
                    if(!hitBoxTransforms.Contains(v.transform))
                        v.gameObject.layer = layer;
                }
            }
            finally
            {
                _subSetLayerInfo.EndProfileOnlyEnableProfile();
            }
        }
    }
}
