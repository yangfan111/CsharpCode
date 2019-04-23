using Core.Utils;
using Shared.Scripts;
using System;
using System.Collections.Generic;
using UnityEngine;
using Utils.AssetManager;
using Utils.CharacterState;
using Utils.Compare;
using Utils.Configuration;
using Utils.Singleton;
using Utils.Utils;

namespace Utils.Appearance.WardrobePackage
{

    public abstract class WardrobeControllerBase
    {
        private class Command
        {
            private Wardrobe _pos;
            private int _id;
            private Action _changeView;
            private Action<int> _dress;
            private Action<Wardrobe> _undress;

            public void SetDress(Action<int> action, int id)
            {
                _dress = action;
                _id = id;
            }

            public void SetUndress(Action<Wardrobe> action, Wardrobe pos)
            {
                _undress = action;
                _pos = pos;
            }

            public void Execute()
            {
                if (null != _dress)
                {
                    _dress.Invoke(_id);
                    _dress = null;
                }

                if (null != _undress)
                {
                    _undress.Invoke(_pos);
                    _undress = null;
                }

                if (null != _changeView)
                {
                    _changeView.Invoke();
                    _changeView = null;
                }
            }
        }

        #region ILatestWardrobeState

        private int[] _latestWardrobeValue = new int[(int) Wardrobe.EndOfTheWorld];
        private bool[] _latestWardrobeRewindFlag = new bool[(int) Wardrobe.EndOfTheWorld];
        private bool _rewindFlag;

        protected int GetLatestWardrobeValue(Wardrobe type)
        {
            return _latestWardrobeValue[(int) type];
        }

        protected void SetLatestWardrobeValue(Wardrobe type, int value)
        {
            var index = (int) type;
            _rewindFlag = (_latestWardrobeRewindFlag[index] = _latestWardrobeRewindFlag[index] ||
                                                              !CompareUtility.IsApproximatelyEqual(_latestWardrobeValue[index], value)) || _rewindFlag;
            _latestWardrobeValue[index] = value;
        }

        #endregion

        private LoggerAdapter Logger = new LoggerAdapter(typeof(WardrobeControllerBase));

        private const int ValidP1Index = 0;
        private const int ValidP3Index = 1;

        private int _p1Index = -1;
        private int _p3Index = -1;

        private MountWardrobeHandler[,] _mountWardrobeHandlers =
            new MountWardrobeHandler[(int) Wardrobe.EndOfTheWorld, 2];

        private readonly List<Command> _outerCommand = new List<Command>();
        private int _currentCommandIndex;

        private GameObject _characterP3;
        private GameObject _characterP1;
        private Animator _animatorP3;
        private Animator _animatorP1;
        private Sex _sex = Sex.Female;
        private CharacterView _view = CharacterView.ThirdPerson;

        private readonly Dictionary<int, Action<int>> _rewindLatestWardrobe;
        private CharacterAvatar _p3Avatar;
        private CharacterAvatar _p1Avatar;
        public bool P3HaveInit = false;

        private Action _bagChanged;

        private int[] _defaultAvatars = new int[(int)Wardrobe.EndOfTheWorld];
        private int[] _initAvatars = new int[(int)Wardrobe.EndOfTheWorld];

        private List<int> _defaultModelParts;
        public List<int> DefaultModelParts
        {
            set
            {
                _defaultModelParts = value;
                for (var i = 0; i < (int)Wardrobe.EndOfTheWorld; ++i)
                {
                    _defaultAvatars[i] = UniversalConsts.InvalidIntId;
                    _initAvatars[i] = UniversalConsts.InvalidIntId;
                    P3HaveInit = false;
                }
            }
            get { return _defaultModelParts; }
        }

