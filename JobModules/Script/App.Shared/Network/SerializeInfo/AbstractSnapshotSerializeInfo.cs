using System.IO;
using Core.EntityComponent;
using Core.Network;
using Core.Prediction;
using Core.Replicaton;

namespace App.Shared.Network
{
    public abstract class AbstractSnapshotSerializeInfo : ISerializeInfo
    {
        
        
        private SerializationStatistics _statistics = new SerializationStatistics("snapshot");
        public SerializationStatistics Statistics { get { return _statistics; } }
        public void Serialize(Stream outStream, object message)
        {
           // Snapshot message1 = CloneAndFilterSnapshot((ISnapshot)message);
            DoSerialize(outStream, (Snapshot)message);
            //message1.ReleaseReference();
        }

       
        public abstract object Deserialize(Stream inStream);
        


        public abstract void DoSerialize(Stream outStream, Snapshot message);

       
        
        private Snapshot CloneAndFilterSnapshot(ISnapshot srcSnapshot)
        {
            
            Snapshot dstSnapshot = Snapshot.Allocate();
            dstSnapshot.Header = srcSnapshot.Header;

            EntityMapDeepCloner.Clone(dstSnapshot, srcSnapshot, new Core.Replicaton.DummyEntityMapFilter());

            // filter entities here, e.g. delete bullet
            /**
             * 
             */
            return dstSnapshot;
        }

	    public abstract void Dispose();
    }
}