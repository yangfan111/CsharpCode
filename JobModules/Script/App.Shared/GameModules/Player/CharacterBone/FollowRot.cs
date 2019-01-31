using Core.CameraControl.NewMotor;
using UnityEngine;
using Utils.Appearance;
using Utils.Configuration;
using Core.Compare;
using App.Shared.GameModules.Player.CharacterBone;
using Core.CharacterBone;
using App.Shared.Components.Player;
using Core.EntityComponent;
using Core.Utils;
using Utils.Singleton;
using XmlConfig;

namespace App.Shared.GameModules.Player.CharacterBone
{
    public class FollowRot : IFollowRot
    {
        private static LoggerAdapter _logger = new LoggerAdapter(typeof(FollowRot));
        
        private GameObject _characterP3;

        private readonly float _horizontalHeadRotMax = SingletonManager.Get<CharacterStateConfigManager>().HorizontalHeadRotMax;
        private readonly float _horizontalHeadRotMin = SingletonManager.Get<CharacterStateConfigManager>().HorizontalHeadRotMin;
        private readonly float _verticalHeadRotMax = SingletonManager.Get<CharacterStateConfigManager>().VerticalHeadRotMax;
        private readonly float _verticalHeadRotMin = SingletonManager.Get<CharacterStateConfigManager>().VerticalHeadRotMin;
        private readonly float _neckRotHorizontalIndex = SingletonManager.Get<CharacterStateConfigManager>().NeckRotHorizontalIndex;
        private readonly float _neckRotVerticalIndex = SingletonManager.Get<CharacterStateConfigManager>().NeckRotVerticalIndex;
        private readonly float _headRotReversalTime = SingletonManager.Get<CharacterStateConfigManager>().HeadRotReversalTime;

        private const float MaxHandPitchDown = 30.0f;

        private Transform _baseLocatorP3;
        private Transform _neckP3;
        private Transform _headP3;
        private Transform _rightClavicleP3;
        private Transform _rightHandP3;
        private Transform _leftHandP3;
        private Transform _leftClavicleP3;

        public FollowRot()
        {
        }

        public void SetThirdPersonCharacter(GameObject obj)
        {
            _characterP3 = obj;
            GetP3Bones(obj);
        }

        public void PreUpdate(FollowRotParam param, ICharacterBone characterBone)
        {
            //人物正向和载具正向相差180度
            var curAngle = param.CameraFreeYaw;
            var needReverse = FollowRotHelper.NeedReverse();

            if (needReverse)
            {
                curAngle = curAngle + (curAngle > 0 ? -180 : 180);
            }

            if (curAngle * characterBone.LastHeadRotAngle < _horizontalHeadRotMax * _horizontalHeadRotMin
            ) //头部水平旋转角穿过+-180度（暂定最大旋转角度为+-60）
            {
                characterBone.LastHeadRotSlerpTime = param.ClientTime;
                characterBone.IsHeadRotCW = curAngle > 0;
            }

            if (FollowRotHelper.ForbidRot())
            {
                characterBone.ForbidRot = true;
            }
            else if (param.CameraFreeNowMode != (byte) ECameraFreeMode.On
                     && param.CameraEulerAngle == Vector3.zero)
            {
                characterBone.ForbidRot = false;
            }

            if (characterBone.ForbidRot)
            {
                characterBone.LastHeadRotAngle = 0;
            }
            else
            {
                characterBone.LastHeadRotAngle = curAngle;
            }
        }

        public void Update(CodeRigBoneParam param)
        {
            FollowRotFunc(param.HeadPitch,
                param.HeadYaw,
                param.HandPitch,
                param.HeadRotProcess,
                param.IsHeadRotCW,
                param.IKActive,
                param.IsServer);
        }

        //头手随动
        private void FollowRotFunc(float headPitch,
            float headYaw,
            float handPitch,
            float headRotProcess,
            bool isHeadRotCw,
            bool iKActive,
            bool isServer)
        {
            if (!ThirdPersonIncluded || isServer) return;
            HandFollow(handPitch, iKActive);
            HeadFollowPitch(headPitch);
            headYaw = SlerpHeadRot(headRotProcess, headYaw, isHeadRotCw);
            HeadFollowYaw(headYaw);
        }

        //处理头部随动时，角度在+-180度切换
        private float SlerpHeadRot(float headRotProcess, float curAngleValue, bool isHeadRotCW)
        {
            if (headRotProcess > _headRotReversalTime)
            {
                return curAngleValue;
            }

            if (curAngleValue <= _horizontalHeadRotMax && curAngleValue >= _horizontalHeadRotMin)
            {
                return curAngleValue;
            }

            var result = _horizontalHeadRotMin +
                         (_horizontalHeadRotMax - _horizontalHeadRotMin) * headRotProcess / _headRotReversalTime;
            return isHeadRotCW ? result : -result;
        }

