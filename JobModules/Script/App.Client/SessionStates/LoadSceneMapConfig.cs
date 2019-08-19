using App.Client.ClientSystems;
using App.Client.GameModules.ClientInit;
using App.Client.GameModules.SceneManagement;
using App.Client.GameModules.Terrain;
using App.Client.GameModules.UserInput;
using App.Client.SceneManagement;
using App.Client.SceneManagement.DistanceCulling;
using App.Shared;
using App.Shared.Components;
using App.Shared.Components.ClientSession;
using App.Shared.Configuration;
using App.Shared.EntityFactory;
using App.Shared.GameModules;
using App.Shared.GameModules.Configuration;
using App.Shared.GameModules.Preparation;
using App.Shared.SceneManagement.Streaming;
using Core;
using Core.EntityComponent;
using Core.GameModule.Module;
using Entitas;
using Core.Prediction.VehiclePrediction.TimeSync;
using Core.SessionState;
using Core.GameModule.System;
using I2.Loc;
using UnityEngine;
using Utils.Singleton;
using XmlConfig;

namespace App.Client.SessionStates
{
    public class LoadSceneMapConfig : AbstractSessionState
    {
        private CompositeGameModule _gameModule;
        private readonly bool _loadSceneAsap;
        private readonly bool _isServer;

        public override Systems CreateUpdateSystems(IContexts contexts)
        {
            
            Contexts _contexts = (Contexts) contexts;

            //SingletonManager.Get<MapConfigManager>().SetCurrentMap(_contexts.session.clientSessionObjects.MapId);

            _gameModule = CreateCompositeGameModule(_contexts);
            var systems = new Entitas.Systems();
            systems.Add(new ClientPreLoginFeature(
                "PreLoginState",
                _gameModule,
                _contexts.session.commonSession
                ));
            return systems;
        }

      

        private CompositeGameModule CreateCompositeGameModule(Contexts contexts)
        {
            var gameModule = new CompositeGameModule();

            var loadSceneSystem = new InitialSceneLoadingSystem(this, contexts, new StreamingGoByDistance(), _isServer);
            loadSceneSystem.AsapMode = _loadSceneAsap;
            gameModule.AddSystem(loadSceneSystem);
            gameModule.AddSystem(new InitTriggerObjectManagerSystem(contexts));
            gameModule.AddSystem(new GameObjectRecordSystem(contexts));
            gameModule.AddSystem(new ClientScenePostprocessorSystem(contexts.session.commonSession));
            gameModule.AddSystem(new TerrainRendererInitSystem(contexts.session.commonSession,
                contexts.session.clientSessionObjects));
            gameModule.AddSystem(new ClientWorldShiftPostProcessSystem(contexts.session.commonSession));

            gameModule.AddSystem(new TerrainDataLoadSystem(this, contexts));

            return gameModule;
        }

        public LoadSceneMapConfig(IContexts contexts, EClientSessionStates state, EClientSessionStates next,
            bool loadSceneAsap, bool isServer) : base(contexts,(int)state, (int) next)
        {
            _loadSceneAsap = loadSceneAsap;
            _isServer = isServer;
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
            get { return ScriptLocalization.client_loadtip.preparing; }
        }

    }
}