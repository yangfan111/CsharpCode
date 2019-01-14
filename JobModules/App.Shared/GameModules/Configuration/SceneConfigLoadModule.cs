using System;
using App.Shared.Components;
using App.Shared.Configuration;
using Assets.Core.Configuration;
using Assets.Utils.Configuration;
using Utils.AssetManager;
using Core.Configuration;
using Core.Configuration.Sound;
using Core.Configuration.Terrains;
using Core.GameModule.Module;
using Core.GameModule.System;
using Core.SessionState;
using Utils.SettingManager;

namespace App.Shared.GameModules.Configuration
{
   
    
    public class SceneConfigLoadModule : GameModule
    {
        public bool _isServer; 
        public SceneConfigLoadModule(Contexts context, ISessionState sessionState,  ICommonSessionObjects sessionObjects, bool IsServer)
        {
            _isServer = IsServer;
        

            AddSystem(new SceneConfigInitSystem(sessionState, sessionObjects));
        }
    }
}
