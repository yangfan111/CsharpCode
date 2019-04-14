using App.Client.GameModules.GamePlay.Free;
using App.Client.GameModules.Room;
using App.Protobuf;
using App.Server.Bullet;
using App.Server.GameModules.GamePlay;
using App.Server.GameModules.GamePlay.Free.map;
using App.Server.GameModules.GamePlay.Free.player;
using App.Server.MessageHandler;
using App.Server.Scripts.Config;
using App.Shared;
using App.Shared.Components;
using App.Shared.Components.Player;
using App.Shared.Components.ServerSession;
using App.Shared.Configuration;
using App.Shared.ContextInfos;
using App.Shared.DebugSystem;
using App.Shared.EntityFactory;
using App.Shared.FreeFramework.framework.trigger;
using App.Shared.FreeFramework.framework.unit;
using App.Shared.FreeFramework.Free.Weapon;
using Core;
using App.Shared.GameMode;
using App.Shared.GameModules.Bullet;
using App.Shared.GameModules.Vehicle;
using App.Shared.GameModules.Weapon;
using App.Shared.GameModules.Weapon.Behavior;
using App.Shared.Network;
using App.Shared.Player;
using com.wd.free.para;
using com.wd.free.trigger;
using Core.Components;
using Core.Configuration;
using Core.Configuration.Sound;
using Core.EntitasAdpater;
using Core.Free;
using Core.GameModule.System;
using Core.MyProfiler;
using Core.Network;
using Core.Prediction.VehiclePrediction.TimeSync;
using Core.Replicaton;
using Core.Room;
using Core.SessionState;
using Core.SpatialPartition;
using Core.UpdateLatest;
using Core.Utils;
using Entitas;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Utils.AssetManager;
using Utils.Configuration;
using Utils.Singleton;
using VehicleCommon;
using Vector3 = UnityEngine.Vector3;

namespace App.Server
{
    /// <summary>
    /// Defines the <see cref="ServerRoom" />
    /// </summary>
    public partial class ServerRoom : IRoom, IPlayerEntityDic<PlayerEntity>, IEcsDebugHelper
    {
        private enum RoomState
        {
            Initialized,
            Running,//is running game
            Disposing,
            Disposed,
        }

        private static LoggerAdapter _logger = new LoggerAdapter(typeof(ServerRoom));

        private Contexts _contexts;

        private Dictionary<INetworkChannel, PlayerEntity> _channelToPlayer =
            new Dictionary<INetworkChannel, PlayerEntity>();

        public NetworkMessageDispatcher MessageDispatcher { get; private set; }

        private ISnapshotPool _compensationSnapshotPool = new SnapshotPool();

        private SnapshotFactory _snapshotFactory;

        private ServerSessionStateMachine _sessionStateMachine;

        private IHallRoom _hallRoom;

        private IGameRule _rule;

        private IPlayerDamager _damager;

        private ICoRoutineManager _coRoutineManager;

        private int _sumCheckTime;

        private IBin2DManager _bin2DManager;

        private RoomState _state;

        private IRoomEventDispatchter _eventDispatcher;

        public void OnInitializeCompleted()
        {
            _logger.Info("Server Room Initialize Completed");
            _state = RoomState.Running;
            _rule.GameStart(_contexts);
            _contexts.session.commonSession.SessionMode = ModeUtil.CreateSharedPlayerMode(_contexts, _contexts.session.commonSession.RoomInfo.ModeId);
            FreeMapPosition.Init(_contexts);
            DebugUtil.LogInUnity("Server Room Initialize Completed");
        }

        private bool _isGameOver;
        private bool _isGameExit;

        public bool IsGameExit
        {
            get { return _isGameExit; }
        }

        public bool IsDiposed
        {
            get { return _state == RoomState.Disposed; }
        }

        private int _testPlayerNum = 0;

        private SendSnapshotManager _sendSnapshotManager;

