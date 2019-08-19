using System;
using App.Client.Console;
using App.Client.Console.MessageHandler;
using App.Client.MessageHandler;
using App.Protobuf;
using App.Shared;
using App.Shared.Client;
using App.Shared.Network;
using Assets.Sources.Free;
using Assets.Sources.Free.UI;
using Core.Free;
using Core.Network;
using Core.Utils;
using Free.framework;
using Utils.Replay;

namespace App.Client.StartUp.Online
{
    public interface IDeveloperConsoleCommands
    {
        void RegisterFreeCommand(string command, string desc, string usage);
    }
    public class ClientNetwork : IClientNetwork
    {
        private static LoggerAdapter _logger = new LoggerAdapter(typeof(ClientRoom));

        private Contexts _contexts;
        private readonly IDeveloperConsoleCommands _console;

        // private readonly IClientRoom _clientRoom;
        private bool _isDisconnected;

        public ClientNetwork(Contexts contexts, IDeveloperConsoleCommands console = null)
        {
            _contexts = contexts;
            _console = console;
           
        }


        public void OnNetworkConnected(INetworkChannel networkChannel)
        {
            Channel = networkChannel;
            networkChannel.Serializer = new NetworkMessageSerializer(new AppMessageTypeInfo());
            if (SharedConfig.IsRecord)
            {
                networkChannel.Recoder = _contexts.session.clientSessionObjects.Record;
            }

            networkChannel.MessageReceived += NetworkChannelOnMessageReceived;
            _contexts.session.clientSessionObjects.NetworkChannel = networkChannel;
        }

        public void NetworkChannelOnMessageReceived(INetworkChannel networkChannel, int messageType, object messageBody)
        {
            var messageDispatcher = _contexts.session.clientSessionObjects.MessageDispatcher;
            messageDispatcher.SaveDispatch(networkChannel, messageType, messageBody);
        }

        public void OnNetworkDisconnected()
        {
            _logger.ErrorFormat("Client Disconnected ");
            _isDisconnected = true;
        }

        public void Init()
        {
            InitNetworMessageHandlers();
        }

        public INetworkChannel Channel { get; private set; }

        private void InitNetworMessageHandlers()
        {
            var sessionObjects = _contexts.session.clientSessionObjects;

            var messageDispatcher = sessionObjects.MessageDispatcher;
            messageDispatcher.RegisterLater((int) EServer2ClientMessage.Snapshot,
                new SnapshotMessageHandler(sessionObjects.SnapshotSelctor, sessionObjects.UpdateLatestHandler,
                    sessionObjects.TimeManager));
            messageDispatcher.RegisterLater((int) EServer2ClientMessage.UdpId, new UdpIdMessageHandler());
            messageDispatcher.RegisterLater((int) EServer2ClientMessage.Snapshot,
                new SnapshotSyncTimeHandler(sessionObjects.TimeManager));
            messageDispatcher.RegisterLater((int) EServer2ClientMessage.FreeData, SimpleMessageManager.Instance);
            messageDispatcher.RegisterLater((int) EServer2ClientMessage.Statistics,
                new StatisticsMessageHandler(_contexts));
            //messageDispatcher.RegisterLater((int)EServer2ClientMessage.Ping, new PingRespMessageHandler(_contexts));
            messageDispatcher.RegisterLater((int) EServer2ClientMessage.DamageInfo,
                new DamageInfoMessageHandler(_contexts.player, _contexts.ui));
            messageDispatcher.RegisterImmediate((int) EServer2ClientMessage.Ping,
                new PingRespMessageHandler(_contexts));
            messageDispatcher.RegisterLater((int) EServer2ClientMessage.UpdateAck,
                new UpdateMessageAckMessageHandler(sessionObjects.UpdateLatestHandler));
            messageDispatcher.RegisterLater((int) EServer2ClientMessage.DebugMessage,
                new ServerDebugMessageHandler(_contexts));
            messageDispatcher.RegisterLater((int) EServer2ClientMessage.ClearScene,
                new ClearSceneMessageHandler(_contexts));
            messageDispatcher.RegisterLater((int) EServer2ClientMessage.GameOver, new ClientGameOverMessageHandler());
            messageDispatcher.RegisterLater((int) EServer2ClientMessage.HeartBeat, new ClientHeartBeatMessageHandler());
            messageDispatcher.RegisterLater((int) EServer2ClientMessage.FireInfoAck, new FireInfoAckMessageHandler());
           
            messageDispatcher.RegisterLater((int) EServer2ClientMessage.FreeData, new ClientRegisterCommandHandler(_console));
        }
    }

    internal class ClientRegisterCommandHandler : AbstractClientMessageHandler<SimpleProto>
    {
        private static LoggerAdapter _logger = new LoggerAdapter(typeof(ClientRoom));
        private readonly IDeveloperConsoleCommands _addCommand;


        public ClientRegisterCommandHandler(IDeveloperConsoleCommands addCommand)
        {
            _addCommand = addCommand;
        }

        public override void DoHandle(int messageType, SimpleProto sp)
        {

            if (sp.Key == FreeMessageConstant.RegisterCommand && _addCommand != null)
            {
                _logger.InfoFormat("RegisterCommand:{0} {1} {2}", sp.Ss[0], sp.Ss[1], sp.Ss[2]);
                _addCommand.RegisterFreeCommand(sp.Ss[0], sp.Ss[1], sp.Ss[2]);
            }
        }
    }
}