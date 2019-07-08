using Core.Utils;
using Shared.Scripts;
using System;
using System.Collections.Generic;
using UnityEngine;
using Utils.Appearance.Bone;
using Utils.Appearance.Effects;
using Utils.AssetManager;
using Utils.Configuration;
using Utils.Singleton;

namespace Utils.Appearance.WardrobePackage
{
    public class CharacterAvatar
    {
        private class WardrobeRegister
        {
            private readonly byte[] _entry = new byte[(int) Wardrobe.EndOfTheWorld];
            public int RegisteredCount { get; private set; }

            public void Register(Wardrobe type)
            {
                if (_entry[(int) type] != 0)
                {
                    Logger.Warn("Duplicated Register");
                    return;
                }

                _entry[(int) type] = 1;
                ++RegisteredCount;
            }

            public void Unregister(Wardrobe type)
            {
                if (_entry[(int) type] == 0)
                {
                    Logger.Warn("Invalid Unregister");
                    return;
                }

                _entry[(int) type] = 0;
                --RegisteredCount;               
            }

            public Wardrobe FirstRegisterType()
            {
                var i = 0;
                for (; i < _entry.Length; i++)
                {
                    if (_entry[i] == 1)
                    {
                        break;
                    }
                }

                return (Wardrobe) i;
            }
        }

        private class WardrobeStatus
        {
            private class SingleWardrobeStatus
            {
                private readonly WardrobeRegister _alter = new WardrobeRegister();
                private readonly WardrobeRegister _hide = new WardrobeRegister();
                private readonly WardrobeRegister _mask = new WardrobeRegister();

                public void RegisterAlter(Wardrobe type)
                {
                    _alter.Register(type);
                }

                public void UnregisterAlter(Wardrobe type)
                {
                    _alter.Unregister(type);
                }

                public void RegisterHide(Wardrobe type)
                {
                    _hide.Register(type);
                }

                public void UnregisterHide(Wardrobe type)
                {
                    _hide.Unregister(type);
                }

                public void RegisterMask(Wardrobe type)
                {
                    _mask.Register(type);
                }

                public void UnregisterMask(Wardrobe type)
                {
                    _mask.Unregister(type);
                }

                public bool ShowDefault
                {
                    get { return 0 == _alter.RegisteredCount && 0 == _hide.RegisteredCount; }
                }

                public bool ShowAlter
                {
                    get { return 0 != _alter.RegisteredCount && 0 == _hide.RegisteredCount; }
                }

                public bool IsMask
                {
                    get { return 0 != _mask.RegisteredCount; }
                }

                public Wardrobe FirstRegisterMask
                {
                    get { return _mask.FirstRegisterType(); }
                }

                public bool LastShowDefault { get; private set; }
                public bool LastShowAlter { get; private set; }
                public bool LastIsMask { get; private set; }

                public SingleWardrobeStatus()
                {
                    UpdateToLatest();
                }

                public void UpdateToLatest()
                {
                    LastShowDefault = ShowDefault;
                    LastShowAlter = ShowAlter;
                    LastIsMask = IsMask;
                }
            }

            private readonly SingleWardrobeStatus[] _status = new SingleWardrobeStatus[(int) Wardrobe.EndOfTheWorld];
            
            private readonly byte[] _defaultChanged = new byte[(int) Wardrobe.EndOfTheWorld];
            private readonly byte[] _alterChanged = new byte[(int) Wardrobe.EndOfTheWorld];
            private readonly byte[] _maskChanged = new byte[(int) Wardrobe.EndOfTheWorld];
            private int _defaultChangedIndex;
            private int _alterChangedIndex;
            private int _maskChangedIndex;

            public WardrobeStatus()
            {
                for (var i = 0; i < _status.Length; i++)
                {
                    _status[i] = new SingleWardrobeStatus();
                }
            }

