using Core.Utils;
using System;
using System.Collections.Generic;
using UnityEngine;
using Utils.Appearance.Bone;
using Utils.AssetManager;
using Utils.CharacterState;
using Utils.Compare;

namespace Utils.Appearance.PropItem
{
    public abstract class PropControllerBase
    {
        class Command
        {
            private Action<int> _addProps;
            private Action _removeProps;
            private int _propId;

            public void SetProp(Action<int> action, int id)
            {
                _addProps = action;
                _propId = id;
            }

            public void RemoveProp(Action action)
            {
                _removeProps = action;
            }

            public void Execute()
            {
                if (null != _addProps)
                {
                    _addProps.Invoke(_propId);
                    _addProps = null;
                }

                if (null != _removeProps)
                {
                    _removeProps.Invoke();
                    _removeProps = null;
                }
            }
        }

        #region ILatestWardrobeState

        private int _propId;
        private bool _rewindFlag;

        protected void SetPropIdValue(int id)
        {
            _rewindFlag = !CompareUtility.IsApproximatelyEqual(_propId, id) || _rewindFlag;
            _propId = id;
        }

        protected int GetPropIdValue()
        {
            return _propId;
        }

        #endregion

        private LoggerAdapter Logger = new LoggerAdapter(typeof(PropControllerBase));

        private const int ValidP1Index = 0;
        private const int ValidP3Index = 1;

        private int _p1Index = -1;
        private int _p3Index = -1;

        private MountPropHandler[] _mountPropsHandlers = new MountPropHandler[2];
        private readonly BoneMount _mount = new BoneMount();
        private readonly List<Command> _outerCommand = new List<Command>();
        private int _currentCommandIndex;

        private GameObject _characterP3;
        private GameObject _characterP1;
        private CharacterView _view = CharacterView.ThirdPerson;

        private GameObject _propObjP3;
        private GameObject _propObjP1;

        public PropControllerBase()
        {
            _mountPropsHandlers[ValidP1Index] = new MountPropHandler(this, ValidP1Index);
            _mountPropsHandlers[ValidP3Index] = new MountPropHandler(this, ValidP3Index);
        }

        #region Sync 

        public void TryRewind()
        {
            if (_rewindFlag)
            {
                _rewindFlag = false;
                RewindPropInHand(_propId);
            }
        }

        #endregion

        #region handle outer command

        public void AddProps(int propId)
        {
            var cmd = GetAvailableCommand();
            cmd.SetProp(AddPropImpl, propId);
        }

        public void RemoveProps()
        {
            var cmd = GetAvailableCommand();
            cmd.RemoveProp(RemovePropImpl);
        }

        public void Update()
        {
        }

        #endregion

        #region Initial Setting

        public void SetThirdPersonCharacter(GameObject obj)
        {
            _characterP3 = obj;
            _p3Index = ValidP3Index;
        }

        public void SetFirstPersonCharacter(GameObject obj)
        {
            _characterP1 = obj;
            _p1Index = ValidP1Index;
        }

        public void SetFirstPerson()
        {
            if (FirstPersonIncluded) AppearanceUtils.EnableRender(_propObjP1);
            if (ThirdPersonIncluded) AppearanceUtils.DisableRender(_propObjP3);
            _view = CharacterView.FirstPerson;
        }

        public void SetThirdPerson()
        {
            if (FirstPersonIncluded) AppearanceUtils.DisableRender(_propObjP1);
            if (ThirdPersonIncluded) AppearanceUtils.EnableRender(_propObjP3);
            _view = CharacterView.ThirdPerson;
        }

        #endregion

        #region Outer State Setter

        public void Execute()
        {
            for (int i = 0; i < _currentCommandIndex; i++)
            {
                _outerCommand[i].Execute();
            }

            _currentCommandIndex = 0;
        }

        #endregion

        #region changeProp

        //穿
        private void AddPropImpl(int id)
        {
            SetPropIdValue(id);
            Logger.DebugFormat("AddPropImpl {0}", id);
        }

        private void RemovePropImpl()
        {
            SetPropIdValue(-1);
            Logger.DebugFormat("RemovePropImpl");
        }

        private void RewindPropInHand(int id)
        {
            if (id < 0)
            {
                RemovePropInHand();
            }
            else
            {
                AssetInfo info = new AssetInfo("item", "H00" + id);
                if (ThirdPersonIncluded)
                {
                    var mount = _mountPropsHandlers[ValidP3Index];
                    _loadRequestBatch.Add(CreateLoadRequest(info, mount));
                }

                if (FirstPersonIncluded)
                {
                    var mount = _mountPropsHandlers[ValidP1Index];
                    _loadRequestBatch.Add(CreateLoadRequest(info, mount));
                }
            }
        }
        
        private void ChangePropsShader(GameObject obj)
        {
            ReplaceMaterialShaderBase.ChangeShader(obj);
        }

        private void ResetPropsShader(GameObject obj)
        {
            ReplaceMaterialShaderBase.ResetShader(obj);
        }

        private void MountPropToHand(int index)
        {
            if (ThirdPersonIncluded && _p3Index == index && null != _propObjP3)
            {
                _mount.MountRightHandWeapon(_propObjP3, _characterP3);
            }

            if (FirstPersonIncluded && _p1Index == index && null != _propObjP1)
            {
                _mount.MountRightHandWeapon(_propObjP1, _characterP1);
                ChangePropsShader(_propObjP1);
            }
        }

        private void RemovePropInHand()
        {
            if (ThirdPersonIncluded && null != _propObjP3)
            {
                _mount.RemountWeaponOnRightHand(_propObjP3, _characterP3);
                AppearanceUtils.DisableRender(_propObjP3);
            }

            if (FirstPersonIncluded && null != _propObjP1)
            {
                _mount.RemountWeaponOnRightHand(_propObjP1, _characterP1);
                AppearanceUtils.DisableRender(_propObjP1);
                ResetPropsShader(_propObjP1);
            }
        }

        protected abstract AbstractLoadRequest CreateLoadRequest(AssetInfo assetInfo, ILoadedHandler mountHandler);

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
            return _recycleRequestBatch;
        }

        public void ClearRequests()
        {
            _loadRequestBatch.Clear();
            _recycleRequestBatch.Clear();
        }

        #endregion

        class MountPropHandler : ILoadedHandler
        {
            private readonly PropControllerBase _dataSource;
            private readonly int _index;

            public MountPropHandler(PropControllerBase dataSource, int index)
            {
                _dataSource = dataSource;
                _index = index;
            }

            public void OnLoadSucc<T>(T source, UnityObject unityObj)
            {
                var go = unityObj.AsGameObject;
                if (null == go) return;
                
                BoneTool.CacheTransform(go);
                
                if (_dataSource._p1Index == _index)
                {
                    AppearanceUtils.DisableRender(_dataSource._propObjP1);
                    _dataSource._propObjP1 = go;
                    if (!_dataSource.IsFirstPerson)
                    {
                        AppearanceUtils.DisableRender(go);
                        _dataSource.AddRecycleObject(unityObj);
                    }
                }
                else if (_dataSource._p3Index == _index)
                {
                    AppearanceUtils.DisableRender(_dataSource._propObjP3);
                    _dataSource._propObjP3 = go;
                    if (_dataSource.IsFirstPerson)
                    {
                        AppearanceUtils.DisableRender(go);
                        _dataSource.AddRecycleObject(unityObj);
                    }
                }
                else
                {
                    AppearanceUtils.DisableRender(go);
                    _dataSource.AddRecycleObject(unityObj);
                }

                _dataSource.MountPropToHand(_index);
            }
        }
    }
}
