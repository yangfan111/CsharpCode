using Utils.AssetManager;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using System;
using Core.Utils;
using Utils.Appearance.Weapon;
using Utils.AssetManager;
using XmlConfig;
using Utils.Compare;
using Utils.CharacterState;
using Utils.Configuration;
using Utils.Singleton;
using Utils.Utils;
using Object = UnityEngine.Object;

namespace Utils.Appearance
{

    public abstract class WeaponControllerBase
    {
        private static readonly LoggerAdapter Logger = new LoggerAdapter(typeof(WeaponControllerBase));

        class Command
        {
            private WeaponInPackage _weaponPosition;
            private int _attachment;
            private WeaponPartLocation _attachmentLocation;
            private int _id;
            private int _reloadState;

            private Action _remountWeaponInPackage;
            private Action _changeWeaponOnLocator;
            private Action<int> _reload;
            private Action<WeaponInPackage> _changeWeaponInHand;
            private Action<WeaponInPackage, int> _changeWeaponInPackage;
            private Action<WeaponInPackage, WeaponPartLocation, int> _changeWeaponAttachment;

            public void SetChangeWeaponInHand(Action<WeaponInPackage> action, WeaponInPackage pos)
            {
                _changeWeaponInHand = action;
                _weaponPosition = pos;
            }

            public void SetChangeWeaponInPackage(Action<WeaponInPackage, int> action, WeaponInPackage pos, int id)
            {
                _changeWeaponInPackage = action;
                _weaponPosition = pos;
                _id = id;
            }

            public void SetChangeAttachment(Action<WeaponInPackage, WeaponPartLocation, int> action,
                WeaponInPackage pos,
                WeaponPartLocation location,
                int id)
            {
                _changeWeaponAttachment = action;
                _weaponPosition = pos;
                _attachmentLocation = location;
                _attachment = id;
            }

            public void SetRemountWeaponInPackage(Action action)
            {
                _remountWeaponInPackage = action;
            }

            public void SetChangeWeaponOnLocator(Action action)
            {
                _changeWeaponOnLocator = action;
            }

            public void SetReload(Action<int> action, int reloadState)
            {
                _reload = action;
                _reloadState = reloadState;
            }

            public void Execute()
            {
                if (_changeWeaponInHand != null)
                {
                    _changeWeaponInHand.Invoke(_weaponPosition);
                    _changeWeaponInHand = null;
                }

                if (_changeWeaponInPackage != null)
                {
                    _changeWeaponInPackage.Invoke(_weaponPosition, _id);
                    _changeWeaponInPackage = null;
                }

                if (_changeWeaponAttachment != null)
                {
                    _changeWeaponAttachment.Invoke(_weaponPosition, _attachmentLocation, _attachment);
                    _changeWeaponAttachment = null;
                }

                if (_remountWeaponInPackage != null)
                {
                    _remountWeaponInPackage.Invoke();
                    _remountWeaponInPackage = null;
                }

                if (null != _changeWeaponOnLocator)
                {
                    _changeWeaponOnLocator.Invoke();
                    _changeWeaponOnLocator = null;
                }

                if (null != _reload)
                {
                    _reload.Invoke(_reloadState);
                    _reload = null;
                }
            }
        }

        #region ILatestWeaponState

        private int[] _latestWeaponValue = new int[(int) LatestWeaponStateIndex.EndOfTheWorld];
        private bool[] _latestWeaponRewindFlag = new bool[(int) LatestWeaponStateIndex.EndOfTheWorld];

        protected int GetLatestWeaponValue(LatestWeaponStateIndex type)
        {
            return _latestWeaponValue[(int) type];
        }

        protected void SetLatestWeaponValue(LatestWeaponStateIndex type, int value)
        {
            var index = (int) type;
            _rewindFlag =
                (_latestWeaponRewindFlag[index] = _latestWeaponRewindFlag[index] ||
                    !CompareUtility.IsApproximatelyEqual(_latestWeaponValue[index], value)) || _rewindFlag;
            _latestWeaponValue[index] = value;
        }

        #endregion

        #region IPredictedWeaponState

        private readonly int[] _predictedWeaponValue = new int[(int) PredictedWeaponStateIndex.EndOfTheWorld];
        private readonly bool[] _predictedWeaponRewindFlag = new bool[(int) PredictedWeaponStateIndex.EndOfTheWorld];

        protected int GetPredictedWeaponValue(PredictedWeaponStateIndex type)
        {
            return _predictedWeaponValue[(int) type];
        }

        protected void SetPredictedWeaponValue(PredictedWeaponStateIndex type, int value)
        {
            var index = (int) type;
            _rewindFlag =
                (_predictedWeaponRewindFlag[index] = _predictedWeaponRewindFlag[index] ||
                    !CompareUtility.IsApproximatelyEqual(_predictedWeaponValue[index], value)) || _rewindFlag;
            _predictedWeaponValue[index] = value;
        }
        
        #endregion
        
        #region IClientWeaponState

        private readonly int[] _clientWeaponValue = new int[(int) ClientWeaponStateIndex.EndOfTheWorld];
        private readonly bool[] _clientWeaponRewindFlag = new bool[(int) ClientWeaponStateIndex.EndOfTheWorld];

        protected int GetClientWeaponValue(ClientWeaponStateIndex type)
        {
            return _clientWeaponValue[(int) type];
        }

        protected void SetClientWeaponValue(ClientWeaponStateIndex type, int value)
        {
            var index = (int) type;
            _rewindFlag =
                (_clientWeaponRewindFlag[index] = _clientWeaponRewindFlag[index] ||
                                                     !CompareUtility.IsApproximatelyEqual(_clientWeaponValue[index], value)) || _rewindFlag;
            _clientWeaponValue[index] = value;
        }
        
        #endregion
        
        protected Action<GameObject, GameObject> _weaponChangedCallBack;
        protected Action _cacheChangeAction;

        private const int ValidP1Index = 0;
        private const int ValidP3Index = 1;
        private const int RightWeapon = 0;
        private const int LeftWeapon = 1;

        private BoneMount _mount = new BoneMount();

        private int _p1Index = UniversalConsts.InvalidIntId;
        private int _p3Index = UniversalConsts.InvalidIntId;
        private GameObject _characterP3;
        private GameObject _characterP1;
        private OverrideAnimator _overrideP1;
        private OverrideAnimator _overrideP3;

        // 第一个始终为空
        private WeaponGameObjectData[,] _weapons = new WeaponGameObjectData[(int) WeaponInPackage.EndOfTheWorld, 2];

        private WeaponGameObjectData[,,] _attachments = new WeaponGameObjectData[(int) WeaponInPackage.EndOfTheWorld, 2,
            (int) WeaponPartLocation.EndOfTheWorld];

        private MountWeaponHandler[,] _mountWeaponHandlers =
            new MountWeaponHandler[(int) WeaponInPackage.EndOfTheWorld, 2];

        private MountAttachmentHandler[,,] _mountAttachmentHandlers =
            new MountAttachmentHandler[(int) WeaponInPackage.EndOfTheWorld, 2, (int) WeaponPartLocation.EndOfTheWorld];

        private bool _rewindFlag;

        private List<Command> _outerCommand = new List<Command>();
        private int _currentCommandIndex;

        private Sex _sex = Sex.Female;
        private bool _unique = false;
        private CharacterView _view = CharacterView.ThirdPerson;

        private readonly Dictionary<int, Action<int>> _rewindLatestWeapon;
        private readonly Dictionary<int, Action<int>> _rewindPredictedWeapon;
        private readonly Dictionary<int, Action<int>> _rewindClientWeapon;

        public bool P3HaveInit = false;

        private int[] _initWeaponInPackage = new int[(int)WeaponInPackage.EndOfTheWorld];
        private int[,] _initAttachments =
            new int[(int) WeaponInPackage.EndOfTheWorld, (int) WeaponPartLocation.EndOfTheWorld];