            public void Record()
            {
                _defaultChangedIndex = 0;
                _alterChangedIndex = 0;
                _maskChangedIndex = 0;
                foreach (var status in _status)
                {
                    status.UpdateToLatest();
                }
            }

            public bool GetDefaultEnabled(Wardrobe type)
            {
                return _status[(int) type].ShowDefault;
            }

            public bool GetAlterEnabled(Wardrobe type)
            {
                return _status[(int) type].ShowAlter;
            }

            public bool GetMaskEnabled(Wardrobe type)
            {
                return _status[(int) type].IsMask;
            }
            
            public void RegisterAlter(Wardrobe source, Wardrobe type)
            {
                if ((int) source < _status.Length)
                {
                    var status = _status[(int) source];
                    status.RegisterAlter(type);
                    if (status.LastShowDefault != status.ShowDefault)
                        _defaultChanged[(int) source] = 1;
                    if (status.LastShowAlter != status.ShowAlter)
                        _alterChanged[(int) source] = 1;
                }
            }

            public void UnregisterAlter(Wardrobe source, Wardrobe type)
            {
                if ((int) source < _status.Length)
                {
                    var status = _status[(int) source];
                    status.UnregisterAlter(type);
                    if (status.LastShowDefault != status.ShowDefault)
                        _defaultChanged[(int) source] = 1;
                    if (status.LastShowAlter != status.ShowAlter)
                        _alterChanged[(int) source] = 1;
                }
            }

            public void RegisterHide(Wardrobe source, Wardrobe type)
            {
                if ((int) source < _status.Length)
                {
                    var status = _status[(int) source];
                    status.RegisterHide(type);
                    if (status.LastShowDefault != status.ShowDefault)
                        _defaultChanged[(int) source] = 1;
                    if (status.LastShowAlter != status.ShowAlter)
                        _alterChanged[(int) source] = 1;
                }
            }

            public void UnregisterHide(Wardrobe source, Wardrobe type)
            {
                if ((int) source < _status.Length)
                {
                    var status = _status[(int) source];
                    status.UnregisterHide(type);
                    if (status.LastShowDefault != status.ShowDefault)
                        _defaultChanged[(int) source] = 1;
                    if (status.LastShowAlter != status.ShowAlter)
                        _alterChanged[(int) source] = 1;
                }
            }

            public void RegisterMask(Wardrobe source, Wardrobe type)
            {
                if ((int) source < _status.Length)
                {
                    var status = _status[(int) source];
                    status.RegisterMask(type);
                    if (status.LastIsMask != status.IsMask)
                        _maskChanged[(int) source] = 1;
                }
            }

            public void UnregisterMask(Wardrobe source, Wardrobe type)
            {
                if ((int) source < _status.Length)
                {
                    var status = _status[(int) source];
                    status.UnregisterMask(type);
                    if (status.LastIsMask != status.IsMask)
                        _maskChanged[(int) source] = 1;
                }
            }

            public Wardrobe GetDefaultChangedType()
            {
                while (_defaultChangedIndex < _defaultChanged.Length && _defaultChanged[_defaultChangedIndex] == 0)
                    ++_defaultChangedIndex;

                if (_defaultChangedIndex < _defaultChanged.Length)
                    _defaultChanged[_defaultChangedIndex] = 0;
                
                return (Wardrobe) _defaultChangedIndex;
            }

            public Wardrobe GetAlterChangedType()
            {
                while (_alterChangedIndex < _alterChanged.Length && _alterChanged[_alterChangedIndex] == 0)
                    ++_alterChangedIndex;

                if (_alterChangedIndex < _alterChanged.Length)
                    _alterChanged[_alterChangedIndex] = 0;

                return (Wardrobe) _alterChangedIndex;
            }

            public Wardrobe GetMaskChangedType()
            {
                while (_maskChangedIndex < _maskChanged.Length && _maskChanged[_maskChangedIndex] == 0)
                    ++_maskChangedIndex;

                if (_maskChangedIndex < _maskChanged.Length)
                    _maskChanged[_maskChangedIndex] = 0;

                return (Wardrobe) _maskChangedIndex;
            }

