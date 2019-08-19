namespace VNet.Base.Interface
{
    public interface IRealTimeConnector
    {
        void RealTimeConnect(int udpPort, int connId);
        int UdpPort { get;}
        bool isRealTimeConnected { get; }
        int RemoteConnId { get; }
    }
}
