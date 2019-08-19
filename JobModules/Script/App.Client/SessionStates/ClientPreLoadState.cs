using App.Client.GameModules.Effect;
using App.Shared.Components;
using App.Shared.GameModules.Configuration;
using App.Shared.GameModules.Preparation;
using Core.GameModule.Module;
using Core.GameModule.System;
using Core.SessionState;
using Entitas;
using I2.Loc;

namespace App.Client.SessionStates
{
    public class ClientPreLoadState : AbstractSessionState
    {
        public ClientPreLoadState(IContexts contexts, EClientSessionStates state, EClientSessionStates next) : base(contexts, (int) state, (int) next)
        {
        }

        public override Systems CreateUpdateSystems(IContexts contexts)
        {
            Contexts _contexts = (Contexts) contexts;
            var systems = new Feature("ClientPreLoadState");
            systems.Add(new PreloadFeature("ClientPreLoadState", CreateSystems(_contexts, this), _contexts.session.commonSession));

            return systems;
        }

        public override string LoadingTip
        {
            get { return ScriptLocalization.client_loadtip.preload; }
        }

        private IGameModule CreateSystems(Contexts contexts, ISessionState sessionState)
        {
            GameModule module = new GameModule();
            module.AddSystem(new InitMapIdSystem(contexts));
            module.AddSystem(new GlobalEffectManagerInitSystem(contexts, sessionState));
            return module;
        }

        public sealed class PreloadFeature : Feature
        {
            public PreloadFeature(string name, IGameModule topLevelGameModule, ICommonSessionObjects commonSessionObjects) : base(name)
            {
                topLevelGameModule.Init();
                Add(new ModuleInitSystem(topLevelGameModule, commonSessionObjects.AssetManager));
                Add(new ResourceLoadSystem(topLevelGameModule, commonSessionObjects.AssetManager));
            }
        }
    }
}