        protected WeaponControllerBase()
        {
            for (var i = 0; i < _mountWeaponHandlers.GetLength(0); i++)
            {
                _mountWeaponHandlers[i, ValidP1Index] = new MountWeaponHandler(this, (WeaponInPackage) i, ValidP1Index);
                _mountWeaponHandlers[i, ValidP3Index] = new MountWeaponHandler(this, (WeaponInPackage) i, ValidP3Index);
            }

            for (var i = 0; i < (int) WeaponInPackage.EndOfTheWorld; i++)
            {
                for (var j = 0; j < (int) WeaponPartLocation.EndOfTheWorld; j++)
                {
                    _mountAttachmentHandlers[i, ValidP1Index, j] = new MountAttachmentHandler(
                        this,
                        ValidP1Index,
                        (WeaponInPackage) i,
                        (WeaponPartLocation) j);
                    _mountAttachmentHandlers[i, ValidP3Index, j] = new MountAttachmentHandler(
                        this,
                        ValidP3Index,
                        (WeaponInPackage) i,
                        (WeaponPartLocation) j);
                }
            }

            for (var i = 0; i < (int) WeaponInPackage.EndOfTheWorld; ++i)
            {
                for (var j = 0; j < 2; ++j)
                {
                    _weapons[i, j] = new WeaponGameObjectData();
                    for (var k = 0; k < (int) WeaponPartLocation.EndOfTheWorld; ++k)
                    {
                        _attachments[i, j, k] = new WeaponGameObjectData();
                    }
                }
            }

            for (var i = 0; i < (int) WeaponInPackage.EndOfTheWorld; ++i)
            {
                _initWeaponInPackage[i] = -1;
                for (var j = 0; j < (int) WeaponPartLocation.EndOfTheWorld; j++)
                {
                    _initAttachments[i, j] = -1;
                }
            }
            
            for (var i = 0; i < (int)LatestWeaponStateIndex.EndOfTheWorld; ++i)
            {
                _latestWeaponValue[i] = UniversalConsts.InvalidIntId;
            }

            _rewindLatestWeapon = new Dictionary<int, Action<int>>
            {
                {
                    (int) LatestWeaponStateIndex.PrimaryWeaponOne,
                    x => RewindWeaponInPackage(WeaponInPackage.PrimaryWeaponOne, x)
                },
                {
                    (int) LatestWeaponStateIndex.PrimaryWeaponOneMuzzle,
                    x => RewindWeaponAttachment(WeaponInPackage.PrimaryWeaponOne, WeaponPartLocation.Muzzle, x)
                },
                {
                    (int) LatestWeaponStateIndex.PrimaryWeaponOneLowRail,
                    x => RewindWeaponAttachment(WeaponInPackage.PrimaryWeaponOne, WeaponPartLocation.LowRail, x)
                },
                {
                    (int) LatestWeaponStateIndex.PrimaryWeaponOneMagazine,
                    x => RewindWeaponAttachment(WeaponInPackage.PrimaryWeaponOne, WeaponPartLocation.Magazine, x)
                },
                {
                    (int) LatestWeaponStateIndex.PrimaryWeaponOneButtstock,
                    x => RewindWeaponAttachment(WeaponInPackage.PrimaryWeaponOne, WeaponPartLocation.Buttstock, x)
                },
                {
                    (int) LatestWeaponStateIndex.PrimaryWeaponOneScope,
                    x => RewindWeaponAttachment(WeaponInPackage.PrimaryWeaponOne, WeaponPartLocation.Scope, x)
                },
                {
                    (int) LatestWeaponStateIndex.PrimaryWeaponTwo,
                    x => RewindWeaponInPackage(WeaponInPackage.PrimaryWeaponTwo, x)
                },
                {
                    (int) LatestWeaponStateIndex.PrimaryWeaponTwoMuzzle,
                    x => RewindWeaponAttachment(WeaponInPackage.PrimaryWeaponTwo, WeaponPartLocation.Muzzle, x)
                },
                {
                    (int) LatestWeaponStateIndex.PrimaryWeaponTwoLowRail,
                    x => RewindWeaponAttachment(WeaponInPackage.PrimaryWeaponTwo, WeaponPartLocation.LowRail, x)
                },
                {
                    (int) LatestWeaponStateIndex.PrimaryWeaponTwoMagazine,
                    x => RewindWeaponAttachment(WeaponInPackage.PrimaryWeaponTwo, WeaponPartLocation.Magazine, x)
                },
                {
                    (int) LatestWeaponStateIndex.PrimaryWeaponTwoButtstock,
                    x => RewindWeaponAttachment(WeaponInPackage.PrimaryWeaponTwo, WeaponPartLocation.Buttstock, x)
                },
                {
                    (int) LatestWeaponStateIndex.PrimaryWeaponTwoScope,
                    x => RewindWeaponAttachment(WeaponInPackage.PrimaryWeaponTwo, WeaponPartLocation.Scope, x)
                },
                {(int) LatestWeaponStateIndex.SideArm, x => RewindWeaponInPackage(WeaponInPackage.SideArm, x)},
                {
                    (int) LatestWeaponStateIndex.SideArmMuzzle,
                    x => RewindWeaponAttachment(WeaponInPackage.SideArm, WeaponPartLocation.Muzzle, x)
                },
                {
                    (int) LatestWeaponStateIndex.SideArmLowRail,
                    x => RewindWeaponAttachment(WeaponInPackage.SideArm, WeaponPartLocation.LowRail, x)
                },
                {
                    (int) LatestWeaponStateIndex.SideArmMagazine,
                    x => RewindWeaponAttachment(WeaponInPackage.SideArm, WeaponPartLocation.Magazine, x)
                },
                {
                    (int) LatestWeaponStateIndex.SideArmButtstock,
                    x => RewindWeaponAttachment(WeaponInPackage.SideArm, WeaponPartLocation.Buttstock, x)
                },
                {
                    (int) LatestWeaponStateIndex.SideArmScope,
                    x => RewindWeaponAttachment(WeaponInPackage.SideArm, WeaponPartLocation.Scope, x)
                },
                {(int) LatestWeaponStateIndex.MeleeWeapon, x => RewindWeaponInPackage(WeaponInPackage.MeleeWeapon, x)},
                {
                    (int) LatestWeaponStateIndex.TacticWeapon,
                    x => RewindWeaponInPackage(WeaponInPackage.TacticWeapon, x)
                },
                {
                    (int) LatestWeaponStateIndex.ThrownWeapon,
                    x => RewindWeaponInPackage(WeaponInPackage.ThrownWeapon, x)
                },
            };
            _rewindPredictedWeapon = new Dictionary<int, Action<int>>
            {
                {(int) PredictedWeaponStateIndex.WeaponInHand, x => RewindWeaponInHand((WeaponInPackage) x)},
                {(int) PredictedWeaponStateIndex.ReloadState, RewindReloadState},
                {(int) PredictedWeaponStateIndex.OverrideControllerState, x => RewindOverrideController((OverrideControllerState) x)},
            };
            _rewindClientWeapon = new Dictionary<int, Action<int>>
            {
                {(int) ClientWeaponStateIndex.AlternativeWeaponLocator, RewindMountWeaponOnAlternativeLocator},
                {(int) ClientWeaponStateIndex.AlternativeP3WeaponLocator, RewindMountP3WeaponOnAlternativeLocator},
            };
        }

        #region Sync

        public void TryRewind()
        {
            if (_rewindFlag)
            {
                _rewindFlag = false;
                RewindIntValue(_latestWeaponRewindFlag, _latestWeaponValue, _rewindLatestWeapon);
                RewindIntValue(_predictedWeaponRewindFlag, _predictedWeaponValue, _rewindPredictedWeapon);
                RewindIntValue(_clientWeaponRewindFlag, _clientWeaponValue, _rewindClientWeapon);
            }
        }

        private void RewindIntValue(bool[] flags, int[] values, Dictionary<int, Action<int>> handler)
        {
            for (int i = 0; i < flags.Length; i++)
            {
                if (flags[i])
                {
                    flags[i] = false;
                    if (handler.ContainsKey(i))
                    {
                        handler[i](values[i]);
                    }
                }
            }
        }

        #endregion

        #region handle outer command

        public void RemountWeaponInPackage()
        {
            //var cmd = GetAvailableCommand();
            //cmd.SetRemountWeaponInPackage(RemountWeaponDueToBag);
            Logger.Debug("RemountWeaponInPackage");
            RemountWeaponDueToBag();
        }

        public void MountWeaponToPackage(WeaponInPackage pos, int id)
        {
            if (ThirdPersonIncluded)
            {
                var cmd = GetAvailableCommand();
                cmd.SetChangeWeaponInPackage(MountWeaponInPackageImpl, pos, id);
            }
            else
            {
                MountInitWeaponInPackage(pos, id);
            }
            
            Logger.DebugFormat("MountWeaponToPackage: {0}, {1}", pos, id);
        }

        public void UnmountWeaponInPackage(WeaponInPackage pos)
        {
            if (ThirdPersonIncluded)
            {
                var cmd = GetAvailableCommand();
                cmd.SetChangeWeaponInPackage(UnmountWeaponInPackageImpl, pos, UniversalConsts.InvalidIntId);
            }
            else
            {
                UnMountInitWeaponInPackage(pos);
            }
            
            Logger.DebugFormat("UnmountWeaponInPackage: {0}", pos);
        }

        public void MountWeaponToHand(WeaponInPackage pos)
        {
            var cmd = GetAvailableCommand();
            cmd.SetChangeWeaponInHand(MountWeaponToHandImpl, pos);
            Logger.DebugFormat("MountWeaponToHand: {0}", pos);
        }

        public void UnmountWeaponFromHand()
        {
            var cmd = GetAvailableCommand();
            cmd.SetChangeWeaponInHand(UnMountWeaponFromHandImpl, WeaponInPackage.EmptyHanded);
            Logger.DebugFormat("UnmountWeaponFromHand");
        }

