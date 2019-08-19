using System;
using System.IO;

namespace Core.Network
{
    public class SerializationStatistics
    {
        public SerializationStatistics(string name)
        {
            Name = name;
        }

        public int TotalSerializeCount;
        public int TotalDeserializeCount;
        

        public long TotalSerializeSize;
        public long TotalDeserializeSize;
        public int LatestSerializeSize;
        public int LatestDeserializeSize;

        public string Name { get; set; }
        public void OnSerialize(int length)
        {
            TotalSerializeCount++;
            TotalSerializeSize += length;
            LatestSerializeSize = length;
        }

        public void OnDeserialize(int length)
        {
            TotalDeserializeCount++;
            TotalDeserializeSize += length;
            LatestDeserializeSize = length;
        }
    }
    public interface ISerializeInfo: IDisposable
    {
        int Serialize(Stream outStream, object message);
        object Deserialize(Stream inStream);
        SerializationStatistics Statistics { get; }

    }
}