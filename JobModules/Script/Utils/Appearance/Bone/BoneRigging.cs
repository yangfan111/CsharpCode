using System;
using System.Collections.Generic;
using Core.Utils;
using UnityEngine;
using Utils.Appearance.Script;
using Utils.Compare;
using Utils.Configuration;
using Utils.Singleton;

namespace Utils.Appearance.Bone
{
    public struct CodeRigBoneParam
    {
        public float PeekAmplitude;
        public float PitchAmplitude;
        public float OverlayAnimationWeight;

        public ThirdPersonPosture PostureWhenOverlay;

        // there is no need to replay sight in first person
        public bool IsSight;
        public bool IsIntoSight;
        public float SightOffset;

        public float SightProgress;

        // 肩射呼吸动作
        public float SightHorizontalShift;

        public float SightVerticalShift;

        // 第一人称快速移动的位移
        public bool IsFirstPerson;
        public float FastMoveHorizontalShift;
        public float FastMoveVerticalShift;
        public float SightMoveHorizontalShift;

        public float SightMoveVerticalShift;

        //
        public bool IKActive;
        public bool IsFire;

        public float HeadPitch;
        public float HeadYaw;
        public float CurrentHandPitch;

        public float SightShiftBuff;
        public Vector3 FirstPersonSightOffset;
        public Vector3 FirstPersonPositionOffset;
        public Vector3 FirstPersonRotationOffset;
        public float ScopeOffset;
        public float ScopeScale;

        public float SightModelOffset;

        public bool IsServer;
        public bool IsEmptyHand;
        public Vector3 MuzzleLocationP3;
        public Vector3 MuzzleLocationP1;
        public float WeaponRot;
    }

    public struct SightStatusParam
    {
        public bool IsFire;
        public bool IsSight;
        public bool IsIntoSight;
        public float PeekAmplitude;
        public float SightOffset;
        public float PitchAmplitude;
        public float SightProgress;
        public float SightHorizontalShift;
        public float SightVerticalShift;
        public Vector3 FirstPersonSightOffset;
        public float ScopeOffset;
        public float SightModelOffset;
    }

    public struct FirstPersonOffsetParam
    {
        public bool IsFirstPerson;
        public bool IsSight;
        public float HorizontalShift;
        public float VerticalShift;
        public Vector3 FirstPersonPositionOffset;
        public Vector3 FirstPersonRotationOffset;
    }

    public class BoneRigging
    {
        private const float SpinePeekDegree = 10;
        private const float Spine1PeekDegree = 20;
        private const float PitchMaxDegree = 30;
        private const float PitchMinDegree = -30;
        private static LoggerAdapter _logger = new LoggerAdapter(typeof(BoneRigging));

        private readonly CustomProfileInfo _mainInfo;
        private Transform _baseLocatorP1;
        private Transform _cameraLocatorP1;

        private GameObject _characterP1;
        private GameObject _characterP3;

        // baseLocator用于旋转和对齐
        private Transform _directionLocatorP1;

        private PlayerIK _ikControllerP1;
        private PlayerIK _ikControllerP3;
        private Transform _leftHandP1;

        private Matrix4x4 _matrix;
        private Transform _pelvisP3;
        private Quaternion _pelvisUntouchedCrouchRotationP3 = Quaternion.identity;

        private Quaternion _pelvisUntouchedStandRotationP3 = Quaternion.identity;
        private Transform _rightHandP1;
        private Transform _sightLocationP1;
        private Transform _spine1P3;

        private Transform _spineP3;
        private bool _updateBone = true;

        private Transform _viewPointP1;

        public Action<bool> EnableSightMove;

        private List<ThirdPersonPosture> ExcludePostures = new List<ThirdPersonPosture>
        {
            ThirdPersonPosture.Prone
        };

        public BoneRigging()
        {
            _mainInfo = SingletonManager.Get<DurationHelp>().GetCustomProfileInfo("BoneRigging");
        }

        public Transform BaseLocatorP1
        {
            get { return _baseLocatorP1; }
        }

        private bool FirstPersonIncluded
        {
            get { return _characterP1 != null; }
        }

        private bool ThirdPersonIncluded
        {
            get { return _characterP3 != null; }
        }