        public void JustUnMountWeaponFromHand()
        {
            var cmd = GetAvailableCommand();
            cmd.SetChangeWeaponInHand(JustUnMountWeaponFromHandImpl, WeaponInPackage.EmptyHanded);
            Logger.InfoFormat("JustUnMountWeaponFromHand");
        }

        public void JustClearOverrideController()
        {
            var cmd = GetAvailableCommand();
            cmd.SetChangeWeaponOnLocator(JustClearOverrideControllerImpl);
            Logger.InfoFormat("JustClearOverrideController");
        }

        public void UnmountWeaponFromHandAtOnce()
        {
            UnMountWeaponFromHandImpl(WeaponInPackage.EmptyHanded);
        }

        public void MountAttachment(WeaponInPackage pos, WeaponPartLocation location, int id)
        {
            if (ThirdPersonIncluded)
            {
                var cmd = GetAvailableCommand();
                cmd.SetChangeAttachment(MountAttachmentImpl, pos, location, id);
            }
            else
            {
                MountInitAttachmentInWeapon(pos, location, id);
            }
            Logger.DebugFormat("MountAttachment: {0}, {1}, {2}", pos, location, id);
        }

        public void UnmountAttachment(WeaponInPackage pos, WeaponPartLocation location)
        {
            if (ThirdPersonIncluded)
            {
                var cmd = GetAvailableCommand();
                cmd.SetChangeAttachment(UnmountAttachmentImpl, pos, location, UniversalConsts.InvalidIntId);
            }
            else
            {
                UnMountInitAttachmentInWeapon(pos, location);
            }
            Logger.DebugFormat("UnMountAttachment: {0}, {1}", pos, location);
        }

        public void MountWeaponOnAlternativeLocator()
        {
            var cmd = GetAvailableCommand();
            cmd.SetChangeWeaponOnLocator(MountWeaponOnAlternativeLocatorImpl);
        }

        public void RemountWeaponOnRightHand()
        {
            var cmd = GetAvailableCommand();
            cmd.SetChangeWeaponOnLocator(RemountWeaponOnRightHandImpl);
        }

        public void MountP3WeaponOnAlternativeLocator()
        {
            var cmd = GetAvailableCommand();
            cmd.SetChangeWeaponOnLocator(MountP3WeaponOnAlternativeLocatorImpl);
        }

        public void RemountP3WeaponOnRightHand()
        {
            var cmd = GetAvailableCommand();
            cmd.SetChangeWeaponOnLocator(RemountP3WeaponOnRightHandImpl);
        }

        #region P3Reload

        public void StartReload()
        {
            var cmd = GetAvailableCommand();
            cmd.SetReload(ChangeReloadState, (int) MagazineReloadState.StartReload);
        }

        public void DropMagazine()
        {
            var cmd = GetAvailableCommand();
            cmd.SetReload(ChangeReloadState, (int) MagazineReloadState.DropMagazine);
        }

        public void AddMagazine()
        {
            var cmd = GetAvailableCommand();
            cmd.SetReload(ChangeReloadState, (int) MagazineReloadState.AddMagazine);
        }

        public void EndReload()
        {
            var cmd = GetAvailableCommand();
            cmd.SetReload(ChangeReloadState, (int) MagazineReloadState.EndReload);
        }

        #endregion

        #endregion

        #region Initial Setting

        public void SetThirdPersonCharacter(GameObject obj)
        {
            _characterP3 = obj;
            _p3Index = ValidP3Index;
            P3HaveInit = true;
            if (null != _cacheChangeAction)
                _cacheChangeAction.Invoke();
            
            InitWeaponInPackage();
        }

        public void SetFirstPersonCharacter(GameObject obj)
        {
            _characterP1 = obj;
            AppearanceUtils.DisableRender(_characterP1);
            _p1Index = ValidP1Index;
            if (null != _cacheChangeAction)
                _cacheChangeAction.Invoke();
        }

        public void SetAnimatorP1(Animator animator)
        {
            _overrideP1 = new OverrideAnimator(animator, true);
            _overrideP1.Change(_sex, _unique, UniversalConsts.InvalidIntId);
            ResetPredictedWeaponValue();
        }

        public void SetAnimatorP3(Animator animator)
        {
            _overrideP3 = new OverrideAnimator(animator, false);
            _overrideP3.Change(_sex, _unique, UniversalConsts.InvalidIntId);
            ResetPredictedWeaponValue();
        }

        private void ResetPredictedWeaponValue()
        {
            _predictedWeaponValue[(int) PredictedWeaponStateIndex.WeaponInHand] = (int) WeaponInPackage.EmptyHanded;
            _predictedWeaponValue[(int) PredictedWeaponStateIndex.OverrideControllerState] = (int) OverrideControllerState.EmptyHanded;
        }

        public void SetSex(Sex sex)
        {
            _sex = sex;
        }

        public void SetUnique(bool unique)
        {
            _unique = unique;
        }

        public void ClearThirdPersonCharacter()
        {
            CommonReset();
            GetRecycleRequests();

            _p3Index = -1;
            _characterP3 = null;
        }
        
        public void CommonReset()
        {
            for (var i = 0; i < (int) WeaponInPackage.EndOfTheWorld; ++i)
            {
                UnmountWeaponInPackageImpl((WeaponInPackage)i, -1);
                for (var j = 0; j < (int) WeaponPartLocation.EndOfTheWorld; ++j)
                    UnmountAttachment((WeaponInPackage)i, (WeaponPartLocation)j);
            }
            TryRewind();
            
            for (var i = 0; i < (int) LatestWeaponStateIndex.EndOfTheWorld; ++i)
            {
                _latestWeaponRewindFlag[i] = false;
            }

            for (var i = 0; i < (int)PredictedWeaponStateIndex.EndOfTheWorld; ++i)
            {
                _predictedWeaponRewindFlag[i] = false;
            }

            for (var i = 0; i < (int)ClientWeaponStateIndex.EndOfTheWorld; ++i)
            {
                _clientWeaponRewindFlag[i] = false;
            }
            _rewindFlag = false;
            _outerCommand.Clear();
            _currentCommandIndex = 0;
        }

        public void ClearInitWeaponData()
        {
            for (var i = 0; i < (int)WeaponInPackage.EndOfTheWorld; ++i)
            {
                _initWeaponInPackage[i] = -1;
                for (var j = 0; j < (int) WeaponPartLocation.EndOfTheWorld; ++j)
                    _initAttachments[i, j] = -1;
            }
            P3HaveInit = false;
        }

        #endregion

        public void SetFirstPerson()
        {
            if (FirstPersonIncluded) EnableWeapon(ValidP1Index, true);
            if (ThirdPersonIncluded) DisableWeapon(ValidP3Index);
            _view = CharacterView.FirstPerson;
        }

        public void SetThirdPerson()
        {
            if (FirstPersonIncluded) DisableWeapon(ValidP1Index);
            if (ThirdPersonIncluded) EnableWeapon(ValidP3Index, false);
            _view = CharacterView.ThirdPerson;
        }

        public void Execute()
        {
            for (int i = 0; i < _currentCommandIndex; i++)
            {
                _outerCommand[i].Execute();
            }

            _currentCommandIndex = 0;
        }

        public void SetP1WeaponTopLayerShader()
        {
            ChangeWeaponShader(_weapons[_predictedWeaponValue[(int)PredictedWeaponStateIndex.WeaponInHand], _p1Index], _p1Index);
        }

        public void ResetP1WeaponShader()
        {
            ResetWeaponShader(_weapons[_predictedWeaponValue[(int)PredictedWeaponStateIndex.WeaponInHand], _p1Index], _p1Index);
        }

        // 限定只能在已加载模型的前提下去获取
        public GameObject GetWeaponP1ObjInHand()
        {
            if (FirstPersonIncluded)
                return _weapons[_predictedWeaponValue[(int)PredictedWeaponStateIndex.WeaponInHand], _p1Index].PrimaryAsGameObject;

            Logger.Warn("try to get p1 weapon while p1 model not loaded");
            return null;
        }

        public GameObject GetWeaponP3ObjInHand()
        {
            if (ThirdPersonIncluded)
                return _weapons[_predictedWeaponValue[(int)PredictedWeaponStateIndex.WeaponInHand], _p3Index].PrimaryAsGameObject;

            Logger.Warn("try to get p3 weapon while p3 model not loaded");
            return null;
        }

        public GameObject GetP3CurrentAttachmentByType(int type)
        {
            if (ThirdPersonIncluded)
                return _attachments[_predictedWeaponValue[(int)PredictedWeaponStateIndex.WeaponInHand], _p3Index, type].PrimaryAsGameObject;

            Logger.Warn("try to get p3 Attachment while p3 model not loaded");
            return null;
        }

        public int GetCurrentScopeId()
        {
            var id = UniversalConsts.InvalidIntId;
            switch ((WeaponInPackage) _predictedWeaponValue[(int)PredictedWeaponStateIndex.WeaponInHand])
            {
                case WeaponInPackage.PrimaryWeaponOne:
                    id = _latestWeaponValue[(int)LatestWeaponStateIndex.PrimaryWeaponOneScope];
                    break;
                case WeaponInPackage.PrimaryWeaponTwo:
                    id = _latestWeaponValue[(int)LatestWeaponStateIndex.PrimaryWeaponTwoScope];
                    break;
            }

            return id;
        }

