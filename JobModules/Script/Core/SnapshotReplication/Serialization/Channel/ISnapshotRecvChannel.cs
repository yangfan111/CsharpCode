using System.IO;
using Core.Replicaton;

namespace Core.SnapshotReplication.Serialization.Channel
{
    interface ISnapshotRecvChannel
    {
        ISnapshot DeSerializeSnapshot(BinaryReader reader);
    }
}
