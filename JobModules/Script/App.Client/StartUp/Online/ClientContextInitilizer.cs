using App.Client.Console;
using App.Client.GameModules.Effect;
using App.Client.GameModules.Vehicle;
using App.Client.StartUp;
using App.Server;
using App.Shared;
using App.Shared.Components;
using App.Shared.Components.ClientSession;
using App.Shared.Components.Player;
using App.Shared.Components.ServerSession;
using App.Shared.Components.Vehicle;
using App.Shared.ContextInfos;
using App.Shared.EntityFactory;
using Core;
using App.Shared.UserPhysics;
using App.Shared.GameModules.Weapon;
using Core;
using Utils.AssetManager;
using Core.Configuration;
using Core.Configuration.Sound;
using Core.GameTime;
using Core.Network;
using Core.Playback;
using Core.Prediction;
using Core.Prediction.UserPrediction;
using Core.Prediction.UserPrediction.Cmd;
using Core.Prediction.VehiclePrediction;
using Core.Prediction.VehiclePrediction.Cmd;
using Core.Prediction.VehiclePrediction.TimeSync;
using Core.Replicaton;
using Core.Sound;
using Core.SyncLatest;
using Core.UpdateLatest;
using Core.WeaponLogic;

using Utils.Configuration;
using VehicleCommon;
using App.Server.GameModules.GamePlay;
using App.Client.GameModules.GamePlay;
using UnityEngine;
using Utils.Singleton;

using Assets.Utils.Configuration;

namespace App.Client
{
    public class ClientContextInitilizer : IClientContextInitilizer
    {
        private IUserCmdGenerator _userCmdGenerator;
        private ICoRoutineManager _coRoutineManager;
        private string _loginToken;
        private Contexts _contexts;
        private IUnityAssetManager _assetManager;

        public Contexts Contexts
        {
            get { return _contexts; }
        }


        public ClientContextInitilizer(IUserCmdGenerator userCmdGenerator,
            ICoRoutineManager coRoutineManager,
            IUnityAssetManager assetManager,
            string loginToken)
        {
            _userCmdGenerator = userCmdGenerator;
            _coRoutineManager = coRoutineManager;
            _loginToken = loginToken;
            _assetManager = assetManager;
        }

        protected virtual Contexts GetContexts()
        {
            return Contexts.sharedInstance;
        }

        public Contexts CreateContexts()
        {
            _contexts = GetContexts();

            var entityIdGenerator = new EntityIdGenerator(EntityIdGenerator.LocalBaseId);
            var equipmentEntityIdGenerator = new EntityIdGenerator(EntityIdGenerator.EquipmentBaseId);
            InitCurrentTimeSession();
            InitEntityFactorySession(equipmentEntityIdGenerator, _contexts.session.currentTimeObject,
                entityIdGenerator);
            InitCommonSession(entityIdGenerator, equipmentEntityIdGenerator);
            InitClientSession();
            _contexts.vehicle.SetSimulationTime(0);

            return _contexts;
        }

        private void InitClientSession()
        {
            _contexts.session.SetClientSessionObjects();
            var gameContexts = _contexts.session.commonSession.GameContexts;
            var sessionObjects = _contexts.session.clientSessionObjects;

            sessionObjects.SnapshotPool = new SnapshotPool();
            sessionObjects.UserCmdGenerator = _userCmdGenerator;
            sessionObjects.MessageDispatcher = new NetworkMessageDispatcher();
            sessionObjects.SnapshotSelectorContainer =
                new SnapshotSelectorContainer(new SnapshotSelector(sessionObjects.SnapshotPool));
            sessionObjects.TimeManager = new TimeManager(_contexts.session.currentTimeObject);
          
            sessionObjects.LoginToken = _loginToken;
            sessionObjects.SyncLatestHandler = new SyncLatestHandler(sessionObjects.SnapshotSelectorContainer,
                gameContexts, new SnapshotEntityMapFilter());
            sessionObjects.SyncLatestManager = new SyncLatestManager(sessionObjects.SyncLatestHandler);

            sessionObjects.PlaybackInfoProvider = new PlaybackInfoProvider(gameContexts);
            sessionObjects.PlaybackManager = new PlaybackManager(sessionObjects.PlaybackInfoProvider);
            sessionObjects.UserPredictionInfoProvider = new UserPredictionInfoProvider(
                sessionObjects.SnapshotSelectorContainer,
                _contexts.player,
                gameContexts);
            sessionObjects.VehiclePredictionInfoProvider = new VehiclePredictionInfoProvider(
                sessionObjects.SnapshotSelectorContainer,
                gameContexts, _contexts.vehicle, SharedConfig.ServerAuthorative);

            sessionObjects.UserPredictionInitManager =
                new PredictionInitManager<IUserPredictionComponent>(sessionObjects.UserPredictionInfoProvider);
            sessionObjects.VehicleCmdExecuteSystemHandler = new ClientVehicleCmdExecuteSystemHandler(_contexts);
            sessionObjects.SimulationTimer = new ClientSimulationTimer(SharedConfig.ServerAuthorative);
            sessionObjects.VehicleTimer = new VehicleTimer();

            sessionObjects.SoundPlayer = new SoundPlayer();
            sessionObjects.FpsSatatus = new FpsSatatus();
            sessionObjects.ServerFpsSatatus = new ServerStatus();
            sessionObjects.UpdateLatestHandler = new UpdateLatestHandler(gameContexts);
            sessionObjects.GlobalEffectManager = new GlobalEffectManager();
        }

        private void InitCommonSession(EntityIdGenerator entityIdGenerator,
            EntityIdGenerator equipmentEntityIdGenerator)
        {
            _contexts.session.SetCommonSession();
            var commonSession = _contexts.session.commonSession;

            commonSession.GameContexts = GameContextsUtility.GetReplicationGameContexts(_contexts);
            commonSession.CoRoutineManager = _coRoutineManager;
            commonSession.AssetManager = _assetManager;
            commonSession.EntityIdGenerator = entityIdGenerator;
            commonSession.EquipmentEntityIdGenerator = equipmentEntityIdGenerator;
            commonSession.RoomInfo = new Core.Room.RoomInfo();
            commonSession.RuntimeGameConfig = new RuntimeGameConfig();
            //commonSession.PlayerStateCollectorPool = new PlayerStateCollectorPool();
            commonSession.FreeArgs = new FreeRuleEventArgs(_contexts);
            commonSession.FreeArgs.Rule = new ClientRule(commonSession.FreeArgs);
            commonSession.InitPosition = Vector3.zero;
        }


        private void InitCurrentTimeSession()
        {
            _contexts.session.SetCurrentTimeObject();
        }

        private void InitEntityFactorySession(EntityIdGenerator equipmentEntityIdGenerator,
            CurrentTimeObjectComponent currentTimeObject, EntityIdGenerator entityIdGenerator)
        {
            _contexts.session.SetEntityFactoryObject();
            var entityFactoryObject = _contexts.session.entityFactoryObject;
            entityFactoryObject.SoundEntityFactory = new ClientSoundEntityFactory(_contexts.sound,
                equipmentEntityIdGenerator,
                currentTimeObject,
                SingletonManager.Get<SoundConfigManager>());
            entityFactoryObject.SceneObjectEntityFactory = new ClientSceneObjectEntityFactory(_contexts.sceneObject,
                _contexts.player, entityIdGenerator, equipmentEntityIdGenerator, currentTimeObject);
            entityFactoryObject.MapObjectEntityFactory =
                new ClientMapObjectEntityFactory(_contexts.mapObject, entityIdGenerator);
        }
    }
}