        public void SetFirstPersonCharacter(GameObject obj)
        {
            _characterP1 = obj;
            GetP1Bones(obj);
        }

        public void SetThirdPersonCharacter(GameObject obj)
        {
            _characterP3 = obj;
            GetP3Bones(obj);
        }

        public void SetStableStandPelvisRotation()
        {
            if (null == _pelvisP3) return;
            _pelvisUntouchedStandRotationP3 = _pelvisP3.localRotation;
            _logger.InfoFormat("CharacterLog-- pelvisUntouchedStandRotationP3:   {0}",
                _pelvisUntouchedStandRotationP3.ToStringExt());
        }

        public void SetStableCrouchPelvisRotation()
        {
            if (null == _pelvisP3) return;
            _pelvisUntouchedCrouchRotationP3 = _pelvisP3.localRotation;
            _logger.InfoFormat("CharacterLog-- pelvisUntouchedCrouchRotationP3:   {0}",
                _pelvisUntouchedCrouchRotationP3.ToStringExt());
        }

        public void SetP1IkTarget(GameObject obj)
        {
            if (!FirstPersonIncluded) return;
            Transform leftIkP1 = null;
            if (obj != null)
            {
                leftIkP1 = BoneMount.FindChildBoneFromCache(obj, BoneName.AttachmentLeftIKP1);
                if (leftIkP1 == null)
                {
                    leftIkP1 = BoneMount.FindChildBoneFromCache(obj, BoneName.WeaponLeftIK);
                }
            }

            _ikControllerP1.ClearAllIKTarget();
            _ikControllerP1.SetIKGoal(AvatarIKGoal.LeftHand);
            _ikControllerP1.SetSource(AvatarIKGoal.LeftHand, leftIkP1);
        }

        public bool SetP3IkTarget(GameObject obj, ref bool weaponHasIk)
        {
            var ret = false;

            if (!ThirdPersonIncluded) return false;
            Transform leftIKP3 = null;
            if (obj != null)
            {
                leftIKP3 = BoneMount.FindChildBoneFromCache(obj, BoneName.AttachmentLeftIKP3);
                ret = null != leftIKP3;

                if (leftIKP3 == null)
                {
                    leftIKP3 = BoneMount.FindChildBoneFromCache(obj, BoneName.WeaponLeftIK);
                }

                weaponHasIk = null != leftIKP3;
            }

            _ikControllerP3.ClearAllIKTarget();
            _ikControllerP3.SetIKGoal(AvatarIKGoal.LeftHand);
            _ikControllerP3.SetSource(AvatarIKGoal.LeftHand, leftIKP3);

            return ret;
        }

        public bool SetIKTarget(GameObject objP1, GameObject objP3, ref bool weaponHasIk)
        {
            bool ret = false;

            if (FirstPersonIncluded)
            {
                Transform leftIKP1 = null;
                if (objP1 != null)
                {
                    leftIKP1 = BoneMount.FindChildBoneFromCache(objP1, BoneName.AttachmentLeftIKP1);
                    if (leftIKP1 == null)
                    {
                        leftIKP1 = BoneMount.FindChildBoneFromCache(objP1, BoneName.WeaponLeftIK);
                    }
                }

                _ikControllerP1.ClearAllIKTarget();
                _ikControllerP1.SetIKGoal(AvatarIKGoal.LeftHand);
                _ikControllerP1.SetSource(AvatarIKGoal.LeftHand, leftIKP1);
            }

            if (ThirdPersonIncluded)
            {
                Transform leftIKP3 = null;
                if (objP3 != null)
                {
                    leftIKP3 = BoneMount.FindChildBoneFromCache(objP3, BoneName.AttachmentLeftIKP3);
                    ret = null != leftIKP3;

                    if (leftIKP3 == null)
                    {
                        leftIKP3 = BoneMount.FindChildBoneFromCache(objP3, BoneName.WeaponLeftIK);
                    }

                    weaponHasIk = null != leftIKP3;
                }

                _ikControllerP3.ClearAllIKTarget();
                _ikControllerP3.SetIKGoal(AvatarIKGoal.LeftHand);
                _ikControllerP3.SetSource(AvatarIKGoal.LeftHand, leftIKP3);
            }

            return ret;
        }

