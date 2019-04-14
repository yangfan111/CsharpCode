using System;
using App.Client.GameModules.GamePlay.Free;
using App.Client.GameModules.GamePlay.Free.Scene;
using App.Client.GameModules.Ui;
using App.Client.MessageHandler;
using App.Client.SessionStates;
using App.Protobuf;
using App.Shared;
using App.Shared.Client;
using App.Shared.Components.Player;
using App.Shared.DebugHandle;
using App.Shared.GameModules.Vehicle;
using App.Shared.Network;
using Assets.App.Client.GameModules.GamePlay.Free;
using Assets.Sources.Free;
using Core.Components;
using Core.EntitasAdpater;
using Core.EntityComponent;
using Core.GameModule.System;
using Core.Network;
using Core.Utils;
using Entitas;
using UnityEngine;
using Core.Free;
using Assets.Sources.Free.UI;
using Core.GameModule.Step;
using Core.MyProfiler;
using Core.SessionState;
using App.Client.Bullet;
using App.Client.Utility;
using Utils.Singleton;

namespace App.Client.Console
{
    public class ClientRoom : IClientRoom, IDebugCommandHandler, IEcsDebugHelper
    {
        private static LoggerAdapter _logger = new LoggerAdapter(typeof(ClientRoom));


        private Contexts _contexts;
        private SessionStateMachine _clientSessionStateMachine;
        private ClientDebugCommandHandler _clientDebugCommandHandler;

        public delegate void AddFreeCommand(string command, string desc, string useage);

        public event AddFreeCommand AddCommand;

        public Contexts Contexts
        {
            get { return _contexts; }
        }

        private bool _isDisconnected = false;
        private bool _isDisposed = false;

        public ClientRoom(IClientContextInitilizer clientContextInitializer)
        {
            _logger.InfoFormat("Platform Endianness is little = {0}", BitConverter.IsLittleEndian);

            _contexts = clientContextInitializer.CreateContexts();
            _clientDebugCommandHandler = new ClientDebugCommandHandler(_contexts);
            if (SharedConfig.InSamplingMode || SharedConfig.InLegacySampleingMode)
                _clientSessionStateMachine = new ClientProfileSessionStateMachine(_contexts);
            else
                _clientSessionStateMachine = new ClientSessionStateMachine(_contexts);
            SingletonManager.Get<MyProfilerManager>().Contexts = _contexts;

            InitNetworMessageHandlers();
        }

        public void OnNetworkConnected(INetworkChannel networkChannel)
        {
            networkChannel.Serializer = new NetworkMessageSerializer(new AppMessageTypeInfo());
            ;
            networkChannel.MessageReceived += NetworkChannelOnMessageReceived;
            _contexts.session.clientSessionObjects.NetworkChannel = networkChannel;

            InitBuleltInfoCollector(networkChannel);
        }

        private void InitBuleltInfoCollector(INetworkChannel networkChannel)
        {
            var sessionObjects = _contexts.session.commonSession;
            sessionObjects.BulletInfoCollector = new ClientBulletInfoCollector(networkChannel);
            _contexts.session.clientSessionObjects.MessageDispatcher.RegisterLater(
                (int) EServer2ClientMessage.FireInfoAck,
                new FireInfoAckMessageHandler(_contexts.player,
                    (ClientBulletInfoCollector) sessionObjects.BulletInfoCollector));
        }

        public void OnNetworkDisconnected()
        {
            _logger.ErrorFormat("Client Disconnected ");
            _isDisconnected = true;
        }

        public string LoginToken
        {
            set { _contexts.session.clientSessionObjects.LoginToken = value; }
        }


        public void Update()
        {
            try
            {
                if (_isDisposed)
                    return;

                if (_isDisconnected)
                {
                    HallUtility.GameOver();
                }
                else
                {
                    SingletonManager.Get<DurationHelp>().ProfileStart(CustomProfilerStep.Room);
                    var sessionObjects = _contexts.session.clientSessionObjects;

                    sessionObjects.MessageDispatcher.DriveDispatch();
                    StepExecuteManager.Instance.Update();
                    _clientSessionStateMachine.Update();
                }
            }
            finally
            {
                SingletonManager.Get<DurationHelp>().ProfileEnd(CustomProfilerStep.Room);
            }
        }

        public void LateUpdate()
        {
            _clientSessionStateMachine.LateUpdate();
        }

        public void Dispose()
        {
            if (_isDisposed) return;
            _isDisposed = true;
            _clientSessionStateMachine.Dispose();
            FreePrefabLoader.Destroy();
            var _sessionObjects = _contexts.session.commonSession;
            _contexts.session.commonSession.Dispose();
            _contexts.session.clientSessionObjects.Dispose();

            if (_contexts.session.clientSessionObjects.NetworkChannel != null)
            {
                _contexts.session.clientSessionObjects.NetworkChannel.Dispose();
            }

            foreach (var info in _sessionObjects.GameContexts.AllContexts)
            {
                foreach (IGameEntity entity in info.GetEntities())
                {
                    foreach (var comp in entity.ComponentList)
                    {
                        if (comp is IAssetComponent)
                        {
                            ((IAssetComponent) comp).Recycle(_sessionObjects.AssetManager);
                        }
                    }

                    if (_sessionObjects.AssetManager != null)
                        _sessionObjects.AssetManager.LoadCancel(entity.RealEntity);
                    entity.Destroy();
                }
            }

            foreach (Entity entity in _contexts.ui.GetEntities())
            {
                DestroyEntity(_sessionObjects, entity);
            }


            foreach (Entity entity in _contexts.sceneObject.GetEntities())
            {
                DestroyEntity(_sessionObjects, entity);
            }

            _clientSessionStateMachine.ShutDown();
            try
            {
                _contexts.Reset();
            }
            catch (Exception ex)
            {
                _logger.ErrorFormat("contexts.Reset error:{0}", ex.Message);
            }

            UiModule.DestroyAll();
            FreeUILoader.Destroy();
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
            entity.Destroy();
        }