        public ServerRoom(IRoomId roomId, Contexts contexts, IRoomEventDispatchter eventDispatcher, ICoRoutineManager coRoutineManager, IUnityAssetManager assetManager, IPlayerTokenGenerator tokenGenerator)
        {
            SingletonManager.Get<ServerFileSystemConfigManager>().Reload();
            _state = RoomState.Initialized;

            _tokenGenerator = tokenGenerator;
            _coRoutineManager = coRoutineManager;

            var entityIdGenerator = new EntityIdGenerator(EntityIdGenerator.GlobalBaseId);
            var equipmentEntityIdGenerator = new EntityIdGenerator(EntityIdGenerator.EquipmentBaseId);
            var ruleId = RuleMap.GetRuleId(SingletonManager.Get<ServerFileSystemConfigManager>().BootConfig.Rule);
            RoomId = roomId;
            _contexts = contexts;
            SingletonManager.Get<MyProfilerManager>().Contexts = _contexts;
            _eventDispatcher = eventDispatcher;
            _bin2DManager = CreateBin2DManager();
            IniCurrentTimeSession();


            InitEntityFactorySession(entityIdGenerator, equipmentEntityIdGenerator);
            _assetManager = assetManager;

            InitCommonSession(entityIdGenerator,
                equipmentEntityIdGenerator, ruleId);

            InitServerSession(_bin2DManager, ruleId);


            _contexts.vehicle.SetSimulationTime(0);
            MessageDispatcher = CreateNetworMessageHandlers();
            _snapshotFactory = new SnapshotFactory(_contexts.session.commonSession.GameContexts);
            _sessionStateMachine = new ServerSessionStateMachine(_contexts, this);

            _damager = new SimplePlayerDamager(this);
            VehicleDamageUtility._damager = _damager;
            _sendSnapshotManager = new SendSnapshotManager(_contexts);

            InitialWeaponSkill();
        }

        public IPlayerInfo GetPlayerInfo(long playerId)
        {
            return _hallRoom == null ? null : _hallRoom.GetPlayer(playerId);
        }

        public RoomDebugInfo GetRoomDebugInfo()
        {
            var debugInfo = new RoomDebugInfo();
            debugInfo.State = _state.ToString();
            debugInfo.HallRoomId = _hallRoom == null ? 0 : _hallRoom.HallRoomId;
            debugInfo.RoomId = RoomId.Id;
            return debugInfo;
        }

        public IList<PlayerDebugInfo> GetPlayerDebugInfo()
        {
            var debugInfo = new List<PlayerDebugInfo>();
            var playerInfoSet = new HashSet<IPlayerInfo>();
            foreach (var player in _channelToPlayer.Values)
            {
                var pdinfo = new PlayerDebugInfo();
                pdinfo.HasPlayerEntity = true;
                pdinfo.EntityKey = player.entityKey.Value;
                pdinfo.EntityId = player.playerInfo.EntityId;
                pdinfo.PlayerId = player.playerInfo.PlayerId;
                pdinfo.TeamId = player.playerInfo.TeamId;
                pdinfo.Name = player.playerInfo.PlayerName;
                pdinfo.HasPlayerInfo = false;
                if (_hallRoom != null)
                {
                    var playerInfo = _hallRoom.GetPlayer(pdinfo.PlayerId);

                    if (playerInfo != null)
                    {
                        pdinfo.HasPlayerInfo = true;
                        pdinfo.IsRobot = playerInfo.IsRobot;
                        pdinfo.IsLogin = playerInfo.IsLogin;
                        pdinfo.Token = playerInfo.Token;
                        pdinfo.CreateTime = playerInfo.CreateTime;
                        pdinfo.GameStartTime = playerInfo.GameStartTime;
                        playerInfoSet.Add(playerInfo);
                    }
                }

                debugInfo.Add(pdinfo);
            }

            if (_hallRoom != null)
            {
                var allPlayerInfos = _hallRoom.GetAllPlayers();
                foreach (var playerInfo in allPlayerInfos)
                {
                    if (!playerInfoSet.Contains(playerInfo))
                    {
                        var pdinfo = new PlayerDebugInfo();
                        pdinfo.HasPlayerEntity = false;
                        pdinfo.HasPlayerInfo = true;

                        pdinfo.PlayerId = playerInfo.PlayerId;
                        pdinfo.EntityId = playerInfo.EntityId;
                        pdinfo.TeamId = playerInfo.TeamId;
                        pdinfo.Name = playerInfo.PlayerName;

                        pdinfo.IsRobot = playerInfo.IsRobot;
                        pdinfo.Token = playerInfo.Token;
                        pdinfo.CreateTime = playerInfo.CreateTime;
                        pdinfo.GameStartTime = playerInfo.GameStartTime;
                        debugInfo.Add(pdinfo);
                    }
                }
            }

            return debugInfo;
        }

        private void InitialWeaponSkill()
        {
            FreeRuleConfig config = FreeRuleConfig.GetRule("weaponSkill", SingletonManager.Get<ServerFileSystemConfigManager>().BootConfig.Mysql);
            foreach (GameTrigger trigger in config.Triggers.GetTriggers())
            {
                WeaponSkillFactory.RegisterSkill(FreeArgs, trigger);
            }
        }

