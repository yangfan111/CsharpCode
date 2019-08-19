namespace Core.Network
{
    public interface INetworkMessageDispatcher
    {
        void RegisterLater(int messageType, INetworkMessageHandler handler);
        void RegisterImmediate(int messageType, INetworkMessageHandler handler);
        void DriveDispatch();
        void SaveDispatch(INetworkChannel channel, int messageType, object messageBody);
    }
}