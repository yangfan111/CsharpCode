using System;
using System.Collections;
using System.Collections.Generic;
using App.Client.GameModules.Room;
using App.Protobuf;
using App.Server.GameModules.GamePlay;
using App.Server.GameModules.GamePlay.Free.map;
using App.Server.GameModules.GamePlay.Free.player;
using App.Server.MessageHandler;
using App.Shared;
using App.Shared.Components.Player;
using App.Shared.DebugSystem;
using App.Shared.EntityFactory;
using App.Shared.GameMode;
using App.Shared.GameModules.Attack;
using App.Shared.GameModules.Vehicle;
using App.Shared.GameModules.Weapon;
using App.Shared.Player;
using com.wd.free.para;
using Core.Components;
using Core.EntityComponent;
using Core.Enums;
using Core.GameModule.System;
using Core.MyProfiler;
using Core.Network;
using Core.Replicaton;
using Core.Room;
using Core.SessionState;
using Core.Utils;
using Entitas;
using UnityEngine;
using UnityEngine.SceneManagement;
using Utils.Configuration;
using Utils.Singleton;
using Object = UnityEngine.Object;

namespace App.Server
{
    /// <summary>
    ///     Defines the <see cref="ServerRoom" />
    /// </summary>
    public partial class ServerRoom : IRoom, IPlayerEntityDic<PlayerEntity>, IEcsDebugHelper
    {
        private static LoggerAdapter logger = new LoggerAdapter(typeof(ServerRoom));

        private Dictionary<INetworkChannel, PlayerEntity> channelToPlayer =
                        new Dictionary<INetworkChannel, PlayerEntity>();

        public ContextsServerWrapper ContextsWrapper { get; private set; }

        private IPlayerDamager damager;

        private IRoomEventDispatchter eventDispatcher;

        private IHallRoom hallRoom;

        private int interval;

        private bool isDisposed;
        private bool isGameExit;

        private bool isGameOver;

        private IGameRule rule;

        private SendSnapshotManager sendSnapshotManager;

        private CompensationManager compensationManager;

        private ServerSessionStateMachine sessionStateMachine;

        private SnapshotFactory snapshotFactory;


        private RoomState state;


        private int sumCheckTime;

        private int testPlayerNum;

        public NetworkMessageDispatcher MessageDispatcher { get; private set; }

        public int UserCount
        {
            get { return channelToPlayer.Count; }
        }


        public IGameRule GameRule
        {
            get { return rule; }
            set { rule = value; }
        }

        public Contexts RoomContexts
        {
            get { return ContextsWrapper.contexts; }
        }

        public IPlayerDamager PlayerDamager
        {
            get { return damager; }
        }

        public SessionStateMachine GetSessionStateMachine()
        {
            return sessionStateMachine;
        }

        public PlayerEntity GetPlayer(INetworkChannel networkChannel)
        {
            PlayerEntity player = GetPlayerEntity(networkChannel);
            if (player != null)
            {
            }
            else
            {
                logger.ErrorFormat("illegal ChannelOnDisonnected event received {0}", networkChannel);
            }

            return player;
        }

        public bool IsGameExit
        {
            get { return isGameExit; }
        }

        public bool IsDiposed
        {
            get { return state == RoomState.Disposed; }
        }


        public IRoomId RoomId { get; private set; }

        public bool LoginPlayer(IPlayerInfo playerInfo, INetworkChannel channel)
        {
            MessageDispatcher.SaveDispatch(channel, (int) EClient2ServerMessage.LocalLogin, playerInfo);
            return true;
        }

