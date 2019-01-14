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
        private List<Tuple<AssetInfo, UnityEngine.Object>> _loadedResources =  new List<Tuple<AssetInfo, Object>>();
        private OnSubResourcesHandled _handledCallback;
        private bool _exit = false;
        public AbstractSubResourceLoadHandler(ILoadRequestManager loadRequestManager)
        {
            _loadRequestManager = loadRequestManager;
        }

        protected void AddLoadRequest(AssetInfo asset)
        {
            if (!_assetSet.Contains(asset))
            {
                _loadRequestManager.AppendLoadRequest(null, asset, OnLoadSucc);
                _assetSet.Add(asset);
            }
        }

        public void LoadSubResources(List<Tuple<AssetInfo, UnityEngine.Object>> subResources, OnSubResourcesHandled handledCallback)
        {
            _handledCallback = handledCallback;
            LoadSubResourcesImpl(subResources);
        }

        public void OnLoadSucc(object source, AssetInfo assetInfo, UnityEngine.Object obj)
        {
            _assetSet.Remove(assetInfo);

            var previousExit = _exit;

            try
            {
                OnLoadSuccImpl(assetInfo, obj);
            }
            catch (Exception e)
            {
                _logger.Error(e);
            }

            if (!previousExit)
            {
                _loadedResources.Add(new Tuple<AssetInfo, Object>(assetInfo, obj));

                if ((_assetSet.Count == 0 ||  _exit) && _handledCallback != null)
                {
                    _handledCallback(_loadedResources);
                    _exit = true;
                    _loadedResources.Clear();
                }
            }
        }

        protected void ForceExit()
        {
            _exit = true;
        }

        protected abstract void LoadSubResourcesImpl(List<Tuple<AssetInfo, UnityEngine.Object>> subResources);
        protected abstract void OnLoadSuccImpl(AssetInfo assetInfo, UnityEngine.Object obj);
    }
}
