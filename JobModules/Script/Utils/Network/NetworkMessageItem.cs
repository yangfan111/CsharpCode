using System.IO;
using Core.ObjectPool;

namespace Core.Network.ENet
{
    public abstract class NetworkMessageItem:BaseRefCounter
    {
        public int MessageType;
        public object MessageBody;
        public abstract MemoryStream MemoryStream { get; }

    }
}