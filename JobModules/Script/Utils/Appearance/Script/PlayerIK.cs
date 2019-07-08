using Core.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Utils.Appearance.Bone;

namespace Utils.Appearance.Script
{
    public class PlayerIK : MonoBehaviour
    {
        private LoggerAdapter _logger = new LoggerAdapter(typeof(PlayerIK));
        class IKParam
        {
            public Transform Source;
            public Animator Animator;
            public int Layer;
            public AvatarIKGoal Goal;
            public bool IsActive;
            
            public Transform RootBone;
            public Transform MiddleBone;
            public Transform EndBone;
            public Vector3 HintPosition;
            public float Weight;
        }

        private bool _isIKActive;

        private IKParam[] _params = new IKParam[(int) AvatarIKGoal.RightHand + 1];

        public PlayerIK()
        {
            for (var i = 0; i <= (int)AvatarIKGoal.RightHand; ++i)
            {
                _params[i] = new IKParam();
            }
        }

        public void SetSource(AvatarIKGoal goal, Transform source)
        {
            _params[(int)goal].Source = source;
        }

        public void SetAnimator(AvatarIKGoal goal, Animator animator)
        {
            _params[(int)goal].Animator = animator;
            FindIkBones(goal, _params[(int)goal]);
        }

        public void SetIKGoal(AvatarIKGoal goal)
        {
            _params[(int)goal].Goal = goal;
            _params[(int)goal].IsActive = true;
        }

        public void SetIKLayer(AvatarIKGoal goal, int layer)
        {
            _params[(int)goal].Layer = layer;
        }

        public void SetIKActive(bool active)
        {
            _isIKActive = active;
            UpdateIKLayerWeight();
        }

        public void UpdateIk()
        {
            foreach (var iKParam in _params)
            {
                var param = iKParam;
                if (_isIKActive)
                {
                    if (param.IsActive)
                    {
                        EnableIK(param);
                    }
                    else
                    {
                        DisableIK(param);
                    }
                }
                else
                {
                    DisableIK(param);
                }
            }
        }

        private void UpdateIKLayerWeight()
        {
            foreach (var iKParam in _params)
            {
                var param = iKParam;
                if (_isIKActive)
                {
                    if (param.IsActive)
                    {
                        EnableIKLayerWeight(param);
                    }
                    else
                    {
                        DisableIKLayerWeight(param);
                    }
                }
                else
                {
                    DisableIKLayerWeight(param);
                }
            }
        }

        private void FindIkBones(AvatarIKGoal goal, IKParam param)
        {
            var animator = param.Animator;
            if (null == animator)
            {
                _logger.ErrorFormat("IkAnimatorIsNull");
                return;
            }
            
            switch (goal)
            {
                case AvatarIKGoal.LeftHand:
                    param.RootBone = GetBoneTransform(animator, HumanBodyBones.LeftUpperArm);
                    param.MiddleBone = GetBoneTransform(animator, HumanBodyBones.LeftLowerArm);
                    param.EndBone = GetBoneTransform(animator, HumanBodyBones.LeftHand);
                    break;
                case AvatarIKGoal.RightHand:
                    param.RootBone = GetBoneTransform(animator, HumanBodyBones.RightUpperArm);
                    param.MiddleBone = GetBoneTransform(animator, HumanBodyBones.RightLowerArm);
                    param.EndBone = GetBoneTransform(animator, HumanBodyBones.RightHand);
                    break;
                case AvatarIKGoal.LeftFoot:
                    param.RootBone = GetBoneTransform(animator, HumanBodyBones.LeftUpperLeg);
                    param.MiddleBone = GetBoneTransform(animator, HumanBodyBones.LeftLowerLeg);
                    param.EndBone = GetBoneTransform(animator, HumanBodyBones.LeftFoot);
                    break;
                case AvatarIKGoal.RightFoot:
                    param.RootBone = GetBoneTransform(animator, HumanBodyBones.RightUpperLeg);
                    param.MiddleBone = GetBoneTransform(animator, HumanBodyBones.RightLowerLeg);
                    param.EndBone = GetBoneTransform(animator, HumanBodyBones.RightFoot);
                    break;
            }
        }

        private Transform GetBoneTransform(Animator animator, HumanBodyBones bone)
        {
            var obj = animator.gameObject;
            if (!HumanBodyBonesMappingTable.Table.ContainsKey(bone)) return null;
            return BoneMount.FindChildBone2(obj, HumanBodyBonesMappingTable.Table[bone]);
        }

        private void EnableIK(IKParam param)
        {
            // 对于第三人称的手，相应的参数均为初始值
            if (param.Animator != null && param.Layer >= 0 && param.Source != null)
            {
                SetIKPosition(param);
                SetIKRotation(param);
            }
        }

        private void EnableIKLayerWeight(IKParam param)
        {
            if (param.Animator != null && param.Layer >= 0 && param.Source != null)
            {
                param.Animator.SetLayerWeight(param.Layer, 1);
                param.Weight = 1;
            }
        }
        
        private void DisableIK(IKParam param)
        {
            if (param.Animator != null && param.Layer >= 0 && param.Source != null)
            {
                param.Weight = 0;
            }
        }

        private void DisableIKLayerWeight(IKParam param)
        {
            if (param.Animator != null && param.Layer >= 0 && param.Source != null)
            {
                param.Animator.SetLayerWeight(param.Layer, 0);
                param.Weight = 0;
            }
        }