        public WardrobeControllerBase(Action bagChanged)
        {
            _bagChanged = bagChanged;
            for (int i = 0; i < _mountWardrobeHandlers.GetLength(0); i++)
            {
                _mountWardrobeHandlers[i, ValidP1Index] = new MountWardrobeHandler(this, ValidP1Index);
                _mountWardrobeHandlers[i, ValidP3Index] = new MountWardrobeHandler(this, ValidP3Index);
            }

            _rewindLatestWardrobe = new Dictionary<int, Action<int>>
            {
                {(int) Wardrobe.Cap, x => RewindWardrobeInBody(x, Wardrobe.Cap)},
                {(int) Wardrobe.PendantFace, x => RewindWardrobeInBody(x, Wardrobe.PendantFace)},
                {(int) Wardrobe.Inner, x => RewindWardrobeInBody(x, Wardrobe.Inner)},
                {(int) Wardrobe.Armor, x => RewindWardrobeInBody(x, Wardrobe.Armor)},
                {(int) Wardrobe.Outer, x => RewindWardrobeInBody(x, Wardrobe.Outer)},
                {(int) Wardrobe.Glove, x => RewindWardrobeInBody(x, Wardrobe.Glove)},
                {(int) Wardrobe.Waist, x => RewindWardrobeInBody(x, Wardrobe.Waist)},
                {(int) Wardrobe.Trouser, x => RewindWardrobeInBody(x, Wardrobe.Trouser)},
                {(int) Wardrobe.Foot, x => RewindWardrobeInBody(x, Wardrobe.Foot)},
                {(int) Wardrobe.Bag, x => RewindWardrobeInBody(x, Wardrobe.Bag)},
                {(int) Wardrobe.Entirety, x => RewindWardrobeInBody(x, Wardrobe.Entirety)},
                {(int) Wardrobe.CharacterHair, x => RewindWardrobeInBody(x, Wardrobe.CharacterHair)},
                {(int) Wardrobe.CharacterHairContainer, x => RewindWardrobeHairColor(x, Wardrobe.CharacterHairContainer)},
                {(int) Wardrobe.CharacterHead, x => RewindWardrobeInBody(x, Wardrobe.CharacterHead)},
                {(int) Wardrobe.CharacterGlove, x => RewindWardrobeInBody(x, Wardrobe.CharacterGlove)},
                {(int) Wardrobe.CharacterInner, x => RewindWardrobeInBody(x, Wardrobe.CharacterInner)},
                {(int) Wardrobe.CharacterTrouser, x => RewindWardrobeInBody(x, Wardrobe.CharacterTrouser)},
                {(int) Wardrobe.CharacterFoot, x => RewindWardrobeInBody(x, Wardrobe.CharacterFoot)}
            };

            for (var i = 0; i < (int)Wardrobe.EndOfTheWorld; ++i)
            {
                _latestWardrobeValue[i] = UniversalConsts.InvalidIntId;
                _defaultAvatars[i] = UniversalConsts.InvalidIntId;
                _initAvatars[i] = UniversalConsts.InvalidIntId;
            }
        }

        public void CommonReset()
        {
            for (var i = 0; i < (int) Wardrobe.EndOfTheWorld; ++i)
                UndressImpl((Wardrobe)i);
            TryRewind();

            for (var i = 0; i < (int)Wardrobe.EndOfTheWorld; ++i)
                _latestWardrobeRewindFlag[i] = false;
            _rewindFlag = false;
            _outerCommand.Clear();
            _currentCommandIndex = 0;
        }

        public void ClearThirdPersonCharacter()
        {
            for (var i = 0; i < (int) Wardrobe.EndOfTheWorld; ++i)
            {
                _defaultAvatars[i] = UniversalConsts.InvalidIntId;
            }
            CommonReset();
            GetRecycleRequests();

            _p3Avatar = null;
            _p3Index = -1;
            _characterP3 = null;
        }
        
        #region Sync 

