using System.Text.RegularExpressions;
using App.Shared.GameModules.HitBox;
using Core.Appearance;
using Core.CharacterController;
using Core.HitBox;
using Core.Utils;
using UnityEngine;
using Utils.Configuration;
using Utils.Singleton;
using XmlConfig;

namespace App.Shared.GameModules.Player.Appearance
{
    public class CharacterControllerManager : ICharacterControllerAppearance
    {
        private static readonly LoggerAdapter Logger = new LoggerAdapter(typeof(CharacterController));
        private GameObject _characterRoot;
        private ICharacterControllerContext _controller;
        private Transform _attachedHead;
        private static GameObject _thirdModel;
        private readonly Regex _attachedHeadNameRegex = new Regex(@"FHead\d*\(Clone\)");
        
        public void SetCharacterRoot(GameObject characterRoot)
        {
            _characterRoot = characterRoot;
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
            SetLayer(_characterRoot, UnityLayerManager.GetLayerIndex(EUnityLayerName.NoCollisionWithEntity));
            closeEye();
            if (!isSelf)
            {
                _controller.enabled = false;
            }

            Logger.InfoFormat("PlayerDead :{0}", _characterRoot);
        }

        public void PlayerReborn()
        {
            SetLayer(_characterRoot, UnityLayerManager.GetLayerIndex(EUnityLayerName.Player));
            openEye();
            _controller.enabled = true;
            Logger.InfoFormat("PlayerReborn: {0}", _characterRoot);
        }

        private void openEye()
        {
            if(_attachedHead==null)
                return;
            HandleOpenEye();
        }

        private void closeEye()
        {
            if (_attachedHead != null)
            {
                HandleOpenEye();
                return;
            }
            var trans = _characterRoot.GetComponentsInChildren<Transform>(true);
            foreach (var transform in trans)
            {  
                if (_attachedHeadNameRegex.IsMatch(transform.name))
                {
                    _attachedHead = transform;
                    HandleCloseEye();
                }
            }
        }
        
        private void HandleCloseEye()
        {
            _attachedHead.SendMessage("PlayerDead");
        }

        private void HandleOpenEye()
        {
            _attachedHead.SendMessage("PlayerRelive");
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
                if (characterController != null)
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
        
        private static void SetLayer(GameObject gameObject, int layer)
        {
            var hitBoxTransforms = HitBoxComponentUtility.GetHitBoxTransforms(_thirdModel);
            foreach (var v in gameObject.GetComponentsInChildren<Transform>())
            {
                if(!hitBoxTransforms.Contains(v))
                    v.gameObject.layer = layer;
            }
        }
    }
}