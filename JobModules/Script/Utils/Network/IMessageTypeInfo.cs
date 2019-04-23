using System;
using System.Text;

namespace Core.Network
{
    public interface IMessageTypeInfo : IDisposable
    {
        ISerializeInfo GetSerializeInfo(int messageType);
        void SetReplicationAckId(int id);
        
        void PrintDebugInfo(StringBuilder sb);
        bool SkipSendSnapShot(int serverTime);
        void IncSendSnapShot();
        int LatestUpdateMessageSeq { get; }
    }
}