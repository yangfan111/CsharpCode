using System;
using System.IO;
using Core.Replicaton;

namespace Core.SnapshotReplication.Serialization.Channel
{
    public interface ISnapshotSendChannel : IDisposable
    {
        int AckedSnapshotId { get; }
        void SnapshotReceived(int id);
        void SerializeSnapshot(ISnapshot snap, Stream stream);   
    }
}
