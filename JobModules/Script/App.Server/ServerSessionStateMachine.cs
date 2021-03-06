﻿using App.Client.ClientGameModules.SceneManagement;
using App.Server.GameModules.GamePlay;
using App.Server.GameModules.Player;
using App.Server.GameModules.SceneObject;
using App.Server.GameModules.Vehicle;
using App.Server.ServerInit;
using App.Shared;
using App.Shared.Components;
using App.Shared.GameModules;
using App.Shared.GameModules.Attack;
using App.Shared.GameModules.Configuration;
using App.Shared.GameModules.Preparation;
using App.Shared.SessionStates;
using App.Shared.VechilePrediction;
using Assets.App.Shared.GameModules.Camera.Utils;
using Core.Compensation;
using Core.Configuration;
using Core.GameModule.Module;
using Core.GameModule.System;
using Core.Prediction.UserPrediction;
using Core.Prediction.VehiclePrediction;
using Core.SessionState;
using Core.Utils;
using Entitas;
using UnityEngine;
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
        LoadOptionConfig,
    }

    public class ServerLoadConfigurationState : AbstractSessionState
    {
        public ServerLoadConfigurationState(IContexts contexts, EServerSessionStates state, EServerSessionStates next) :
                        base(contexts, (int) state, (int) next)
        {
        }

        public override Systems CreateUpdateSystems(IContexts contexts1)
        {
            var gameModule = new CompositeGameModule();
            var contexts   = contexts1 as Contexts;
            gameModule.AddModule(new ServerInitModule(contexts, this));

            var feature = new Feature("ServerLoadConfigurationState");
            feature.Add(new ServerPrepareFeature("loadConfig", gameModule,
            contexts.session.commonSession.AssetManager));
            feature.Add(new BaseConfigurationInitModule(this, contexts.session.commonSession.AssetManager));
            return feature;
        }
    }

    public class ServerLoadSubResourceState : AbstractSessionState
    {
        public ServerLoadSubResourceState(IContexts contexts, EServerSessionStates state, EServerSessionStates next) :
                        base(contexts, (int) state, (int) next)
        {
        }

        public override Systems CreateUpdateSystems(IContexts contexts1)
        {
            var gameModule = new CompositeGameModule();
            var contexts   = contexts1 as Contexts;
            gameModule.AddModule(new ServerInitModule(contexts, this));
            gameModule.AddModule(new SubResourceConfigurationInitModule(this));

            var featrue = new ServerPrepareFeature("loadSubResourceConfig", gameModule,
            contexts.session.commonSession.AssetManager);
            return featrue;
        }
    }

    public class ServerLoadMapConfigState : AbstractSessionState
    {
        public ServerLoadMapConfigState(IContexts contexts, EServerSessionStates state, EServerSessionStates next) :
                        base(contexts, (int) state, (int) next)
        {
        }

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

            var featrue = new ServerPrepareFeature("loadMapConfig", gameModule, contexts.session.commonSession.AssetManager);
            featrue.Add(new MapConfigInitModule(contexts, this));
            if (contexts.session.commonSession.RoomInfo.MapId == 2){
                contexts.session.commonSession.InitPosition = new Vector3(1906, 20, -1964);
            }else {
                contexts.session.commonSession.InitPosition = Vector3.zero;
            }
            return featrue;
        }
    }

    public class GameSessionState : AbstractSessionState
    {
        private CompositeGameModule _gameModule;
        private ServerRoom room;

        public GameSessionState(IContexts contexts, ServerRoom room, EServerSessionStates state,
                                EServerSessionStates next) : base(contexts, (int) state, (int) next)
        {
            this.room = room;
        }


        public override Systems CreateUpdateSystems(IContexts contexts1)
        {
            var contexts          = contexts1 as Contexts;
            var sessionObjects    = contexts.session.commonSession;
            var entityIdGenerator = sessionObjects.EntityIdGenerator;


            _gameModule = new CompositeGameModule();

            _gameModule.AddModule(new ServerPlayerModule(contexts));
            //if (GameRules.IsChicken(contexts.session.commonSession.RoomInfo.ModeId))
            //{
               _gameModule.AddModule(new ServerEntityInitModule(contexts));
            //}

            IHitBoxEntityManager hitBoxEntityManager = new HitBoxEntityManager(contexts, true);
            var SnapshotSelector =
                            contexts.session.serverSessionObjects.SnapshotSelector;
            ICompensationWorldFactory factory =
                            new ServerCompensationWorldFactory(SnapshotSelector, hitBoxEntityManager);
            _gameModule.AddModule(new UserCmdGameModule(contexts, factory,
            new BulletHitHandler(contexts, entityIdGenerator, room.PlayerDamager),
            new MeleeHitHandler(room.PlayerDamager),
            new ThrowingHitHandler(room.PlayerDamager), sessionObjects,
            MotorsFactory.CraeteMotors(contexts, SingletonManager.Get<CameraConfigManager>().Config)));
            _gameModule.AddModule(new ServerPostPredictionModule(contexts));
            _gameModule.AddModule(new ServerVehicleModule(contexts,
            contexts.session.serverSessionObjects.VehicleTimer));
            _gameModule.AddModule(new ServerGamePlayModule(contexts, room));
            _gameModule.AddModule(
            new ServerSceneObjectModule(contexts, this, sessionObjects.EquipmentEntityIdGenerator));

            IServerUserCmdList serrverServerUserCmdList = new ServerServerUserCmdList(contexts);
            IVehicleCmdExecuteSystemHandler vehicleCmdExecuteSystemHandler =
                            new ServerVehicleCmdExecuteSystemHandler(contexts);

            var systems = new Feature("GameSessionState");
            systems.Add(new PingSystem(contexts));
            systems.Add(new ServerMainFeature("ServerSystems", _gameModule, serrverServerUserCmdList,
            vehicleCmdExecuteSystemHandler, contexts.session.serverSessionObjects.SimulationTimer,
            new VehicleExecutionSelector(contexts), sessionObjects, room));

            return systems;
        }

        public override Systems CreateLateUpdateSystems(IContexts contexts)
        {
            Systems system = new Systems();
            system.Add(new LateUpdateSystem(_gameModule));
            return system;
        }
    }

    public class ServerPreLoadState : BasePreLoadState
    {
        public ServerPreLoadState(IContexts contexts, EServerSessionStates state, EServerSessionStates next) : base(contexts, (int) state, (int) next)
        {
        }

        public override Systems CreateUpdateSystems(IContexts contexts)
        {
            Contexts _contexts = (Contexts) contexts;
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
            public PreloadFeature(string name, IGameModule topLevelGameModule, ICommonSessionObjects commonSessionObjects) : base(name)
            {
                topLevelGameModule.Init();
                Add(new ModuleInitSystem(topLevelGameModule, commonSessionObjects.AssetManager));
                Add(new ResourceLoadSystem(topLevelGameModule, commonSessionObjects.AssetManager));
            }
        }
    }

    public class ServerOptionLoadState : AbstractSessionState
    {
        public ServerOptionLoadState(IContexts contexts, EServerSessionStates state, EServerSessionStates next) : base(contexts, (int)state, (int)next)
        {
        }

        public override Systems CreateUpdateSystems(IContexts contexts)
        {
            Contexts _contexts = (Contexts)contexts;
            var systems = new Feature("ServerOptionLoadState");
            systems.Add(new OptionConfigurationInitModule(this, _contexts.session.commonSession.AssetManager));
            return systems;
        }
    }

    public class ServerSessionStateMachine : SessionStateMachine
    {
        private static LoggerAdapter _logger = new LoggerAdapter(typeof(ServerSessionStateMachine));

        public ServerSessionStateMachine(Contexts contexts, ServerRoom room) : base(
        new ServerSessionStateMachineMonitor(room))
        {
            BackroundloadSettings defaultSettings = BackroundloadSettings.GetServerCurrentSettings();
            AddState(new ServerLoadConfigurationState(contexts, EServerSessionStates.LoadConfig,
            EServerSessionStates.LoadSubResourceConfig).WithEnterAction(() =>
            {
                BackroundloadSettings.SetCurrentSettings(BackroundloadSettings.LoadSettsings);
                //Application.targetFrameRate = 1000;
                _logger.WarnFormat("LoadConfig backgroundLoadingPriority:{0}, targetFrameRate:{1}",
                Application.backgroundLoadingPriority, Application.targetFrameRate);
            }));
            AddState(new ServerLoadSubResourceState(contexts, EServerSessionStates.LoadSubResourceConfig,
            EServerSessionStates.PreLoad));
            AddState(new ServerPreLoadState(contexts, EServerSessionStates.PreLoad,
            EServerSessionStates.LoadOptionConfig));
            AddState(new ServerOptionLoadState(contexts, EServerSessionStates.LoadOptionConfig, EServerSessionStates.LoadSceneMapConfig));
            AddState(new ServerLoadMapConfigState(contexts, EServerSessionStates.LoadSceneMapConfig,
            EServerSessionStates.Gaming));
            AddState(new GameSessionState(contexts, room, EServerSessionStates.Gaming, EServerSessionStates.Gaming)
                            .WithEnterAction(() =>
                            {
                                BackroundloadSettings.SetCurrentSettings(defaultSettings);
                                if (SharedConfig.DisableGc)
                                {
                                    SingletonManager.Get<gc_manager>().disable_gc();
                                    SingletonManager.Get<gc_manager>().manual_gc_factor_threshold = 2.5f;
                                    SingletonManager.Get<gc_manager>().manual_gc_bytes_threshold_mb = 1500;
                                    SingletonManager.Get<gc_manager>().manual_gc_min_time_delta_seconds = 60;                                 
                                }

                                SingletonManager.Get<gc_manager>().gc_collect();
                                //Application.targetFrameRate = SharedConfig.ServerFrameRate;
                                _logger.WarnFormat("Gaming backgroundLoadingPriority:{0}, targetFrameRate:{1}",
                                Application.backgroundLoadingPriority, Application.targetFrameRate);
                            }).WithLevelAction(() =>
                            {
                                if (SharedConfig.DisableGc)
                                {
                                    SingletonManager.Get<gc_manager>().enable_gc();
                                }

                                SingletonManager.Get<gc_manager>().gc_collect();
                            }));
            Initialize((int) EServerSessionStates.LoadConfig);
        }
    }
}