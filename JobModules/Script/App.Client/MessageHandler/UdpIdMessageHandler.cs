using App.Protobuf;
using Core.Network;
using Utils.Singleton;

namespace App.Client.MessageHandler
{
    public class UdpIdMessageHandler : INetworkMessageHandler
    {
        public UdpIdMessageHandler()
        {
        }

        public void Handle(INetworkChannel networkChannel, int messageType, object messageBody)
        {
            UdpIdMessage message = (UdpIdMessage) messageBody;
            networkChannel.UdpPort = message.Port;
            networkChannel.RemoteConnId = message.Id;
            SingletonManager.Get<Core.Utils.ServerInfo>().ServerId = message.ServerId;
            Core.Utils.Version.Instance.RemoteAsset = message.ServerAsset;
            Core.Utils.Version.Instance.RemoteVersion = message.ServerVersion;
        }
    }
}