        // left -> peek < 0
        private void Peek(float peek, bool isSight)
        {
            if (CompareUtility.IsApproximatelyEqual(peek, 0)) return;
            if (FirstPersonIncluded) PeekP1(peek, isSight);
            if (ThirdPersonIncluded) PeekP3(peek);
        }

        private void PitchP1(float pitch)
        {
            if (FirstPersonIncluded && !CompareUtility.IsApproximatelyEqual(pitch, 0))
                _viewPointP1.localRotation = Quaternion.AngleAxis(pitch, Vector3.right) * _viewPointP1.localRotation;
        }

        private void PeekP1(float peek, bool isSight)
        {
            if (!isSight)
            {
                var r = ConvertRotation(_baseLocatorP1.transform, _baseLocatorP1,
                    Quaternion.AngleAxis(CharacterStateConfigManager.Instance.PeekDegreeP1 * peek, Vector3.back));
                _baseLocatorP1.rotation = _baseLocatorP1.rotation * r;
            }

            _viewPointP1.localPosition = ConvertPosition(_characterP1.transform, _viewPointP1,
                new Vector3(CharacterStateConfigManager.Instance.PeekXTransition * peek,
                    CharacterStateConfigManager.Instance.PeekYTransition * Mathf.Abs(peek), 0));
        }

        private void PeekP3(float peek)
        {
            peek = Mathf.Clamp(peek, PitchMinDegree, PitchMaxDegree);

            var r = ConvertRotation(_characterP3.transform, _spineP3,
                Quaternion.AngleAxis(SpinePeekDegree * peek, Vector3.back));
            _spineP3.rotation = _spineP3.rotation * r;

            r = ConvertRotation(_characterP3.transform, _spine1P3,
                Quaternion.AngleAxis(Spine1PeekDegree * peek, Vector3.back));
            _spine1P3.rotation = _spine1P3.rotation * r;
        }

        private void StableUpperBody(float upperBodyOverlayWeight, ThirdPersonPosture type)
        {
            if (upperBodyOverlayWeight > 0 && !ExcludePostures.Contains(type))
            {
                Quaternion untouchedRotation = GetUntouchedRotation(type);
                var rotation = Quaternion.Inverse(_pelvisP3.localRotation);
                // spineP3父亲的旋转为 newPelvis = （1 - t) * (当前pelvis旋转) + t * (默认idle的pelvis旋转),t为上半身层的权重
                rotation = rotation * SlerpRotation(_pelvisP3.localRotation, untouchedRotation,
                               upperBodyOverlayWeight);
                _spineP3.localRotation = rotation * _spineP3.localRotation;
            }
        }

        private Quaternion GetUntouchedRotation(ThirdPersonPosture type)
        {
            Quaternion ret;
            switch (type)
            {
                case ThirdPersonPosture.Crouch:
                    ret = _pelvisUntouchedCrouchRotationP3;
                    break;
                case ThirdPersonPosture.Stand:
                    ret = _pelvisUntouchedStandRotationP3;
                    break;
                default:
                    ret = _pelvisUntouchedStandRotationP3;
                    break;
            }

            return ret;
        }

        /// <summary>
        ///     旋转插值
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <param name="t"></param>
        /// <returns></returns>
        private Quaternion SlerpRotation(Quaternion from, Quaternion to, float t)
        {
            Quaternion ret = Quaternion.Slerp(from, to, t);
            return ret;
        }