        public bool SendLoginSucc(IPlayerInfo playerInfo, INetworkChannel channel)
        {
            switch (state)
            {
                case RoomState.Initialized:
                case RoomState.Running:
                    var sessionObjects = ContextsWrapper.contexts.session.serverSessionObjects;
                    var msg            = LoginSuccMessage.Allocate();
                    msg.GameRule = sessionObjects.GameRule;
                    ContextsWrapper.RoomInfo.ToLoginSuccMsg(msg);
                    msg.Camp = playerInfo.Camp;
                    //FreeArgs.Trigger(FreeTriggerConstant.PRELOADRESOURCE, new TempUnit("roomInfo", new ObjectUnit(msg)), new TempUnit("playerInfo", new ObjectUnit(playerInfo)));
                    channel.SendReliable((int) EServer2ClientMessage.SceneInfo, msg);
                    if (RoomState.Running == state)
                        channel.SendReliable((int) EServer2ClientMessage.LoginSucc, msg);
                    msg.ReleaseReference();
                    logger.InfoFormat("player SendLoginSucc with name {0}", playerInfo.PlayerName);
                    break;
            }

            return true;
        }

        public void SetPlayerStageRunning(IPlayerInfo playerInfo, INetworkChannel channel)
        {
            if (channelToPlayer.ContainsKey(channel))
            {
                var playerEntity = channelToPlayer[channel];
                playerEntity.ReplaceStage(EPlayerLoginStage.EnterRunning);
                playerEntity.isFlagCompensation = true;
                playerEntity.isFlagSyncNonSelf  = true;
            }
        }

        public void GameOver(bool forceExit, RoomState state)
        {
            logger.InfoFormat("ServerRoom GameOver ... Player Count:{0} GameExit {1}",
                ContextsWrapper.contexts.player.GetEntities().Length, forceExit);
            SetGameOverStatus(false);

            if (forceExit)
            {
                GameExit(state);
            }
        }

        public void Start()
        {
            hallRoom.UpdateRoomGameStatus(ERoomGameStatus.BEGIN, ERoomEnterStatus.CanEnter);
        }


        public void Update(int interval)
        {
            if (state == RoomState.Disposing || state == RoomState.Disposed)
            {
                return;
            }

            this.interval                                                       =  interval;
            ContextsWrapper.contexts.session.currentTimeObject.CurrentTime += interval;
            if (!isDisposed)
                MessageDispatcher.DriveDispatch();

            sessionStateMachine.Update();

            CheckRoomGameStatus(interval);
            if (ContextsWrapper.contexts.player.GetEntities().Length == 0)
            {
                SingletonManager.Get<MyProfilerManager>().IsRecordOn = false;
            }
        }

        public void LateUpdate()
        {
            if (state != RoomState.Disposing && state != RoomState.Disposed)
                sessionStateMachine.LateUpdate();
        }

        public void RunFreeGameRule()
        {
            rule.Update(ContextsWrapper.contexts, interval);
            if (rule.GameOver)
            {
                logger.InfoFormat("Rule Game Over!");
                GameOver(false, RoomState.RRuleOver);
                rule.GameEnd(ContextsWrapper.contexts);
                rule.GameOver = false;
            }

            if (rule.GameExit)
            {
                logger.InfoFormat("Rule Game Exit!");
                GameExit(RoomState.RRuleOver);
            }
        }

        public void SendSnapshot()
        {
            sendSnapshotManager.SendSnapshot(interval, snapshotFactory);
        }

        public void CompensationSnapshot()
        {
            compensationManager.CompensationSnapshot(ContextsWrapper,snapshotFactory);
        }

        public void Dispose()
        {
#if UNITYSOURCEMODIFIED && !UNITYEDITOR
            UnityProfiler.EnableProfiler(false);

#endif
            state      = RoomState.Disposing;
            isDisposed = true;
            DisposePlayerConnections();
            if (sendSnapshotManager != null)
            {
                sendSnapshotManager.Dispose();
            }

            GameModuleManagement.Dispose();
            try
            {
                ResetContexts(true);
                sessionStateMachine.ShutDown();
            }
            catch (Exception e)
            {
                logger.ErrorFormat("Reset Contexts Error {0}", e);
            }

       

            DestoryObjectUnderDefaultGoBattleServer();
            ContextsWrapper.CoRoutineManager.StartCoRoutine(UnloadScene());
            ContextsWrapper.Dispose();
            logger.InfoFormat("Server Room is Disposing...");
        }