            public Wardrobe GetFirstMaskProvider(Wardrobe type)
            {
                return _status[(int) type].FirstRegisterMask;
            }
        }

        private static readonly LoggerAdapter Logger = new LoggerAdapter(typeof(CharacterAvatar));
        
        private Action _bagChanged;
        private GameObject _characterGameObject;
        private Transform[] _allBones;
        private Transform _attachment6Parent;
        private Transform _attachment7Parent;
        private Transform _attachment6ParentInChar;
        private Transform _attachment7ParentInChar;
        private WardrobeParam[] _wardrobes = new WardrobeParam[(int)Wardrobe.EndOfTheWorld];
        private readonly WardrobeStatus _wardrobesStatus = new WardrobeStatus();
        
        private readonly Dictionary<Wardrobe, Transform[]> _allAvatarBones = new Dictionary<Wardrobe, Transform[]>();

        private readonly BoneMount _mount = new BoneMount();
        private bool _enabled;
        private const int AlternativeStartNum = 1000;
        private const int DefaultAvatarId = 1;
        private const Wardrobe StandardPart = Wardrobe.CharacterHead;
        private GameObject _rootGo;
        
        private readonly CustomProfileInfo _mainInfo;
        private readonly CustomProfileInfo _updateBagTransformInfo;
        private readonly CustomProfileInfo _updateMappingTransformInfo;
        private readonly CustomProfileInfo _updateLodInfo;
        private readonly CustomProfileInfo _getLodLevelInfo;
        private readonly CustomProfileInfo _setLodLevelInfo;
        
        public CharacterAvatar(GameObject character)
        {
            _mainInfo = SingletonManager.Get<DurationHelp>().GetCustomProfileInfo("CharacterAvatar");
            _updateBagTransformInfo = SingletonManager.Get<DurationHelp>().GetCustomProfileInfo("CharacterAvatarUpdateBagTransform");
            _updateMappingTransformInfo = SingletonManager.Get<DurationHelp>().GetCustomProfileInfo("CharacterAvatarUpdateMappingTransform");
            _updateLodInfo = SingletonManager.Get<DurationHelp>().GetCustomProfileInfo("CharacterAvatarCalcLod");
            _getLodLevelInfo = SingletonManager.Get<DurationHelp>().GetCustomProfileInfo("MyLodGroupGetLodLevel");
            _setLodLevelInfo = SingletonManager.Get<DurationHelp>().GetCustomProfileInfo("MyLodGroupSetLodLevel");
            
            ReInit(character);
        }

        public void SetRootGo(GameObject obj)
        {
            _rootGo = obj;
        }
        
        private void ReInit(GameObject character)
        {
            _characterGameObject = character;
            _allBones = _characterGameObject.GetComponentsInChildren<Transform>();
            
            _wardrobes = new WardrobeParam[(int)Wardrobe.EndOfTheWorld];
            var wardrobeTypes = _characterGameObject.GetComponentsInChildren<WardrobeType>();
            if(null == wardrobeTypes) return;
            foreach (var param in wardrobeTypes)
            {
                var assetData = SingletonManager.Get<AvatarAssetConfigManager>().GetAvatarAssetItemById(DefaultAvatarId);
                if(null == assetData) continue;
                _wardrobes[(int)param.Type] = new WardrobeParam(assetData);
                _wardrobes[(int)param.Type].DefaultGameObject = new UnityObject(param.transform.gameObject, new AssetInfo());
            }
        }
        
        public void HandleSingleAvatar(Wardrobe type, Action<UnityObject> act)
        {
            if (_wardrobes[(int) type] == null) return; 
            if(_wardrobes[(int)type].DefaultGameObject!=null)
                act(_wardrobes[(int)type].DefaultGameObject);
            if(_wardrobes[(int)type].AlternativeGameObject!=null)
                act(_wardrobes[(int) type].AlternativeGameObject);
        }
        
