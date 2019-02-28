using System.Text.RegularExpressions;
using Core.Appearance;
using Core.CharacterController;
using Core.Utils;
using UnityEngine;
using Utils.Configuration;
using Utils.Singleton;
using XmlConfig;

namespace App.Shared.GameModules.Player.Appearance
{
    public class CharacterControllerManager : ICharacterControllerAppearance
    {
        private GameObject _characterRoot;
        private ICharacterControllerContext _controller;
        private Transform _attachedHead;
        private readonly Regex _attachedHeadNameRegex = new Regex(@"FHead\d*\(Clone\)");
        
        public void SetCharacterRoot(GameObject characterRoot)
        {
            _characterRoot = characterRoot;
        }
        
        public void SetCharacterController(ICharacterControllerContext controller)
        {
            _controller = controller;
        }
        
        public void PlayerDead()
        {
            SetLayer(_characterRoot, UnityLayerManager.GetLayerIndex(EUnityLayerName.NoCollisionWithEntity));
            closeEye();
            //_controller.enabled = false;
        }

        public void PlayerReborn()
        {
            SetLayer(_characterRoot, UnityLayerManager.GetLayerIndex(EUnityLayerName.Player));
            openEye();
            _controller.enabled = true;
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
        public void SetCharacterControllerHeight(float height, bool baseOnFoot)
        {
            var characterController = _characterRoot.GetComponent<CharacterController>();
            if (characterController != null)
            {
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
                    newCenter.y = SingletonManager.Get<CharacterStateConfigManager>().GetCharacterControllerCapsule(PostureInConfig.Stand).Height - 0.5f * height;
                    characterController.center = newCenter;
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

                return SingletonManager.Get<CharacterStateConfigManager>().GetCharacterControllerCapsule(PostureInConfig.Stand).Height;
            }
        }

        public void SetCharacterControllerCenter(Vector3 value)
        {
            var characterController = _characterRoot.GetComponent<CharacterController>();
            if (characterController != null)
            {
                characterController.center = value;
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
                    0.5f * SingletonManager.Get<CharacterStateConfigManager>().GetCharacterControllerCapsule(PostureInConfig.Stand)
                        .Height, 0);
            }
        }

        public void SetCharacterControllerRadius(float value)
        {
            var characterController = _characterRoot.GetComponent<CharacterController>();
            if (characterController != null)
            {
                characterController.radius = value;
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

                return SingletonManager.Get<CharacterStateConfigManager>().GetCharacterControllerCapsule(PostureInConfig.Stand).Radius;
            }
        }
        
        private static void SetLayer(GameObject gameObject, int layer)
        {
            foreach (var v in gameObject.GetComponentsInChildren<Transform>())
            {
                v.gameObject.layer = layer;
            }
        }
    }
}