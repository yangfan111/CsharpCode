using Core.CharacterBone;
using Core.CharacterState;
using Core.Fsm;
using Core.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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

        private float _weaponPitchPercent;

        public void SetWeaponPitch(Action<FsmOutput> addOutput, float pitch)
        {
            FsmOutput.Cache.SetValue(AnimatorParametersHash.Instance.HandStateHash,
                                                 AnimatorParametersHash.Instance.HandStateName,
                                                 pitch,
                                                 CharacterView.ThirdPerson | CharacterView.FirstPerson);
            addOutput(FsmOutput.Cache);
        }

        public void WeaponRotUpdate(CodeRigBoneParam param)
        {
            RotateWeapon(param);
        }

        // 旋转枪械，防穿模
        private void RotateWeapon(CodeRigBoneParam param)
        {
            if (!param.IsEmptyHand)
            {
                if (null != param.MuzzleLocationP3 && null != _rightHandP3)
                {
                    // 计算经过手臂旋转后的枪口位置
                    Vector3 muzzleP3 = RotateRound(param.MuzzleLocationP3, _rightClavicleP3.position, _characterP3.transform.right, param.HandPitch);
                    RotateWeaponHelper(muzzleP3, _rightHandP3, param.HandPitch, param.WeaponPitch);
                    return;
                }
                _weaponPitchPercent = 0;
            }
            _weaponPitchPercent = 0;
        }

        private void RotateWeaponHelper(Vector3 muzzleLocation, Transform rightHand, float handPitch, float weaponPitchPercent)
        {
            RaycastHit hitInfo = new RaycastHit();
            var handMuzzleDistance = Vector3.Distance(muzzleLocation, _rightClavicleP3.position);
            if (Physics.Raycast(_rightClavicleP3.position, _characterP3.transform.forward, out hitInfo, handMuzzleDistance, UnityLayers.SceneCollidableLayerMask))
            {
                if (Physics.Linecast(rightHand.position, muzzleLocation, out hitInfo))
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
                    //// 旋转极限值
                    //if (angle + handPitch > 85)
                    //{
                    //    angle = 85 - handPitch;
                    //}
                    //else if (angle + handPitch < -85)
                    //{
                    //    angle = -85 - handPitch;
                    //}

                    _weaponPitchPercent = weaponPitchPercent - (angle / 90.0f);
                    if (_weaponPitchPercent > 1) _weaponPitchPercent = 1;
                    else if (_weaponPitchPercent < -1) _weaponPitchPercent = -1;
                }
                else
                {
                    _weaponPitchPercent = weaponPitchPercent;
                }

                // 旋转极限值
                //var percent = handPitch / 90.0f;
                //if (_weaponPitchPercent - percent > 1)
                //{
                //    _weaponPitchPercent = 1 + percent * 0.8f;
                //}
                //else if (_weaponPitchPercent - percent < -1f)
                //{
                //    _weaponPitchPercent = -1f + percent * 0.8f;
                //}
            }
            else
            {
                _weaponPitchPercent = 0;
            }
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
            _rightClavicleP3 = BoneMount.FindChildBoneFromCache(obj, BoneName.CharacterRightClavicleName);
            _rightHandP3 = BoneMount.FindChildBoneFromCache(obj, BoneName.CharacterRightHandName);
        }

        public void SyncTo(ICharacterBoneState state)
        {
            state.WeaponPitch = _weaponPitchPercent;
        }
    }
}
