using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Core.CharacterState.Posture.Transitions;
using Core.Fsm;

namespace Core.CharacterState.Posture.States
{
    class PeekRightState : PostureState
    {
        public PeekRightState(PostureStateId id) : base(id)
        {
            #region PeekRight to NoPeek

            AddTransition(new PeekRightToNoPeekTransition(AvailableTransitionId(),
                                                          (int) PostureStateId.NoPeek,
                                                          AnimatorParametersHash.PeekTime),
                          new[] { FsmInput.NoPeek, FsmInput.PeekLeft });

            #endregion
        }

        public override void Update(int frameInterval, Action<FsmOutput> addOutput)
        {
            base.Update(frameInterval, addOutput);

            if (ActiveTransition == null)
            {
                FsmOutput.Cache.SetValue(FsmOutputType.Peek, FsmOutput.PeekRightDegree);
                addOutput(FsmOutput.Cache);
            }
        }
    }
}
