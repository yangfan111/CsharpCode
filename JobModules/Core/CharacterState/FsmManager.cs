using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Core.Fsm;

namespace Core.CharacterState
{
    abstract class FsmManager : IGetSetFsmSnapshot
    {
        private readonly List<FiniteStateMachine> _fsms = new List<FiniteStateMachine>();

        protected void AddFsm(FiniteStateMachine fsm)
        {
            _fsms.Add(fsm);
        }

        #region  IGetSetFsmSnapshot

        public int SnapshotCount()
        {
            return _fsms.Count;
        }

        public void GetSnapshot(IList<FsmSnapshot> snapshots, int index)
        {
            for (int i = 0; i < _fsms.Count; i++)
            {
                _fsms[i].GetFsmSnapshot(snapshots[index + i]);
            }
        }

        public void SetSnapshot(IList<FsmSnapshot> snapshots, int index)
        {
            for (int i = 0; i < _fsms.Count; i++)
            {
                _fsms[i].SetFsmSnapshot(snapshots[index + i]);
            }
        }

        #endregion
    }
}
