using App.Shared;
using App.Shared.GameModules.Configuration;
using Core.GameModule.Module;
using Core.SessionState;

namespace App.Server.ServerInit
{
    public class ServerInitModule : GameModule
    {
        public ServerInitModule(Contexts contexts, ISessionState sessionState) 
        {
            //AddSystem(new ClientWeaponConfigInitSystem(contexts.serverSession.sessionObjects, sessionState));
        }
    }

    public class ServerEntityInitModule : GameModule
    {
        public ServerEntityInitModule(Contexts contexts)
        {
           
            AddSystem(new ServerEntitiesInitSystem(contexts));

        }
    }
}