        private void InitServerSession(IBin2DManager bin2DManager, int ruleId)
        {
            _contexts.session.SetServerSessionObjects();
            var sessionObjects = _contexts.session.serverSessionObjects;
            var gameContexts = _contexts.session.commonSession.GameContexts;
            sessionObjects.CompensationSnapshotPool = _compensationSnapshotPool;
            sessionObjects.CompensationSnapshotSelector =
                new SnapshotSelectorContainer(new SnapshotSelector(_compensationSnapshotPool));
            sessionObjects.Bin2DConfig = new Bin2DConfig(-9000, -9000, 9000, 9000, 100, 16000);

            sessionObjects.Bin2dManager = bin2DManager;

            sessionObjects.SimulationTimer = new ServerSimulationTimer();
            sessionObjects.SimulationTimer.CurrentTime = 0;
            sessionObjects.VehicleTimer = new VehicleTimer();

            sessionObjects.GameRule = ruleId;
            sessionObjects.FpsSatatus = new FpsSatatus();
        }

        private static IBin2DManager CreateBin2DManager()
        {
            return GameContextsUtility.GetReplicationBin2DManager(-9000, -9000, 9000, 9000,
                16000,
                new Dictionary<int, int>
                {
                    {(int) EEntityType.SceneObject, 32},
                    {(int) EEntityType.MapObject, 256},
                    {(int) EEntityType.Bullet, 1000},
                    {(int) EEntityType.Sound, 32},
                    {(int) EEntityType.Player, 1000},
                    {(int) EEntityType.FreeMove, 10000},

                    {(int) EEntityType.Throwing, 1000},
                    {(int) EEntityType.Vehicle, 1000},
                    {(int) EEntityType.ClientEffect, 1000}
                });
        }

        private void InitCommonSession(
            EntityIdGenerator entityIdGenerator,
            EntityIdGenerator equipmentEntityIdGenerator, int ruleId)
        {
            _contexts.session.SetCommonSession();
            var commonSession = _contexts.session.commonSession;
            commonSession.InitPosition = Vector3.zero;

            commonSession.AssetManager = _assetManager;

            commonSession.CoRoutineManager = _coRoutineManager;
            commonSession.GameContexts =
                GameContextsUtility.GetReplicationGameContexts(_contexts, _bin2DManager);
            commonSession.EntityIdGenerator = entityIdGenerator;
            commonSession.EquipmentEntityIdGenerator = equipmentEntityIdGenerator;
            commonSession.RoomInfo = new RoomInfo
            {
                MapId = SingletonManager.Get<ServerFileSystemConfigManager>().BootConfig.MapId,
                ModeId = ruleId
            };
            commonSession.RuntimeGameConfig = new RuntimeGameConfig();
            commonSession.BulletInfoCollector = new ServerBulletInfoCollector();

            MakeWeaponLogicManager();
        }

        private void MakeWeaponLogicManager()
        {
            var commonSession = _contexts.session.commonSession;
            //commonSession.PlayerWeaponResourceConfigManager = new PlayerWeaponResourceConfigManager(SingletonManager.Get<WeaponPartsConfigManager>(),
            //    SingletonManager.Get<WeaponDataConfigManager>());

//            var weaponComponentsFacoty = new WeaponFireScriptsInitializer(_contexts,
 //               _contexts.session.commonSession.EntityIdGenerator);
            var fireScriptsProvider = new WeaponFireScriptsProvider(_contexts);
            commonSession.WeaponFireUpdateManager= new WeaponFireUpdateManagaer(fireScriptsProvider, _contexts.session.commonSession.FreeArgs);
        }

        private void InitEntityFactorySession(EntityIdGenerator entityIdGenerator,
            EntityIdGenerator equipmentEntityIdGenerator)
        {
            _contexts.session.SetEntityFactoryObject();
            var entityFactoryObject = _contexts.session.entityFactoryObject;
            entityFactoryObject.SoundEntityFactory = new ServerSoundEntityFactory(_contexts.sound,
                _contexts.player,
                entityIdGenerator,
                _contexts.session.currentTimeObject,
                SingletonManager.Get<SoundConfigManager>());
            entityFactoryObject.SceneObjectEntityFactory = new ServerSceneObjectEntityFactory(_contexts.sceneObject, _contexts.player,
                entityIdGenerator, equipmentEntityIdGenerator, _contexts.session.currentTimeObject);
            entityFactoryObject.MapObjectEntityFactory =
                new ServerMapObjectEntityFactory(_contexts.mapObject, entityIdGenerator);
        }

