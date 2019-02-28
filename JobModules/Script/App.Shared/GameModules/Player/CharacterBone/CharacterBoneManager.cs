using Assets.Utils.Configuration;
using Core.Appearance;
using Core.CameraControl;
using Core.CharacterBone;
using Core.CharacterState;
using Core.Compare;
using Core.Fsm;
using Core.Utils;
using System;
using System.Collections.Generic;
using App.Shared.Components.Player;
using App.Shared.GameModules.Player.Appearance.WardrobeControllerPackage;
using App.Shared.GameModules.Player.Appearance.WeaponControllerPackage;
using App.Shared.GameModules.Player.CharacterBone.IkControllerPackage;
using Core.EntityComponent;
using UnityEngine;
using Utils.Appearance;
using Utils.CharacterState;
using Utils.Configuration;
using Utils.Singleton;

namespace App.Shared.GameModules.Player.CharacterBone
{
    public class CharacterBoneManager : ICharacterBone
    {
        private static readonly LoggerAdapter Logger = new LoggerAdapter(typeof(CharacterBoneManager));
        private readonly BoneRigging _boneRigging = new BoneRigging();
        private readonly FollowRot _followRot = new FollowRot();
        private readonly WeaponRot _weaponRot = new WeaponRot();
        private readonly BoneMount _mount = new BoneMount();

        private GameObject _characterP3;
        private GameObject _characterP1;
        private WeaponController _weaponController;
        private WardrobeController _wardrobeController;
        private PlayerIkController _playerIkController;

        private bool _cacheP3Dirty = true;
        private bool _cacheP1Dirty = true;

        private float _sightProgress;
        private float _fastMoveHorizontalShift;
        private float _fastMoveVerticalShift;
        private float _sightMoveHorizontalShift;
        private float _sightMoveVerticalShift;

        private bool _attachmentNeedIK = false;
        private bool _weaponHasIK = false;

        private GameObject _characterRoot;
        private CharacterView _view = CharacterView.ThirdPerson;

        private FsmOutputCache _directOutputs = new FsmOutputCache();

        private PlayerIK _firstPlayerIk;
        private PlayerIK _thirdPlayerIk;


        public Action CacheChangeCacheAction { private set; get; }
        public float PeekDegree { get; private set; }

        public bool IsFirstPerson
        {
            get { return _view == CharacterView.FirstPerson; }
        }

        public bool IsIKActive
        {
            set
            {
                if (value) _playerIkController.SetEnableIk();
                else _playerIkController.SetDisableIk();
            }
            get { return _playerIkController.EnableIK; }
        }

        public int LastHeadRotSlerpTime { get; set; }
        public float LastHeadRotAngle { get; set; }
        public bool IsHeadRotCW { get; set; }
        public bool ForbidRot { get; set; }

        public CharacterBoneManager()
        {
            _playerIkController = new PlayerIkController();
            CacheChangeCacheAction = () =>
            {
                _cacheP1Dirty = true;
                _cacheP3Dirty = true;
            };
        }

        public void SetWardrobeController(WardrobeControllerBase value)
        {
            _wardrobeController = value as WardrobeController;
        }

        public void SetWeaponController(WeaponControllerBase value)
        {
            _weaponController = value as WeaponController;
        }

        public Transform FastGetBoneTransform(string boneName, CharacterView view)
        {
            Transform ret = null;
            if (view == CharacterView.ThirdPerson)
            {
                CheckP3Cache();
                foreach (TransformCache transformCache in _cacheP3)
                {
                    transformCache.Cache.TryGetValue(boneName, out ret);
                    if (ret != null)
                    {
                        break;
                    }
                }
            }
            else
            {
                CheckP1Cache();
                foreach (TransformCache transformCache in _cacheP1)
                {
                    transformCache.Cache.TryGetValue(boneName, out ret);
                    if (ret != null)
                    {
                        break;
                    }
                }
            }

            return ret;
        }

        public void SetThirdPersonCharacter(GameObject obj)
        {
            _characterP3 = obj;
            _boneRigging.SetThirdPersonCharacter(obj);
            _followRot.SetThirdPersonCharacter(obj);
            _weaponRot.SetThirdPersonCharacter(obj);
            if (obj != null)
                _thirdPlayerIk = obj.GetComponent<PlayerIK>();
        }

        public void SetFirstPersonCharacter(GameObject obj)
        {
            _characterP1 = obj;
            _boneRigging.SetFirstPersonCharacter(obj);
            _weaponRot.SetFirstPersonCharacter(obj);
            if (null != obj)
                _firstPlayerIk = obj.GetComponent<PlayerIK>();
        }

        public void SetCharacterRoot(GameObject characterRoot)
        {
            _characterRoot = characterRoot;
        }

        public void SetFirstPerson()
        {
            _view = CharacterView.FirstPerson;
        }

