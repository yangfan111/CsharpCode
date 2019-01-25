using System.IO;
using Core.Network;
using Core.Replicaton;
using Core.SnapshotReplication.Serialization.Channel;
using Core.SnapshotReplication.Serialization.Serializer;
using Core.Utils;

namespace Core.SnapshotReplication
{
    public class SnapshotReplicator :ISerializeInfo
    {
        private SerializationStatistics _statistics = new SerializationStatistics("snapshot");
        public SerializationStatistics Statistics { get { return _statistics; } }
        private SnapshotRecvChannel _recvChannel;
        private SnapshotSendChannel _sendChannel;
        
        public SnapshotReplicator(INetworkObjectSerializerManager manager)
        {
            
            var serializer = new SnapshotSerializer(manager);
            _recvChannel = new SnapshotRecvChannel(serializer);
            _sendChannel = new SnapshotSendChannel(serializer);
        }
        public void Serialize(Stream outStream, object message)
        {
            AssertUtility.Assert(message is ISnapshot);
            _sendChannel.SerializeSnapshot(message as ISnapshot, outStream);
        }

        public object Deserialize(Stream inStream)
        {
            BinaryReader reader = new BinaryReader(inStream);
            return _recvChannel.DeSerializeSnapshot(reader);
        }

        public SnapshotRecvChannel RecvChannel
        {
            get { return _recvChannel; }
        }

        public SnapshotSendChannel SendChannel
        {
            get { return _sendChannel; }
        }

	    public void Dispose()
	    {
		    _recvChannel.Dispose();
		    _sendChannel.Dispose();
	    }
    }
}