        public void SetBagChangedDelegate(Action action)
        {
            _bagChanged = action;
        }

        public void SetWardrobeTopLayerShader()
        {
            for (var i = 0; i < (int)Wardrobe.EndOfTheWorld; ++i)
            {
                if(null == _wardrobes[i]) continue;
                ReplaceMaterialShaderBase.ChangeShader(_wardrobes[i].DefaultGameObject);
                ReplaceMaterialShaderBase.ChangeShader(_wardrobes[i].AlternativeGameObject);
            }
        }

        public void ResetWardrobeShader()
        {
            for (var i = 0; i < (int)Wardrobe.EndOfTheWorld; ++i)
            {
                if(null == _wardrobes[i]) continue;
                ReplaceMaterialShaderBase.ResetShader(_wardrobes[i].DefaultGameObject);
                ReplaceMaterialShaderBase.ResetShader(_wardrobes[i].AlternativeGameObject);
            }
        }

        public void AddWardrobe(WardrobeParam param)
        {
            if(null != param.DefaultGameObject && null != param.DefaultGameObject.AsGameObject)
                Logger.InfoFormat("CharacterLog-- CharacterAvatar Add Wardrobe:  {0}", param.DefaultGameObject.AsGameObject.name);
            
            // 先移除对应位置
            RemoveWardrobe(param.Type, false);
            
            _wardrobes[(int) param.Type] = param;
 
            // 蒙皮or硬挂
            PutOn(param.DefaultGameObject, (int)param.Type, param.IsSkinned, param.NeedMappingBones);
            EffectUtility.ReflushGodModeEffect(_rootGo, param.DefaultGameObject);
            EffectUtility.RegistEffect(_rootGo, param.DefaultGameObject);
            
            if (param.HasAlterAppearance)
            {
                PutOn(param.AlternativeGameObject, (int)param.Type + AlternativeStartNum, param.IsSkinned, param.NeedMappingBones);
                EffectUtility.ReflushGodModeEffect(_rootGo, param.AlternativeGameObject);
            }
            
            // 设置显示状态，由其他位置决定
            RefreshSingleWardrobeDefault(param,
                                         _wardrobesStatus.GetDefaultEnabled(param.Type));
            RefreshSingleWardrobeAlter(param,
                                       _wardrobesStatus.GetAlterEnabled(param.Type));
            RefreshSingleWardrobeMask(param,
                                      _wardrobesStatus.GetFirstMaskProvider(param.Type),
                                      _wardrobesStatus.GetMaskEnabled(param.Type));
            
            // 影响其他位置的显示状态
            RegisterAlternativeAppearance(param);
            // 影响其他位置的显示
            ShowAccordingToStatus();

            // 背包挂点
            AddBag(param);
        }

        public void RemoveWardrobe(Wardrobe position, bool updateAppearance = true)
        {
            if (_wardrobes[(int) position] == null) return;
            
            if(null != _wardrobes[(int) position] && 
               null != _wardrobes[(int) position].DefaultGameObject &&
               null != _wardrobes[(int) position].DefaultGameObject.AsGameObject)
                Logger.InfoFormat("CharacterLog-- CharacterAvatar Remove Wardrobe:  {0}", _wardrobes[(int) position].DefaultGameObject.AsGameObject.name);
            
            var param = _wardrobes[(int) position];
            _wardrobes[(int) position] = null;
            
            // 移除显示
            TakeOff(param.DefaultGameObject);
            AddRecycleObject(param.DefaultGameObject);
            EffectUtility.DeleteGodModeEffect(_rootGo, param.DefaultGameObject);
            
            if (param.AlternativeGameObject != null)
            {
                TakeOff(param.AlternativeGameObject);
                AddRecycleObject(param.AlternativeGameObject);
                EffectUtility.DeleteGodModeEffect(_rootGo, param.AlternativeGameObject);
            }
            
            // 移除对其他位置的影响
            UnregisterAlternativeAppearance(param);
            
            // 更新其他位置的显示
            if (updateAppearance)
                ShowAccordingToStatus();

            // 移除RootBone映射
            if(_mappingBones.ContainsKey((int)position))
                _mappingBones.Remove((int)position);
            if (_mappingBones.ContainsKey((int)position + AlternativeStartNum))
                _mappingBones.Remove((int)position + AlternativeStartNum);

            if (_allAvatarBones.ContainsKey(position))
            {
                ResetAvatarRenderBones(_allAvatarBones[position], param.DefaultGameObject);
                if (param.AlternativeGameObject != null)
                    ResetAvatarRenderBones(_allAvatarBones[position], param.AlternativeGameObject);
            }

            // 背包挂点
            RemoveBag(position);
        }

