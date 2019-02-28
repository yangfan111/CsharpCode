using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using App.Shared.Components;
using Core.AssetManager;
using Core.GameModule.Interface;
using Core.GameModule.Module;
using Core.GameModule.System;
using Core.SessionState;
using Core.Utils;
using Entitas;
using UnityEngine;
using Utils.AssetManager;
using Utils.Singleton;

namespace App.Shared.SessionStates
{
    internal class PreLoadSystem : IResourceLoadSystem
    {
        private static LoggerAdapter _logger = new LoggerAdapter(typeof(PreLoadSystem));

        private ISessionState _sessionState;
        private IAssetInfoProvider _assetInfoProvider;
        private bool _isLoading = false;
        private int _loadingCount = 0;
        private IUnityAssetManager _assetManager;
        public PreLoadSystem(ISessionState sessionState,  IAssetInfoProvider assetInfoProvider)
        {
            _sessionState = sessionState;
            _assetInfoProvider = assetInfoProvider;

            _sessionState.CreateExitCondition(typeof(PreLoadSystem));

            SingletonManager.Get<SubProgressBlackBoard>().Add((uint) _assetInfoProvider.AssetInfos.Count);
        }

        public void OnLoadResources(IUnityAssetManager assetManager)
        {
            if (!_isLoading)
            {
                _assetManager = assetManager;

                var assetInfos = _assetInfoProvider.AssetInfos;
                foreach (var assetInfo in assetInfos)
                {
                    _assetManager.LoadAssetAsync("PreLoadSystem", assetInfo, OnLoadSucc);
                }

                _loadingCount = assetInfos.Count;
                _isLoading = true;

                if (_loadingCount == 0)
                {
                    _sessionState.FullfillExitCondition(typeof(PreLoadSystem));
                }

                _logger.InfoFormat("Loading count {0}", _loadingCount);
            }
        }

        public void OnLoadSucc(string source, UnityObject unityObj)
        {
            SingletonManager.Get<SubProgressBlackBoard>().Step();
            _loadingCount--;

            if (unityObj.AsObject == null)
            {
                _logger.ErrorFormat("Preload asset {0} failed", unityObj.Address);
            }
            else
            {
                _assetManager.Recycle(unityObj);
            }

            if (_loadingCount <= 0)
            {
                _sessionState.FullfillExitCondition(typeof(PreLoadSystem));
            }
        }
    }

    

    internal class PreLoadModule : GameModule
    {
        public PreLoadModule(ISessionState sessionState, IContexts contexts)
        {
            var commonSessionObjects = (contexts as Contexts).session.commonSession;
            AddSystem(new PreLoadSystem(sessionState, new PreLoadAssetProvider(commonSessionObjects)));
        }
    }

    public class BasePreLoadState : AbstractSessionState
    {
       

        
        public BasePreLoadState(IContexts contexts, int state, int next) : base(contexts,(int)state, (int) next)
        {
            
        }

        public override Systems CreateUpdateSystems(IContexts contexts)
        {
            var systems = new Feature("PreLoadingState");
            var contextsImpl = contexts as Contexts;
            var commonSession = contextsImpl.session.commonSession;
            systems.Add(new UnityAssetManangerSystem(commonSession));
            systems.Add(new ResourceLoadSystem(
                new PreLoadModule(this,  contexts),
                commonSession.AssetManager));
           
            return systems;
        }

        public override int LoadingProgressNum
        {
            get { return 1; }
        }

        public override string LoadingTip
        {
            get { return ""; }
        }
    }
}