        public void SetHallRoom(IHallRoom hallRoom)
        {
            this.hallRoom = hallRoom;
        }

        public void SetGameMode(int mode, int mapId)
        {
            ContextsWrapper.contexts.session.serverSessionObjects.GameRule = mode;
            ContextsWrapper.RoomInfo.ModeId = mode;
            ContextsWrapper.RoomInfo.MapId  = mapId;
            FreeRuleEventArgs args = new FreeRuleEventArgs(ContextsWrapper.contexts);
            ContextsWrapper.FreeArgs = args;
            rule                     = new FreeGameRule(this);


            SimpleParaList spl = (SimpleParaList) args.GetDefault().GetParameters();
            spl.AddFields(new ObjectFields(ContextsWrapper.RoomInfo));
            spl.AddPara(new BoolPara("hxMode", SharedConfig.IsHXMod));
            spl.AddPara(new StringPara("version", FreeRuleConfig.GetVersion()));
        }

        private NetworkMessageDispatcher CreateNetworMessageHandlers()
        {
            var messageDispatcher = new NetworkMessageDispatcher((int)EClient2ServerMessage.Max);
            messageDispatcher.RegisterLater((int) EClient2ServerMessage.UserCmd, new UserCmdMessageHandler(this));
            messageDispatcher.RegisterLater((int) EClient2ServerMessage.LocalDisconnect,
                new DelegateNetworkMessageHandler(OnDisconnect));
            messageDispatcher.RegisterLater((int) EClient2ServerMessage.LocalLogin,
                new DelegateNetworkMessageHandler(AsyncLoginPlayer));
            messageDispatcher.RegisterLater((int) EClient2ServerMessage.VehicleCmd,
                new VehicleCmdMessageHandler(ContextsWrapper.contexts, this));
            messageDispatcher.RegisterLater((int) EClient2ServerMessage.DebugCommand,
                new DebugMessageHandler(ContextsWrapper.contexts, this));
            messageDispatcher.RegisterLater((int) EClient2ServerMessage.SimulationTimeSync,
                new SimulationTimeServerSyncHandler(ContextsWrapper.contexts, this));
            messageDispatcher.RegisterLater((int) EClient2ServerMessage.FreeEvent,
                new FreeEventMessageHandler(ContextsWrapper.contexts, this));
            messageDispatcher.RegisterLater((int) EClient2ServerMessage.VehicleEvent,
                new VehicleEventMessageHandler(ContextsWrapper.contexts, this));
            messageDispatcher.RegisterLater((int) EClient2ServerMessage.TriggerObjectEvent,
                new TriggerObejctEventMessageHandler(ContextsWrapper.contexts, this));
            messageDispatcher.RegisterImmediate((int) EClient2ServerMessage.Ping,
                new PingReqMessageHandler(ContextsWrapper.contexts, this));
            messageDispatcher.RegisterLater((int) EClient2ServerMessage.UpdateMsg,
                new UserUpdateMsgHandler(this, ContextsWrapper.contexts));
            messageDispatcher.RegisterLater((int) EClient2ServerMessage.UpdateMsg, new UserUpdateAckMsgHandler(this));
            messageDispatcher.RegisterLater((int) EClient2ServerMessage.FireInfo,
                new FireInfoMessageHandler(ContextsWrapper.contexts, this));
            messageDispatcher.RegisterLater((int) EClient2ServerMessage.DebugScriptInfo,
                new DebugScriptInfoMessageHandler(this));
            messageDispatcher.RegisterLater((int) EClient2ServerMessage.GameOver,
                new ServerGameOverMessageHandler(this));
            return messageDispatcher;
        }


