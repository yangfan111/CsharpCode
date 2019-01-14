using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Core.Utils;
using Sharpen;
using UnityEngine;
using Utils.AssetManager;
using Object = UnityEngine.Object;

namespace App.Shared.GameModules.Configuration
{
    public delegate void OnSubResourcesHandled(List<Tuple<AssetInfo, UnityEngine.Object>> subResources);

    public delegate void SubReousourcesHandler(List<Tuple<AssetInfo, UnityEngine.Object>> subResources, OnSubResourcesHandled handledCallback);

    public class SubResourceLoadSystem : ISubResourceLoadSystem
    {
        private static LoggerAdapter _logger = new LoggerAdapter(typeof(SubResourceLoadSystem));

        private SubReousourcesHandler _resourceHandler;
        private bool _isDone = false;
        private ISubResourceLoadSystem _subSystem;
        
        public SubResourceLoadSystem(SubReousourcesHandler resourceHandler)
        {
            _resourceHandler = resourceHandler;
            _isDone = resourceHandler == null;
        }


        public bool IsDone
        {
            get { return _isDone && (_subSystem == null || _subSystem.IsDone); }
        }

        public ISubResourceLoadSystem Chain(SubReousourcesHandler responseHandler)
        {
            if (_subSystem != null)
            {
                throw new RuntimeException("The SubSystem Has Existed!");
            }

            _subSystem = new SubResourceLoadSystem(responseHandler);
            return _subSystem;
        }

        public void OnLoadSucc(AssetInfo assetInfo, UnityEngine.Object obj)
        {
            if (_resourceHandler != null)
            {
                var subResource =
                    new List<Tuple<AssetInfo, UnityEngine.Object>>() {new Tuple<AssetInfo, Object>(assetInfo, obj)};
                OnLoadSucc(subResource);
            }
        }

        public void OnLoadSucc(List<Tuple<AssetInfo, UnityEngine.Object>> subResources)
        {
            if (_resourceHandler != null)
            {
                var resources = new List<Tuple<AssetInfo, UnityEngine.Object>>();

                foreach (var resource in subResources)
                {
                    if (resource.Item2 == null)
                    {
                        _logger.WarnFormat("Loaded Object SubResource {0} is NULL", resource.Item1);
                        continue;
                    }

                    _logger.InfoFormat("SubResource {0} Loaded", resource.Item1);
                    resources.Add(resource);
                }

                if (resources.Count > 0)
                    _resourceHandler(resources, Done);
                else
                    _isDone = true;
            }
            else
            {
                _isDone = true;
            }

        }

        private void Done(List<Tuple<AssetInfo, UnityEngine.Object>> subResources)
        {
            _isDone = true;
            if(_subSystem  != null)
                _subSystem.OnLoadSucc(subResources);
        }
    }
}