        public int GetCurrentLowRailId()
        {
            var id = UniversalConsts.InvalidIntId;
            switch ((WeaponInPackage) _predictedWeaponValue[(int)PredictedWeaponStateIndex.WeaponInHand])
            {
                case WeaponInPackage.PrimaryWeaponOne:
                    id = _latestWeaponValue[(int)LatestWeaponStateIndex.PrimaryWeaponOneLowRail];
                    break;
                case WeaponInPackage.PrimaryWeaponTwo:
                    id = _latestWeaponValue[(int)LatestWeaponStateIndex.PrimaryWeaponTwoLowRail];
                    break;
            }

            return id;
        }

        public int GetWeaponIdInHand()
        {
            var id = UniversalConsts.InvalidIntId;
            switch ((WeaponInPackage) _predictedWeaponValue[(int)PredictedWeaponStateIndex.WeaponInHand])
            {
                case WeaponInPackage.PrimaryWeaponOne:
                    id = _latestWeaponValue[(int)LatestWeaponStateIndex.PrimaryWeaponOne];
                    break;
                case WeaponInPackage.PrimaryWeaponTwo:
                    id = _latestWeaponValue[(int)LatestWeaponStateIndex.PrimaryWeaponTwo];
                    break;
                case WeaponInPackage.SideArm:
                    id = _latestWeaponValue[(int)LatestWeaponStateIndex.SideArm];
                    break;
                case WeaponInPackage.MeleeWeapon:
                    id = _latestWeaponValue[(int)LatestWeaponStateIndex.MeleeWeapon];
                    break;
                case WeaponInPackage.ThrownWeapon:
                    id = _latestWeaponValue[(int)LatestWeaponStateIndex.ThrownWeapon];
                    break;
                case WeaponInPackage.TacticWeapon:
                    id = _latestWeaponValue[(int)LatestWeaponStateIndex.TacticWeapon];
                    break;
            }

            return id;
        }

        public bool IsPrimaryWeaponOrSideArm()
        {
            switch ((WeaponInPackage) _predictedWeaponValue[(int)PredictedWeaponStateIndex.WeaponInHand])
            {
                case WeaponInPackage.PrimaryWeaponOne:
                case WeaponInPackage.PrimaryWeaponTwo:
                case WeaponInPackage.SideArm:
                    return true;
                default:
                    return false;
            }
        }

        public bool IsEmptyHand()
        {
            return (WeaponInPackage) _predictedWeaponValue[(int)PredictedWeaponStateIndex.WeaponInHand] == WeaponInPackage.EmptyHanded;
        }

        private void RewindWeaponInPackage(WeaponInPackage pos, int id)
        {
            if (id != UniversalConsts.InvalidIntId)
            {
                MountWeaponInPackageImpl(pos, id);
            }
            else
            {
                UnmountWeaponInPackageImpl(pos, id);
            }

            if (pos == (WeaponInPackage) _predictedWeaponValue[(int)PredictedWeaponStateIndex.WeaponInHand])
            {
                MountWeaponToHandImpl(pos);
            }
        }

        private void RewindWeaponInHand(WeaponInPackage pos)
        {
            if (WeaponInPackage.EmptyHanded == pos)
            {
                UnMountWeaponFromHandImpl(pos);
            }
            else
            {
                MountWeaponToHandImpl(pos);
            }
        }

        private void RewindOverrideController(OverrideControllerState state)
        {
            if(OverrideControllerState.EmptyHanded == state)
                ClearOverrideController();
        }

        private void RewindWeaponAttachment(WeaponInPackage pos, WeaponPartLocation location, int id)
        {
            if (id != UniversalConsts.InvalidIntId)
            {
                MountAttachmentImpl(pos, location, id);
            }
            else
            {
                UnmountAttachmentImpl(pos, location, id);
            }
        }

        private void RewindMountP3WeaponOnAlternativeLocator(int type)
        {
            if (0 == type)
            {
                _mount.RemountWeaponOnRightHand(GetWeaponP3ObjInHand(), _characterP3);
            }
            else
            {
                _mount.MountWeaponOnAlternativeLocator(GetWeaponP3ObjInHand(), _characterP3);
            }
        }

        private void RewindMountWeaponOnAlternativeLocator(int type)
        {
            if (0 == type)
            {
                Logger.InfoFormat("MyDebug   RemountWeaponOnRightHand");
                _mount.RemountWeaponOnRightHand(GetWeaponP3ObjInHand(), _characterP3);
                _mount.RemountWeaponOnRightHand(GetWeaponP1ObjInHand(), _characterP1);
            }
            else
            {
                Logger.InfoFormat("MyDebug   MountWeaponOnAlternativeLocator");
                _mount.MountWeaponOnAlternativeLocator(GetWeaponP3ObjInHand(), _characterP3);
                _mount.MountWeaponOnAlternativeLocator(GetWeaponP1ObjInHand(), _characterP1);
            }
        }

        private void RewindReloadState(int state)
        {
            switch ((MagazineReloadState) state)
            {
                case MagazineReloadState.StartReload:
                    StartReloadImpl();
                    break;
                case MagazineReloadState.DropMagazine:
                    DropMagazineImpl();
                    break;
                case MagazineReloadState.AddMagazine:
                    AddMagazineImpl();
                    break;
                case MagazineReloadState.EndReload:
                    EndReloadImpl();
                    break;
            }
        }

        private void MountWeaponInPackageImpl(WeaponInPackage pos, int id)
        {
            var weaponAvatarManager = SingletonManager.Get<WeaponAvatarConfigManager>();
            if (FirstPersonIncluded)
            {
                _mountWeaponHandlers[(int) pos, ValidP1Index].SetInfo(id);

                var assetInfo = weaponAvatarManager.GetFirstPersonWeaponModel(id);
                if ((null == assetInfo.AssetName || assetInfo.AssetName.Equals(String.Empty)) &&
                    (null == assetInfo.BundleName || assetInfo.BundleName.Equals(String.Empty)))
                    Logger.ErrorFormat("ErrorWeaponId Try To MountInPackage  id:  {0}", id);
                else
                    _loadRequestBatch.Add(CreateLoadRequest(
                        assetInfo,
                        _mountWeaponHandlers[(int) pos, ValidP1Index]));

                ClearCacheData(pos, _p1Index);
            }

            if (ThirdPersonIncluded)
            {
                _mountWeaponHandlers[(int) pos, ValidP3Index].SetInfo(id);

                var assetInfo = weaponAvatarManager.GetThirdPersonWeaponModel(id);
                if ((null == assetInfo.AssetName || assetInfo.AssetName.Equals(String.Empty)) &&
                    (null == assetInfo.BundleName || assetInfo.BundleName.Equals(String.Empty)))
                    Logger.ErrorFormat("ErrorWeaponId Try To MountInPackage  id:  {0}", id);
                else
                    _loadRequestBatch.Add(CreateLoadRequest(
                        weaponAvatarManager.GetThirdPersonWeaponModel(id),
                        _mountWeaponHandlers[(int) pos, ValidP3Index]));

                ClearCacheData(pos, _p3Index);
            }

            UpdateWeaponIdInPackage(pos, id);
            if (null != _cacheChangeAction)
                _cacheChangeAction.Invoke();
        }

        private void UnmountWeaponInPackageImpl(WeaponInPackage pos, int id)
        {
            if (_predictedWeaponValue[(int)PredictedWeaponStateIndex.WeaponInHand] == (int) pos)
            {
                UnMountWeaponFromHandImpl(pos);
            }

            if (ThirdPersonIncluded)
            {
                _mount.UnmountWeaponInPackage(_characterP3, pos);
            }

            ClearWeaponAndAttachment(pos, id);
            ClearWeaponHandlers(pos);
            
            if (null != _cacheChangeAction)
                _cacheChangeAction.Invoke();
        }

        private void ClearWeaponHandlers(WeaponInPackage pos)
        {
            _mountWeaponHandlers[(int) pos, ValidP1Index].SetInfo(UniversalConsts.InvalidIntId);
            _mountWeaponHandlers[(int) pos, ValidP3Index].SetInfo(UniversalConsts.InvalidIntId);
        }

        private void ClearWeaponAndAttachment(WeaponInPackage pos, int id)
        {
            //clear obj
            ClearWeapon(pos, _p1Index);
            ClearWeapon(pos, _p3Index);
            // clear data
            UpdateWeaponIdInPackage(pos, id);
            UpdateAttachmentType(pos, WeaponPartLocation.Buttstock, 0);
            UpdateAttachmentType(pos, WeaponPartLocation.Magazine, 0);
            UpdateAttachmentType(pos, WeaponPartLocation.Muzzle, 0);
            UpdateAttachmentType(pos, WeaponPartLocation.Scope, 0);
            UpdateAttachmentType(pos, WeaponPartLocation.LowRail, 0);
        }

