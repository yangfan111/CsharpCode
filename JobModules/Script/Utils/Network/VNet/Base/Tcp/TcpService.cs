using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;

namespace VNet.Base.Tcp
{
    public abstract class TcpService
    {
        protected Dictionary<int, TcpConnection> _connections = new Dictionary<int, TcpConnection>(); 
        protected const int OpsToPreAllloc = 2;
        protected bool IsRunning { get; set; }

        public void Init(int maxConnection = 100, int receiveBufferSize = 4096)
        {

        }
    }
}
