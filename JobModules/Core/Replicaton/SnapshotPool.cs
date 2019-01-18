using System;
using System.Collections.Generic;

namespace Core.Replicaton
{
    public class SnapshotPool : ISnapshotPool
    {
        public const int MaxSnapshotSize = 64;

        List<ISnapshot> _list = new List<ISnapshot>();

        public IList<ISnapshot> SortedSnapshotList { get { return _list; } }

        public ISnapshot OldestSnapshot
        {
            get
            {
                if (_list.Count > 0)
                    return _list[0];
                return null;
            } 
            
        }

        public ISnapshot LatestSnapshot
        {
            get
            {
                if (_list.Count > 0)
                {
                    return _list[_list.Count - 1];
                }
                return null;
            }
        }

        public bool IsEmpty
        {
            get { return _list.Count == 0; }
        }


        public ISnapshot GetSnapshot(int snapshotSeq)
        {
            foreach (var snapshot in _list)
            {
                if (snapshot.SnapshotSeq == snapshotSeq)
                {
                    return snapshot;
                }
            }
            return null;
        }

        public void AddSnapshot(ISnapshot snapshot)
        {
            snapshot.AcquireReference();
            var snapshotList = _list;

            var insertIndex = snapshotList.Count;
            for (var i = 0; i < snapshotList.Count; i++)
            {
                var snap = snapshotList[i];

                if (snap.SnapshotSeq > snapshot.SnapshotSeq)
                {
                    insertIndex = i;
                    break;
                }
            }

            snapshotList.Insert(insertIndex, snapshot);

            //控制队列长度
            if (snapshotList.Count > MaxSnapshotSize)
            {
                RefCounterRecycler.Instance.ReleaseReference(snapshotList[0]);
                //  snapshotList[0].ReleaseReference();
                //snapshotList[0].DelRef();
                snapshotList.RemoveAt(0);
            }

        }


        public void Dispose()
        {
            foreach (var snapshot in _list)
            {
                RefCounterRecycler.Instance.ReleaseReference(snapshot);
            }
            _list.Clear();
        }
    }
}