        protected abstract AbstractLoadRequest CreateLoadRequest(AssetInfo assetInfo, ILoadedHandler loadedHandler);

        private void MountWeaponOnBackImpl(WeaponGameObjectData obj, GameObject character, WeaponInPackage pos)
        {
            if(null == obj) return;
            _mount.MountWeaponInPackage(obj.PrimaryAsGameObject, character, pos);
            _mount.MountWeaponInPackage(obj.DeputyAsGameObject, character, pos, false);
        }

        private void MountWeaponToHandImpl(WeaponInPackage pos)
        {
            MountWeaponToRightHandImpl(pos);
            MountWeaponToLeftHandImpl(pos);
        }

        private void MountWeaponToRightHandImpl(WeaponInPackage pos)
        {
            // 一三人称恢复背包武器状态，不区分是空手加载武器，还是替换武器
            RestoreWeaponInPackage();
            var index = GetIndexFromPos(pos);

            if (FirstPersonIncluded && _weapons[(int) pos, _p1Index].PrimaryAsGameObject != null && _weapons[(int) pos, _p1Index].Valid)
            {
                var weaponP1 = _weapons[(int) pos, _p1Index].PrimaryAsGameObject;
                _mount.MountRightHandWeapon(weaponP1, _characterP1);
                if (IsFirstPerson)
                {
                    EnableRender(_weapons[(int) pos, _p1Index]);
                    // Enable整把枪后，机瞄与镜同时显示，再判断一下
                    if (weaponP1 != null)
                        RefreshRemovableAttachment(weaponP1,
                            _attachments[(int) pos, _p1Index, (int) WeaponPartLocation.Scope].PrimaryAsGameObject != null);
                }

                _overrideP1.Change(_sex, _unique, _latestWeaponValue[index]);
            }

            if (ThirdPersonIncluded && _weapons[(int) pos, _p3Index].PrimaryAsGameObject != null && _weapons[(int) pos, _p3Index].Valid)
            {
                var weaponP3 = _weapons[(int) pos, _p3Index].PrimaryAsGameObject;
                if (!IsFirstPerson)
                    EnableRender(_weapons[(int) pos, _p3Index]);
                _mount.UnmountWeaponInPackage(_characterP3, pos);
                _mount.MountRightHandWeapon(weaponP3, _characterP3);
                _overrideP3.Change(_sex, _unique, _latestWeaponValue[index]);
            }

            _predictedWeaponValue[(int) PredictedWeaponStateIndex.WeaponInHand] = (int) pos;
            _predictedWeaponValue[(int) PredictedWeaponStateIndex.OverrideControllerState] = 
                (int) OverrideControllerState.WeaponInHand;
            
            if (null != _weaponChangedCallBack)
                _weaponChangedCallBack.Invoke(FirstPersonIncluded ? _weapons[(int) pos, _p1Index].PrimaryAsGameObject : null,
                    ThirdPersonIncluded ? _weapons[(int) pos, _p3Index].PrimaryAsGameObject : null);
            if (null != _cacheChangeAction)
                _cacheChangeAction.Invoke();
        }
        
        private void MountWeaponToLeftHandImpl(WeaponInPackage pos)
        {
            if (FirstPersonIncluded && _weapons[(int) pos, _p1Index].DeputyAsGameObject != null && _weapons[(int) pos, _p1Index].Valid)
                _mount.MountLeftHandWeapon(_weapons[(int) pos, _p1Index].DeputyAsGameObject, _characterP1);

            if (ThirdPersonIncluded && _weapons[(int) pos, _p3Index].DeputyAsGameObject != null && _weapons[(int) pos, _p3Index].Valid)
                _mount.MountLeftHandWeapon(_weapons[(int) pos, _p3Index].DeputyAsGameObject, _characterP3);
        }

        private void JustUnMountWeaponFromHandImpl(WeaponInPackage pos)
        {
            // 切换过渡状态机
            ChangeTransitionOverrideController();
            UnMountWeaponFromRightHandImpl(pos);
            UnMountWeaponFromLeftHandImpl(pos);
        }

        private void JustClearOverrideControllerImpl()
        {
            ClearOverrideController();
        }

        private void UnMountWeaponFromHandImpl(WeaponInPackage pos)
        {
            UnMountWeaponFromRightHandImpl(pos);
            UnMountWeaponFromLeftHandImpl(pos);
            ClearOverrideController();
        }

        private void UnMountWeaponFromRightHandImpl(WeaponInPackage pos)
        {
            if (FirstPersonIncluded)
            {
                _mount.UnmountRightHandWeapon(_characterP1);
                _mount.UnmountWeaponOnAlternativeLocator(_characterP1);
            }

            if (ThirdPersonIncluded)
            {
                _mount.UnmountRightHandWeapon(_characterP3);
                _mount.UnmountWeaponOnAlternativeLocator(_characterP3);
            }

            // 不去记载究竟是哪把武器从手上卸下
            RestoreWeaponInPackage();
            _predictedWeaponValue[(int) PredictedWeaponStateIndex.WeaponInHand] = (int) WeaponInPackage.EmptyHanded;
            if (null != _weaponChangedCallBack)
                _weaponChangedCallBack.Invoke(null, null);
            if (null != _cacheChangeAction)
                _cacheChangeAction.Invoke();
        }

        private void ChangeTransitionOverrideController()
        {
            if (ThirdPersonIncluded)
                _overrideP3.ChangeTransition(_sex, GetWeaponIdInHand());
        }

        // 恢复空手状态机
        private void ClearOverrideController()
        {
            if(FirstPersonIncluded)
                _overrideP1.Change(_sex, _unique, UniversalConsts.InvalidIntId);
            
            if (ThirdPersonIncluded)
                _overrideP3.Change(_sex, _unique, UniversalConsts.InvalidIntId);
            
            _predictedWeaponValue[(int) PredictedWeaponStateIndex.OverrideControllerState] = (int) OverrideControllerState.EmptyHanded;
        }

        private void UnMountWeaponFromLeftHandImpl(WeaponInPackage pos)
        {
            if (FirstPersonIncluded)
            {
                _mount.UnMountLeftHandWeapon(_characterP1);
            }

            if (ThirdPersonIncluded)
            {
                _mount.UnMountLeftHandWeapon(_characterP3);
            }
        }

        private void MountAttachmentImpl(WeaponInPackage pos, WeaponPartLocation location, int attachmentId)
        {
            if(location >= WeaponPartLocation.EndOfTheWorld)
            {
                return;
            }

            var weaponId = _latestWeaponValue[GetIndexFromPos(pos)];
            var addr = SingletonManager.Get<WeaponPartsConfigManager>().GetAsset(attachmentId);

            if ((null == addr.AssetName || addr.AssetName.Equals(String.Empty)) &&
                (null == addr.BundleName || addr.BundleName.Equals(String.Empty)))
            {
                Logger.ErrorFormat("ErrorAttachmentId Try To Mount  id:  {0}", attachmentId);
                return;
            }

            if (FirstPersonIncluded)
            {
                var p1Handler = _mountAttachmentHandlers[(int) pos, _p1Index, (int) location];
                p1Handler.SetInfo(weaponId, attachmentId);
                _loadRequestBatch.Add(CreateLoadRequest(addr, p1Handler));
            }

            if (ThirdPersonIncluded)
            {
                var p3Handler = _mountAttachmentHandlers[(int) pos, _p3Index, (int) location];
                p3Handler.SetInfo(weaponId, attachmentId);
                _loadRequestBatch.Add(CreateLoadRequest(addr, p3Handler));
            }

            UpdateAttachmentType(pos, location, attachmentId);
            if (null != _cacheChangeAction)
                _cacheChangeAction.Invoke();
        }

        private void UnmountAttachmentImpl(WeaponInPackage pos, WeaponPartLocation location, int type)
        {
            OverrideAttachment(pos, _p1Index, location, null);
            OverrideAttachment(pos, _p3Index, location, null);

            UpdateAttachmentType(pos, location, 0);
            if (null != _cacheChangeAction)
                _cacheChangeAction.Invoke();
        }

        private void MountP3WeaponOnAlternativeLocatorImpl()
        {
            _clientWeaponValue[(int)ClientWeaponStateIndex.AlternativeP3WeaponLocator] = 1;
            _mount.MountWeaponOnAlternativeLocator(GetWeaponP3ObjInHand(), _characterP3);
            if (null != _cacheChangeAction)
                _cacheChangeAction.Invoke();
        }

        private void RemountP3WeaponOnRightHandImpl()
        {
            _clientWeaponValue[(int)ClientWeaponStateIndex.AlternativeP3WeaponLocator] = 0;
            _mount.RemountWeaponOnRightHand(GetWeaponP3ObjInHand(), _characterP3);
            if (null != _cacheChangeAction)
                _cacheChangeAction.Invoke();
        }