        public void HandleAllWardrobe(Action<UnityObject> act)
        {
            for (int i = 0; i < (int) Wardrobe.EndOfTheWorld; i++)
            {
                if (_wardrobes[i] == null) continue;
                if (_wardrobes[i].DefaultGameObject != null)
                    act(_wardrobes[i].DefaultGameObject);
                if (_wardrobes[i].AlternativeGameObject != null)
                    act(_wardrobes[i].AlternativeGameObject);
            }
        }
        
        private void AddBag(WardrobeParam param)
        {
            if (param.Type == Wardrobe.Bag && _bagChanged != null)
            {
                var attachment6 = BoneMount.FindChildBone(param.DefaultGameObject, BoneName.WeaponAttachment6);
                var attachment7 = BoneMount.FindChildBone(param.DefaultGameObject, BoneName.WeaponAttachment7);

                if (attachment6 != null)
                {
                    _attachment6Parent = attachment6.parent;
                    _attachment6ParentInChar = GetBoneOfTheSameName(_attachment6Parent, _allBones);
                }
                if (attachment7 != null)
                {
                    _attachment7Parent = attachment7.parent;
                    _attachment7ParentInChar = GetBoneOfTheSameName(_attachment7Parent, _allBones);
                }
                _bagChanged.Invoke();
            }
        }

        private void RemoveBag(Wardrobe position)
        {
            if (position == Wardrobe.Bag && _bagChanged != null)
            {
                _attachment6Parent = null;
                _attachment7Parent = null;
                _attachment6ParentInChar = null;
                _attachment7ParentInChar = null;
                _bagChanged.Invoke();
            }
        }

        public void Show()
        {
            _enabled = true;
            for (var i = 0; i < (int) Wardrobe.EndOfTheWorld; i++)
            {
                var param = _wardrobes[i];
                if (param != null)
                {
                    if (_wardrobesStatus.GetDefaultEnabled((Wardrobe) i))
                        AppearanceUtils.EnableRender(param.DefaultGameObject);
                    if (_wardrobesStatus.GetAlterEnabled((Wardrobe) i))
                        AppearanceUtils.EnableRender(param.AlternativeGameObject);
                }
            }
        }

        public void Hide()
        {
            _enabled = false;
            for (var i = 0; i < (int) Wardrobe.EndOfTheWorld; i++)
            {
                var param = _wardrobes[i];
                if (param != null)
                {
                    if (_wardrobesStatus.GetDefaultEnabled((Wardrobe) i))
                        AppearanceUtils.DisableRender(param.DefaultGameObject);
                    if (_wardrobesStatus.GetAlterEnabled((Wardrobe) i))
                        AppearanceUtils.DisableRender(param.AlternativeGameObject);
                }
            }
        }

        public void UpdateAvatarSkin(Wardrobe pos, int skinId)
        {
            var param = _wardrobes[(int)pos];
            if (param != null)
            {
                param.ApplySkin(skinId);
            }
        }

