using System.Collections.Generic;
using XmlConfig;

namespace Core.Replicaton
{
    public class SnapshotSelector : ISnapshotSelector
    {
        public SnapshotSelector(ISnapshotPool snapshotPool)
        {
            _snapshotPool = snapshotPool;
        }

        private ISnapshotPool _snapshotPool;
        public SnapshotPair SelectSnapshot(int renderTime)
        {
            //第一次启动
           
            IList<ISnapshot> l = _snapshotPool.SortedSnapshotList;
            var count = l.Count;
            for (int i = count - 1; i > 0 ; i--)
            {
                ISnapshot leftSnapshot = l[i - 1];
                ISnapshot rightSnapshot = l[i];
                if (leftSnapshot.ServerTime <= renderTime && renderTime < rightSnapshot.ServerTime)
                {
                    return new SnapshotPair(leftSnapshot, rightSnapshot, renderTime);
                }
                if (renderTime > rightSnapshot.ServerTime)
                {
                    return null;
                }
            }
            if (count >= 2)
            {
                ISnapshot leftSnapshot = l[count - 2];
                ISnapshot rightSnapshot = l[count - 1];
                return new SnapshotPair(leftSnapshot, rightSnapshot, renderTime);
            }
            return null;
        }

        public ISnapshot LatestSnapshot {
            get { return _snapshotPool.LatestSnapshot; }
        }

        public ISnapshot OldestSnapshot { get { return _snapshotPool.OldestSnapshot; } }
    }
}