        private void MountWeaponOnAlternativeLocatorImpl()
        {
            _clientWeaponValue[(int)ClientWeaponStateIndex.AlternativeWeaponLocator] = 1;
            RewindMountWeaponOnAlternativeLocator(_clientWeaponValue[(int)ClientWeaponStateIndex.AlternativeWeaponLocator]);
            if (null != _cacheChangeAction)
                _cacheChangeAction.Invoke();
        }

        private void RemountWeaponOnRightHandImpl()
        {
            _clientWeaponValue[(int)ClientWeaponStateIndex.AlternativeWeaponLocator] = 0;
            RewindMountWeaponOnAlternativeLocator(_clientWeaponValue[(int)ClientWeaponStateIndex.AlternativeWeaponLocator]);
            if (null != _cacheChangeAction)
                _cacheChangeAction.Invoke();
        }

        private void UpdateWeaponIdInPackage(WeaponInPackage pos, int id)
        {
            var index = GetIndexFromPos(pos);
            if (index >= 0 && index < _latestWeaponValue.Length)
                _latestWeaponValue[index] = id;
        }

        private void UpdateAttachmentType(WeaponInPackage pos, WeaponPartLocation location, int type)
        {
            if (WeaponInPackage.PrimaryWeaponOne != pos && 
                WeaponInPackage.PrimaryWeaponTwo != pos &&
                WeaponInPackage.SideArm != pos) return;
            
            var index = GetIndexFromPos(pos) + (int) location + 1;
            if (index < 0 || index >= _latestWeaponValue.Length) return;
            _latestWeaponValue[index] = type;
        }

        private void InitWeaponInPackage()
        {
            for (var i = 0; i < (int)WeaponInPackage.EndOfTheWorld; ++i)
            {
                if(_initWeaponInPackage[i] >= 0)
                    MountWeaponInPackageImpl((WeaponInPackage)i, _initWeaponInPackage[i]);

                for (var j = 0; j < (int)WeaponPartLocation.EndOfTheWorld; ++j)
                {
                    if(_initAttachments[i, j] >= 0)
                        MountAttachmentImpl((WeaponInPackage)i, (WeaponPartLocation)j, _initAttachments[i, j]);
                }
            }
        }

        private void MountInitWeaponInPackage(WeaponInPackage pos, int id)
        {
            if((int)pos >= _initWeaponInPackage.Length) return;
            _initWeaponInPackage[(int) pos] = id;
        }

        private void UnMountInitWeaponInPackage(WeaponInPackage pos)
        {
            if((int)pos >= _initWeaponInPackage.Length) return;
            _initWeaponInPackage[(int) pos] = -1;
        }

        private void MountInitAttachmentInWeapon(WeaponInPackage pos, WeaponPartLocation location, int id)
        {
            if((int)pos >= _initAttachments.GetLength(0) ||
               (int)location >= _initAttachments.GetLength(1)) return;
            _initAttachments[(int) pos, (int) location] = id;
        }
        
        private void UnMountInitAttachmentInWeapon(WeaponInPackage pos, WeaponPartLocation location)
        {
            if((int)pos >= _initAttachments.GetLength(0) ||
               (int)location >= _initAttachments.GetLength(1)) return;
            _initAttachments[(int) pos, (int) location] = -1;
        }

        #region reload

        private void ChangeReloadState(int state)
        {
            _predictedWeaponValue[(int)PredictedWeaponStateIndex.ReloadState] = state;
            RewindReloadState(_predictedWeaponValue[(int)PredictedWeaponStateIndex.ReloadState]);
        }

        private void StartReloadImpl()
        {
            var attachmentObj = GetP3CurrentAttachmentByType((int) WeaponPartLocation.Magazine);
            BoundAttachmentToHand(attachmentObj);
            ShowOrHideAttachment(attachmentObj);
        }

        private void DropMagazineImpl()
        {
            var attachmentObj = GetP3CurrentAttachmentByType((int) WeaponPartLocation.Magazine);
            if (null == attachmentObj) return;
            attachmentObj.AddComponent<Rigidbody>();
            attachmentObj.transform.SetParent(null, false);
            ShowOrHideAttachment(attachmentObj);
        }

        private void AddMagazineImpl()
        {
            var attachmentObj = GetP3CurrentAttachmentByType((int) WeaponPartLocation.Magazine);
            RemoveRigidbody(attachmentObj);
            BoundAttachmentToHand(attachmentObj);
            ShowOrHideAttachment(attachmentObj);
        }

        private void EndReloadImpl()
        {
            var weaponObj = GetWeaponP3ObjInHand();
            var attachmentObj = GetP3CurrentAttachmentByType((int) WeaponPartLocation.Magazine);
            RemoveRigidbody(attachmentObj);
            _mount.MountWeaponAttachment(attachmentObj, weaponObj, WeaponPartLocation.Magazine);
            ShowOrHideAttachment(attachmentObj);
        }

        private void RemoveRigidbody(GameObject obj)
        {
            if (null == obj) return;
            var rigidbody = obj.GetComponent<Rigidbody>();
            if (null != rigidbody)
                GameObject.Destroy(rigidbody);
        }

        private void ShowOrHideAttachment(GameObject obj)
        {
            if (IsFirstPerson) // 当前一人称隐藏
                AppearanceUtils.DisableRender(obj);
            else
                AppearanceUtils.EnableRender(obj);
        }

        private void BoundAttachmentToHand(GameObject attachmentObj)
        {
            var leftHand = BoneMount.FindChildBoneFromCache(_characterP3, BoneName.CharLeftHand);
            if (null == attachmentObj || null == leftHand) return;
            attachmentObj.transform.SetParent(leftHand, false);
            // 美术提供初始位移旋转
            attachmentObj.transform.localPosition = new Vector3(-0.0385f, 0.0287f, 0);
            attachmentObj.transform.localEulerAngles = new Vector3(-180, 90, -90);
        }

        #endregion

        private void RemountWeaponDueToBag()
        {
            if (ThirdPersonIncluded)
            {
                if ((WeaponInPackage) _predictedWeaponValue[(int)PredictedWeaponStateIndex.WeaponInHand] != WeaponInPackage.PrimaryWeaponOne)
                {
                    _mount.MountWeaponInPackage(_weapons[(int) WeaponInPackage.PrimaryWeaponOne, _p3Index].PrimaryAsGameObject,
                        _characterP3,
                        WeaponInPackage.PrimaryWeaponOne);
                    if (!IsFirstPerson)
                    {
                        AppearanceUtils.ActiveGameobject(_weapons[(int) WeaponInPackage.PrimaryWeaponOne, _p3Index]
                            .PrimaryAsGameObject);
                    }
                }

                if ((WeaponInPackage) _predictedWeaponValue[(int)PredictedWeaponStateIndex.WeaponInHand] != WeaponInPackage.PrimaryWeaponTwo)
                {
                    _mount.MountWeaponInPackage(_weapons[(int) WeaponInPackage.PrimaryWeaponTwo, _p3Index].PrimaryAsGameObject,
                        _characterP3,
                        WeaponInPackage.PrimaryWeaponTwo);
                    if (!IsFirstPerson)
                    {
                        AppearanceUtils.ActiveGameobject(_weapons[(int) WeaponInPackage.PrimaryWeaponTwo, _p3Index]
                            .PrimaryAsGameObject);
                    }
                }
            }

            if (null != _cacheChangeAction)
                _cacheChangeAction.Invoke();
        }

        private void RestoreWeaponInPackage()
        {
            if (ThirdPersonIncluded)
            {
                // 不记录5个武器P3的挂载状态
                for (var i = 0; i < (int) WeaponInPackage.EndOfTheWorld; ++i)
                {
                    if (i >= _weapons.Length || !_weapons[i, _p3Index].Valid) continue;
                    MountWeaponOnBackImpl(_weapons[i, _p3Index], _characterP3, (WeaponInPackage)i);
                }
            }

            // 背包状态的P1武器的处理策略就是隐藏
            DisableWeapon(ValidP1Index);
            if (null != _cacheChangeAction)
                _cacheChangeAction.Invoke();
        }

        private void ClearCacheData(WeaponInPackage pos, int index)
        {
            ClearWeaponData(pos, index);
            ClearAttachmentData(pos, index);
        }

        private void ClearWeaponData(WeaponInPackage pos, int index)
        {
            if (index < 0) return;
            _weapons[(int) pos, index].Valid = false;
        }

        private void ClearAttachmentData(WeaponInPackage pos, int index)
        {
            if (index < 0) return;
            _attachments[(int) pos, index, (int) WeaponPartLocation.Muzzle].Valid = false;
            _attachments[(int) pos, index, (int) WeaponPartLocation.LowRail].Valid = false;
            _attachments[(int) pos, index, (int) WeaponPartLocation.Magazine].Valid = false;
            _attachments[(int) pos, index, (int) WeaponPartLocation.Buttstock].Valid = false;
            _attachments[(int) pos, index, (int) WeaponPartLocation.Scope].Valid = false;
        }