        public void OnInitializeCompleted()
        {
            logger.Info("Server Room Initialize Completed");
            state = RoomState.Running;
            rule.GameStart(ContextsWrapper.contexts);
            ContextsWrapper.SessionMode =
                            ModeUtil.CreateSharedPlayerMode(ContextsWrapper.contexts,
                                ContextsWrapper.RoomInfo.ModeId);
            DebugUtil.InitShootArchiveS(ContextsWrapper.RoomInfo.RoomDisplayId);
            FreeMapPosition.Init(ContextsWrapper.contexts);
            DebugUtil.LogInUnity("Server Room Initialize Completed");

            if (channelToPlayer.Count == 0)
            {
                /*GameOver(true);*/
            }
        }


        public IPlayerInfo GetPlayerInfo(long playerId)
        {
            return hallRoom == null ? null : hallRoom.GetPlayer(playerId);
        }

        public RoomDebugInfo GetRoomDebugInfo()
        {
            var debugInfo = new RoomDebugInfo();
            debugInfo.State      = state.ToString();
            debugInfo.HallRoomId = hallRoom == null ? 0 : hallRoom.HallRoomId;
            debugInfo.RoomId     = RoomId.Id;
            return debugInfo;
        }

        public IList<PlayerDebugInfo> GetPlayerDebugInfo()
        {
            var debugInfo     = new List<PlayerDebugInfo>();
            var playerInfoSet = new HashSet<IPlayerInfo>();
            foreach (var player in channelToPlayer.Values)
            {
                var pdinfo = new PlayerDebugInfo();
                pdinfo.HasPlayerEntity = true;
                pdinfo.EntityKey       = player.entityKey.Value;
                pdinfo.EntityId        = player.playerInfo.EntityId;
                pdinfo.PlayerId        = player.playerInfo.PlayerId;
                pdinfo.TeamId          = player.playerInfo.TeamId;
                pdinfo.Name            = player.playerInfo.PlayerName;
                pdinfo.HasPlayerInfo   = false;
                if (hallRoom != null)
                {
                    var playerInfo = hallRoom.GetPlayer(pdinfo.PlayerId);

                    if (playerInfo != null)
                    {
                        pdinfo.HasPlayerInfo = true;
                        pdinfo.IsRobot       = playerInfo.IsRobot;
                        pdinfo.IsLogin       = playerInfo.IsLogin;
                        pdinfo.Token         = playerInfo.Token;
                        pdinfo.CreateTime    = playerInfo.CreateTime;
                        pdinfo.GameStartTime = playerInfo.GameStartTime;
                        playerInfoSet.Add(playerInfo);
                    }
                }

                debugInfo.Add(pdinfo);
            }

            if (hallRoom != null)
            {
                var allPlayerInfos = hallRoom.GetAllPlayers();
                foreach (var playerInfo in allPlayerInfos)
                {
                    if (!playerInfoSet.Contains(playerInfo))
                    {
                        var pdinfo = new PlayerDebugInfo();
                        pdinfo.HasPlayerEntity = false;
                        pdinfo.HasPlayerInfo   = true;

                        pdinfo.PlayerId = playerInfo.PlayerId;
                        pdinfo.EntityId = playerInfo.EntityId;
                        pdinfo.TeamId   = playerInfo.TeamId;
                        pdinfo.Name     = playerInfo.PlayerName;

                        pdinfo.IsRobot       = playerInfo.IsRobot;
                        pdinfo.Token         = playerInfo.Token;
                        pdinfo.CreateTime    = playerInfo.CreateTime;
                        pdinfo.GameStartTime = playerInfo.GameStartTime;
                        debugInfo.Add(pdinfo);
                    }
                }
            }

            return debugInfo;
        }


        public void RegisterDebugInfoHandler(Action<string> handler)
        {
            DebugScriptInfoMessageHandler.SetHandler(handler);
        }

