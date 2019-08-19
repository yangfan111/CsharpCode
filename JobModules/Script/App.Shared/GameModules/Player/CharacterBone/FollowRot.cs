using System;
using UnityEngine;
using Utils.Configuration;
using Core.Compare;
using Core.CharacterBone;
using App.Shared.Components.Player;
using Core.EntityComponent;
using Core.Utils;
using Utils.Appearance.Bone;
using Utils.Singleton;
using XmlConfig;

namespace App.Shared.GameModules.Player.CharacterBone
{
    public class FollowRot : IFollowRot
    {
        //private static LoggerAdapter _logger = new LoggerAdapter(typeof(FollowRot));

        private GameObject _characterP3;

        private readonly float _horizontalHeadRotMax =
            SingletonManager.Get<CharacterStateConfigManager>().HorizontalHeadRotMax;

        private readonly float _horizontalHeadRotMin =
            SingletonManager.Get<CharacterStateConfigManager>().HorizontalHeadRotMin;

        private readonly float _verticalHeadRotMax =
            SingletonManager.Get<CharacterStateConfigManager>().VerticalHeadRotMax;

        private readonly float _verticalHeadRotMin =
            SingletonManager.Get<CharacterStateConfigManager>().VerticalHeadRotMin;

        private readonly float _neckRotHorizontalIndex =
            SingletonManager.Get<CharacterStateConfigManager>().NeckRotHorizontalIndex;

        private readonly float _neckRotVerticalIndex =
            SingletonManager.Get<CharacterStateConfigManager>().NeckRotVerticalIndex;

        private readonly float NoHeadRotStart = 
            SingletonManager.Get<CharacterStateConfigManager>().NoHeadRotStart;
        
        private readonly float FixedRotSpeed = 
            SingletonManager.Get<CharacterStateConfigManager>().HeadRotSpeed;
        
        private const float HandPitchChangeSpeed = 200.0f;
        private Transform _neckP3;
        private Transform _headP3;
        private Transform _rightClavicleP3;
        private Transform _spine;
        private Transform _spine1;

        private float _currentHandPitch;
        
        private readonly CustomProfileInfo _mainInfo;

        public FollowRot()
        {
            _mainInfo = SingletonManager.Get<DurationHelp>().GetCustomProfileInfo("FollowRot");
        }

        public void SetThirdPersonCharacter(GameObject obj)
        {
            _characterP3 = obj;
            GetP3Bones(obj);
        }
        
        public void PreUpdate(FollowRotParam param, ICharacterBone characterBone, float deltaTime)
        {
            //人物正向和载具正向相差180度
            var curAngle = param.CameraFreeYaw;
            var needReverse = FollowRotHelper.NeedReverse();

            if (needReverse)
            {
                curAngle = curAngle + (curAngle > 0 ? -180 : 180);
            }

            if (curAngle > NoHeadRotStart) curAngle = 0;
            if (curAngle < -NoHeadRotStart) curAngle = 0;

            if (FollowRotHelper.ForbidRot())
            {
                characterBone.ForbidRot = true;
            }
            else
            {
                characterBone.ForbidRot = false;
            }

            var aimAngle = characterBone.ForbidRot ? 0 : curAngle;
            characterBone.LastHeadRotAngle =
                CalcuStepRotAngle(aimAngle,
                    characterBone.LastHeadRotAngle, deltaTime);

            CalcCurrentHandPitch(FollowRotHelper.PitchHandAngle(), deltaTime);
        }

        private float CalcuStepRotAngle(float curAngle, float lastAngle,float deltaTime)
        {
            if (CompareUtility.IsApproximatelyEqual(lastAngle, curAngle))
                return curAngle;
            var speed = curAngle > lastAngle ? FixedRotSpeed : -FixedRotSpeed;
            var step = speed * (deltaTime * 1000);
            var result = lastAngle + step;
            if ((step + lastAngle) > curAngle && curAngle > lastAngle ||
                (step + lastAngle) < curAngle && curAngle < lastAngle)
                result = curAngle;
            return result;
        }
        
        public void Update(CodeRigBoneParam param)
        {
            try
            {
                _mainInfo.BeginProfileOnlyEnableProfile();
                FollowRotFunc(param.HeadPitch,
                    param.HeadYaw,
                    param.CurrentHandPitch,
                    param.IsServer);
            }
            finally
            {
                _mainInfo.EndProfileOnlyEnableProfile();
            }
        }

        private void CalcCurrentHandPitch(float handPitch, float deltaTime)
        {
            var delta = deltaTime * HandPitchChangeSpeed;
            if (CompareUtility.IsApproximatelyEqual(_currentHandPitch, handPitch, delta))
            {
                _currentHandPitch = handPitch;
                return;
            }

            if (_currentHandPitch > handPitch)
                _currentHandPitch -= delta;
            else if (_currentHandPitch < handPitch)
                _currentHandPitch += delta;
        }

