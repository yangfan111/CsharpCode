using App.Protobuf;
using Core.Network;
using Core.Utils;
using Utils.Singleton;

namespace App.Client.MessageHandler
{
    public class UdpIdMessageHandler : INetworkMessageHandler
    {
        LoggerAdapter _logger = new LoggerAdapter( typeof(UdpIdMessageHandler));
        public UdpIdMessageHandler()
        {
        }

        public void Handle(INetworkChannel networkChannel, int messageType, object messageBody)
        {
           
            UdpIdMessage message = (UdpIdMessage) messageBody;
           
            _logger.ErrorFormat("UdpIdMessag handle :{0} {1}", message.Port,message.Id );
            networkChannel.RealTimeConnect( message.Port, message.Id);
            SingletonManager.Get<Core.Utils.ServerInfo>().ServerId = message.ServerId;
            Core.Utils.Version.Instance.RemoteAsset = message.ServerAsset;
            Core.Utils.Version.Instance.RemoteVersion = message.ServerVersion;
        }
    }
}
