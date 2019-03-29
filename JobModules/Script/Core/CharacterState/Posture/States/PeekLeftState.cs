using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Core.CharacterState.Posture.Transitions;
using Core.Fsm;

namespace Core.CharacterState.Posture.States
{
    class PeekLeftState : PostureState
    {
        public PeekLeftState(PostureStateId id) : base(id)
        {
            #region PeekLeft to NoPeek

            AddTransition(new PeekLeftToNoPeekTransition(AvailableTransitionId(),
                                                         (int) PostureStateId.NoPeek,
                                                         AnimatorParametersHash.PeekTime),
                          new[] { FsmInput.NoPeek, FsmInput.PeekRight });

            #endregion
        }

        public override void Update(int frameInterval, Action<FsmOutput> addOutput)
        {
            base.Update(frameInterval, addOutput);

            if (ActiveTransition == null)
            {
                FsmOutput.Cache.SetValue(FsmOutputType.Peek, FsmOutput.PeekLeftDegree);
                addOutput(FsmOutput.Cache);
            }
        }
    }
}