        // 腰射与肩射的切换，与肩射呼吸动作
        private void SetSightStatus(SightStatusParam param)
        {
            if (param.IsSight)
            {
                if (_sightLocationP1 == null)
                {
                    _sightLocationP1 = BoneMount.FindChildBoneFromCache(_characterP1, BoneName.AttachmentSight);
                    if (_sightLocationP1 == null)
                        _sightLocationP1 = BoneMount.FindChildBoneFromCache(_characterP1, BoneName.WeaponSight);
                    if (_sightLocationP1 == null)
                    {
                        _logger.ErrorFormat("characterP1:{0} contains no Bone:{1} or {2}", _characterP1.name,
                            BoneName.AttachmentSight, BoneName.WeaponSight);
                    }
                }

                var tempPos = _sightLocationP1.localPosition;
                var tempRot = _sightLocationP1.localRotation;
                var tempWorldPos = _sightLocationP1.position;

                // 记录viewPoint与sightLocator的相对位移
                if (_updateBone)
                    _matrix = _viewPointP1.worldToLocalMatrix * _sightLocationP1.localToWorldMatrix;

                SetSightMove(param);

                _updateBone = param.SightProgress < 1;
                var finalMatrix = _viewPointP1.localToWorldMatrix * _matrix;
                _sightLocationP1.position = finalMatrix.ExtractPosition();
                _sightLocationP1.rotation = finalMatrix.ExtractRotation();

                var dir = tempWorldPos - _sightLocationP1.position;

                //camera local position
                var alignedPosition = _cameraLocatorP1.localPosition;
                alignedPosition.z += param.SightOffset;

                if (CompareUtility.IsApproximatelyEqual(param.SightProgress, 1))
                {
                    alignedPosition.x += param.SightHorizontalShift;
                    alignedPosition.y += param.SightVerticalShift;
                }

                // 一人称开镜状态下，人物偏移(枪，分辨率相关)
                alignedPosition += param.FirstPersonSightOffset;
                // 镜与人眼距离(瞄准镜相关)
                alignedPosition.z += param.ScopeOffset;

                var alignedRotation =
                    Quaternion.AngleAxis( /*param.PitchAmplitude*/ -param.SightModelOffset, Vector3.right) *
                    _cameraLocatorP1.localRotation;
                alignedPosition = alignedRotation * alignedPosition;

                var alignedMatrix = _cameraLocatorP1.parent.localToWorldMatrix *
                                    Matrix4x4.TRS(alignedPosition, alignedRotation, Vector3.one);

                AlignCoordinateSystems(_sightLocationP1,
                    Vector3.Lerp(_sightLocationP1.position, alignedMatrix.ExtractPosition(),
                        Mathf.Clamp01(param.SightProgress)),
                    Quaternion.Slerp(_sightLocationP1.rotation, alignedMatrix.ExtractRotation(),
                        Mathf.Clamp01(param.SightProgress)), _directionLocatorP1);

                AlignLocatorRotation(param, dir, alignedMatrix);
                
                // 目标：sightlocater的向前方向旋转SightPeekDegree的角度,实现瞄准QE
                // 做法，把baseLocator先转换到sightLocation关节空间，对sightLocation的局部坐标系沿着自身向前旋转SightPeekDegree的角度，转换到世界坐标系，最后把结果转换到baseLocator父关节的坐标系
                var povitMatrix = _sightLocationP1.parent.localToWorldMatrix * Matrix4x4.TRS(
                                      _sightLocationP1.localPosition,
                                      Quaternion.AngleAxis(
                                          CharacterStateConfigManager.Instance.SightPeekDegree * param.PeekAmplitude,
                                          -_sightLocationP1.localRotation.Forward()) * _sightLocationP1.localRotation,
                                      Vector3.one);

                AlignCoordinateSystems(_sightLocationP1, povitMatrix.ExtractPosition(), povitMatrix.ExtractRotation(),
                    _directionLocatorP1);



                _sightLocationP1.localPosition = tempPos;
                _sightLocationP1.localRotation = tempRot;
            }
            else
            {
                _sightLocationP1 = null;
            }
        }

        private void SetSightMove(SightStatusParam param)
        {
            if (null == EnableSightMove) return;
            EnableSightMove.Invoke(!param.IsIntoSight && param.SightProgress >= 1);
        }
        
        private void AlignLocatorRotation(SightStatusParam param, Vector3 dir, Matrix4x4 alignedMatrix)
        {
            var dir1 = _sightLocationP1.position - _cameraLocatorP1.position;
            var dir2 = Vector3.Normalize(dir1 + dir);
            dir1 = dir1.normalized;
            var axis = Vector3.Cross(dir1, dir2).normalized;

            if (CompareUtility.IsApproximatelyEqual(axis, Vector3.zero) ||
                CompareUtility.IsApproximatelyEqual(Vector3.Dot(dir1, dir2), 1, 0)) return;

            var angle = Mathf.Acos(Vector3.Dot(dir1, dir2)) * Mathf.Rad2Deg;
            if (float.IsNaN(angle)) return;
            
            if (param.IsFire || _updateBone) return;
            _sightLocationP1.Rotate(axis, -angle);
            AlignCoordinateSystems(_sightLocationP1,
                Vector3.Lerp(_sightLocationP1.position, alignedMatrix.ExtractPosition(),
                    Mathf.Clamp01(param.SightProgress)),
                Quaternion.Slerp(_sightLocationP1.rotation, alignedMatrix.ExtractRotation(),
                    Mathf.Clamp01(param.SightProgress)), _directionLocatorP1);
        }

