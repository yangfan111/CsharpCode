using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Core.Fsm;

namespace Core.CharacterState
{
    public interface ISyncFsmSnapshot
    {
        IList<FsmSnapshot> GetSnapshots();
        void TryRewind();
    }
}
