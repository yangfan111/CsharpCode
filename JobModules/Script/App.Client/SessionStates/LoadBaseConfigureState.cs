using App.Client.ClientSystems;
using App.Client.GameModules.ClientInit;
using App.Client.GameModules.UserInput;
using App.Shared;
using App.Shared.Components;
using App.Shared.Components.ClientSession;
using App.Shared.EntityFactory;
using App.Shared.GameModules;
using App.Shared.GameModules.Configuration;
using Core;
using Core.EntitasAdpater;
using Core.GameModule.Module;
using Entitas;
using Core.Prediction.VehiclePrediction.TimeSync;
using Core.SessionState;
using Core.GameModule.System;
using I2.Loc;
using UnityEngine;

namespace App.Client.SessionStates
{
    public class LoadBaseConfigureState : AbstractSessionState
    {
        private CompositeGameModule _gameModule;


        public override Systems CreateUpdateSystems(IContexts contexts)
        {
            
            Contexts _contexts = (Contexts) contexts;

            //SingletonManager.Get<MapConfigManager>().SetCurrentMap(_contexts.session.clientSessionObjects.MapId);

            _gameModule = CreateCompositeGameModule(_contexts);
            var systems = new Entitas.Systems();
            systems.Add(new LocalizeSessionSystem(this, _contexts.session.commonSession.AssetManager));
            systems.Add(new BaseConfigurationInitModule(this, _contexts.session.commonSession.AssetManager));
            systems.Add(new ClientPreLoginFeature(
                "BasePreparing",
                _gameModule,
                _contexts.session.commonSession
                ));
            return systems;
        }

      

        private CompositeGameModule CreateCompositeGameModule(Contexts contexts)
        {
            var gameModule = new CompositeGameModule();
            
           
            gameModule.AddModule(new ClientInitModule(contexts, this));

            return gameModule;
        }

        public LoadBaseConfigureState(IContexts contexts, EClientSessionStates state, EClientSessionStates next) : base(contexts,(int)state, (int) next)
        {
           
        }

        public override Systems CreateOnGuiSystems(IContexts contexts)
        {
            Systems system = new Systems();
            system.Add(new OnGuiSystem(_gameModule));
            return system;
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