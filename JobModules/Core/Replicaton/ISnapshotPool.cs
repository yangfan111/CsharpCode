using System.Collections.Generic;

namespace Core.Replicaton
{
    public interface ISnapshotPool
    {
        ISnapshot LatestSnapshot { get; }
        ISnapshot GetSnapshot(int snapshotSeq);
        void AddSnapshot(ISnapshot snapshot);
        bool IsEmpty { get; }
        IList<ISnapshot> SortedSnapshotList { get; }
        ISnapshot OldestSnapshot { get; }
    }
}