        private void IniCurrentTimeSession()
        {
            if (!_contexts.session.hasCurrentTimeObject)
            {
                _contexts.session.SetCurrentTimeObject();
            }
            else
            {
                Debug.LogError("CurrentTimeObject already exist");
            }
            var currentTimeObject = _contexts.session.currentTimeObject;
            currentTimeObject.CurrentTime = 0;
        }

        public IRoomId RoomId { get; private set; }

        private NetworkMessageDispatcher CreateNetworMessageHandlers()
        {
            var messageDispatcher = new NetworkMessageDispatcher();
            messageDispatcher.RegisterLater((int)EClient2ServerMessage.UserCmd, new UserCmdMessageHandler(this));
            messageDispatcher.RegisterLater((int)EClient2ServerMessage.LocalDisconnect,
                new DelegateNetworkMessageHandler(OnDisconnect));
            messageDispatcher.RegisterLater((int)EClient2ServerMessage.LocalLogin,
                new DelegateNetworkMessageHandler(AsyncLoginPlayer));
            messageDispatcher.RegisterLater((int)EClient2ServerMessage.VehicleCmd,
                new VehicleCmdMessageHandler(_contexts, this));
            messageDispatcher.RegisterLater((int)EClient2ServerMessage.DebugCommand,
                new DebugMessageHandler(_contexts, this));
            messageDispatcher.RegisterLater((int)EClient2ServerMessage.SimulationTimeSync,
                new SimulationTimeServerSyncHandler(_contexts, this));
            messageDispatcher.RegisterLater((int)EClient2ServerMessage.FreeEvent,
                new FreeEventMessageHandler(_contexts, this));
            messageDispatcher.RegisterLater((int)EClient2ServerMessage.VehicleEvent,
                new VehicleEventMessageHandler(_contexts, this));
            messageDispatcher.RegisterLater((int)EClient2ServerMessage.TriggerObjectEvent,
                new TriggerObejctEventMessageHandler(_contexts, this));
            messageDispatcher.RegisterImmediate((int)EClient2ServerMessage.Ping,
                new PingReqMessageHandler(_contexts, this));
            messageDispatcher.RegisterLater((int)EClient2ServerMessage.UpdateMsg,
                new UserUpdateMsgHandler(this, _contexts));
            messageDispatcher.RegisterImmediate((int)EClient2ServerMessage.UpdateMsg,
                new UserUpdateAckMsgHandler(this));
            messageDispatcher.RegisterLater((int)EClient2ServerMessage.FireInfo,
                new FireInfoMessageHandler(_contexts, this));
            messageDispatcher.RegisterLater((int)EClient2ServerMessage.DebugScriptInfo, new DebugScriptInfoMessageHandler(this));
            messageDispatcher.RegisterLater((int)EClient2ServerMessage.GameOver, new ServerGameOverMessageHandler(this));
            return messageDispatcher;
        }

        public void RegisterDebugInfoHandler(Action<string> handler)
        {
            DebugScriptInfoMessageHandler.SetHandler(handler);
        }

        public bool LoginPlayer(IPlayerInfo playerInfo, INetworkChannel channel)
        {
            MessageDispatcher.SaveDispatch(channel, (int)EClient2ServerMessage.LocalLogin, playerInfo);
            return true;
        }

        public bool SendLoginSucc(IPlayerInfo playerInfo, INetworkChannel channel)
        {
            if (_state == RoomState.Running)
            {
                var sessionObjects = _contexts.session.serverSessionObjects;
                var msg = LoginSuccMessage.Allocate();
                msg.GameRule = (int)sessionObjects.GameRule;
                _contexts.session.commonSession.RoomInfo.ToLoginSuccMsg(msg);
                msg.Camp = playerInfo.Camp;
                FreeArgs.Trigger(FreeTriggerConstant.PRELOAD_RESOURCE, new TempUnit("roomInfo", new ObjectUnit(msg)), new TempUnit("playerInfo", new ObjectUnit(playerInfo)));

                channel.SendReliable((int)EServer2ClientMessage.LoginSucc, msg);
                msg.ReleaseReference();
                _logger.InfoFormat("player SendLoginSucc with name {0}",
                    playerInfo.PlayerName);
            }

            return true;
        }

        public void SetPlayerStageRunning(IPlayerInfo playerInfo, INetworkChannel channel)
        {
            if (_channelToPlayer.ContainsKey(channel))
            {
                var playerEntity = _channelToPlayer[channel];
                playerEntity.ReplaceStage(EPlayerLoginStage.EnterRunning);
                playerEntity.isFlagCompensation = true;
                playerEntity.isFlagSyncNonSelf = true;
            }
        }

