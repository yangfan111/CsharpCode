using System.Collections.Generic;
using UnityEngine;
using Utils.Appearance;
using System.Linq;
using App.Shared.Components.Player;
using Core.Utils;
using Utils.Appearance.Bone;

namespace App.Shared.GameModules.Player.Appearance
{
    public class RagDollController
    {
        private readonly LoggerAdapter _logger = new LoggerAdapter(typeof(RagDollController));

        private RagDollComponent _ragDollComponent;
        private GameObject _characterP3;
        private HitBoxCache _hitBoxHandler;
        private readonly List<Rigidbody> _ragDollList = new List<Rigidbody>();
        private readonly List<Collider> _ragDollColliderList = new List<Collider>();

        public void SetThirdPersonCharacter(GameObject obj)
        {
            _characterP3 = obj;
            _hitBoxHandler = obj.GetComponent<HitBoxCache>();
            
            if(null == _characterP3) return;
            
            _ragDollList.Clear();
            _characterP3.GetComponentsInChildren(_ragDollList);
            
            _ragDollColliderList.Clear();
            _characterP3.GetComponentsInChildren(_ragDollColliderList);
        }

        public void SetRagDollComponent(RagDollComponent component)
        {
            _ragDollComponent = component;
        }
        
        public void ControlRagDoll(bool enable)
        {
            foreach (var v in _ragDollList)
            {
                if(null == v) continue;
                v.detectCollisions = enable;
                v.isKinematic = !enable;

                if (enable && v.transform.name.Equals(_ragDollComponent.RigidBodyTransformName))
                    AddImpulse(v, _ragDollComponent.ForceAtPosition, _ragDollComponent.Impulse);
            }
            
            var hitBoxList = _hitBoxHandler.GetHitBox().Values;
            foreach (var v in _ragDollColliderList)
            {
                if(null == v || hitBoxList.Contains(v)) continue;
                v.enabled = enable;
            }
            
            if (!enable)
                ResetRagDollRootBoneTransform();
        }

        private void AddImpulse(Rigidbody rigid, Vector3 position, Vector3 force)
        {
            if (null == rigid)
            {
                _logger.ErrorFormat("HitBox RigidBody is Null");
                return;
            }
            
            rigid.AddForceAtPosition(force, position, ForceMode.Impulse);
        }
        
        private void ResetRagDollRootBoneTransform()
        {
            if (null == _characterP3) return;
            var rootBone = BoneMount.FindChildBoneFromCache(_characterP3, BoneName.CharacterBipPelvisBoneName);
            if(null == rootBone) return;
            rootBone.localPosition = Vector3.zero;
            rootBone.localRotation = Quaternion.identity;
        }
    }
}
