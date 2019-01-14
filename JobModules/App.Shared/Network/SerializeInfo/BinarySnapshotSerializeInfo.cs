using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using Core.Replicaton;

namespace App.Shared.Network
{
    public class BinarySnapshotSerializeInfo : AbstractSnapshotSerializeInfo
    {
     

        public override void DoSerialize(Stream outStream, Snapshot message)
        {
            BinaryFormatter serializer = new BinaryFormatter();
            serializer.Serialize(outStream, message);
        }

	    public override void Dispose()
	    {
	    }

	    public override object Deserialize(Stream inStream)
        {
            BinaryFormatter serializer = new BinaryFormatter();
            return serializer.Deserialize(inStream);
        }


        
    }
}