        public void AsyncLoginPlayer(INetworkChannel channel, int messageType, object playerInfoObj)
        {
            IPlayerInfo playerInfo = (IPlayerInfo) playerInfoObj;
            logger.InfoFormat("Received LocalLogin Message ... playerName:{0}", playerInfo.PlayerName);

            if (channelToPlayer.Count == 0 && ContextsWrapper.contexts.player.count == 0)
            {
                ResetContexts(false);
                logger.InfoFormat("Reset All Entity Finish ...");
            }

            if (!channelToPlayer.ContainsKey(channel))
            {
                UpdateTestPlayerInfo(playerInfo);
                // 大厅传入错误RoleModelId
                if (null == SingletonManager.Get<RoleConfigManager>().GetRoleItemById(playerInfo.RoleModelId))
                {
                    logger.Error("RoleModelIdError:  " + playerInfo.RoleModelId);
                    playerInfo.RoleModelId = 2;
                }

                var player = CreateNewPlayerEntity(playerInfo);
                playerInfo.PlayerEntity = player;
                player.ReplaceNetwork(channel);

                playerInfo.StatisticsData = player.statisticsData.Statistics;
                if (ContextsWrapper.FreeArgs.Rule.GameStartTime > 0)
                {
                    playerInfo.StatisticsData.GameJoinTime =
                                    ContextsWrapper.FreeArgs.Rule.ServerTime;
                }

                channelToPlayer[channel] =  player;
                channel.MessageReceived  += ChannelOnMessageReceived;
                channel.Disconnected     += ChannelOnDisonnected;


                if (!player.hasUpdateMessagePool)
                    player.AddUpdateMessagePool();
                player.updateMessagePool.LastestExecuteUserCmdSeq = -1;

                playerInfo.InitPosition = player.position.Value;

                NoticeHallPlayerLoginSucc(player);
                player.ReplaceStage(EPlayerLoginStage.CreateEntity);
                var msg = PlayerInfoMessage.Allocate();
                msg.ConvertFrom(playerInfo);
                channel.SendReliable((int) EServer2ClientMessage.PlayerInfo, msg);
                logger.InfoFormat("player login with name {0}, key {1}, game rule {2}, msp id {3}",
                    playerInfo.PlayerName, player.entityKey, 0, 0);
                msg.ReleaseReference();
            }
            else
            {
                logger.ErrorFormat("player duplicate login from name:{0}, channe:{1}", playerInfo.PlayerName, channel);
            }
        }

        private void NoticeHallPlayerLoginSucc(PlayerEntity player)
        {
            if (hallRoom != null)
            {
                hallRoom.UpdatePlayerStatus(player.playerInfo.PlayerId, EPlayerGameStatus.MIDDLE);
                hallRoom.PlayerLoginSucc(player.playerInfo.PlayerId);
            }
        }

        private void AddRoomInfoToMessage(RoomInfo roomInfo, LoginSuccMessage message)
        {
        }

        private void UpdateTestPlayerInfo(IPlayerInfo playerInfo)
        {
            if (playerInfo.Token == TestUtility.TestToken)
            {
                playerInfo.PlayerId    = TestUtility.NewPlayerId;
                playerInfo.PlayerName  = "Test" + playerInfo.PlayerId;
                playerInfo.Num         = ++testPlayerNum;
                playerInfo.Camp        = testPlayerNum % 2 == 0 ? 2 : 1;
                playerInfo.RoleModelId = 1;
                playerInfo.TeamId      = playerInfo.Camp;
                playerInfo.BadgeId     = 15;
                playerInfo.AvatarIds   = new List<int> {77, 319, 320, 321};
                playerInfo.WeaponBags  = PlayerEntityFactory.MakeFakeWeaponBag();
                playerInfo.CampInfo = new CampInfo(1, new List<Preset>
                {
                                new Preset(2, 12, new List<int> {325, 326, 327}, 1005),
                                new Preset(1, 1, new List<int> {77, 319, 320, 321}, 1003)
                });
            }
            else if (playerInfo.Token == TestUtility.RobotToken)
            {
                playerInfo.PlayerId   = TestUtility.NewPlayerId;
                playerInfo.PlayerName = "Robot" + playerInfo.PlayerId;
                playerInfo.Num        = ++testPlayerNum;
                playerInfo.Camp       = testPlayerNum % 2 == 0 ? 1 : 2;
                playerInfo.TeamId     = playerInfo.Camp;
                playerInfo.AvatarIds  = new List<int> {354};
                playerInfo.WeaponBags = PlayerEntityFactory.MakeFakeWeaponBag();
            }
        }