        public void AsyncLoginPlayer(INetworkChannel channel, int messageType, object playerInfoObj)
        {
            IPlayerInfo playerInfo = (IPlayerInfo)playerInfoObj;
            _logger.InfoFormat("Received LocalLogin Message ... playerName:{0}", playerInfo.PlayerName);

            if (_channelToPlayer.Count == 0)
            {
                ResetContexts(false);
                _logger.InfoFormat("Reset All Entity Finish ...");
            }

            if (!_channelToPlayer.ContainsKey(channel))
            {
                UpdateTestPlayerInfo(playerInfo);
                // 大厅传入错误RoleModelId
                if (null == SingletonManager.Get<RoleConfigManager>().GetRoleItemById(playerInfo.RoleModelId))
                {
                    _logger.Error("RoleModelIdError:  " + playerInfo.RoleModelId);
                    playerInfo.RoleModelId = 2;
                }

                var player = CreateNewPlayerEntity(playerInfo);
                playerInfo.PlayerEntity = player;
                player.AddNetwork(channel);

                playerInfo.StatisticsData = player.statisticsData.Statistics;
                _channelToPlayer[channel] = player;
                channel.MessageReceived += ChannelOnMessageReceived;
                channel.Disconnected += ChannelOnDisonnected;

               // var info = new AppMessageTypeInfo();
                if (!player.hasUpdateMessagePool)
                    player.AddUpdateMessagePool();
                player.updateMessagePool.LastestExecuteUserCmdSeq = -1;
               
                //channel.Serializer = new NetworkMessageSerializer(info);
               
                NoticeHallPlayerLoginSucc(player);
                player.ReplaceStage(EPlayerLoginStage.CreateEntity);
                var msg = App.Protobuf.PlayerInfoMessage.Allocate();
                msg.ConvertFrom(playerInfo);
                channel.SendReliable((int)EServer2ClientMessage.PlayerInfo, msg);
                _logger.InfoFormat("player login with name {0}, key {1}, game rule {2}, msp id {3}",
                    playerInfo.PlayerName,
                    player.entityKey,
                    0, 0);
                msg.ReleaseReference();
            }
            else
            {
                _logger.ErrorFormat("player duplicate login from name:{0}, channe:{1}", playerInfo.PlayerName, channel);
            }
        }

        private void NoticeHallPlayerLoginSucc(PlayerEntity player)
        {
            if (_hallRoom != null)
            {
                _hallRoom.UpdatePlayerStatus(player.playerInfo.PlayerId, EPlayerGameStatus.MIDDLE);
                _hallRoom.PlayerLoginSucc(player.playerInfo.PlayerId);
            }
        }

        private void AddRoomInfoToMessage(RoomInfo roomInfo, LoginSuccMessage message)
        {
        }

        private void UpdateTestPlayerInfo(IPlayerInfo playerInfo)
        {
            if (playerInfo.Token == TestUtility.TestToken)
            {
                playerInfo.PlayerId = TestUtility.NewPlayerId;
                playerInfo.PlayerName = "Test_" + playerInfo.PlayerId;
                playerInfo.Num = ++_testPlayerNum;
                playerInfo.Camp = _testPlayerNum % 2 == 0 ? 2 : 1;
                playerInfo.TeamId = playerInfo.Camp;
                playerInfo.AvatarIds = new List<int> { 1, 8 };
                playerInfo.WeaponBags = PlayerEntityFactory.MakeFakeWeaponBag();
            }
            else if (playerInfo.Token == TestUtility.RobotToken)
            {
                playerInfo.PlayerId = TestUtility.NewPlayerId;
                playerInfo.PlayerName = "Robot_" + playerInfo.PlayerId;
                playerInfo.Num = ++_testPlayerNum;
                playerInfo.Camp = _testPlayerNum % 2 == 0 ? 1 : 2;
                playerInfo.TeamId = playerInfo.Camp;
                playerInfo.AvatarIds = new List<int> { 1, 8 };
                playerInfo.WeaponBags = PlayerEntityFactory.MakeFakeWeaponBag();

            }
        }

        private PlayerEntity CreateNewPlayerEntity(IPlayerInfo playerInfo)
        {
            return PlayerEntityFactory.CreateNewServerPlayerEntity(_contexts.player,
                _contexts.session.commonSession,
                _contexts.session.commonSession.EntityIdGenerator,
                SingletonManager.Get<MapConfigManager>().SceneParameters.PlayerBirthPosition,
                playerInfo);
        }

        public PlayerEntity GetPlayerEntity(INetworkChannel channel)
        {
            PlayerEntity player;
            if (_channelToPlayer.TryGetValue(channel, out player))
            {
            }

            return player;
        }

