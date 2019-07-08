//#define UnitTest
using System;
using System.Collections.Generic;
using Utils.AssetManager;
using Core.Utils;
using UnityEngine;

namespace Utils.Appearance.Weapon
{
    public class WeaponModelLoader : IWeaponModelLoader
    {
        private class LoadKey { }

        private static readonly LoggerAdapter Logger = new LoggerAdapter(typeof(WeaponModelLoader));
        IUnityAssetManager _assetManager;
        Action<AssetInfo, object> _loadedCb;
        Dictionary<AssetInfo, UnityObject> _loadedGo = new Dictionary<AssetInfo, UnityObject>(AssetInfo.AssetInfoComparer.Instance);
        Dictionary<AssetInfo, LoadKey> _loadKeys = new Dictionary<AssetInfo, LoadKey>(AssetInfo.AssetInfoComparer.Instance);

        public WeaponModelLoader(IUnityAssetManager assetManager)
        {
            _assetManager = assetManager;
        }
        
        public void LoadAsset(AssetInfo asset)
        {
#if !UnitTest
            if (Logger.IsDebugEnabled)
            {
                Logger.DebugFormat("LoadAsset {0}", asset);
            }
#endif
            if(!_loadKeys.ContainsKey(asset))
            {
                var loadKey = new LoadKey();
                _loadKeys[asset] = loadKey;
                _assetManager.LoadAssetAsync("WeaponModelLoader", asset, OnLoadSucc, new AssetLoadOption(recyclable: true));
            }
        }

        public void OnLoadSucc(string source, UnityObject unityObj)
        {
#if !UnitTest
            if(Logger.IsDebugEnabled)
            {
                Logger.DebugFormat("OnLoadSucc {0}", unityObj.Address);
            }
#endif
            if (!_loadKeys.ContainsKey(unityObj.Address))
            {
                _assetManager.Recycle(unityObj);
                return;
            }
            _loadKeys.Remove(unityObj.Address);
            var go = unityObj.AsGameObject;
            _loadedGo[unityObj.Address] = unityObj;
            AppearanceUtils.EnableRender(go);
            if (null != _loadedCb)
            {
                _loadedCb(unityObj.Address, go);
            }
            }

        public void RegisterLoadedCb(Action<AssetInfo, object> cb)
        {
            _loadedCb = cb;
        }

        public void UnloadAsset(AssetInfo asset)
        {
#if !UnitTest
            if(Logger.IsDebugEnabled)
            {
                Logger.DebugFormat("unload asset {0}", asset);
            }
#endif
            if(_loadedGo.ContainsKey(asset))
            {
                var unityObj = _loadedGo[asset];

                _loadedGo.Remove(asset);
                _assetManager.Recycle(unityObj);
            }
            else
            {
                if(_loadKeys.ContainsKey(asset))
                {
                    var key = _loadKeys[asset];
                    _assetManager.LoadCancel(key);
                    _loadKeys.Remove(asset);
                }
                else
                {
                    Logger.ErrorFormat("no load key for asset {0}", asset); 
                }
            }
        }
    }
}