        public void Update()
        {
            try
            {
                _mainInfo.BeginProfileOnlyEnableProfile();

                try
                {
                    _updateBagTransformInfo.BeginProfileOnlyEnableProfile();
                    var param = _wardrobes[(int) Wardrobe.Bag];
                    if (param != null)
                    {
                        if (_attachment6Parent != null && _attachment6ParentInChar != null)
                        {
                            _attachment6Parent.SetPositionAndRotation(_attachment6ParentInChar.position,
                                _attachment6ParentInChar.rotation);
                        }

                        if (_attachment7Parent != null && _attachment7ParentInChar != null)
                        {
                            _attachment7Parent.SetPositionAndRotation(_attachment7ParentInChar.position,
                                _attachment7ParentInChar.rotation);
                        }
                    }
                }
                finally
                {
                    _updateBagTransformInfo.EndProfileOnlyEnableProfile();
                }

                try
                {
                    _updateMappingTransformInfo.BeginProfileOnlyEnableProfile();
                    if (null != _mappingBones)
                    {
                        foreach (var items in _mappingBones)
                        {
                            for (var i = 0; i < items.Value.Count; ++i)
                            {
                                var mapping = items.Value[i];
                                mapping.WardrobeBone.SetPositionAndRotation(mapping.CharacterBone.position,
                                    mapping.CharacterBone.rotation);
                            }
                        }
                    }
                }
                finally
                {
                    _updateMappingTransformInfo.EndProfileOnlyEnableProfile();
                }

                CalcCurrentLod();
            }
            finally
            {
                _mainInfo.EndProfileOnlyEnableProfile();
            }
        }

        private void CalcCurrentLod()
        {
            try
            {
                _updateLodInfo.BeginProfileOnlyEnableProfile();
                var head = _wardrobes[(int)StandardPart];
                if(null == head) return;
                
                int lodLevel;
                try
                {
                    lodLevel = MyLodGroup.GetLogLevel(head.DefaultGameObject);
                    _getLodLevelInfo.BeginProfileOnlyEnableProfile();
                }
                finally
                {
                    _getLodLevelInfo.EndProfileOnlyEnableProfile();
                }

                foreach (var value in _wardrobes)
                {
                    if(null == value) continue;
                    try
                    {
                        _setLodLevelInfo.BeginProfileOnlyEnableProfile();
                        MyLodGroup.SetLogLevel(value.DefaultGameObject, lodLevel);
                        MyLodGroup.SetLogLevel(value.AlternativeGameObject, lodLevel);
                    }
                    finally
                    {
                        _setLodLevelInfo.EndProfileOnlyEnableProfile();
                    }
                }
            }
            finally
            {
                _updateLodInfo.EndProfileOnlyEnableProfile();
            }
        }

        private void PutOn(GameObject go, int type, bool isSkinned, bool needMapping)
        {
            if(null == go) return;
            
            if (isSkinned)
            {
                go.transform.SetParent(_characterGameObject.transform, false);
                go.transform.localPosition = Vector3.zero;
                go.transform.localRotation = Quaternion.identity;
                //                go.transform.localScale = Vector3.one;

                var allWardrobeBones = go.GetComponentsInChildren<Transform>();
                _allAvatarBones[(Wardrobe) type] = allWardrobeBones;
                
                foreach (var renderer in go.GetComponentsInChildren<SkinnedMeshRenderer>())
                {
                    // 映射装扮与人物骨骼
                    if (needMapping)
                    {
                        var wardrobeBones = GetBonesOfTheSameName(renderer.bones, allWardrobeBones);
                        MappingBones(wardrobeBones, type);
                    }
                    renderer.bones = GetBonesOfTheSameName(renderer.bones, _allBones);
                    renderer.rootBone = GetBoneOfTheSameName(renderer.rootBone, _allBones);
                }
            }
            else
            {
                _mount.MountWardrobe(go, _characterGameObject);
            }
            Logger.InfoFormat("CharacterLog-- SureAddWardrobe:  {0}", go.name);
        }
        
