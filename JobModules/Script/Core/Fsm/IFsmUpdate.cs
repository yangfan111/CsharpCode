using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Utils.Appearance;

namespace Core.Fsm
{
    public interface IFsmUpdate
    {
        void Update(IAdaptiveContainer<IFsmInputCommand> commands,
                    int frameInterval,
                    Action<FsmOutput> addOutput,
                    FsmUpdateType updateType);
    }
}
