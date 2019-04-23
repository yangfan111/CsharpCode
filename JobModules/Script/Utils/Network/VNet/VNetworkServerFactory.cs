using Core.Network;
using VNet.Base.LiteNet;
using VNet.Base.Tcp;

namespace VNet
{
    public class VNetworkServerFactory : INetworkServerFactory
    {
        public INetworkServer CreateNetworkServer(bool littleEndian,string name="default")
        {
            return new VNetworkServer(new TcpServer(littleEndian), new LiteNetServer(), littleEndian, name);
        }

        public INetworkServer CreateTcpNetworkServer(bool littleEndian,string name="default")
        {
            return new VNetworkServer(new TcpServer(littleEndian), null, littleEndian,name);
        }
    }
}