        // 清除attachment数据
        private void ClearWeapon(WeaponInPackage pos, int index)
        {
            if(index < 0) return;

            if (_weapons[(int) pos, index].PrimaryAsGameObject != null)
            {
                // 武器从背包中移除
                OverrideAttachment(pos, index, WeaponPartLocation.Muzzle, null);
                OverrideAttachment(pos, index, WeaponPartLocation.LowRail, null);
                OverrideAttachment(pos, index, WeaponPartLocation.Magazine, null);
                OverrideAttachment(pos, index, WeaponPartLocation.Buttstock, null);
                OverrideAttachment(pos, index, WeaponPartLocation.Scope, null);
                OverrideWeapon(pos, index, null);
            }
        }

        private void ClearInvaildAttachment(WeaponInPackage pos, int index)
        {
            if (index < 0) return;
            for (var i = 0; i < (int) WeaponPartLocation.EndOfTheWorld; ++i)
            {
                if (_attachments[(int) pos, index, i].Valid) continue;
                OverrideAttachment(pos, index, (WeaponPartLocation) i, null);
            }
        }

        private void OverrideWeapon(WeaponInPackage pos, int index, UnityObject unityObj)
        {
            if (index < 0) return;
            if (_weapons[(int) pos, index].PrimaryAsGameObject != null)
            {
                DisableRender(_weapons[(int) pos, index]);
                EnableWeaponEffect(_weapons[(int) pos, index], true);
                ResetWeaponShader(_weapons[(int) pos, index], index);
                AddRecycleObject(_weapons[(int) pos, index].GetRecycleUnityObject());
            }

            _weapons[(int) pos, index].Obj = unityObj;
            ChangeWeaponShader(_weapons[(int) pos, index], index);

            // 枪和配件一起装备时，可能出现枪后于配件加载
            if (null == unityObj || unityObj.AsGameObject == null) return;
            _mount.MountWeaponAttachment(_attachments[(int) pos, index, (int) WeaponPartLocation.Muzzle].PrimaryAsGameObject, unityObj,
                WeaponPartLocation.Muzzle);
            _mount.MountWeaponAttachment(_attachments[(int) pos, index, (int) WeaponPartLocation.LowRail].PrimaryAsGameObject, unityObj,
                WeaponPartLocation.LowRail);
            _mount.MountWeaponAttachment(_attachments[(int) pos, index, (int) WeaponPartLocation.Magazine].PrimaryAsGameObject, unityObj,
                WeaponPartLocation.Magazine);
            _mount.MountWeaponAttachment(_attachments[(int) pos, index, (int) WeaponPartLocation.Buttstock].PrimaryAsGameObject, unityObj,
                WeaponPartLocation.Buttstock);
            _mount.MountWeaponAttachment(_attachments[(int) pos, index, (int) WeaponPartLocation.Scope].PrimaryAsGameObject, unityObj,
                WeaponPartLocation.Scope);

            // 新增枪械时，OverrideWeapon后有Enable/Disable，卸下枪械无需处理
            RefreshRemovableAttachment(unityObj, _attachments[(int) pos, index, (int) WeaponPartLocation.Scope].PrimaryAsGameObject != null);
        }

        private void OverrideAttachment(WeaponInPackage pos, int index, WeaponPartLocation location,
            UnityObject unityObj)
        {
            if (index >= 0 && location < WeaponPartLocation.EndOfTheWorld)
            {
                var attachment = _attachments[(int) pos, index, (int) location].PrimaryAsGameObject;
                if (attachment != null)
                {
                    if (index == _p1Index)
                    {
                        foreach (var render in attachment.GetComponentsInChildren<Renderer>())
                        {
                            render.gameObject.layer = UnityLayerManager.GetLayerIndex(EUnityLayerName.Default);
                        }
                    }

                    DisableRender(_attachments[(int) pos, index, (int) location]);
                    AddRecycleObject(_attachments[(int) pos, index, (int) location].GetRecycleUnityObject());
                    _attachments[(int) pos, index, (int) location].Valid = false;
                    attachment.transform.SetParent(null, false);
                }

                _attachments[(int) pos, index, (int) location].Obj = unityObj;
                ChangeWeaponShader(_attachments[(int) pos, index, (int) location], index);

                if (unityObj != null && _weapons[(int) pos, index].Valid)
                {
                    _mount.MountWeaponAttachment(unityObj, _weapons[(int) pos, index].PrimaryAsGameObject, location);
                }

                if (pos == (WeaponInPackage) _predictedWeaponValue[(int)PredictedWeaponStateIndex.WeaponInHand])
                {
                    if (null != _weaponChangedCallBack)
                        _weaponChangedCallBack.Invoke(FirstPersonIncluded ? _weapons[(int) pos, _p1Index].PrimaryAsGameObject : null,
                            ThirdPersonIncluded ? _weapons[(int) pos, _p3Index].PrimaryAsGameObject : null);
                }

                // 一人称武器只操作 当前手上武器 (与三人称不同，一人称背上武器隐藏)
                if ((!IsFirstPerson || (IsFirstPerson && pos == (WeaponInPackage) _predictedWeaponValue[(int)PredictedWeaponStateIndex.WeaponInHand])) &&
                    _weapons[(int) pos, index].PrimaryAsGameObject != null &&
                    location == WeaponPartLocation.Scope &&
                    IsFirstPerson == (index == _p1Index))
                    RefreshRemovableAttachment(_weapons[(int) pos, index].PrimaryAsGameObject, unityObj != null);

                if (null != _cacheChangeAction)
                    _cacheChangeAction.Invoke();
            }
        }

        private void DisableWeapon(int personIndex)
        {
            for (var i = 0; i < (int) WeaponInPackage.EndOfTheWorld; ++i)
            {
                DisableRender(_weapons[i, personIndex]);
            }
        }

        private void EnableWeapon(int personIndex, bool onlyShowWeaponInHand)
        {
            for (var i = 0; i < (int) WeaponInPackage.EndOfTheWorld; ++i)
            {
                if (!onlyShowWeaponInHand || i == _predictedWeaponValue[(int)PredictedWeaponStateIndex.WeaponInHand])
                {
                    EnableRender(_weapons[i, personIndex]);
                    RefreshRemovableAttachment(_weapons[i, personIndex].PrimaryAsGameObject,
                        _attachments[i, personIndex, (int) WeaponPartLocation.Scope].PrimaryAsGameObject != null);
                }
            }
        }
        
        private void EnableRender(WeaponGameObjectData obj)
        {
            if(null == obj) return;
            EnableWeaponEffect(obj, true);
            AppearanceUtils.EnableRender(obj.PrimaryAsGameObject);
            AppearanceUtils.EnableRender(obj.DeputyAsGameObject);
        }

        private void DisableRender(WeaponGameObjectData obj)
        {
            if(null == obj) return;
            EnableWeaponEffect(obj, false);
            AppearanceUtils.DisableRender(obj.PrimaryAsGameObject);
            AppearanceUtils.DisableRender(obj.DeputyAsGameObject);
        }

        private void DisableShadow(WeaponGameObjectData obj)
        {
            if(null == obj) return;
            AppearanceUtils.DisableShadow(obj.PrimaryAsGameObject);
            AppearanceUtils.DisableShadow(obj.DeputyAsGameObject);
        }
        
        private void EnableWeaponEffect(WeaponGameObjectData obj, bool enable)
        {
            if(null == obj) return;
            
            var location = _mount.GetLocation(obj.PrimaryAsGameObject, SpecialLocation.EffectLocation, true);
            if(null != location)
                location.gameObject.SetActive(enable);
            
            location = _mount.GetLocation(obj.DeputyAsGameObject, SpecialLocation.EffectLocation, true);
            if(null != location)
                location.gameObject.SetActive(enable);
        }

        private void ChangeWeaponShader(WeaponGameObjectData obj, int index)
        {
            if (null == obj || index != _p1Index || !FirstPersonIncluded) return;
            ReplaceMaterialShaderBase.ChangeShader(obj.PrimaryAsGameObject);
            ReplaceMaterialShaderBase.ChangeShader(obj.DeputyAsGameObject);
        }

        private void ResetWeaponShader(WeaponGameObjectData obj, int index)
        {
            if (null == obj || index != _p1Index || !FirstPersonIncluded) return;
            ReplaceMaterialShaderBase.ResetShader(obj.PrimaryAsGameObject);
            ReplaceMaterialShaderBase.ResetShader(obj.DeputyAsGameObject);
        }

        private void AddRecycleObject(UnityObject obj)
        {
            if (obj != null)
            {
                _recycleRequestBatch.Add(obj);
            }
        }

        private Command GetAvailableCommand()
        {
            if (_currentCommandIndex >= _outerCommand.Count)
            {
                _outerCommand.Add(new Command());
            }

            return _outerCommand[_currentCommandIndex++];
        }

        private bool IsFirstPerson
        {
            get { return _view == CharacterView.FirstPerson; }
        }

