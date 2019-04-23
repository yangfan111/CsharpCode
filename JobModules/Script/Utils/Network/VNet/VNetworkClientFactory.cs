using Core.Network;
using VNet.Base.LiteNet;
using VNet.Base.Tcp;

namespace VNet
{
    public class VNetworkClientFactory : INetworkClientFactory
    {
        public INetworkClient CreateNetworkClient(bool littleEndian, bool isMultiThread, string name = "default")
        {
            return new VNetworkClient(new TcpClient(littleEndian), new LiteNetClient(), littleEndian, name,
                isMultiThread);
        }

        public INetworkClient CreateTcpNetworkClient(bool littleEndian, bool isMultiThread, string name = "default")
        {
            return new VNetworkClient(new TcpClient(littleEndian), null, littleEndian, name, isMultiThread);
        }
    }
}