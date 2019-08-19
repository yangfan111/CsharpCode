using Core.Replicaton;

using System.Collections.Generic;

namespace Core.Replicaton
{
    public class SnapshotPool :ISnapshotSelector
    {
        public const int MaxSnapshotSize = 128;

       public readonly  LinkedList<ISnapshot> Snapshots = new LinkedList<ISnapshot>();
       public SnapshotPair SelectSnapshot(int renderTime)
       {
           
           var l     = Snapshots;
           if (IsEmpty) return null;
           var              count = l.Count;
           var right = Snapshots.Last;
           var left = right.Previous;
           while (right != null && left != null)
           {
               if (renderTime > right.Value.ServerTime)
                   return null;
               if (left.Value.ServerTime <= renderTime && renderTime < right.Value.ServerTime)
                   return new SnapshotPair(left.Value, right.Value, renderTime);
               right = left;
               left = right.Previous;
           }
           if (count >= 2)
           {
               ISnapshot rightSnapshot = l.Last.Value;
               ISnapshot leftSnapshot = l.Last.Previous.Value;
               return new SnapshotPair(leftSnapshot, rightSnapshot, renderTime);
           }

           return null;
       }
        public ISnapshot OldestSnapshot
        {
            get
            {
                if (IsEmpty)
                    return null;
                return Snapshots.First.Value;
            } 
            
        }

        public ISnapshot LatestSnapshot
        {
            get
            {
                if (IsEmpty)
                    return null;
                return Snapshots.Last.Value;
            }
        }

        public bool IsEmpty
        {
            get { return Snapshots.Count == 0; }
        }


        public ISnapshot GetSnapshot(int snapshotSeq)
        {
            if (IsEmpty) return null;
            var curr = Snapshots.First;
            while (curr != null)
            {
                if (curr.Value.SnapshotSeq == snapshotSeq)
                    return curr.Value;
                if (curr.Value.SnapshotSeq < snapshotSeq)
                    break;
                curr = curr.Next;
            }
            return null;
        }

        public void AddSnapshot(ISnapshot snapshot)
        {
          //   snapshot.AcquireReference();
            if (IsEmpty)
            {
                Snapshots.AddFirst(snapshot);
                return;
            }
            var curr = Snapshots.First;
            while (curr != null)
            {
                if (curr.Value.SnapshotSeq > snapshot.SnapshotSeq)
                    break; 
                curr = curr.Next;
            }
            if (curr != null)
            {
                Snapshots.AddBefore(curr, snapshot);
                //snapshot.AcquireReference();
             //   Snapshots.AddBefore(curr, snapshot);
            }
            else
            {
                //snapshot.AcquireReference();
               Snapshots.AddLast(snapshot);
            }
            //控制队列长度
            if (Snapshots.Count > MaxSnapshotSize)
            {
                RefCounterRecycler.Instance.ReleaseReference(Snapshots.First.Value);
                //  snapshotList[0].ReleaseReference();
                //snapshotList[0].DelRef();
                Snapshots.RemoveFirst();
            }

        }


        public void Dispose()
        {
            foreach (var snapshot in Snapshots)
            {
                RefCounterRecycler.Instance.ReleaseReference(snapshot);
            }
            Snapshots.Clear();
        }
    }
}
   
