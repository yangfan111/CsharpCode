using System.IO;
using Core.EntitasAdpater;
using Core.Replicaton;
using Core.SnapshotReplication;

namespace App.Shared.Network.SerializeInfo
{
    public class ReplicatedSnapshotSerializeInfo : AbstractSnapshotSerializeInfo
    {
        private SnapshotReplicator _snapshotReplicator;

        public ReplicatedSnapshotSerializeInfo(SnapshotReplicator snapshotReplicator) : base()
        {
            _snapshotReplicator = snapshotReplicator;

        }

        public override void DoSerialize(Stream outStream, Snapshot message)
        {

            _snapshotReplicator.Serialize(outStream, message);
        }

        public override void Dispose()
        {
            _snapshotReplicator.Dispose();
        }


        public override object Deserialize(Stream inStream)
        {
            var rc = _snapshotReplicator.Deserialize(inStream);


#pragma warning disable RefCounter002
            ISnapshot old = (ISnapshot) rc;
#pragma warning restore RefCounter002
            ISnapshot copy = SnapshotFactory.CloneSnapshot(old);
            old.ReleaseReference();

            // boost performance
            {
                var a = copy.LatestEntityMap;
                var b = copy.CompensationEntityMap;
                var c = copy.NonSelfEntityMap;
                var d = copy.SelfEntityMap;
                foreach (var entity in copy.EntityMap.Values)
                {
                    var e = entity.SortedComponentList;
                    var h= entity.PlayBackComponentDictionary;
                    var j= entity.SyncLatestComponentDictionary;
                }
            }
            return copy;

        }

        public SnapshotReplicator SnapshotReplicator
        {
            get { return _snapshotReplicator; }
        }
    }
}