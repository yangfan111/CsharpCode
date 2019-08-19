//#define UnitTest
using Utils.AssetManager;
using Core.Utils;
using System.Collections.Generic;
using UnityEngine;
using Utils.Appearance.Bone;
using Utils.Utils;
using XmlConfig;

namespace Utils.Appearance.Weapon
{
    public class WeaponModelLoadController<T> : IWeaponModelLoadController<T> where T : class
    {
#if !UnitTest
        private static readonly LoggerAdapter Logger = new LoggerAdapter(typeof(WeaponModelLoadController<T>));
#endif
        private Dictionary<AssetInfo, T> _cache = new Dictionary<AssetInfo, T>(AssetInfo.AssetInfoComparer.Instance);
        private AssetInfo? _weaponInfo;
        private T _weaponGo;
        private Dictionary<WeaponPartLocation, AssetInfo> _partAssetDic = new Dictionary<WeaponPartLocation, AssetInfo>(CommonIntEnumEqualityComparer<WeaponPartLocation>.Instance);
        private Dictionary<WeaponPartLocation, T> _partGoDic = new Dictionary<WeaponPartLocation, T>(CommonIntEnumEqualityComparer<WeaponPartLocation>.Instance);
        private Dictionary<WeaponPartLocation, T> _waitPartGoDic = new Dictionary<WeaponPartLocation, T>(CommonIntEnumEqualityComparer<WeaponPartLocation>.Instance);
        private IWeaponModelLoader _weaponModelLoader;
        private IWeaponModelAssemblyController _weaponModelAssemblyController;

        public WeaponModelLoadController(IWeaponModelLoader loader, 
            IWeaponModelAssemblyController weaponModelAssemblyController)
        {
            _weaponModelLoader = loader;
            _weaponModelAssemblyController = weaponModelAssemblyController;
            _weaponModelLoader.RegisterLoadedCb(OnLoaded);
        }

        public T GetAssetGo(AssetInfo info)
        {
            if(_cache.ContainsKey(info))
            {
                return _cache[info];
            }
            return default(T);
        }

        public void OnLoaded(AssetInfo info, object obj)
        {
            var go = obj as T;
            _cache[info] = go;
            if(info == _weaponInfo)
            {
                OnWeaponLoaded(go);
                return;
            }
            else
            {
                foreach(var data in _partAssetDic)
                {
                    if(info == data.Value)
                    {
                        OnPartLoaded(go, data.Key);
                        return;
                    }
                }
            }
            OnMismatchLoaded(info);
        }

        private void OnMismatchLoaded(AssetInfo asset)
        {
            _weaponModelLoader.UnloadAsset(asset);
        }

        private void OnWeaponLoaded(T go)
        {
#if !UnitTest
            if(Logger.IsDebugEnabled)
            {
                Logger.DebugFormat("on weapon loaded");
            }
#endif
            _weaponGo = go;
            BoneTool.CacheTransform(go as GameObject);
            if (_waitPartGoDic.Count > 0)
            {
                foreach(var pair in _waitPartGoDic)
                {
                    Attach(pair.Key, pair.Value, _weaponGo);
                }
                _waitPartGoDic.Clear();
            }
            else
            {
                ShowWeapon(go);
            }
       }

        private void OnPartLoaded(T go, WeaponPartLocation partType)
        {
            BoneTool.CacheTransform(go as GameObject);
            _partGoDic[partType] = go;
            if(null != _weaponGo)
            {
                Attach(partType, go, _weaponGo);
            }
            else
            {
                _waitPartGoDic[partType] = go;
            }

            if (partType.Equals(WeaponPartLocation.Scope)) 
            {
                _weaponModelAssemblyController.RefreshRemovableAttachment(_weaponGo as GameObject,true);
            }
           
        }

        public void Attach(WeaponPartLocation partLocation, T partGo, T weaponGo)
        {
            _weaponModelAssemblyController.Attach(partLocation, partGo, weaponGo);
        }

        public void ShowWeapon(T weaponGo)
        {
            _weaponModelAssemblyController.ShowWeapon(weaponGo);
        }

        public void Clear()
        {
            _cache.Clear();
            _partAssetDic.Clear();
            _partGoDic.Clear();
        }

        public void LoadWeapon(AssetInfo asset)
        {
#if !UnitTest
            if(Logger.IsDebugEnabled)
            {
                Logger.DebugFormat("LoadWeapon {0}", asset);
            }
#endif
            _weaponInfo = asset;
            _weaponModelLoader.LoadAsset(asset);
        }

        public void LoadPart(AssetInfo asset, WeaponPartLocation location)
        {
#if !UnitTest
            if (Logger.IsDebugEnabled)
            {
                Logger.DebugFormat("LoadPart {0} in {1}", asset, location);
            }
#endif
            _partAssetDic[location] = asset;
            _weaponModelLoader.LoadAsset(asset);
        }

        public void Unload(AssetInfo asset)
        {
#if !UnitTest
            if(Logger.IsDebugEnabled)
            {
                Logger.DebugFormat("Unload {0}", asset);
            }
#endif
            if(_weaponInfo == asset)
            {
                _weaponInfo = null;
                _weaponGo = null;
            }
            else
            {
                var location = WeaponPartLocation.EndOfTheWorld;
                foreach(var pair in _partAssetDic)
                {
                    if(pair.Value == asset)
                    {
                        location = pair.Key;
                    }
                }
                if(location != WeaponPartLocation.EndOfTheWorld)
                {
                    if(_partGoDic.ContainsKey(location))
                    {
                        _partGoDic.Remove(location);
                    }
                    if(_waitPartGoDic.ContainsKey(location))
                    {
                        _waitPartGoDic.Remove(location);
                    }
                    if(_partAssetDic.ContainsKey(location))
                    {
                        _partAssetDic.Remove(location);
                    }
                    else
                    {
#if !UnitTest
                        Logger.ErrorFormat("asset with location {0} doesn't exist in part asset dic ", location);
#endif
                    }
                    if (location.Equals(WeaponPartLocation.Scope))
                    {
                        _weaponModelAssemblyController.RefreshRemovableAttachment(_weaponGo as GameObject, false);
                    }
                }
            }
           _weaponModelLoader.UnloadAsset(asset);
        }
    }
}
