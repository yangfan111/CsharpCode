using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;

namespace Core.Network
{
    public class SimpleBinaryMessageTypeInfo : IMessageTypeInfo
    {
        private ISerializeInfo _serializeInfo = new BinarySrializeInfo();

        public ISerializeInfo GetSerializeInfo(int messageType)
        {
            return _serializeInfo;
        }

        public void SetReplicationAckId(int id)
        {
        }

      

        public void PrintDebugInfo(StringBuilder sb)
        {
        }

        public bool SkipSendSnapShot(int serverTime)
        {
            return false;
        }

        public void IncSendSnapShot()
        {
            throw new System.NotImplementedException();
        }

        public int LatestUpdateMessageSeq { get; set; }

        public void Dispose()
	    {
	    }
    }

    public class BinarySrializeInfo : ISerializeInfo
    {
        private SerializationStatistics _statistics = new SerializationStatistics("binary");
        public SerializationStatistics Statistics { get { return _statistics; } }
        public int Serialize(Stream outStream, object message)
        {
            var start = outStream.Position;
            BinaryFormatter serializer = new BinaryFormatter();
            serializer.Serialize(outStream, message);
            return  (int) (outStream.Position - start);
        }

        public object Deserialize(Stream inStream)
        {
            BinaryFormatter serializer = new BinaryFormatter();
            return serializer.Deserialize(inStream);
        }

	    public void Dispose()
	    {
	    }
    }
}