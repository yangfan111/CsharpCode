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
        private ILoadRequestManager _loadRequestManager;
        private HashSet<AssetInfo> _assetSet = new HashSet<AssetInfo>(AssetInfo.AssetInfoComparer.Instance);
        private OnSubResourcesHandled _handledCallback;

        protected void AddLoadRequest(AssetInfo asset)
        {
            if (!_assetSet.Contains(asset))
            {
                _loadRequestManager.AppendLoadRequest(null, asset, OnLoadSucc);
                _assetSet.Add(asset);
            }
        }

        public void LoadSubResources(ILoadRequestManager loadRequestManager, OnSubResourcesHandled handledCallback)
        {
            _loadRequestManager = loadRequestManager;
            _handledCallback = handledCallback;
            if (!LoadSubResourcesImpl())
            {
                _handledCallback();
            }
        }

        private void OnLoadSucc(object source, AssetInfo assetInfo, UnityEngine.Object obj)
        {
            _assetSet.Remove(assetInfo);

            try
            {
                OnLoadSuccImpl(assetInfo, obj);
            }
            catch (Exception e)
            {
                _logger.Error(e);
            }


            if (_assetSet.Count == 0 && _handledCallback != null)
            {
                _handledCallback();
            }

        }

        //return wether we have asset to load
        protected abstract bool LoadSubResourcesImpl();
        protected abstract void OnLoadSuccImpl(AssetInfo assetInfo, UnityEngine.Object obj);
    }
}
