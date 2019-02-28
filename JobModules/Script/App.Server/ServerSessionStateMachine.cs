using App.Client.ClientGameModules.SceneManagement;
using App.Server.Damage;
using App.Server.GameModules.Bullet;
using App.Server.GameModules.GamePlay;
using App.Server.GameModules.Player;
using App.Server.GameModules.SceneObject;
using App.Server.GameModules.Vehicle;
using App.Server.ServerInit;
using App.Shared.GameModules;
using App.Shared.GameModules.Attack;
using App.Shared.GameModules.Bullet;
using App.Shared.GameModules.Configuration;
using App.Shared.GameModules.Preparation;
using App.Shared.GameModules.Throwing;
using App.Shared.SessionStates;
using App.Shared.VechilePrediction;
using Assets.App.Shared.GameModules.Camera.Utils;
using Core.AssetManager;
using Core.Compensation;
using Core.Configuration;
using Core.GameModule.Module;
using Core.GameModule.System;
using Core.Prediction.UserPrediction;
using Core.Prediction.VehiclePrediction;
using Core.SessionState;
using Entitas;
using Utils.Configuration;
using Utils.Singleton;

namespace App.Server
{
    public enum EServerSessionStates
    {
        LoadConfig,
        LoadSubResourceConfig,
        LoadSceneMapConfig,
        PreLoad,
        Gaming,
    }

    public class ServerLoadConfigurationState : AbstractSessionState
    {
        public ServerLoadConfigurationState(IContexts contexts, EServerSessionStates state, EServerSessionStates next) : base(
            contexts, (int)state, (int)next)
        {
        }

        public override Systems CreateUpdateSystems(IContexts contexts1)
        {
            var gameModule = new CompositeGameModule();
            var contexts = contexts1 as Contexts;
            gameModule.AddModule(new ServerInitModule(contexts, this));
            gameModule.AddModule(new BaseConfigurationInitModule(
                contexts, this));

            var featrue = new ServerPrepareFeature("loadConfig", gameModule, contexts.session.commonSession);
            return featrue;
        }
    }

    public class ServerLoadSubResourceState : AbstractSessionState
    {

        public ServerLoadSubResourceState(IContexts contexts, EServerSessionStates state, EServerSessionStates next) : base(
            contexts, (int)state, (int)next)
        {
        }

        public override Systems CreateUpdateSystems(IContexts contexts1)
        {
            var gameModule = new CompositeGameModule();
            var contexts = contexts1 as Contexts;
            gameModule.AddModule(new ServerInitModule(contexts, this));
            gameModule.AddModule(new SubResourceConfigurationInitModule(this));

            var featrue = new ServerPrepareFeature("loadSubResourceConfig", gameModule, contexts.session.commonSession);
            return featrue;

        }
    }


    public class ServerLoadMapConfigState : AbstractSessionState
    {
        public override Systems CreateUpdateSystems(IContexts contexts1)
        {
            var gameModule = new CompositeGameModule();
            var contexts = contexts1 as Contexts;

            var loadSceneSystem = new InitialSceneLoadingSystem(this, contexts, null, true);
            loadSceneSystem.AsapMode = true;
            gameModule.AddSystem(loadSceneSystem);
            gameModule.AddSystem(new InitTriggerObjectManagerSystem(contexts));

            gameModule.AddSystem(new ServerScenePostprocessorSystem(contexts.session.commonSession));
            //gameModule.AddModule(new ResourcePreloadModule(this));
            var featrue = new ServerPrepareFeature("loadMapConfig", gameModule, contexts.session.commonSession);
            return featrue;
        }

        public ServerLoadMapConfigState(IContexts contexts, EServerSessionStates state, EServerSessionStates next) : base(
            contexts, (int) state, (int) next)
        {
        }
    }

    public class GameSessionState : AbstractSessionState
    {
        private CompositeGameModule _gameModule;
        private ServerRoom room;