        private string GetString(Matrix4x4 matrix4X4)
        {
            return string.Format("pos:{0}, rot:{1}", matrix4X4.ExtractPosition().ToStringExt(),
                matrix4X4.ExtractRotation().eulerAngles.ToStringExt());
        }

        // 第一人称腰射时手/枪的随动
        private void SetFirstPersonShift(FirstPersonOffsetParam param)
        {
            if (!FirstPersonIncluded || !param.IsFirstPerson || param.IsSight) return;

            if (!CompareUtility.IsApproximatelyEqual(param.HorizontalShift, 0) ||
                !CompareUtility.IsApproximatelyEqual(param.VerticalShift, 0))
            {
                if (!CompareUtility.IsApproximatelyEqual(param.HorizontalShift, 0))
                {
                    var modifiedRotation =
                        Quaternion.AngleAxis(param.HorizontalShift,
                            _cameraLocatorP1.TransformDirection(Vector3.up)) * _leftHandP1.rotation;
                    AlignCoordinateSystems(_leftHandP1, _leftHandP1.position, modifiedRotation, _directionLocatorP1);
                }

                if (!CompareUtility.IsApproximatelyEqual(param.VerticalShift, 0))
                {
                    var modifiedRotation =
                        Quaternion.AngleAxis(param.VerticalShift,
                            _cameraLocatorP1.TransformDirection(Vector3.right)) * _rightHandP1.rotation;
                    AlignCoordinateSystems(_rightHandP1, _rightHandP1.position, modifiedRotation, _directionLocatorP1);
                }
            }

            // 一人称腰射状态下人物偏移(枪，分辨率相关)
            _directionLocatorP1.localPosition += param.FirstPersonPositionOffset;
            _directionLocatorP1.localEulerAngles += param.FirstPersonRotationOffset;
        }

        private void GetP1Bones(GameObject obj)
        {
            if (null == obj) return;
            _viewPointP1 = BoneMount.FindChildBone(obj, BoneName.ViewPoint);
            _baseLocatorP1 = BoneMount.FindChildBone(obj, BoneName.FirstPersonHandLocator);
            _directionLocatorP1 = BoneMount.FindChildBone(obj, BoneName.FirstPersonSubHandLocator);
            _cameraLocatorP1 = BoneMount.FindChildBone(obj, BoneName.FirstPersonCameraLocator);
            _rightHandP1 = BoneMount.FindChildBone(obj, BoneName.CharacterRightHandName);
            _leftHandP1 = BoneMount.FindChildBone(obj, BoneName.CharacterLeftHandName);
            _ikControllerP1 = obj.GetComponent<PlayerIK>();
            _ikControllerP1.SetIKGoal(AvatarIKGoal.LeftHand);
        }

        private void GetP3Bones(GameObject obj)
        {
            if (null == obj) return;
            _spineP3 = BoneMount.FindChildBone(obj, BoneName.CharacterSpineName); //"Bip01 Spine");
            _spine1P3 = BoneMount.FindChildBone(obj, BoneName.CharacterSpine1Name); // "Bip01 Spine1");
            _pelvisP3 = BoneMount.FindChildBone(obj, BoneName.CharacterBipPelvisName); //"Bip01 Pelvis");
            _ikControllerP3 = obj.GetComponent<PlayerIK>();
            _ikControllerP3.SetIKGoal(AvatarIKGoal.LeftHand);
        }

