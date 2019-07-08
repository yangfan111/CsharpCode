namespace VNet.Base.Interface
{
    public interface IRealTimeConnector
    {
        void RealTimeConnect(int connId);
        int UdpPort { get; set; }
        bool isRealTimeConnected { get; }
    }
}