        private PlayerEntity CreateNewPlayerEntity(IPlayerInfo playerInfo)
        {
            return PlayerEntityFactory.CreateNewServerPlayerEntity(ContextsWrapper.contexts.player,
                ContextsWrapper.contexts.session.commonSession,
                ContextsWrapper.contexts.session.commonSession.EntityIdGenerator,
                ContextsWrapper.contexts.session.commonSession.InitPosition,
                //SingletonManager.Get<MapConfigManager>().SceneParameters.PlayerBirthPosition,
                playerInfo, hallRoom.AllowReConnect);
        }

        public PlayerEntity GetPlayerEntity(INetworkChannel channel)
        {
            PlayerEntity player;
            if (channelToPlayer.TryGetValue(channel, out player))
            {
            }

            return player;
        }

        public void ChannelOnDisonnected(INetworkChannel channel)
        {
            MessageDispatcher.SaveDispatch(channel, (int) EClient2ServerMessage.LocalDisconnect, null);
            channel.Serializer.Dispose();
        }


        public void OnDisconnect(INetworkChannel channel, int messageType, object messageBody)
        {
            PlayerEntity player = GetPlayerEntity(channel);

            if (player != null)
            {
                try
                {
                    logger.InfoFormat("player disconnected id {0}", player.entityKey);
                    if (hallRoom != null && hallRoom.AllowReConnect)
                    {
                        logger.InfoFormat("player AllowReConnect id {0}", player.entityKey);
                        player.stage.Value = EPlayerLoginStage.Offline;
                        player.RemoveNetwork();
                        player.RemoveUpdateMessagePool();
                    }
                    else
                    {
                        BulletPlayerUtil.DoProcessPlayerHealthDamage(ContextsWrapper.contexts,
                            ContextsWrapper.FreeArgs.Rule as IGameRule, null, player,
                            new PlayerDamageInfo(0, (int) EUIDeadType.NoHelp, 0, 0, false, false, true));

                        player.isFlagDestroy = true;

                        if (player.hasFreeData)
                        {
                            if (player.isInitialized)
                            {
                                logger.InfoFormat("player PlayerLeave id {0}", player.entityKey);
                                rule.PlayerLeave(ContextsWrapper.contexts, player);
                                player.isInitialized = false;
                            }
                        }
                        else
                        {
                            logger.ErrorFormat("Leave Player {0} Id {1} without Free Data ",
                                player.playerInfo.PlayerName, player.playerInfo.PlayerId);
                        }

                        if (hallRoom != null)
                        {
                            logger.InfoFormat("player PlayerLeaveRoom id {0}", player.entityKey);
                            hallRoom.PlayerLeaveRoom(player.playerInfo.PlayerId);
                        }
                    }
                }
                catch (Exception e)
                {
                    logger.ErrorFormat("player disconnected error: {0} \n {1}", e.Message, e.StackTrace);
                }
                finally
                {
                    int channelToPlayerCount = channelToPlayer.Count;
                    try
                    {
                        channel.MessageReceived -= ChannelOnMessageReceived;
                        channel.Disconnected    -= ChannelOnDisonnected;
                        channelToPlayer.Remove(channel);
                        channel.Dispose();
                    }
                    catch (Exception e)
                    {
                        logger.ErrorFormat("player Remove error: {0} \n {1}", e.Message, e.StackTrace);
                    }
                    finally
                    {
                        channelToPlayerCount--;
                    }

                    if (channelToPlayerCount == 0)
                    {
                        if (hallRoom == null && hallRoom.AllowReConnect)
                        {
                            logger.ErrorFormat("AllowReConnect");
                        }
                        else
                        {
                            GameOver(true, RoomState.RGameOver);
                        }
                    }
                }
            }
            else
            {
                logger.ErrorFormat("illegal ChannelOnDisonnected event received {0}", channel);
            }
        }