        //头手随动
        private void FollowRotFunc(float headPitch,
            float headYaw,
            float handPitch,
            bool isServer)
        {
            if (!ThirdPersonIncluded || isServer) return;
            HandFollow(handPitch);
            HeadFollowPitch(headPitch);
            HeadFollowYaw(headYaw);
        }

        private void HeadFollowPitch(float headPitch)
        {
            if (CompareUtility.IsApproximatelyEqual(headPitch, 0f))
            {
                return;
            }

            var pitchHead = (headPitch > _verticalHeadRotMax) ? _verticalHeadRotMax : headPitch;
            pitchHead = (pitchHead < _verticalHeadRotMin) ? _verticalHeadRotMin : pitchHead;

            _neckP3.rotation =
                Quaternion.AngleAxis(pitchHead * _neckRotHorizontalIndex, _characterP3.transform.right) *
                _neckP3.rotation;
            _headP3.rotation =
                Quaternion.AngleAxis(pitchHead * (1 - _neckRotHorizontalIndex), _characterP3.transform.right) *
                _headP3.rotation;
        }

        private void HeadFollowYaw(float headYaw)
        {
            if (CompareUtility.IsApproximatelyEqual(headYaw, 0f))
            {
                return;
            }

            var yawHead = (headYaw > _horizontalHeadRotMax) ? _horizontalHeadRotMax : headYaw;
            yawHead = (yawHead < _horizontalHeadRotMin) ? _horizontalHeadRotMin : yawHead;

            _neckP3.rotation =
                Quaternion.AngleAxis(yawHead * _neckRotVerticalIndex, _characterP3.transform.up) * _neckP3.rotation;
            _headP3.rotation =
                Quaternion.AngleAxis(yawHead * (1 - _neckRotVerticalIndex), _characterP3.transform.up) *
                _headP3.rotation;
        }

        private void HandFollow(float handPitch)
        {
            if (CompareUtility.IsApproximatelyEqual(handPitch, 0f))
                return;

            var spineRotateAngle = GetCurrentRotateAngle(handPitch, 0, FollowRotHelper.SpineRotateMax);
            var spine1RotateAngle = GetCurrentRotateAngle(handPitch, FollowRotHelper.SpineRotateMax, FollowRotHelper.Spine1RotateMax);
            var clavicleRotateAngle = GetCurrentRotateAngle(handPitch, FollowRotHelper.SpineRotateMax + FollowRotHelper.Spine1RotateMax, float.MaxValue);

            _spine.rotation =
                Quaternion.AngleAxis(spineRotateAngle, _characterP3.transform.right) *
                _spine.rotation;
            _spine1.rotation =
                Quaternion.AngleAxis(spine1RotateAngle, _characterP3.transform.right) *
                _spine1.rotation;
            _rightClavicleP3.rotation =
                Quaternion.AngleAxis(clavicleRotateAngle, _characterP3.transform.right) * _rightClavicleP3.rotation;
        }

        private static float GetCurrentRotateAngle(float handPitch, float beforeLimitPitch, float currentLimitPitch)
        {
            var direction = handPitch / Mathf.Abs(handPitch);

            handPitch -= beforeLimitPitch * direction;
            if (handPitch * direction <= 0) return 0;
            
            if (Mathf.Abs(handPitch) > currentLimitPitch)
                return currentLimitPitch * direction;
            
            return handPitch;
        }

        private bool ThirdPersonIncluded
        {
            get { return _characterP3 != null; }
        }

        private void GetP3Bones(GameObject obj)
        {
            if (null == obj) return;
            _neckP3 = BoneMount.FindChildBone(obj, BoneName.CharacterNeckBoneName);
            _headP3 = BoneMount.FindChildBone(obj, BoneName.CharacterHeadBoneName);
            _rightClavicleP3 = BoneMount.FindChildBone(obj, BoneName.CharacterRightClavicleName);

            _spine = BoneMount.FindChildBoneFromCache(obj, BoneName.CharacterSpineName);
            _spine1 = BoneMount.FindChildBoneFromCache(obj, BoneName.CharacterSpine1Name);
        }

        public void SyncTo(IGameComponent characterBoneComponent)
        {
            var value = characterBoneComponent as CharacterBoneComponent;
            if (null == value) return;
            value.PitchHeadAngle = FollowRotHelper.PitchHeadAngle();
            value.RotHeadAngle = FollowRotHelper.YawHeadAngle();
            value.CurrentPitchHandAngle = _currentHandPitch;
        }
    }
}
