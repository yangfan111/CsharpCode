namespace Core.Network
{
    public interface INetworkClientFactory
    {
        INetworkClient CreateNetworkClient(bool littleEndian, bool isMultiThread, string name = "default");
        INetworkClient CreateTcpNetworkClient(bool littleEndian, bool isMultiThread, string name = "default");
    }
}