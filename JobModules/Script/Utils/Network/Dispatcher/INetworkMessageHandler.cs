namespace Core.Network
{
    public interface INetworkMessageHandler 
    {
        void Handle(INetworkChannel networkChannel, int messageType, object messageBody);
    }
}