        public void SetThridPerson()
        {
            _view = CharacterView.ThirdPerson;
        }

        public void Reborn()
        {
        }

        public void Dead()
        {
            EndIK();
            SetThridPerson();
            _weaponHasIK = false;
            _attachmentNeedIK = false;
        }

        public void SetStablePelvisRotation()
        {
            _boneRigging.SetStablePelvisRotation();
        }

        public void Peek(float amplitude)
        {
            if (CompareUtility.IsApproximatelyEqual(amplitude, 0))
            {
                amplitude = 0;
            }
            else if (CompareUtility.IsApproximatelyEqual(amplitude, 1))
            {
                amplitude = 1;
            }
            else if (CompareUtility.IsApproximatelyEqual(amplitude, -1))
            {
                amplitude = -1;
            }

            PeekDegree = amplitude;
        }

        public void SightProgress(float progress)
        {
            _sightProgress = progress;
        }

        public void FirstPersonFastMoveShift(float horizontal, float vertical, float sightHorizontal,
            float sightVertical)
        {
            _fastMoveHorizontalShift = horizontal;
            _fastMoveVerticalShift = vertical;
            _sightMoveHorizontalShift = sightHorizontal;
            _sightMoveVerticalShift = sightVertical;
        }

        public void Execute(Action<FsmOutput> addOutput)
        {
            _playerIkController.Execute();
        }

        public void PreUpdate(FollowRotParam param, ICharacterBone characterBone)
        {
            _followRot.PreUpdate(param, characterBone);
        }

        public void WeaponRotUpdate(CodeRigBoneParam param)
        {
            if (!param.IsEmptyHand)
            {
                // 获取一，三人称枪口位置
                var muzzleTransform = GetLocation(SpecialLocation.MuzzleEffectPosition, CharacterView.ThirdPerson);
                if (muzzleTransform)
                    param.MuzzleLocationP3 = muzzleTransform.position;
                else
                    param.IsEmptyHand = true;
                
                muzzleTransform = GetLocation(SpecialLocation.MuzzleEffectPosition, CharacterView.FirstPerson);
                if (muzzleTransform)
                    param.MuzzleLocationP1 = muzzleTransform.position;
            }

            _weaponRot.WeaponRotUpdate(param);
        }
        
        public void WeaponRotPlayback(CodeRigBoneParam param)
        {
            _weaponRot.WeaponRotPlayback(param);
        }

        public void Update(CodeRigBoneParam param)
        {
            param.PeekAmplitude = PeekDegree;
            param.SightProgress = _sightProgress;
            param.FastMoveHorizontalShift = _fastMoveHorizontalShift;
            param.FastMoveVerticalShift = _fastMoveVerticalShift;
            param.SightMoveHorizontalShift = _sightMoveHorizontalShift;
            param.SightMoveVerticalShift = _sightMoveVerticalShift;
            param.IsFirstPerson = IsFirstPerson;

            var needIk = UpdateSightData(ref param);

            // 有动作状态控制是否需要开启IK
            param.IKActive = IsIKActive || param.IKActive && needIk;
            param.IsEmptyHand = _weaponController.IsEmptyHand();
            if (param.IsEmptyHand)
            {
                if (!IsFirstPerson)
                {
                    //第三人称空手取消pitch
                    param.PitchAmplitude = 0;
                }

                // 空手状态没有随动效果
                param.FastMoveHorizontalShift = 0;
                param.FastMoveVerticalShift = 0;
                _attachmentNeedIK = false;
                _weaponHasIK = false;
            }
            
            param.SightModelOffset = YawPitchUtility.Normalize(_characterRoot.transform.rotation).x;

            _boneRigging.Update(param);
            _followRot.Update(param);
            UpdateIk();
        }

        public void SyncTo(IGameComponent characterBoneComponent)
        {
            var boneComponent = characterBoneComponent as CharacterBoneComponent;
            _followRot.SyncTo(boneComponent);
            _weaponRot.SyncTo(boneComponent);
            _playerIkController.SyncTo(boneComponent);
        }

        // 包括枪配件的改变
        public void CurrentWeaponChanged(GameObject objP1, GameObject objP3)
        {
            if (_boneRigging.SetIKTarget(objP1, objP3, ref _weaponHasIK))
            {
                var lowRailInCurrentWeapon = _weaponController.GetCurrentLowRailId();
                var gripHandPoseState =
                    SingletonManager.Get<WeaponPartsConfigManager>().GetSubType(lowRailInCurrentWeapon);

                _directOutputs.CacheFsmOutput(AnimatorParametersHash.Instance.GripHandPoseHash,
                    AnimatorParametersHash.Instance.GripHandPoseName,
                    AnimatorParametersHash.Instance.GripHandPoseEnableValue,
                    CharacterView.FirstPerson | CharacterView.ThirdPerson);

                _directOutputs.CacheFsmOutput(AnimatorParametersHash.Instance.GripHandPoseStateHash,
                    AnimatorParametersHash.Instance.GripHandPoseStateName,
                    gripHandPoseState,
                    CharacterView.FirstPerson | CharacterView.ThirdPerson);
                _attachmentNeedIK = true; //握把需要开启IK
            }
            else
            {
                _directOutputs.CacheFsmOutput(AnimatorParametersHash.Instance.GripHandPoseHash,
                    AnimatorParametersHash.Instance.GripHandPoseName,
                    AnimatorParametersHash.Instance.GripHandPoseDisableValue,
                    CharacterView.FirstPerson | CharacterView.ThirdPerson);
                _attachmentNeedIK = false;
            }
        }