        public void TryRewind()
        {
            if (_rewindFlag)
            {
                _rewindFlag = false;
                RewindIntValue(_latestWardrobeRewindFlag, _latestWardrobeValue, _rewindLatestWardrobe);
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

        public void Dress(int id)
        {
            if (P3HaveInit)
            {
                var cmd = GetAvailableCommand();
                cmd.SetDress(DressImpl, id);
            }
            else
            {
                DressInitAvatar(id);
            }
        }

        public void Undress(Wardrobe pos)
        {
            if (P3HaveInit)
            {
                var cmd = GetAvailableCommand();
                cmd.SetUndress(UndressImpl, pos);
            }
            else
            {
                UndressInitAvatar(pos);
            }
        }

        public void SetP1WardrobeTopLayerShader()
        {
            if (FirstPersonIncluded)
                _p1Avatar.SetWardrobeTopLayerShader();
        }

        public void ResetP1WardrobeShader()
        {
            if(FirstPersonIncluded)
                _p1Avatar.ResetWardrobeShader();
        }

        public void Update()
        {
            if (FirstPersonIncluded) _p1Avatar.Update();
            if (ThirdPersonIncluded) _p3Avatar.Update();
        }

        #endregion

        #region Initial Setting

        public void SetThirdPersonCharacter(GameObject obj)
        {
            _characterP3 = obj;
            _p3Index = ValidP3Index;
            P3HaveInit = true;

            _p3Avatar = new CharacterAvatar(_characterP3);
            _p3Avatar.SetBagChangedDelegate(_bagChanged);
            _p3Avatar.Show();

            InitWardrobe(DefaultModelParts);
        }

        public void SetFirstPersonCharacter(GameObject obj)
        {
            _characterP1 = obj;
            _p1Index = ValidP1Index;

            _p1Avatar = new CharacterAvatar(_characterP1);
            _p1Avatar.Hide();
        }

        public void SetAnimatorP3(Animator animator)
        {
            _animatorP3 = animator;
        }

        public void SetAnimatorP1(Animator animator)
        {
            _animatorP1 = animator;
        }

        #endregion

        #region Outer State Setter

        public void SetFirstPerson()
        {
            _view = CharacterView.FirstPerson;
            if (ThirdPersonIncluded) _p3Avatar.Hide();
            if (FirstPersonIncluded) _p1Avatar.Show();
            //Logger.DebugFormat("ChangeView:   {0}", "SetFirstPerson");
        }

        public void SetThirdPerson()
        {
            _view = CharacterView.ThirdPerson;
            if (ThirdPersonIncluded) _p3Avatar.Show();
            if (FirstPersonIncluded) _p1Avatar.Hide();
            //Logger.DebugFormat("ChangeView:   {0}", "SetThirdPerson");
        }

        public void Execute()
        {
            for (var i = 0; i < _currentCommandIndex; i++)
            {
                _outerCommand[i].Execute();
            }

            _currentCommandIndex = 0;
        }

        #endregion

        #region changeAvatar

        //穿
        private void DressImpl(int id)
        {
            Wardrobe pos = SingletonManager.Get<AvatarAssetConfigManager>().GetAvatarType(id);
            SetLatestWardrobeValue(pos, id);
            Logger.InfoFormat("Dress {0}", id);
        }

        //脱
        private void UndressImpl(Wardrobe pos)
        {
            var defaultId = -1;

            if ((int) pos < _defaultAvatars.Length)
                defaultId = _defaultAvatars[(int) pos];
            
            SetLatestWardrobeValue(pos, defaultId);
            Logger.InfoFormat("Undress {0}", pos);
        }

        private void RewindWardrobeHairColor(int id, Wardrobe pos)
        {
            var hairHandle = _mountWardrobeHandlers[(int) Wardrobe.CharacterHair, ValidP3Index];

            if (hairHandle == null)
            {
                return;
            }
            
            if (id < 0)
            {
                hairHandle.UnRegister();
                //Logger.InfoFormat("unregister id:{0}, pos:{1}", id, pos);
            }
            
            if (id > 0)
            {
                
                var assetData = SingletonManager.Get<AvatarAssetConfigManager>().GetAvatarAssetItemById(id);
                if (assetData == null) return;
                if (ThirdPersonIncluded)
                {
                    
                    hairHandle.Register(delegate(WardrobeParam param)
                    {
                        param.ApplySkin(assetData.Id);
                    });
                    _p3Avatar.UpdateAvatarSkin(Wardrobe.CharacterHair, assetData.Id);
                }
            }
        }

        private void RewindWardrobeInBody(int id, Wardrobe pos)
        {
            if (id < 0) //脱
            {
                if (FirstPersonIncluded)
                {
                    _p1Avatar.RemoveWardrobe(pos);
                    _mountWardrobeHandlers[(int) pos, ValidP1Index].SetInfo(null);
                    UpdateRenderBelowAnimatorP1();
                }

                if (ThirdPersonIncluded)
                {
                    _p3Avatar.RemoveWardrobe(pos);
                    _mountWardrobeHandlers[(int) pos, ValidP3Index].SetInfo(null);
                    UpdateRenderBelowAnimatorP3();
                }
                if(!P3HaveInit)
                    UndressInitAvatar(pos);
            }
            else //穿
            {
                if (!P3HaveInit)
                {
                    DressInitAvatar(id);
                    return;
                }
                    
                var assetData = SingletonManager.Get<AvatarAssetConfigManager>().GetAvatarAssetItemById(id);
                if (assetData == null) return;

                if (FirstPersonIncluded)
                {
                    var param = new WardrobeParam(assetData);
                    if (!param.HaveP1Avatar) // 当前部位没有一人称换装,脱掉之前的部位换装
                    {
                        _p1Avatar.RemoveWardrobe(pos);
                        _mountWardrobeHandlers[(int) pos, ValidP1Index].SetInfo(null);
                    }
                    else
                    {
                        LoadResource(_p1Index, param);
                    }
                }

                if (ThirdPersonIncluded)
                    LoadResource(_p3Index, new WardrobeParam(assetData));
            }

            Logger.DebugFormat("rewindWardrobe:     pos  {0}      id  {1}", pos, id);
        }

        private void LoadResource(int index, WardrobeParam param)
        {
            var mount = _mountWardrobeHandlers[(int) param.Type, index];
            mount.SetInfo(param);
            
            if(index == ValidP3Index)
            {
                _loadRequestBatch.Add(CreateLoadRequest(param.P3DefaultResAddr, mount));
                if (param.HasAlterAppearance)
                    _loadRequestBatch.Add(CreateLoadRequest(param.AlterResAddr, mount));
            } else if (index == ValidP1Index)
            {
                _loadRequestBatch.Add(CreateLoadRequest(param.P1DefaultResAddr, mount));
            }
        }
        
        private void ChangeWardrobeShader(GameObject obj, int index)
        {
            if (null == obj || index != _p1Index || !FirstPersonIncluded) return;
            ReplaceMaterialShaderBase.ChangeShader(obj);
        }

        private void ResetWardrobeShader(GameObject obj, int index)
        {
            if (null == obj || index != _p1Index || !FirstPersonIncluded) return;
            ReplaceMaterialShaderBase.ResetShader(obj);
        }

        private void FinishLoadResource(WardrobeParam param, int index)
        {
            if (index == _p1Index && FirstPersonIncluded)
            {
                if (IsFirstPerson)
                {
                    AppearanceUtils.EnableRender(param.DefaultGameObject);
                    AppearanceUtils.EnableRender(param.AlternativeGameObject);
                }
                else
                {
                    AppearanceUtils.DisableRender(param.DefaultGameObject);
                    AppearanceUtils.DisableRender(param.AlternativeGameObject);
                }

                AppearanceUtils.DisableShadow(param.DefaultGameObject);
                AppearanceUtils.DisableShadow(param.AlternativeGameObject);
                
                _p1Avatar.AddWardrobe(param);
                UpdateRenderBelowAnimatorP1();
            }

            if (index == _p3Index && ThirdPersonIncluded)
            {
                if (IsFirstPerson)
                {
                    AppearanceUtils.DisableRender(param.DefaultGameObject);
                    AppearanceUtils.DisableRender(param.AlternativeGameObject);
                }
                else
                {
                    AppearanceUtils.EnableRender(param.DefaultGameObject);
                    AppearanceUtils.EnableRender(param.AlternativeGameObject);
                }

                _p3Avatar.AddWardrobe(param);
                UpdateRenderBelowAnimatorP3();
            }

            ChangeWardrobeShader(param.DefaultGameObject, index);
            ChangeWardrobeShader(param.AlternativeGameObject, index);
        }

        private void DressInitAvatar(int id)
        {
            var pos = SingletonManager.Get<AvatarAssetConfigManager>().GetAvatarType(id);
            if((int)pos >= _initAvatars.Length) return;
            _initAvatars[(int) pos] = id;
        }

        private void UndressInitAvatar(Wardrobe pos)
        {
            if((int)pos >= _initAvatars.Length) return;
            _initAvatars[(int) pos] = -1;
        }

        private void InitWardrobe(List<int> initAvatars)
        {
            if (null == initAvatars) return;
            
            _defaultAvatars[(int)Wardrobe.Foot] = _sex == Sex.Female ? 224 : 217; // 默认ID读策划配表

            foreach (var id in initAvatars)
            {
                SetDefaultAvatar(id);
                Dress(id);
            }

            for (var i = 0; i < _initAvatars.Length; ++i)
            {
                var id = _initAvatars[i];
                if(id < 0) continue;
                Dress(id);
            }
        }

        private void SetDefaultAvatar(int id)
        {
            var pos = SingletonManager.Get<AvatarAssetConfigManager>().GetAvatarType(id);
            if((int)pos >= _defaultAvatars.Length) return;
            _defaultAvatars[(int) pos] = id;
        }

        public void SetSex(Sex sex)
        {
            _sex = sex;
        }

        public void RefreshP1BonePosition()
        {
            UpdateRenderBelowAnimatorP1();
        }

        // Animator.cullingMode在设置时，会记录其下的Render，因此当Render发生改变时，要触发其更新Render列表
        private void UpdateRenderBelowAnimatorP1()
        {
            if (null != _animatorP1)
            {
                var previousMode = _animatorP1.cullingMode;
                _animatorP1.cullingMode = AnimatorCullingMode.AlwaysAnimate;
                _animatorP1.cullingMode = previousMode;
            }
        }

        private void UpdateRenderBelowAnimatorP3()
        {
            if (null != _animatorP3)
            {
                var previousMode = _animatorP3.cullingMode;
                _animatorP3.cullingMode = AnimatorCullingMode.AlwaysAnimate;
                _animatorP3.cullingMode = previousMode;
            }
        }

        #endregion

        private bool FirstPersonIncluded
        {
            get { return _p1Index == ValidP1Index; }
        }

        private bool ThirdPersonIncluded
        {
            get { return _p3Index == ValidP3Index; }
        }

        private bool IsFirstPerson
        {
            get { return _view == CharacterView.FirstPerson; }
        }

        private Command GetAvailableCommand()
        {
            if (_currentCommandIndex >= _outerCommand.Count)
            {
                _outerCommand.Add(new Command());
            }

            return _outerCommand[_currentCommandIndex++];
        }

        #region ICharacterLoadResource

        private List<AbstractLoadRequest> _loadRequestBatch = new List<AbstractLoadRequest>();
        private List<UnityObject> _recycleRequestBatch = new List<UnityObject>();

        private void AddRecycleObject(UnityObject obj)
        {
            if (obj != null)
                _recycleRequestBatch.Add(obj);
        }

        public List<AbstractLoadRequest> GetLoadRequests()
        {
            return _loadRequestBatch;
        }

        public List<UnityObject> GetRecycleRequests()
        {
            if (FirstPersonIncluded)
                _recycleRequestBatch.AddRange(_p1Avatar.GetRecycleRequests());
            if (ThirdPersonIncluded)
                _recycleRequestBatch.AddRange(_p3Avatar.GetRecycleRequests());

            return _recycleRequestBatch;
        }

        public void ClearRequests()
        {
            _loadRequestBatch.Clear();
            _recycleRequestBatch.Clear();

            if (FirstPersonIncluded) _p1Avatar.ClearRequests();
            if (ThirdPersonIncluded) _p3Avatar.ClearRequests();
        }

        protected abstract AbstractLoadRequest CreateLoadRequest(AssetInfo assetInfo, ILoadedHandler mountHandler);

        #endregion

        private class MountWardrobeHandler : ILoadedHandler
        {
            private readonly WardrobeControllerBase _dataSource;
            private readonly int _index;

            private WardrobeParam _param;

            private Action<WardrobeParam> _loadFinishHandle = null;

            public void Register(Action<WardrobeParam> action)
            {
                _loadFinishHandle = action;
            }

            public void UnRegister()
            {
                _loadFinishHandle = null;
            }

            public MountWardrobeHandler(WardrobeControllerBase dataSource, int index)
            {
                _dataSource = dataSource;
                _index = index;
            }

            public void SetInfo(WardrobeParam param)
            {
                _param = param;
            }

            private void SetObjParam(GameObject obj, int layer)
            {
                if (obj == null) return;
                BoneTool.CacheTransform(obj);

                var childCount = obj.transform.childCount;
                for (var i = 0; i < childCount; ++i)
                {
                    obj.transform.GetChild(i).gameObject.layer = layer;
                }
                
                AppearanceUtils.EnableShadow(obj);
                _dataSource.ResetWardrobeShader(obj, _dataSource._p1Index);
            }

            public void OnLoadSucc<T>(T source, UnityObject obj)
            {
                if (_param == null)
                {
                    _dataSource.AddRecycleObject(obj);
                    return;
                }

                if (obj.Address.Equals(_param.P3DefaultResAddr) ||
                    obj.Address.Equals(_param.P1DefaultResAddr))
                {
                    if (_param.DefaultGameObject == null)
                    {
                        _param.DefaultGameObject = obj;
                        SetObjParam(_param.DefaultGameObject, UnityLayerManager.GetLayerIndex(EUnityLayerName.Player));
                    }
                    else
                        _dataSource.AddRecycleObject(obj);
                }
                else if (_param.HasAlterAppearance && obj.Address.Equals(_param.AlterResAddr))
                {
                    if (_param.AlternativeGameObject == null)
                    {
                        _param.AlternativeGameObject = obj;
                        SetObjParam(_param.AlternativeGameObject, UnityLayerManager.GetLayerIndex(EUnityLayerName.Player));
                    }
                    else
                        _dataSource.AddRecycleObject(obj);
                }
                else
                    _dataSource.AddRecycleObject(obj);

                if (_param.DefaultGameObject != null &&
                    _param.HasAlterAppearance == (_param.AlternativeGameObject != null))
                {
                    _param.PrepareMasks();
                    if (_loadFinishHandle != null)
                    {
                        _loadFinishHandle.Invoke(_param);
                    }
                    _dataSource.FinishLoadResource(_param, _index);
                    _param = null;
                }
            }
        }
    }
}