        private static int GetIndexFromPos(WeaponInPackage pos)
        {
            switch (pos)
            {
                case WeaponInPackage.EmptyHanded:
                    return (int)LatestWeaponStateIndex.EmptyHand;
                case WeaponInPackage.PrimaryWeaponOne:
                    return (int) LatestWeaponStateIndex.PrimaryWeaponOne;
                case WeaponInPackage.PrimaryWeaponTwo:
                    return (int) LatestWeaponStateIndex.PrimaryWeaponTwo;
                case WeaponInPackage.SideArm:
                    return (int) LatestWeaponStateIndex.SideArm;
                case WeaponInPackage.MeleeWeapon:
                    return (int) LatestWeaponStateIndex.MeleeWeapon;
                case WeaponInPackage.ThrownWeapon:
                    return (int) LatestWeaponStateIndex.ThrownWeapon;
                case WeaponInPackage.TacticWeapon:
                    return (int) LatestWeaponStateIndex.TacticWeapon;
            }

            throw new System.Exception(string.Format("Unexpected Exception: {0}", pos));
        }

        private bool FirstPersonIncluded
        {
            get { return _p1Index == ValidP1Index; }
        }

        private bool ThirdPersonIncluded
        {
            get { return _p3Index == ValidP3Index; }
        }

        // 仅限于瞄镜的变化
        private void RefreshRemovableAttachment(GameObject go, bool hasSights)
        {
            var ironSights = BoneMount.FindChildBoneFromCache(go, BoneName.RemovableIronSights);
            var rail = BoneMount.FindChildBoneFromCache(go, BoneName.RemovableRail);

            if (ironSights != null)
            {
                if (hasSights)
                    AppearanceUtils.DisableRender(ironSights.gameObject);
                else
                    AppearanceUtils.EnableRender(ironSights.gameObject);
            }

            if (rail != null)
            {
                if (hasSights)
                    AppearanceUtils.EnableRender(rail.gameObject);
                else
                    AppearanceUtils.DisableRender(rail.gameObject);
            }
        }

        #region ICharacterLoadResource

        private List<AbstractLoadRequest> _loadRequestBatch = new List<AbstractLoadRequest>();
        private List<UnityObject> _recycleRequestBatch = new List<UnityObject>();

        public List<AbstractLoadRequest> GetLoadRequests()
        {
            return _loadRequestBatch;
        }

        public List<UnityObject> GetRecycleRequests()
        {
            return _recycleRequestBatch;
        }

        public void ClearRequests()
        {
            _loadRequestBatch.Clear();
            _recycleRequestBatch.Clear();
        }

        #endregion

        class MountWeaponHandler : ILoadedHandler
        {
            private readonly WeaponControllerBase _dataSource;
            private readonly WeaponInPackage _pos;
            private readonly int _index;

            private int _id;

            public MountWeaponHandler(WeaponControllerBase dataSource, WeaponInPackage pos, int index)
            {
                _dataSource = dataSource;
                _pos = pos;
                _index = index;
            }

            public void SetInfo(int id)
            {
                _id = id;
            }

            private void SetObjParam(GameObject obj, int layer)
            {
                if(null == obj) return;
                BoneTool.CacheTransform(obj);
                var childCount = obj.transform.childCount;
                for (var i = 0; i < childCount; ++i)
                {
                    obj.transform.GetChild(i).gameObject.layer = layer;
                }
                
                AppearanceUtils.EnableShadow(obj);
                ReplaceMaterialShaderBase.ResetShader(obj);
            }

            public void OnLoadSucc<T>(T source, UnityObject unityObj)
            {
                Logger.InfoFormat("Load Weapon: {0}", unityObj.Address);

                SetObjParam(unityObj, UnityLayerManager.GetLayerIndex(EUnityLayerName.Player));

                if (_id < 0)
                {
                    _dataSource.AddRecycleObject(unityObj);
                    return;
                }

                if (_index == _dataSource._p1Index)
                {
                    OnLoadP1(unityObj);
                    _id = UniversalConsts.InvalidIntId;
                }

                if (_index == _dataSource._p3Index)
                {
                    OnLoadP3(unityObj);
                    _id = UniversalConsts.InvalidIntId;
                }
            }

            private void OnLoadP1(UnityObject unityObj)
            {
                var expectedAddr = SingletonManager.Get<WeaponAvatarConfigManager>().GetFirstPersonWeaponModel(_id);
                
                if (expectedAddr.Equals(unityObj.Address))
                {
                    _dataSource.ClearInvaildAttachment(_pos, _dataSource._p1Index);
                    _dataSource.OverrideWeapon(_pos, _dataSource._p1Index, unityObj);
                    
                    var weapon = _dataSource._weapons[(int) _pos, _dataSource._p1Index];

                    _dataSource.DisableRender(weapon);
                    _dataSource.DisableShadow(weapon);

                    if (_dataSource._predictedWeaponValue[(int)PredictedWeaponStateIndex.WeaponInHand] == (int) _pos)
                    {
                        _dataSource.MountWeaponToHand(_pos);
                    }
                }
                else
                {
                    _dataSource.AddRecycleObject(unityObj);
                }
            }

            private void OnLoadP3(UnityObject unityObj)
            {
                var expectedAddr = SingletonManager.Get<WeaponAvatarConfigManager>().GetThirdPersonWeaponModel(_id);
                
                if (expectedAddr.Equals(unityObj.Address))
                {
                    _dataSource.ClearInvaildAttachment(_pos, _dataSource._p3Index);
                    _dataSource.OverrideWeapon(_pos, _dataSource._p3Index, unityObj);

                    var weapon = _dataSource._weapons[(int) _pos, _dataSource._p3Index];
                    
                    _dataSource.DisableRender(weapon);

                    if (_dataSource._predictedWeaponValue[(int)PredictedWeaponStateIndex.WeaponInHand] == (int) _pos)
                    {
                        _dataSource.MountWeaponToHandImpl(_pos);
                    }
                    else
                    {
                        if (!_dataSource.IsFirstPerson)
                            _dataSource.EnableRender(weapon);
                        _dataSource.MountWeaponOnBackImpl(weapon, _dataSource._characterP3, _pos);
                    }
                }
                else
                {
                    _dataSource.AddRecycleObject(unityObj);
                }
            }
        }

        class MountAttachmentHandler : ILoadedHandler
        {
            private readonly WeaponControllerBase _dataSource;

            private int _weaponId;
            private int _attachmentId;
            private WeaponInPackage _pos;
            private WeaponPartLocation _location;
            private int _index;

            public MountAttachmentHandler(
                WeaponControllerBase dataSource,
                int index,
                WeaponInPackage pos,
                WeaponPartLocation location)
            {
                _dataSource = dataSource;
                _index = index;
                _pos = pos;
                _location = location;
            }

            public void SetInfo(int weaponId, int attachmentId)
            {
                _weaponId = weaponId;
                _attachmentId = attachmentId;
            }

            private void SetObjParams(GameObject obj, int layer)
            {
                if(null == obj) return;
                
                BoneTool.CacheTransform(obj);
                
                var childCount = obj.transform.childCount;
                for (var i = 0; i < childCount; ++i)
                {
                    obj.transform.GetChild(i).gameObject.layer = layer;
                }
                
                AppearanceUtils.EnableShadow(obj);
                ReplaceMaterialShaderBase.ResetShader(obj);
            }

            public void OnLoadSucc<T>(T source, UnityObject unityObj)
            {
                if(null == unityObj || null == unityObj.AsGameObject) return;
                
                SetObjParams(unityObj, UnityLayerManager.GetLayerIndex(EUnityLayerName.Player));

                var addr = SingletonManager.Get<WeaponPartsConfigManager>().GetAsset(_attachmentId);
                if (!addr.Equals(unityObj.Address))
                {
                    _dataSource.AddRecycleObject(unityObj);
                    return;
                }

                if (_weaponId == _dataSource._latestWeaponValue[GetIndexFromPos(_pos)])
                {
                    _dataSource.OverrideAttachment(_pos, _index, _location, unityObj);
                    // 只加载第一人称 -> IsFirstPerson为true, index == _dataSource.P1Index为true
                    // 只加载第三人称 -> IsFirstPerson为false, index == _dataSource.P1Index为false
                    bool mustHidden = _dataSource.IsFirstPerson != (_index == _dataSource._p1Index);

                    var attachment = _dataSource._attachments[(int) _pos, _index, (int) _location];

                    if (mustHidden)
                    {
                        _dataSource.DisableRender(attachment);
                    }
                    else
                    {
                        if (_dataSource.IsFirstPerson)
                        {
                            if (_pos == (WeaponInPackage) _dataSource._predictedWeaponValue[(int)PredictedWeaponStateIndex.WeaponInHand])
                                _dataSource.EnableRender(attachment);
                            else
                                _dataSource.DisableRender(attachment);
                        }
                        else
                            _dataSource.EnableRender(attachment);
                    }

                    if (_index == _dataSource._p1Index)
                    {
                        _dataSource.DisableShadow(attachment);
                    }
                }
                else
                {
                    _dataSource.AddRecycleObject(unityObj);
                }
            }
        }
    }
}