        private void InitNetworMessageHandlers()
        {
            var sessionObjects = _contexts.session.clientSessionObjects;

            var messageDispatcher = sessionObjects.MessageDispatcher;
            messageDispatcher.RegisterLater((int) EServer2ClientMessage.Snapshot,
                new SnapshotMessageHandler(sessionObjects.SnapshotPool, sessionObjects.UpdateLatestHandler,
                    sessionObjects.TimeManager));
            messageDispatcher.RegisterLater((int) EServer2ClientMessage.UdpId, new UdpIdMessageHandler());
            messageDispatcher.RegisterImmediate((int) EServer2ClientMessage.Snapshot,
                new SnapshotSyncTimeHandler(sessionObjects.TimeManager));
            messageDispatcher.RegisterLater((int) EServer2ClientMessage.FreeData, SimpleMessageManager.Instance);
            messageDispatcher.RegisterLater((int) EServer2ClientMessage.Statistics,
                new StatisticsMessageHandler(_contexts));
            //messageDispatcher.RegisterLater((int)EServer2ClientMessage.Ping, new PingRespMessageHandler(_contexts));
            messageDispatcher.RegisterLater((int) EServer2ClientMessage.DamageInfo,
                new DamageInfoMessageHandler(_contexts.player, _contexts.ui));
            messageDispatcher.RegisterImmediate((int) EServer2ClientMessage.Ping,
                new PingRespMessageHandler(_contexts));
            messageDispatcher.RegisterImmediate((int) EServer2ClientMessage.UpdateAck,
                new UpdateMessageAckMessageHandler(sessionObjects.UpdateLatestHandler));
            messageDispatcher.RegisterLater((int) EServer2ClientMessage.DebugMessage,
                new ServerDebugMessageHandler(_contexts));
            messageDispatcher.RegisterLater((int) EServer2ClientMessage.ClearScene,
                new ClearSceneMessageHandler(_contexts));
            messageDispatcher.RegisterLater((int) EServer2ClientMessage.GameOver, new ClientGameOverMessageHandler());
            messageDispatcher.RegisterLater((int) EServer2ClientMessage.HeartBeat, new ClientHeartBeatMessageHandler());
        }

        public void NetworkChannelOnMessageReceived(INetworkChannel networkChannel, int messageType, object messageBody)
        {
            if (RegisterCommandHandler.canHandle(messageBody))
            {
                RegisterCommandHandler.Handle(this, messageBody);
            }
            else
            {
                var messageDispatcher = _contexts.session.clientSessionObjects.MessageDispatcher;
                messageDispatcher.SaveDispatch(networkChannel, messageType, messageBody);
            }
        }

        public void RegisterCommand(string command, string desc, string usage)
        {
            if (AddCommand != null)
            {
                AddCommand(command, desc, usage);
            }
        }

        public void SendGameOverMessage()
        {
            try
            {
                var selfPlayer = _contexts.player.flagSelfEntity;
                if (selfPlayer.hasNetwork)
                {
                    var channel = selfPlayer.network.NetworkChannel;
                    if (channel != null)
                    {
                        var msg = GameOverMesssage.Allocate();
                        channel.SendReliable((int) EClient2ServerMessage.GameOver, msg);
                        msg.ReleaseReference();

                        _logger.InfoFormat("Send GameOver Message to Server");
                    }
                }
            }
            catch (Exception e)
            {
                _logger.Error("Fail to Send GameOver Message to Server", e);
            }
        }

        public void SendDebugScriptInfo(string info)
        {
            try
            {
                var selfPlayer = _contexts.player.flagSelfEntity;
                if (selfPlayer.hasNetwork)
                {
                    var channel = selfPlayer.network.NetworkChannel;
                    if (channel != null)
                    {
                        var msg = DebugScriptInfo.Allocate();
                        msg.Info = info;
                        channel.SendReliable((int) EClient2ServerMessage.DebugScriptInfo, msg);
                        msg.ReleaseReference();

                        _logger.DebugFormat("Send Debug Script Message {0}", info);
                    }
                }
            }
            catch (Exception e)
            {
                _logger.Error("Fail to Send DebugScriptInfo", e);
            }
        }

        public void OnDrawGizmos()
        {
            _clientSessionStateMachine.OnDrawGizmos();
        }

        public void OnGUI()
        {
            _clientSessionStateMachine.OnGUI();
        }

        public string OnDebugMessage(DebugCommand message)
        {
            var channel = _contexts.session.clientSessionObjects.NetworkChannel;
            if (channel != null)
            {
                var msg = DebugCommandMessage.Allocate();
                msg.Command = message.Command;
                if (message.Args != null && message.Command != DebugCommands.TestMap &&
                    message.Command != DebugCommands.ClientMove)
                {
                    msg.Args.AddRange(message.Args);
                }

                channel.SendReliable((int) EClient2ServerMessage.DebugCommand, msg);
                msg.ReleaseReference();
            }

            return _clientDebugCommandHandler.OnDebugMessage(message, _clientSessionStateMachine);
        }

        public SessionStateMachine GetSessionStateMachine()
        {
            return _clientSessionStateMachine;
        }
    }
}