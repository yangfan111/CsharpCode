using App.Protobuf;
using App.Shared;
using App.Shared.Components.Serializer;
using Core.Network;
using Core.SessionState;
using Core.Utils;
using Entitas;
using Utils.Singleton;

namespace App.Client.GameModules.ClientInit
{
   
    public class RequestSnapshotSystem : IInitializeSystem, INetworkMessageHandler, IExecuteSystem
    {
        private static LoggerAdapter _logger = new LoggerAdapter(typeof(RequestSnapshotSystem));

        private ISessionState _sessionState;

        private Contexts _contexts;

        public RequestSnapshotSystem(
            Contexts contexts,
            ISessionState sessionState)
        {
            _contexts = contexts;

            _sessionState = sessionState;
            _contexts.session.clientSessionObjects.MessageDispatcher.RegisterLater((int)EServer2ClientMessage.Snapshot, this);
            _sessionState.CreateExitCondition(typeof(RequestSnapshotSystem));
            SingletonManager.Get<SubProgressBlackBoard>().Add();
        }

        private bool _loginSuccReceived;

        private bool _loginSended;

        public void Handle(INetworkChannel networkChannel, int messageType, object messageBody)
        {
            if (messageType == (int)EServer2ClientMessage.Snapshot)
            {
                SingletonManager.Get<SubProgressBlackBoard>().Step();
                _loginSuccReceived = true;
                
            }
        }

        public void Initialize()
        {
            if (SharedConfig.IsOffline)
            {
                _sessionState.FullfillExitCondition(typeof(RequestSnapshotSystem));
            }
        }

        public void Execute()
        {
            var channel = _contexts.session.clientSessionObjects.NetworkChannel;
            var token = _contexts.session.clientSessionObjects.LoginToken;
            if (channel != null && channel.IsConnected)
            {
                if (!_loginSended)
                {
                    _loginSended = true;
                    var message = LoginMessage.Allocate();
                    message.Token = token;
                    message.LoginStage = ELoginStage.RequestSnapshot;
                    message.ComponentSerializerVersion = ComponentSerializerManager.HashMd5;
                    channel.SendReliable((int)EClient2ServerMessage.Login, message);

                    _logger.InfoFormat("Sending Login Message {0}", message);
                    message.ReleaseReference();
                }

                if (_loginSuccReceived)
                {
                    _sessionState.FullfillExitCondition(typeof(RequestSnapshotSystem));
                }
            }
        }
    }
}