        private void SetGameOverStatus(bool forceOver)
        {
            logger.InfoFormat("Set ServerRoom GameOver Status, GameOver {0} ForceOver {1}", isGameOver, forceOver);
            if (!isGameOver)
            {
                isGameOver    = true;
                testPlayerNum = 0;
                if (null != hallRoom)
                {
                    hallRoom.GameOver(forceOver);
                    if (forceOver)
                        hallRoom = null;
                }
            }
        }

        private void GameExit(RoomState state)
        {
            SetGameOverStatus(true);

            if (!isGameExit)
            {
                SendGameOverMessageToAllPlayers();
                var evt = RoomEvent.AllocEvent<GameExitEvent>();
                eventDispatcher.AddEvent(evt);
                isGameExit = true;
            }
        }

        private void SendGameOverMessageToAllPlayers()
        {
            foreach (var channel in channelToPlayer.Keys)
            {
                if (channel.IsConnected)
                {
                    var msg = GameOverMesssage.Allocate();
                    channel.SendReliable((int) EServer2ClientMessage.GameOver, msg);
                    msg.ReleaseReference();
                }
            }
        }

        public void ChannelOnMessageReceived(INetworkChannel networkChannel, int messageType, object messageBody)
        {
            MessageDispatcher.SaveDispatch(networkChannel, messageType, messageBody);
        }

        private void ResetContexts(bool dispose)
        {
            ICommonSessionObjects sessionObjects = ContextsWrapper.contexts.session.commonSession;

            foreach (var entity in ContextsWrapper.contexts.player.GetEntities())
            {
                DestroyEntity(sessionObjects, entity);
                entity.isFlagDestroy = true;
            }

            foreach (var entity in ContextsWrapper.contexts.mapObject.GetEntities())
            {
                if (dispose || !entity.hasReset)
                {
                    DestroyEntity(sessionObjects, entity);
                    entity.isFlagDestroy = true;
                }
                else
                {
                    entity.reset.ResetAction(entity);
                }
            }

            foreach (var entity in ContextsWrapper.contexts.sceneObject.GetEntities())
            {
                DestroyEntity(sessionObjects, entity);
                entity.isFlagDestroy = true;
            }

            foreach (var entity in ContextsWrapper.contexts.freeMove.GetEntities())
            {
                DestroyEntity(sessionObjects, entity);
                entity.isFlagDestroy = true;
            }

            foreach (var entity in ContextsWrapper.contexts.bullet.GetEntities())
            {
                DestroyEntity(sessionObjects, entity);
                entity.isFlagDestroy = true;
            }

            foreach (var entity in ContextsWrapper.contexts.sound.GetEntities())
            {
                DestroyEntity(sessionObjects, entity);
                entity.isFlagDestroy = true;
            }

            foreach (var entity in ContextsWrapper.contexts.vehicle.GetEntities())
            {
                if (entity.hasEntityKey)
                {
                    DestroyEntity(sessionObjects, entity);
                    entity.isFlagDestroy = true;
                }
            }

            ContextsWrapper.contexts.session.currentTimeObject.CurrentTime = 0;
        }

