using App.Shared.Components;
using App.Shared.GameModules;
using App.Shared.GameModules.Configuration;
using Core.SessionState;
using Entitas;
using I2.Loc;

namespace App.Client.SessionStates
{
    public class LoadSubResourceConfigureState : AbstractSessionState
    {

        public LoadSubResourceConfigureState(IContexts contexts, EClientSessionStates stateId, EClientSessionStates nextStateId):
            base(contexts, (int) stateId, (int) nextStateId)
        {
            
        }

        public override Systems CreateUpdateSystems(IContexts contexts)
        {
            Contexts _contexts = (Contexts)contexts;

            //SingletonManager.Get<MapConfigManager>().SetCurrentMap(_contexts.session.clientSessionObjects.MapId);

            var gameModule = new SubResourceConfigurationInitModule(this);
            var systems = new Entitas.Systems();
            systems.Add(new ClientPreLoginFeature(
                "SubResourceLoad",
                gameModule,
                _contexts.session.commonSession
            ));
            return systems;
        }

        public override int LoadingProgressNum
        {
            get { return 1; }
        }

        public override string LoadingTip
        {
            get { return ScriptLocalization.client_loadtip.basepreparing; }
        }
    }
}
