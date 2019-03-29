using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Core.Utils;
using UnityEngine;
using Utils.AssetManager;
using Object = UnityEngine.Object;

namespace App.Shared.GameModules.Configuration
{
    public abstract class AbstractSubResourceLoadHandler 
    {
        private static LoggerAdapter _logger = new LoggerAdapter(typeof(AbstractSubResourceLoadHandler));
        private IUnityAssetManager _assetManager;
        private HashSet<AssetInfo> _assetSet = new HashSet<AssetInfo>(AssetInfo.AssetInfoComparer.Instance);
        private HashSet<AssetInfo> _loadingAsset = new HashSet<AssetInfo>(AssetInfo.AssetInfoComparer.Instance);
        private OnSubResourcesHandled _handledCallback;

        protected bool AddLoadRequest(AssetInfo asset)
        {
            if (!_assetSet.Contains(asset))
            {
                _loadingAsset.Add(asset);
                _assetManager.LoadAssetAsync(GetType().ToString(), asset, OnLoadSucc);
                _assetSet.Add(asset);
                return true;
            }

            return false;
        }

        public void LoadSubResources(IUnityAssetManager assetManager, OnSubResourcesHandled handledCallback)
        {
            _assetManager = assetManager;
            _handledCallback = handledCallback;
            if (!LoadSubResourcesImpl())
            {
                _handledCallback();
            }
        }

        private void OnLoadSucc(string source, UnityObject unityObj)
        {
            var assetInfo = unityObj.Address;

            _loadingAsset.Remove(assetInfo);

            try
            {
                OnLoadSuccImpl(unityObj);
            }
            catch (Exception e)
            {
                _logger.Error(e);
            }


            if (_loadingAsset.Count == 0 && _handledCallback != null)
            {
                _handledCallback();
            }

        }

        //return wether we have asset to load
        protected abstract bool LoadSubResourcesImpl();
        protected abstract void OnLoadSuccImpl(UnityObject unityObj);
    }
}
