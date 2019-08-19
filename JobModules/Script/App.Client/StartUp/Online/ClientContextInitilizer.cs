using System;
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
using Core.Prediction.VehiclePrediction.TimeSync;
using Core.Replicaton;
using Core.Sound;
using Core.SyncLatest;
using Core.UpdateLatest;
using VehicleCommon;
using App.Server.GameModules.GamePlay;
using App.Client.GameModules.GamePlay;
using App.Client.GameModules.Replay;
using App.Shared.Network;
using Core.Utils;
using UnityEngine;
using Utils.Replay;
using Utils.Singleton;
using NetworkMessageRecoder = Utils.Replay.NetworkMessageRecoder;
using Random = System.Random;

namespace App.Client
{
    public class ClientContextInitilizer : IClientContextInitilizer
    {
        LoggerAdapter _logger = new LoggerAdapter(typeof(ClientContextInitilizer));
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

            // var recoder = 
            sessionObjects.SnapshotSelctor = new SnapshotPool();
            sessionObjects.UserCmdGenerator = _userCmdGenerator;
            
            if (SharedConfig.IsRecord)
            {
                var name = String.Format("replay_{0:yyyy_M_d_HH_mm_ss}_{1}", DateTime.Now, Guid.NewGuid());
                _logger.InfoFormat("SetUp MessageRecoder :{0}",name);
                sessionObjects.Record = new RecordManager(name);
            }
            else if (SharedConfig.IsReplay)
            {
                _logger.InfoFormat("SetUp MessageReplay :{0}",SharedConfig.RecodFile);
                sessionObjects.Replay = new ReplayManager(SharedConfig.RecodFile);
               
            }

            sessionObjects.MessageDispatcher = new NetworkMessageDispatcher(sessionObjects.Record);
          
            sessionObjects.TimeManager = new TimeManager(_contexts.session.currentTimeObject);

            sessionObjects.LoginToken = _loginToken;
            sessionObjects.netSyncManager = new SyncLastestManager(gameContexts,sessionObjects.SnapshotSelctor);

            sessionObjects.PlaybackInfoProvider = new PlaybackInfoProvider(gameContexts);
            sessionObjects.PlaybackManager = new PlaybackManager(sessionObjects.PlaybackInfoProvider);
            sessionObjects.UserPredictionProvider = new UserPredictionProvider(
                sessionObjects.SnapshotSelctor,
                _contexts.player,
                gameContexts);

            sessionObjects.UserPredictionManager = new PredictionManager(sessionObjects.UserPredictionProvider);
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