        public void ChannelOnDisonnected(INetworkChannel channel)
        {
            
            MessageDispatcher.SaveDispatch(channel, (int)EClient2ServerMessage.LocalDisconnect, null);
            channel.Serializer.Dispose();
        }

        public void OnDisconnect(INetworkChannel channel, int messageType, object messageBody)
        {
            PlayerEntity player = GetPlayerEntity(channel);

            if (player != null)
            {
                try
                {
                    _logger.InfoFormat("player disconnected id {0}", player.entityKey);
                    player.isFlagDestroy = true;

                    if (player.hasFreeData)
                    {
                        _rule.PlayerLeave(_contexts, player);
                    }
                    else
                    {
                        _logger.ErrorFormat("Leave Player {0} Id {1} without Free Data ", player.playerInfo.PlayerName,
                            player.playerInfo.PlayerId);
                    }

                    if (_hallRoom != null)
                    {
                        _hallRoom.PlayerLeaveRoom(player.playerInfo.PlayerId);
                    }
                }
                catch (Exception e)
                {
                    _logger.ErrorFormat("player disconnected error: {0} \n {1}", e.Message, e.StackTrace);
                }

                channel.MessageReceived -= ChannelOnMessageReceived;
                channel.Disconnected -= ChannelOnDisonnected;
                _channelToPlayer.Remove(channel);
                channel.Dispose();

                if (_channelToPlayer.Count == 0)
                {
                    GameOver(true);
                }
            }
            else
            {
                _logger.ErrorFormat("illegal ChannelOnDisonnected event received {0}", channel);
            }
        }

        public void GameOver(bool forceExit)
        {
            _logger.InfoFormat("ServerRoom GameOver ... Player Count:{0} GameExit {1}", _contexts.player.GetEntities().Length, forceExit);
            SetGameOverStatus(false);

            if (forceExit)
            {
                GameExit();
            }
        }

        private void SetGameOverStatus(bool forceOver)
        {
            _logger.InfoFormat("Set ServerRoom GameOver Status, GameOver {0} ForceOver {1}", _isGameOver, forceOver);
            if (!_isGameOver)
            {
                _isGameOver = true;
                _testPlayerNum = 0;
                if (null != _hallRoom)
                {
                    _hallRoom.GameOver(forceOver);
                    _hallRoom = null;
                }
            } 
        }

        private void GameExit()
        {
            SetGameOverStatus(true);

            if (!_isGameExit)
            {
                SendGameOverMessageToAllPlayers();
                var evt = RoomEvent.AllocEvent<GameExitEvent>();
                _eventDispatcher.AddEvent(evt);
                _isGameExit = true;
            }
           
        }

        private void SendGameOverMessageToAllPlayers()
        {
            foreach (var channel in _channelToPlayer.Keys)
            {
                if (channel.IsConnected)
                {
                    var msg = GameOverMesssage.Allocate();
                    channel.SendReliable((int)EServer2ClientMessage.GameOver, msg);
                    msg.ReleaseReference();
                }
            }
        }

        public int UserCount
        {
            get { return _channelToPlayer.Count; }
        }

        public void ChannelOnMessageReceived(INetworkChannel networkChannel, int messageType, object messageBody)
        {
            MessageDispatcher.SaveDispatch(networkChannel, messageType, messageBody);
        }

        public void Start()
        {
            _hallRoom.UpdateRoomGameStatus(ERoomGameStatus.BEGIN, ERoomEnterStatus.CanEnter);
        }

        private int _interval;

        private IUnityAssetManager _assetManager;

        public void Update(int interval)
        {
            if (_state == RoomState.Disposing || _state == RoomState.Disposed)
            {
                return;
            }

            _interval = interval;
            _contexts.session.currentTimeObject.CurrentTime += _interval;
            MessageDispatcher.DriveDispatch();

            _sessionStateMachine.Update();

            CheckRoomGameStatus(interval);
            if (_contexts.player.GetEntities().Length == 0)
            {
                SingletonManager.Get<MyProfilerManager>().IsRecordOn = false;
            }
        }

        public void LateUpdate()
        {
            if (_state != RoomState.Disposing && _state != RoomState.Disposed)
                _sessionStateMachine.LateUpdate();
        }

        public void RunFreeGameRule()
        {
            _rule.Update(_contexts, _interval);
            if (_rule.GameOver)
            {
                _logger.InfoFormat("Rule Game Over!");
                GameOver(false);
                _rule.GameOver = false;
            }

            if (_rule.GameExit)
            {
                _logger.InfoFormat("Rule Game Exit!");
                GameExit();
            }
        }

