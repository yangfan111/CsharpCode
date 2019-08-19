using System;

namespace Core.Replicaton
{
    public interface ISnapshotSelector:IDisposable
    {
        SnapshotPair SelectSnapshot(int renderTime);
        ISnapshot LatestSnapshot { get;  }
        ISnapshot OldestSnapshot { get; }
        void AddSnapshot(ISnapshot messageBody);
    }
}