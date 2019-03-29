using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Core.Fsm
{
    public class FsmSnapshot
    {
        public short StateId = -1;
        public int StateProgress = -1;
        public short TransitoinId = -1;
        public float TransitionProgress = -1;

        public override string ToString()
        {
            return string.Format("StateId: {0}, StateProgress: {1}, TransitoinId: {2}, TransitionProgress: {3}", StateId, StateProgress, TransitoinId, TransitionProgress);
        }
    }

    public interface IGetSetFsmSnapshot
    {
        int SnapshotCount();
        void GetSnapshot(IList<FsmSnapshot> snapshots, int index);
        void SetSnapshot(IList<FsmSnapshot> snapshots, int index);
    }
}