        public void SendSnapshot()
        {
            _sendSnapshotManager.SendSnapshot(_interval, _snapshotFactory, _channelToPlayer);
        }

        public void CompensationSnapshot()
        {
            var sessionObjects = _contexts.session.serverSessionObjects;
            int snapshotSeq = sessionObjects.GetNextSnapshotSeq();
            int vehicleSimulationTime = sessionObjects.SimulationTimer.CurrentTime;
            int serverTime = _contexts.session.currentTimeObject.CurrentTime;
            ISnapshot compensationSnapshot = _snapshotFactory.GenerateCompensationSnapshot();
            compensationSnapshot.ServerTime = serverTime;
            compensationSnapshot.SnapshotSeq = snapshotSeq;
            compensationSnapshot.VehicleSimulationTime = vehicleSimulationTime;
            _compensationSnapshotPool.AddSnapshot(compensationSnapshot);
            compensationSnapshot.ReleaseReference();
        }

        private void ResetContexts(bool dispose)
        {
            ICommonSessionObjects _sessionObjects = _contexts.session.commonSession;

            foreach (var entity in _contexts.player.GetEntities())
            {
                DestroyEntity(_sessionObjects, entity);
                entity.isFlagDestroy = true;
            }

            foreach (var entity in _contexts.mapObject.GetEntities())
            {
                if (dispose || !entity.hasReset)
                {
                    DestroyEntity(_sessionObjects, entity);
                    entity.isFlagDestroy = true;
                }
                else
                {
                    entity.reset.ResetAction(entity);
                }


            }

            foreach (var entity in _contexts.sceneObject.GetEntities())
            {
                DestroyEntity(_sessionObjects, entity);
                entity.isFlagDestroy = true;
            }

            foreach (var entity in _contexts.freeMove.GetEntities())
            {
                DestroyEntity(_sessionObjects, entity);
                entity.isFlagDestroy = true;
            }

            foreach (var entity in _contexts.bullet.GetEntities())
            {
                DestroyEntity(_sessionObjects, entity);
                entity.isFlagDestroy = true;
            }

            foreach (var entity in _contexts.sound.GetEntities())
            {
                DestroyEntity(_sessionObjects, entity);
                entity.isFlagDestroy = true;
            }

            foreach (var entity in _contexts.vehicle.GetEntities())
            {
                if (entity.hasEntityKey)
                {
                    DestroyEntity(_sessionObjects, entity);
                    entity.isFlagDestroy = true;
                }
            }

            _contexts.session.currentTimeObject.CurrentTime = 0;
        }

        private void DestroyEntity(ICommonSessionObjects sessionObjects, Entity entity)
        {
            foreach (var comp in entity.GetComponents())
            {
                if (comp is IAssetComponent)
                {
                    ((IAssetComponent)comp).Recycle(sessionObjects.AssetManager);
                }
            }

            if (sessionObjects.AssetManager != null)
                sessionObjects.AssetManager.LoadCancel(entity);
        }

        public void Dispose()
        {
#if UNITY_SOURCE_MODIFIED && !UNITY_EDITOR
       
            UnityProfiler.EnableProfiler(false);
        
#endif
            _state = RoomState.Disposing;
            DisposePlayerConnections();
            if (_bin2DManager != null)
            {
                _bin2DManager.Dispose();
            }

            if (_sendSnapshotManager != null)
            {
                _sendSnapshotManager.Dispose();
            }
            _contexts.session.commonSession.Dispose();
            _contexts.session.serverSessionObjects.Dispose();
           
            GameModuleManagement.Dispose();
            try
            {
                ResetContexts(true);
                _sessionStateMachine.ShutDown();
                _contexts.Reset();
            }
            catch (Exception e)
            {
                _logger.ErrorFormat("Reset Contexts Error {0}", e);
            }
            _contexts = null;

            DestoryObjectUnderDefaultGoBattleServer();
            _coRoutineManager.StartCoRoutine(UnloadScene());

            _logger.InfoFormat("Server Room is Disposing...");
        }

        private void DisposePlayerConnections()
        {
            foreach (var channelPair in _channelToPlayer)
            {
                _logger.InfoFormat("Disconnect player {0} on disposing...", channelPair.Value.entityKey);
                var channel = channelPair.Key;
                if (channel.IsConnected)
                {
                    channel.Disconnect();
                    channel.Dispose();
                }
            }
        }

        private bool IsSceneUnloadable(Scene scene)
        {
            if (scene.name.Equals("ServerScene") || scene.name.Equals("DontDestroyOnLoad"))
            {
                return false;
            }

            return true;
        }