        /// <summary>
        ///     在ancestor空间进行旋转，得到在target空间的旋转
        /// </summary>
        /// <param name="ancestor"></param>
        /// <param name="target"></param>
        /// <param name="rotation"></param>
        /// <returns></returns>
        public static Quaternion ConvertRotation(Transform ancestor, Transform target, Quaternion rotation)
        {
            var r = Matrix4x4.TRS(Vector3.zero, rotation, Vector3.one);
            var m = target.worldToLocalMatrix * ancestor.localToWorldMatrix * r * ancestor.worldToLocalMatrix *
                    target.localToWorldMatrix;
            return m.ExtractRotation();
        }

        public static Vector3 ConvertPosition(Transform ancestor, Transform target, Vector3 transition)
        {
            var r = Matrix4x4.TRS(transition, Quaternion.identity, Vector3.one);
            var m = target.parent.worldToLocalMatrix * ancestor.localToWorldMatrix * r * ancestor.worldToLocalMatrix *
                    target.localToWorldMatrix;
            return m.ExtractPosition();
        }

        /**
         * 使得rigTarget与source对齐
         * rigTarget is an ancestor of source
         * rigTarget isn't an ancestor of target
         * targetPosition/targetRotation represented in world space
         */
        public static void AlignCoordinateSystems(Transform source, Vector3 targetPosition, Quaternion targetRotation,
            Transform rigTarget)
        {
            var modified = rigTarget.parent.worldToLocalMatrix *
                           Matrix4x4.TRS(targetPosition, targetRotation, Vector3.one) * source.worldToLocalMatrix *
                           rigTarget.transform.localToWorldMatrix;
            rigTarget.localPosition = modified.ExtractPosition();
            rigTarget.localRotation = modified.ExtractRotation();
            //rigTarget.localScale = modified.ExtractScale();
        }

        #region IBoneRigging

        public void Update(CodeRigBoneParam param)
        {
            try
            {
                _mainInfo.BeginProfileOnlyEnableProfile();
                if (ThirdPersonIncluded)
                {
                    StableUpperBody(param.OverlayAnimationWeight, param.PostureWhenOverlay);
                    _ikControllerP3.SetIKActive(param.IKActive);
                }

                Peek(param.PeekAmplitude, param.IsSight);
                PitchP1(param.PitchAmplitude);

                if (FirstPersonIncluded)
                {
                    var parent = _characterP1.transform.parent;
                    try
                    {
                        _characterP1.transform.SetParent(null, false);
                        SetFirstPersonShift(CreateFirstPersonOffsetParam(param));
                        SetSightStatus(CreateSightStatusParam(param));
                        _ikControllerP1.SetIKActive(param.IKActive);
                    }
                    finally
                    {
                        _characterP1.transform.SetParent(parent, false);
                    }
                }
            }
            finally
            {
                _mainInfo.EndProfileOnlyEnableProfile();
            }
        }

        #region CreateData

        private static SightStatusParam CreateSightStatusParam(CodeRigBoneParam param)
        {
            return new SightStatusParam
            {
                IsFire = param.IsFire,
                IsSight = param.IsSight,
                IsIntoSight = param.IsIntoSight,
                PeekAmplitude = param.PeekAmplitude,
                SightOffset = param.SightOffset,
                PitchAmplitude = param.PitchAmplitude,
                SightProgress = param.SightProgress,
                SightHorizontalShift = param.SightHorizontalShift /* * param.SightShiftBuff*/,
                SightVerticalShift = param.SightVerticalShift /* * param.SightShiftBuff*/,
                FirstPersonSightOffset = param.FirstPersonSightOffset,
                ScopeOffset = param.ScopeOffset,
                SightModelOffset = param.SightModelOffset
            };
        }

        private static FirstPersonOffsetParam CreateFirstPersonOffsetParam(CodeRigBoneParam param)
        {
            return new FirstPersonOffsetParam
            {
                IsFirstPerson = param.IsFirstPerson,
                IsSight = param.IsSight,
                HorizontalShift = param.IsSight
                    ? param.SightMoveHorizontalShift
                    : param.FastMoveHorizontalShift,
                VerticalShift =
                    param.IsSight ? param.SightMoveVerticalShift : param.FastMoveVerticalShift,
                FirstPersonPositionOffset = param.FirstPersonPositionOffset,
                FirstPersonRotationOffset = param.FirstPersonRotationOffset
            };
        }

        #endregion

        #endregion
    }
}
