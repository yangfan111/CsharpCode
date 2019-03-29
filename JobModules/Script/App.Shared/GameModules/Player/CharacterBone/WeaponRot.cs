using Core.CharacterBone;
using Core.CharacterState;
using Core.Fsm;
using Core.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using App.Shared.Components.Player;
using UnityEngine;
using Utils.Appearance;
using Utils.CharacterState;

namespace App.Shared.GameModules.Player.CharacterBone
{
    public class WeaponRot : IWeaponRot
    {
        private GameObject _characterP3;
        private Transform _rightClavicleP3;
        private Transform _rightHandP3;
        
        private GameObject _characterP1;
        private Transform _rightClavicleP1;
        private Transform _rightHandP1;

        private float _weaponPitchPercent;
        private float _weaponRot;

        public void SetWeaponPitch(Action<FsmOutput> addOutput, float pitch)
        {
            FsmOutput.Cache.SetValue(AnimatorParametersHash.Instance.HandStateHash,
                                                 AnimatorParametersHash.Instance.HandStateName,
                                                 pitch,
                                                 CharacterView.ThirdPerson | CharacterView.FirstPerson);
            addOutput(FsmOutput.Cache);
        }
        
        public void WeaponRotPlayback(CodeRigBoneParam param)
        {
            RotateWeaponPlayback(param);
        }

        public void WeaponRotUpdate(CodeRigBoneParam param)
        {
            RotateWeapon(param);
        }
        
        // 旋转枪械，防穿模
        private void RotateWeapon(CodeRigBoneParam param)
        {
            Clear();
            if (!param.IsEmptyHand)
            {
                if (param.IsProne)
                {
                    RotateWeaponHelperByProne(param.MuzzleLocationP3, _rightHandP3);
                }
                else
                {
                    if (null != param.MuzzleLocationP3 && null != _rightHandP3)
                    {
                        // 计算经过手臂旋转后的枪口位置
                        Vector3 muzzleLocationP3 = RotateRound(param.MuzzleLocationP3, _rightClavicleP3.position, _characterP3.transform.right, param.HandPitch);
                        RotateWeaponHelper(muzzleLocationP3, _rightHandP3, param.HandPitch);
                    }
                
                    if (null != param.MuzzleLocationP1 && null != _rightHandP1)
                    {
                        // 计算经过手臂旋转后的枪口位置
                        Vector3 muzzleLocationP1 = RotateRound(param.MuzzleLocationP1, _rightClavicleP1.position, _characterP1.transform.right, param.HandPitch);
                        RotateWeaponHelper(muzzleLocationP1, _rightHandP1, param.HandPitch);
                    }
                }
            }
        }

        private void RotateWeaponHelper(Vector3 muzzleLocation, Transform rightHand, float handPitch)
        {
            RaycastHit hitInfo = new RaycastHit();
            if (Physics.Linecast(rightHand.position, muzzleLocation, out hitInfo, UnityLayers.SceneCollidableLayerMask))
            {
                var hitPoint = hitInfo.point;
                var distance = Vector3.Distance(muzzleLocation, hitPoint);
                // 计算修正后目标点
                var up = Vector3.Cross(hitInfo.transform.right, hitInfo.transform.up).normalized;
                up.Scale(new Vector3(distance, distance, distance));
                var target = hitPoint + up;
                // 计算两条向量
                var directionA = muzzleLocation - rightHand.position;
                directionA.Normalize();
                var directionB = target - rightHand.position;
                directionB.Normalize();
                // 计算旋转方向
                int directionIndex = handPitch > 0 ? 1 : -1;
                // 计算旋转角度
                var angle = directionIndex * Mathf.Acos(Vector3.Dot(directionA, directionB)) * 180.0f / Mathf.PI;
                // 旋转极限值
                if (angle + handPitch > 90)
                {
                    angle = 90 - handPitch;
                }
                else if (angle + handPitch < -90)
                {
                    angle = -90 - handPitch;
                }
                // 绕人物右方向上下旋转
                rightHand.rotation =
                Quaternion.AngleAxis(angle, _characterP3.transform.right) * rightHand.rotation;
                _weaponRot = angle;
            }
        }

        private void RotateWeaponPlayback(CodeRigBoneParam param)
        {
            if (!param.IsEmptyHand)
            {
                if (null != param.MuzzleLocationP3 && null != _rightHandP3)
                {
                    _rightHandP3.rotation =
                        Quaternion.AngleAxis(param.WeaponRot, _characterP3.transform.right) * _rightHandP3.rotation;
                }
            
                if (null != param.MuzzleLocationP1 && null != _rightHandP1)
                {
                    _rightHandP1.rotation =
                        Quaternion.AngleAxis(param.WeaponRot, _characterP3.transform.right) * _rightHandP1.rotation;
                }
            }
        }

        private void RotateWeaponHelperByProne(Vector3 muzzleLocation, Transform rightHand)
        {
            RaycastHit hitInfo = new RaycastHit();
            var handMuzzleDistance = Vector3.Distance(muzzleLocation, rightHand.position);
            if (Physics.Raycast(rightHand.position, _characterP3.transform.forward, out hitInfo,
                handMuzzleDistance, UnityLayers.SceneCollidableLayerMask))
            {
                _weaponPitchPercent = 1;
            }
        }

        private void Clear()
        {
            _weaponPitchPercent = 0;
            _weaponRot = 0;
        }

        public static Vector3 RotateRound(Vector3 position, Vector3 center, Vector3 axis, float angle)
        {
            Vector3 point = Quaternion.AngleAxis(angle, axis) * (position - center);
            Vector3 resultVec3 = center + point;
            return resultVec3;
        }

        public void SetThirdPersonCharacter(GameObject obj)
        {
            _characterP3 = obj;
            _rightClavicleP3 = BoneMount.FindChildBone(obj, BoneName.CharacterRightClavicleName);
            _rightHandP3 = BoneMount.FindChildBone(obj, BoneName.CharacterRightHandName);
        }
        
        public void SetFirstPersonCharacter(GameObject obj)
        {
            _characterP1 = obj;
            _rightClavicleP1 = BoneMount.FindChildBone(obj, BoneName.CharacterRightClavicleName);
            _rightHandP1 = BoneMount.FindChildBone(obj, BoneName.CharacterRightHandName);
        }

        public void SyncTo(CharacterBoneComponent value)
        {
            value.WeaponPitch = _weaponPitchPercent;
            value.WeaponRot = _weaponRot;
        }
    }
}