        public override Systems CreateUpdateSystems(IContexts contexts1)
        {
            var contexts = contexts1 as Contexts;
            var sessionObjects = contexts.session.commonSession;
            var entityIdGenerator = sessionObjects.EntityIdGenerator;


            _gameModule = new CompositeGameModule();

            _gameModule.AddModule(new ServerPlayerModule(contexts));
            _gameModule.AddModule(new ServerEntityInitModule(contexts));
            _gameModule.AddModule(new ServerBulletModule(contexts));

            IHitBoxEntityManager hitBoxEntityManager = new HitBoxEntityManager(contexts,true);
            var snapshotSelectorContainer = contexts.session.serverSessionObjects.CompensationSnapshotSelector;
            ICompensationWorldFactory factory =
                new ServerCompensationWorldFactory(snapshotSelectorContainer, hitBoxEntityManager);
            var serverDamageInfoCollector = new ServerDamageInfoCollector(contexts.player);
            _gameModule.AddModule(new UserCmdGameModule(contexts,
                factory,
                new BulletHitHandler(contexts,
                    entityIdGenerator,
                    room.PlayerDamager,
                    serverDamageInfoCollector,
                    contexts.session.entityFactoryObject.SoundEntityFactory,
                    SingletonManager.Get<EnvironmentTypeConfigManager>()),
                new MeleeHitHandler(contexts, room.PlayerDamager, entityIdGenerator,
                    serverDamageInfoCollector, contexts.session.entityFactoryObject.SoundEntityFactory),
                new ThrowingHitHandler(contexts, room.PlayerDamager),
                sessionObjects,
                MotorsFactory.CraeteMotors(contexts, SingletonManager.Get<CameraConfigManager>().Config)));
            _gameModule.AddModule(new ServerPostPredictionModule(contexts));
            _gameModule.AddModule(new ServerVehicleModule(contexts,
                contexts.session.serverSessionObjects.VehicleTimer));
            _gameModule.AddModule(new ServerGamePlayModule(contexts, room));
            _gameModule.AddModule(
                new ServerSceneObjectModule(contexts, this, sessionObjects.EquipmentEntityIdGenerator));

            IUserCmdExecuteSystemHandler userCmdExecuteSystemHandler = new UserCmdExecuteSystemHandler(contexts);
            IVehicleCmdExecuteSystemHandler vehicleCmdExecuteSystemHandler =
                new ServerVehicleCmdExecuteSystemHandler(contexts);

            var systems = new Feature("GameSessionState");
            systems.Add(new PingSystem(contexts));
            systems.Add(new ServerMainFeature(
                "ServerSystems",
                _gameModule,
                userCmdExecuteSystemHandler,
                vehicleCmdExecuteSystemHandler,
                contexts.session.serverSessionObjects.SimulationTimer,
                new VehicleExecutionSelector(contexts),
                sessionObjects,
                room));

            return systems;
        }

        public override Systems CreateLateUpdateSystems(IContexts contexts)
        {
            Systems system = new Systems();
            system.Add(new LateUpdateSystem(_gameModule));
            return system;
        }

        public GameSessionState(IContexts contexts, ServerRoom room, EServerSessionStates state,
            EServerSessionStates next) : base(contexts, (int) state, (int) next)
        {
            this.room = room;
        }
    }

    public class ServerPreLoadState : BasePreLoadState
    {
        public ServerPreLoadState(IContexts contexts, EServerSessionStates state, EServerSessionStates next) : base(
            contexts, (int) state, (int) next)
        {
        }
        
        public override Systems CreateUpdateSystems(IContexts contexts)
        {
            Contexts _contexts = (Contexts)contexts;
     
            var systems = base.CreateUpdateSystems(contexts);
            systems.Add(new PreloadFeature("ServerPreLoadState", CreateSystems(_contexts), _contexts.session.commonSession));
            
            return systems;
        }

        private IGameModule CreateSystems(Contexts contexts)
        {
            GameModule module = new GameModule();
            module.AddSystem(new InitMapIdSystem(contexts));

            return module;
        }
        
        public sealed class PreloadFeature : Feature
        {
            public PreloadFeature(string name,
                IGameModule topLevelGameModule,
                ICommonSessionObjects commonSessionObjects) : base(name)
            {
                topLevelGameModule.Init();
            
                Add(new ModuleInitSystem(topLevelGameModule, commonSessionObjects.AssetManager));
                Add(new UnityAssetManangerSystem(commonSessionObjects));
                Add(new ResourceLoadSystem(topLevelGameModule, commonSessionObjects.AssetManager));
            }
        } 
    }

 
    public class ServerSessionStateMachine : SessionStateMachine
    {
        public ServerSessionStateMachine(Contexts contexts, ServerRoom room):
            base(new ServerSessionStateMachineMonitor(contexts, room))
        {
            AddState(new ServerLoadConfigurationState(contexts, EServerSessionStates.LoadConfig, EServerSessionStates.LoadSubResourceConfig));
            AddState(new ServerLoadSubResourceState(contexts, EServerSessionStates.LoadSubResourceConfig, EServerSessionStates.PreLoad));
            AddState(new ServerPreLoadState(contexts, EServerSessionStates.PreLoad, EServerSessionStates.LoadSceneMapConfig));
            AddState(new ServerLoadMapConfigState(contexts, EServerSessionStates.LoadSceneMapConfig, EServerSessionStates.Gaming));
            AddState(new GameSessionState(contexts, room, EServerSessionStates.Gaming, EServerSessionStates.Gaming));
            Initialize((int) EServerSessionStates.LoadConfig);
        }
    }
}