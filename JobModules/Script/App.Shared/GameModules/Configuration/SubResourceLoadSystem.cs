using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using App.Shared.SessionStates;
using Core.SessionState;
using Core.Utils;
using Sharpen;
using UnityEngine;
using Utils.AssetManager;
using Utils.Configuration;
using Utils.Singleton;
using Object = UnityEngine.Object;

namespace App.Shared.GameModules.Configuration
{
    public delegate void OnSubResourcesHandled();


    public class SubResourceLoadSystem : ISubResourceLoadSystem
    {
        private static LoggerAdapter _logger = new LoggerAdapter(typeof(SubResourceLoadSystem));

        private AbstractSubResourceLoadHandler _resourceHandler;
        private ISessionState _sessionState;
        private IUnityAssetManager _assetManager;
        
        public SubResourceLoadSystem(ISessionState sessionState, AbstractSubResourceLoadHandler resourceHandler)
        {
            _sessionState = sessionState;
            _resourceHandler = resourceHandler;

           _sessionState.CreateExitCondition(ConditionString);

            SingletonManager.Get<SubProgressBlackBoard>().Add();
        }

        private string ConditionString
        {
            get { return string.Format("SubResourceLoadSystem : {0}", _resourceHandler.GetType()); }
        }

        public void OnLoadResources(IUnityAssetManager assetManager)
        {
            _assetManager = assetManager;
            _resourceHandler.LoadSubResources(_assetManager, Done);
        }

        private void Done()
        {
            _sessionState.FullfillExitCondition(ConditionString);
            SingletonManager.Get<SubProgressBlackBoard>().Step();
        }
    }
}
