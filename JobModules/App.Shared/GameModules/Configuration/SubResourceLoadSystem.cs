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
        private ILoadRequestManager _loadRequestManager;
        
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

        public void OnLoadResources(ILoadRequestManager loadRequestManager)
        {
            _loadRequestManager = loadRequestManager;
            _resourceHandler.LoadSubResources(_loadRequestManager, Done);
        }

        private void Done()
        {
            _sessionState.FullfillExitCondition(ConditionString);
            SingletonManager.Get<SubProgressBlackBoard>().Step();
        }
    }
}