        private void HeadFollowPitch(float headPitch)
        {
            if (CompareUtility.IsApproximatelyEqual(headPitch, 0f))
            {
                return;
            }

            float pitchHead = (headPitch > _verticalHeadRotMax) ? _verticalHeadRotMax : headPitch;
            pitchHead = (pitchHead < _verticalHeadRotMin) ? _verticalHeadRotMin : pitchHead;

            _neckP3.rotation = 
                Quaternion.AngleAxis(pitchHead * _neckRotHorizontalIndex, _characterP3.transform.right) * _neckP3.rotation;
            _headP3.rotation = 
                Quaternion.AngleAxis(pitchHead * (1 - _neckRotHorizontalIndex), _characterP3.transform.right) * _headP3.rotation;
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
                Quaternion.AngleAxis(yawHead * (1 - _neckRotVerticalIndex), _characterP3.transform.up) * _headP3.rotation;
        }

        private void HandFollow(float handPitch, bool iKActive)
        {
            if (CompareUtility.IsApproximatelyEqual(handPitch, 0f))
            {
                return;
            }

            var realClaviclePitch = handPitch;
            var realHandPitch = 0f;
            if (handPitch > MaxHandPitchDown)
            {
                realClaviclePitch = MaxHandPitchDown;
                realHandPitch = handPitch - realClaviclePitch;
            }

            _rightClavicleP3.rotation = 
                Quaternion.AngleAxis(realClaviclePitch, _characterP3.transform.right) * _rightClavicleP3.rotation;
            if (realHandPitch > 0)
            {
                _rightHandP3.rotation =
                    Quaternion.AngleAxis(realHandPitch, _characterP3.transform.right) * _rightHandP3.rotation;
            }

            //baseWeaponLocator跟随右肩旋转
            _baseLocatorP3.transform.RotateAround(_rightClavicleP3.transform.position, _characterP3.transform.right,
                handPitch);
            
            if(iKActive) return;
            
            var leftHandToLeftClav =                                              //手到肩的距离
                Vector3.Dot(_leftHandP3.position - _leftClavicleP3.position, _characterP3.transform.forward);
            var leftHandToRightClav =
                Vector3.Dot(_leftHandP3.position - _rightClavicleP3.position, _characterP3.transform.forward);

            //左肩在右肩之前，旋转角不一致。 左手到左肩距离表示为LL，左手到右肩距离为LR，右肩旋转角theta
            //左肩旋转角 A = arctan(   LR*sin(theta) / (LL-(1-LR*cos(theta)))   )
            //左臂旋转后会有轻微穿模，应向前伸展到LR*sin(theta)/sinA长度，暂忽略
            var leftClavAngle = Mathf.Rad2Deg * Mathf.Atan((leftHandToRightClav * Mathf.Sin(realClaviclePitch * Mathf.Deg2Rad)) /
                                   (leftHandToLeftClav -
                                    (1 - Mathf.Cos(realClaviclePitch * Mathf.Deg2Rad)) * leftHandToRightClav));

            _leftClavicleP3.rotation = Quaternion.AngleAxis(leftClavAngle, _characterP3.transform.right) *
                                       _leftClavicleP3.rotation;
        }

        private bool ThirdPersonIncluded
        {
            get { return _characterP3 != null; }
        }

        private void GetP3Bones(GameObject obj)
        {
            if (null == obj) return;
            _baseLocatorP3 = BoneMount.FindChildBone(obj, BoneName.AlternativeWeaponLocator);
            _neckP3 = BoneMount.FindChildBone(obj, BoneName.CharacterNeckBoneName);
            _headP3 = BoneMount.FindChildBone(obj, BoneName.CharacterHeadBoneName);
            _rightClavicleP3 = BoneMount.FindChildBone(obj, BoneName.CharacterRightClavicleName);
            _rightHandP3 = BoneMount.FindChildBone(obj, BoneName.CharacterRightHandName);
            _leftHandP3 = BoneMount.FindChildBone(obj, BoneName.CharLeftHand);
            _leftClavicleP3 = BoneMount.FindChildBone(obj, BoneName.CharacterLeftClavicleName);
        }

        public void SyncTo(IGameComponent characterBoneComponent)
        {
            var value = characterBoneComponent as CharacterBoneComponent;
            if(null == value) return;
            value.PitchHeadAngle = FollowRotHelper.PitchHeadAngle();
            value.RotHeadAngle = FollowRotHelper.YawHeadAngle();
            value.HeadRotProcess = FollowRotHelper.HeadRotProcess();
            value.PitchHandAngle = FollowRotHelper.PitchHandAngle();
            value.IsHeadRotCW = FollowRotHelper.IsHeadRotCw();
        }
    }
}