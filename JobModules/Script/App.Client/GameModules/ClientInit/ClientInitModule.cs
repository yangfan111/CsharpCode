using App.Client.GameModules.UserInput;
using App.Shared;
using App.Shared.GameModules.Configuration;
using Assets.App.Client.GameModules.UserInput;
using Utils.AssetManager;
using Core.Configuration;
using Core.GameModule.Module;
using Core.SessionState;
using Utils.Configuration;

namespace App.Client.GameModules.ClientInit
{
    

    public class ClientInitModule : GameModule
    {
        public ClientInitModule(Contexts contexts, ISessionState sessionState) 
        {
            AddSystem(new InputManagerConfigInitSystem(contexts, sessionState));
            AddSystem(new InputConfigInitSystem(contexts, sessionState));
        }
    }
}