        private void DestroyEntity(ICommonSessionObjects sessionObjects, Entity entity)
        {
            foreach (var comp in entity.GetComponents())
            {
                if (comp is IAssetComponent)
                {
                    ((IAssetComponent) comp).Recycle(sessionObjects.AssetManager);
                }
            }

            if (sessionObjects.AssetManager != null)
                sessionObjects.AssetManager.LoadCancel(entity);
        }

        private void DisposePlayerConnections()
        {
            foreach (var channelPair in channelToPlayer)
            {
                logger.InfoFormat("Disconnect player {0} on disposing...", channelPair.Value.entityKey);
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
            yield return ContextsWrapper.AssetManager.Clear();

            var count     = SceneManager.sceneCount;
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
            logger.InfoFormat("Server Room is Disposed!");
            state = RoomState.Disposed;
        }

        private void DestoryObjectUnderDefaultGoBattleServer()
        {
            //unload gameobject under 'ServerScene/DefaultGoBattleServer'
            var serverScene = SceneManager.GetSceneByName("ServerScene");
            if (serverScene.isLoaded)
            {
                var rootObjects = serverScene.GetRootGameObjects();
                var children    = new List<GameObject>();
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
                    Object.Destroy(child);
                }
            }
        }

        public void SetRoomInfo(IHallRoom room)
        {
            ContextsWrapper.RoomInfo.CopyFrom(room);
        }

        private void CheckRoomGameStatus(int interval)
        {
            //check hall room timeout
            if (null != hallRoom && !hallRoom.IsValid)
            {
                GameOver(true, RoomState.RValid);
            }

            //check player
            sumCheckTime += interval;
            if (sumCheckTime > 20000)
            {
                sumCheckTime = 0;
                string playerName  = "";
                string playerIp    = "";
                bool   hasHallRoom = hallRoom != null;
                if (ContextsWrapper.contexts.player.GetEntities().Length > 0)
                {
                    PlayerEntity player = ContextsWrapper.contexts.player.GetEntities()[0];
                    playerName = player.playerInfo.PlayerName;
                    playerIp   = player.hasNetwork ? player.network.NetworkChannel.IdInfo() : "offline";
                }

                //update status
                if (hallRoom != null)
                {
                    hallRoom.UpdateRoomGameStatus((ERoomGameStatus) rule.GameStage,
                        (ERoomEnterStatus) rule.EnterStatus);
                }

                logger.InfoFormat(
                    "ServerStatus: PlayerCount:{0}, FirstPlayerName:{1}, PlayerIp:{2}, LoginServer:{3}, HallRoomServer:{4}, AllocationClient:{5}, HasHallRoom:{6}",
                    channelToPlayer.Count, playerName, playerIp, ServerStatusCollectUtil.LoginServerStatus,
                    ServerStatusCollectUtil.HallRoomServerStatus, ServerStatusCollectUtil.AllocationClientStatus,
                    hasHallRoom);
            }
        }


        #region intialize

        public ServerRoom(RoomId id, ContextsServerWrapper contextsServerWrapper, RoomEventDispatcher dispatcher,
                          IPlayerTokenGenerator playerTokenGenerator)
        {
            state               = RoomState.Initialized;
            isDisposed          = false;
            RoomId              = id;
            ContextsWrapper     = contextsServerWrapper;
            MessageDispatcher   = CreateNetworMessageHandlers();
            tokenGenerator      = playerTokenGenerator;
            eventDispatcher     = dispatcher;
            snapshotFactory     = new SnapshotFactory(ContextsWrapper.GameContexts);
            sessionStateMachine = new ServerSessionStateMachine(ContextsWrapper.contexts, this);
            damager             = new SimplePlayerDamager(this);
            compensationManager = new CompensationManager();

            VehicleDamageUtility._damager = damager;
            sendSnapshotManager           = new SendSnapshotManager(ContextsWrapper.contexts);
        }



        #endregion
    }
}