        private void TakeOff(GameObject go)
        {
            if(null == go) return;
            
            go.SetActive(false);
            go.transform.SetParent(null, false);
            SetMaskTexture(go, null);
            Logger.InfoFormat("CharacterLog-- SureRemoveWardrobe:  {0}", go.name);
        }

        private void RegisterAlternativeAppearance(WardrobeParam param)
        {
            _wardrobesStatus.Record();
            
            if (param.EnableOtherAlter)
                _wardrobesStatus.RegisterAlter(param.AlterType, param.Type);
            if (param.HasHideAvatar)
            {
                foreach (var pos in param.HidePositions)
                {
                    _wardrobesStatus.RegisterHide(pos, param.Type);
                }
            }
            if (param.Masks != null)
            {
                foreach (var mask in param.Masks)
                {
                    _wardrobesStatus.RegisterMask(mask.Item1, param.Type);
                }
            }
        }

        private void UnregisterAlternativeAppearance(WardrobeParam param)
        {
            _wardrobesStatus.Record();

            if (param.EnableOtherAlter)
                _wardrobesStatus.UnregisterAlter(param.AlterType, param.Type);
            if (param.HasHideAvatar)
            {
                foreach (var pos in param.HidePositions)
                {
                    _wardrobesStatus.UnregisterHide(pos, param.Type);
                }
            }
            if (param.Masks != null)
            {
                foreach (var mask in param.Masks)
                {
                    _wardrobesStatus.UnregisterMask(mask.Item1, param.Type);
                }
            }           
        }

        private void ShowAccordingToStatus()
        {
            Wardrobe type;
            while ((type = _wardrobesStatus.GetDefaultChangedType()) != Wardrobe.EndOfTheWorld)
            {
                if (_wardrobes[(int) type] != null)
                    RefreshSingleWardrobeDefault(_wardrobes[(int) type],
                                                 _wardrobesStatus.GetDefaultEnabled(type));
            }

            while ((type = _wardrobesStatus.GetAlterChangedType()) != Wardrobe.EndOfTheWorld)
            {
                if (_wardrobes[(int) type] != null)
                    RefreshSingleWardrobeAlter(_wardrobes[(int) type],
                                               _wardrobesStatus.GetAlterEnabled(type));
            }

            while ((type = _wardrobesStatus.GetMaskChangedType()) != Wardrobe.EndOfTheWorld)
            {
                if (_wardrobes[(int) type] != null)
                    RefreshSingleWardrobeMask(_wardrobes[(int) type],
                                              _wardrobesStatus.GetFirstMaskProvider(type),
                                              _wardrobesStatus.GetMaskEnabled(type));
            }
        }

        private void RefreshSingleWardrobeDefault(WardrobeParam param, bool enable)
        {
            if (enable)
            {
                ActiveGameObject(param.DefaultGameObject);
                AddBag(param);
            }
            else
            {
                UnactiveGameobject(param.DefaultGameObject);
                RemoveBag(param.Type);
            }
        }

        private void RefreshSingleWardrobeAlter(WardrobeParam param, bool enable)
        {
            if (enable)
                EnableRender(param.AlternativeGameObject);
            else
                DisableRender(param.AlternativeGameObject);
        }

        private void RefreshSingleWardrobeMask(WardrobeParam param, Wardrobe provider, bool enable)
        {
            if (enable)
            {
                var maskProvider = _wardrobes[(int) provider];
                foreach (var mask in maskProvider.Masks)
                {
                    if (mask.Item1 == param.Type)
                    {
                        SetMaskTexture(param.DefaultGameObject, mask.Item2);
                        SetMaskTexture(param.AlternativeGameObject, mask.Item2);
                        break;
                    }
                }
            }
            else
            {
                SetMaskTexture(param.DefaultGameObject, null);
                SetMaskTexture(param.AlternativeGameObject, null);
            }
        }