        private IEnumerator UnloadScene()
        {
            yield return _assetManager.Clear();

            var count = SceneManager.sceneCount;
            var sceneList = new List<Scene>();
            for (int i = 0; i < count; ++i)
            {
                var scene = SceneManager.GetSceneAt(i);
                sceneList.Add(scene);
            }

            foreach (var scene in sceneList)
            {
                if (IsSceneUnloadable(scene))
                {
                    yield return SceneManager.UnloadSceneAsync(scene.name);
                }
            }

            yield return Resources.UnloadUnusedAssets();
            GC.Collect();
            _logger.InfoFormat("Server Room is Disposed!");
            _state = RoomState.Disposed;
        }

        private void DestoryObjectUnderDefaultGoBattleServer()
        {
            //unload gameobject under 'ServerScene/DefaultGoBattleServer'
            var serverScene = SceneManager.GetSceneByName("ServerScene");
            if (serverScene.isLoaded)
            {
                var rootObjects = serverScene.GetRootGameObjects();
                var children = new List<GameObject>();
                foreach (var go in rootObjects)
                {

                    if (go.name.Equals("DefaultGoBattleServer"))
                    {
                        int childCount = go.transform.childCount;
                        for (int i = 0; i < childCount; ++i)
                        {
                            children.Add(go.transform.GetChild(i).gameObject);
                        }
                    }
                }

                foreach (var child in children)
                {
                    GameObject.Destroy(child);
                }
            }
        }

        public FreeRuleEventArgs FreeArgs
        {
            get { return (FreeRuleEventArgs)_contexts.session.commonSession.FreeArgs; }
        }

        public IGameRule GameRule
        {
            get { return _rule; }
            set { _rule = value; }
        }

        public Contexts RoomContexts
        {
            get { return _contexts; }
        }

        public IPlayerDamager PlayerDamager
        {
            get { return _damager; }
        }

        public PlayerEntity GetPlayer(INetworkChannel networkChannel)
        {
            PlayerEntity player = GetPlayerEntity(networkChannel);
            if (player != null)
            {
            }
            else
            {
                _logger.ErrorFormat("illegal ChannelOnDisonnected event received {0}", networkChannel);
            }

            return player;
        }

        public void SetHallRoom(IHallRoom hallRoom)
        {
            this._hallRoom = hallRoom;
        }

        public void SetGameMode(int mode)
        {
            _contexts.session.serverSessionObjects.GameRule = mode;
            _contexts.session.commonSession.RoomInfo.ModeId = mode;
            FreeRuleEventArgs args = new FreeRuleEventArgs(_contexts);
            _contexts.session.commonSession.FreeArgs = args;
            _rule = new FreeGameRule(this);

            RoomInfo info = _contexts.session.commonSession.RoomInfo;

            SimpleParaList spl = (SimpleParaList)args.GetDefault().GetParameters();
            spl.AddFields(new ObjectFields(info));
        }

        public void SetRoomInfo(IHallRoom room)
        {
            _contexts.session.commonSession.RoomInfo.CopyFrom(room);
        }

        private void CheckRoomGameStatus(int interval)
        {
            //check hall room timeout
            if (null != _hallRoom && !_hallRoom.IsValid)
            {
                GameOver(true);
            }

            //check player
            _sumCheckTime += interval;
            if (_sumCheckTime > 20000)
            {
                _sumCheckTime = 0;
                string playerName = "";
                string playerIp = "";
                bool hasHallRoom = _hallRoom != null;
                if (_contexts.player.GetEntities().Length > 0)
                {
                    PlayerEntity player = _contexts.player.GetEntities()[0];
                    playerName = player.playerInfo.PlayerName;
                    playerIp = player.network.NetworkChannel.IdInfo();
                }

                //update status
                if (_hallRoom != null)
                {
                    _hallRoom.UpdateRoomGameStatus((ERoomGameStatus)_rule.GameStage,
                        (ERoomEnterStatus)_rule.EnterStatus);
                }

                _logger.InfoFormat(
                    "ServerStatus: PlayerCount:{0}, FirstPlayerName:{1}, PlayerIp:{2}, LoginServer:{3}, HallRoomServer:{4}, AllocationClient:{5}, HasHallRoom:{6}",
                    _channelToPlayer.Count, playerName, playerIp,
                    ServerStatusCollectUtil.LoginServerStatus, ServerStatusCollectUtil.HallRoomServerStatus,
                    ServerStatusCollectUtil.AllocationClientStatus, hasHallRoom);
            }
        }

        public SessionStateMachine GetSessionStateMachine()
        {
            return _sessionStateMachine;
        }
    }
}
