using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using App.Client.GameModules.ClientInit;
using App.Client.GameModules.OC;
using App.Shared.Components;
using App.Shared.GameModules;
using App.Shared.GameModules.Configuration;
using Core.GameModule.Module;
using Core.SessionState;
using Entitas;
using I2.Loc;

namespace App.Client.SessionStates
{
    public class LoadOCConfigState : AbstractSessionState
    {

        public LoadOCConfigState(IContexts contexts, EClientSessionStates state, EClientSessionStates next) : base(
            contexts, (int) state, (int) next)
        {
            
        }

        public override Systems CreateUpdateSystems(IContexts contexts)
        {
            Contexts _contexts = (Contexts)contexts;

            //SingletonManager.Get<MapConfigManager>().SetCurrentMap(_contexts.session.clientSessionObjects.MapId);

            var gameModule = CreateCompositeGameModule(_contexts);
            var systems = new Entitas.Systems();
            systems.Add(new ClientPreLoginFeature(
                "LoadOCConfigState",
                gameModule,
                _contexts.session.commonSession
            ));
            return systems;
        }

        private CompositeGameModule CreateCompositeGameModule(Contexts contexts)
        {
            var gameModule = new CompositeGameModule();


            gameModule.AddSystem(new InitOcclusionCullingControllerSystem(this, contexts));

            return gameModule;
        }


        public override int LoadingProgressNum
        {
            get { return 1; }
        }

        public override string LoadingTip
        {
            get { return ScriptLocalization.client_loadtip.preparing; }
        }
    }
}