        private void SetMaskTexture(GameObject go, Texture tex)
        {
            if (go == null)
                return;
            
            foreach (var renderer in go.GetComponentsInChildren<SkinnedMeshRenderer>())
            {
                if (renderer.material != null)
                {
                    if(null != tex)
                    {
                        renderer.material.SetTexture("_MainMaskMap", tex);
                        renderer.material.EnableKeyword("_MAIN_MASK_MAP");
                    }
                    else
                    {
                        renderer.material.SetTexture("_MainMaskMap", tex);
                        renderer.material.DisableKeyword("_MAIN_MASK_MAP");
                    }
                }
            }
        }
        
        private Transform[] GetBonesOfTheSameName(Transform[] nameProvider, Transform[] allBones)
        {
            Transform[] ret = new Transform[nameProvider.Length];

            for (var i = 0; i < ret.Length; i++)
            {
                ret[i] = GetBoneOfTheSameName(nameProvider[i], allBones);
            }

            return ret;
        }

        private Transform GetBoneOfTheSameName(Transform wardrobeBone, Transform[] allBones)
        {
            if (null == wardrobeBone)
            {
                Logger.ErrorFormat("Avatar : {0}  skinnedMeshRenderer Bones Error");
                return wardrobeBone;
            }
            string boneName = wardrobeBone.name;
            foreach (var bone in allBones)
            {
                if (bone.name == boneName)
                {
                    return bone;
                }
            }

            //Logger.WarnFormat("Could not fine corresponding bone \"{0}\" in avatar  : maybe is dynamicBone", wardrobeBone.name);
            return wardrobeBone;
        }

        private Dictionary<int, List<MappingBone>> _mappingBones = new Dictionary<int, List<MappingBone>>();
        private void MappingBones(Transform[] wardrobeBones, int type)
        {
            _mappingBones[type] = new List<MappingBone>();
            for (var i = 0; i < _allBones.Length; ++i)
            {
                for (var j = 0; j < wardrobeBones.Length; ++j)
                {
                    if (_allBones[i].name.Equals(wardrobeBones[j].name, StringComparison.Ordinal))
                    {
                        var item = new MappingBone { WardrobeBone = wardrobeBones[j], CharacterBone = _allBones[i] };
                        _mappingBones[type].Add(item);
                        break;
                    }
                }
            }
        }

        private void ResetAvatarRenderBones(Transform[] avatarBones, GameObject avatar)
        {
            if(null == avatar || null == avatarBones) return;
            foreach (var renderer in avatar.GetComponentsInChildren<SkinnedMeshRenderer>())
            {
                renderer.bones = GetBonesOfTheSameName(renderer.bones, avatarBones);
                renderer.rootBone = GetBoneOfTheSameName(renderer.rootBone, avatarBones);
            }
        }

        private void EnableRender(GameObject go)
        {
            if (_enabled)
                AppearanceUtils.EnableRender(go);
        }

        private void DisableRender(GameObject go)
        {
            if (_enabled)
                AppearanceUtils.DisableRender(go);
        }

        private void ActiveGameObject(GameObject go)
        {
            if (_enabled)
            {
                AppearanceUtils.ActiveGameobject(go);
            }
        }

        private void UnactiveGameobject(GameObject go)
        {
            if (_enabled)
            {
                AppearanceUtils.UnactiveGameobject(go);
            }
        }

        #region recycleRequest

        private List<UnityObject> _recycleRequestBatch = new List<UnityObject>();
        private void AddRecycleObject(UnityObject obj)
        {
            if (obj != null)
                _recycleRequestBatch.Add(obj);
        }

        public IList<UnityObject> GetRecycleRequests()
        {
            return _recycleRequestBatch;
        }

        public void ClearRequests()
        {
            _recycleRequestBatch.Clear();
        }
        
        #endregion
    }

    struct MappingBone
    {
        public Transform WardrobeBone;
        public Transform CharacterBone;
    }
}