        public void ClearAllIKTarget()
        {
            foreach (var item in _params)
            {
                item.Source = null;
                item.IsActive = false;
            }
        }
        
        /// <summary>
        /// Set IK Position
        /// </summary>
        /// <param name="ikPosition"></param>
        private void SetIKPosition(IKParam param)
        {
            if (!(param.RootBone && param.MiddleBone && param.EndBone) || param.Weight <= 0.0f) return;
            // Calculate middleBone Direction  

            Vector3 middleBoneDirection = Vector3.zero;
            var rootBonePosition = param.RootBone.position;
            var endBonePosition = param.EndBone.position;
            var middleBonePosition = param.MiddleBone.position;
            var sourcePosition = param.Source.position;
            var rootBoneRotation = param.RootBone.rotation;
            var middleBoneRotation = param.MiddleBone.rotation;
	        if (param.HintPosition != Vector3.zero) // if middleBoneGoal is null, the direction will be calculated with forearm's point
                middleBoneDirection = param.HintPosition - rootBonePosition;
            else
                middleBoneDirection = Vector3.Cross(endBonePosition -rootBonePosition, 
                    Vector3.Cross(endBonePosition -rootBonePosition, 
                        endBonePosition - middleBonePosition));

            // Get lengths of Arm
            float rootBoneLength = (middleBonePosition -rootBonePosition).magnitude;
            float middleBoneLength = (endBonePosition - middleBonePosition).magnitude;

            // Calculate the desired middleBone  position	
            Vector3 middleBonePos = GetHintPosition(rootBonePosition, sourcePosition, rootBoneLength, middleBoneLength, middleBoneDirection);

            // Rotate the bone transformations to align correctly
            Quaternion upperarmRotation = Quaternion.FromToRotation(middleBonePosition -rootBonePosition, 
                                              middleBonePos -rootBonePosition) * rootBoneRotation;
            if (!(System.Single.IsNaN(upperarmRotation.x) || System.Single.IsNaN(upperarmRotation.y) || System.Single.IsNaN(upperarmRotation.z)))
            {
                //Rotate with transition
                param.RootBone.rotation = Quaternion.Slerp(rootBoneRotation, upperarmRotation, param.Weight);
                Quaternion r = Quaternion.FromToRotation(endBonePosition - middleBonePosition, 
                                                    sourcePosition - middleBonePos) * middleBoneRotation;
                param.MiddleBone.rotation = Quaternion.Slerp(middleBoneRotation, r, param.Weight);

            }
            param.HintPosition = Vector3.zero;
        }

        /// <summary>
        /// Set IK Rotation
        /// </summary>
        /// <param name="rotation"></param>
        private void SetIKRotation(IKParam param)
        {           
            if (!(param.RootBone && param.MiddleBone && param.EndBone) || param.Weight <= 0.0f) return;
            param.EndBone.rotation = Quaternion.Slerp(param.EndBone.rotation, param.Source.rotation, param.Weight);
        }

        /// <summary>
        /// Set IK Hint Position
        /// ps: Call before SetIKPosition
        /// </summary>
        /// <param name="hintPosition"></param>
        private void SetIKHintPosition(IKParam param, Vector3 hintPosition)
        {
            param.HintPosition = hintPosition;
        }

        /// <summary>
        /// Get IK Hint Position
        /// </summary>
        /// <param name="rootPos"></param>
        /// <param name="endPos"></param>
        /// <param name="rootBoneLength"></param>
        /// <param name="middleBoneLength"></param>
        /// <param name="middleBoneDirection"></param>
        /// <returns></returns>
        protected virtual Vector3 GetHintPosition(Vector3 rootPos, Vector3 endPos, float rootBoneLength, float middleBoneLength, Vector3 middleBoneDirection)
        {
            Vector3 rootToEndDir = endPos - rootPos;
            float rootToEndMag = rootToEndDir.magnitude;

            float maxDist = (rootBoneLength + middleBoneLength) * 0.999f;
            if (rootToEndMag > maxDist)
            {
                endPos = rootPos + (rootToEndDir.normalized * maxDist);
                rootToEndDir = endPos - rootPos;
                rootToEndMag = maxDist;
            }

            float minDist = Mathf.Abs(rootBoneLength - middleBoneLength) * 1.001f;
            if (rootToEndMag < minDist)
            {
                endPos = rootPos + (rootToEndDir.normalized * minDist);
                rootToEndDir = endPos - rootPos;
                rootToEndMag = minDist;
            }

            float aa = ((rootToEndMag * rootToEndMag + rootBoneLength * rootBoneLength - middleBoneLength * middleBoneLength) * 0.5f) / rootToEndMag;
            float bb = Mathf.Sqrt(rootBoneLength * rootBoneLength - aa * aa);
            Vector3 crossElbow = Vector3.Cross(rootToEndDir, Vector3.Cross(middleBoneDirection, rootToEndDir));
            return rootPos + (aa * rootToEndDir.normalized) + (bb * crossElbow.normalized);
        }
    }

    class AvatarIKGoalComparer : IEqualityComparer<AvatarIKGoal>
    {
        public bool Equals(AvatarIKGoal x, AvatarIKGoal y)
        {
            return x == y;
        }

        public int GetHashCode(AvatarIKGoal obj)
        {
            return (int)obj;
        }

        private static AvatarIKGoalComparer _instance = new AvatarIKGoalComparer();
        public static AvatarIKGoalComparer Instance
        {
            get
            {
                return _instance;
            }
        }
    }
}
