using System;
using System.IO;

namespace VNet.Base.Interface
{
    public interface IVNetServer
    {
        void Init();
        void Bind(int port);
        void Poll();
        void Stop();
        event Action<IVNetPeer, MemoryStream> OnReceiveListener;
        event Action<IVNetPeer> OnDisconnectListener;
        event Action<IVNetPeer> OnAcceptListener;
    }
}
