using System;
using System.IO;

namespace Core.Network
{
    enum MessageTypes
    {
        Login, 
        Snapshot,
        RoomInfo,
        Chat
    }
    
    public interface ISnapshotMessage
    {

    }
    /// <summary>
    /// 序列化的接口根据网络层实现来确定，外部没有约束
    /// </summary>
    public interface INetworkMessageSerializer : IDisposable
    {
        long Serialize(Stream outStream, int messageType, object message);
        object Deserialize(Stream inStream, int messageType);
        IMessageTypeInfo MessageTypeInfo { get; }
    }
}