using System.IO;
using Core.ObjectPool;

namespace Core.Network.ENet
{
    public class NetworkMessageItem
    {
        public int MessageType;
        public object MessageBody;
        public MemoryStream MemoryStream;
        public NetworkMessageItem(int messageType, object messageBody)
        {
            MessageType = messageType;
            MessageBody = messageBody;
        }

        public void RefMessageBody()
        {
            if (MessageBody != null && MessageBody is IRefCounter)
            {
                (MessageBody as IRefCounter).AcquireReference();
            }
        }

        public void ReleaseMessageBody()
        {
            if (MessageBody != null && MessageBody is IRefCounter)
            {
                (MessageBody as IRefCounter).ReleaseReference();
            }
            MessageBody = null;
        }
    }
}