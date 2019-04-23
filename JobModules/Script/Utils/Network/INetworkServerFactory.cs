namespace Core.Network
{
    public interface INetworkServerFactory
    {
        INetworkServer CreateNetworkServer(bool littleEndian,string name="default");
        INetworkServer CreateTcpNetworkServer(bool littleEndian,string name="default");
    }
}