        private bool UpdateSightData(ref CodeRigBoneParam param)
        {
            var isEmptyInHand = _weaponController.IsEmptyHand();
            var weaponIdInHand = _weaponController.GetWeaponIdInHand();
            // 当前枪瞄准镜ID
            var scopeIdInCurrentWeapon = _weaponController.GetCurrentScopeId();
            var needIk = false;
            if (!isEmptyInHand)
            {
                needIk = _weaponHasIK;
                
                if (_characterP1 == null) return needIk;
                
                param.SightOffset = SingletonManager.Get<WeaponAvatarConfigManager>().GetSightDistance(weaponIdInHand);

                if (scopeIdInCurrentWeapon > 0)
                {
                    // 瞄准镜距人眼距离
                    param.ScopeOffset = SingletonManager.Get<WeaponPartsConfigManager>()
                        .GetScopeOffset(scopeIdInCurrentWeapon);
                    if (param.IsSight)
                    {
                        //瞄准镜缩放
                        param.ScopeScale = SingletonManager.Get<WeaponPartsConfigManager>()
                            .GetSightModelScale(scopeIdInCurrentWeapon);
                    }
                }
                else if (param.IsSight)
                {
                    param.ScopeScale = SingletonManager.Get<WeaponResourceConfigManager>().GetAimModelScale(weaponIdInHand);
                }
            }

            return needIk;
        }

        private void UpdateIk()
        {
            if (_thirdPlayerIk != null)
                _thirdPlayerIk.UpdateIk();
            if (_firstPlayerIk != null)
                _firstPlayerIk.UpdateIk();
        }

        public void EnableIK()
        {
            if (_attachmentNeedIK)
            {
                _playerIkController.SetEnableIk();
            }
        }

        public void EndIK()
        {
            _playerIkController.SetDisableIk();
        }

        private List<TransformCache> _cacheP3 = new List<TransformCache>(16);
        private List<TransformCache> _cacheP1 = new List<TransformCache>(16);
        private Transform[] _cacheP1Transform = new Transform[(int) SpecialLocation.EndOfTheWorld];
        private Transform[] _cacheP3Transform = new Transform[(int) SpecialLocation.EndOfTheWorld];
        
        public Transform GetLocation(SpecialLocation location, CharacterView view)
        {
            if (view == CharacterView.FirstPerson)
            {
                CheckP1Cache();
                _wardrobeController.RefreshP1BonePosition();

                return _cacheP1Transform[(int) location];
            }
            else
            {
                CheckP3Cache();
                return _cacheP3Transform[(int) location];
            }
        }

        private void CheckP1Cache()
        {
            if (_cacheP1Dirty)
            {
                for (int i = 0; i < _cacheP1Transform.Length; ++i)
                {
                    _cacheP1Transform[i] = null;
                }
                _cacheP1.Clear();
                _characterP1.GetComponentsInChildren<TransformCache>(_cacheP1);

                for (int location = 0; location < (int) SpecialLocation.EndOfTheWorld; ++location)
                {
                    _cacheP1Transform[(int) location] = _mount.GetLocation(
                        _mount.SearchingStart(_characterP1, (SpecialLocation) location), (SpecialLocation) location);
                }

                _cacheP1Dirty = false;
            }
        }

        private void CheckP3Cache()
        {
            if (_cacheP3Dirty)
            {
                for (int i = 0; i < _cacheP3Transform.Length; ++i)
                {
                    _cacheP3Transform[i] = null;
                }

                _cacheP3.Clear();
                _characterP3.GetComponentsInChildren<TransformCache>(_cacheP1);
                
                for (int location = 0; location < (int) SpecialLocation.EndOfTheWorld; ++location)
                {
                    _cacheP3Transform[(int) location] = _mount.GetLocation(
                        _mount.SearchingStart(_characterP3, (SpecialLocation) location), (SpecialLocation) location);
                }

                _cacheP3Dirty = false;
            }
        }

        public void SetWeaponPitch(Action<FsmOutput> addOutput, float pitch)
        {
            _weaponRot.SetWeaponPitch(addOutput, pitch);
        }
    }
}