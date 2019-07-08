using Core.CharacterBone;
using Core.CharacterState;
using Core.Fsm;
using Core.Utils;
using System;
using App.Shared.Components.Player;
using Core.Compare;
using UnityEngine;
using Utils.Appearance.Bone;
using Utils.CharacterState;

namespace App.Shared.GameModules.Player.CharacterBone
{
    public class WeaponRot : IWeaponRot
    {
        //private static LoggerAdapter Logger = new LoggerAdapter(typeof(WeaponRot));
        private const float ClavicleWeaponDistance = 0.8f;
        
        private GameObject _characterP3;
        private Transform _rightClavicleP3;
        
        private float _weaponPitchPercent;
        private float _currentWeaponPitchPercent;
        private float _weaponPitchChangeSpeed = 5;
        private bool _isWeaponRotState;

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
        }

        public void WeaponRotUpdate(CodeRigBoneParam param, float deltaTime)
        {
            CalcWeaponRotAngle(_rightClavicleP3);
            CalcCurrentWeaponRotAngle(deltaTime);

            _isWeaponRotState = _currentWeaponPitchPercent >= -1 && _currentWeaponPitchPercent < 0;
        }
        
        private void CalcCurrentWeaponRotAngle(float deltaTime)
        {
            var delta = deltaTime * _weaponPitchChangeSpeed;
            if (CompareUtility.IsApproximatelyEqual(_currentWeaponPitchPercent, _weaponPitchPercent, delta))
            {
                _currentWeaponPitchPercent = _weaponPitchPercent;
                return;
            }

            if (_currentWeaponPitchPercent > _weaponPitchPercent)
                _currentWeaponPitchPercent -= delta;
            else if (_currentWeaponPitchPercent < _weaponPitchPercent)
                _currentWeaponPitchPercent += delta;
        }

        private void CalcWeaponRotAngle(Transform rightClavicle)
        {
            if (FollowRotHelper.CanRotWeapon() && Physics.Raycast(rightClavicle.position, _characterP3.transform.forward,
                    ClavicleWeaponDistance, UnityLayers.SceneCollidableLayerMask))
                _weaponPitchPercent = -1;
            else
                _weaponPitchPercent = 0;
        }

        public void SetThirdPersonCharacter(GameObject obj)
        {
            _characterP3 = obj;
            _rightClavicleP3 = BoneMount.FindChildBoneFromCache(obj, BoneName.CharacterRightClavicleName);
        }

        public void SyncTo(CharacterBoneComponent value)
        {
            value.WeaponPitch = _currentWeaponPitchPercent;
            value.IsWeaponRotState = _isWeaponRotState;
        }
    }
}
