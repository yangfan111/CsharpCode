using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Core.Fsm;

namespace Core.CharacterState.Posture.Transitions
{
    class PeekLeftToNoPeekTransition : FsmTransition
    {
        public PeekLeftToNoPeekTransition(short id, short target, int duration) : base(id, target, duration)
        {
            _simpleTransferCondition = (command, addOutput) => SimpleCommandHandler(command, FsmInput.NoPeek)
                                                            || SimpleCommandHandler(command, FsmInput.PeekRight);
            _interruptCondition = (command, addOutput) =>
            {
                if (command.IsMatch(FsmInput.PeekLeft))
                {
                    command.AdditioanlValue = NormalizedTime;
                    return FsmTransitionResponseType.ForceEnd;
                }
                return FsmTransitionResponseType.NoResponse;
            };
            _update = GetLerpFunc(FsmOutputType.Peek, FsmOutput.PeekLeftDegree, FsmOutput.NoPeekDegree);
        }

        public override void Init(IFsmInputCommand command)
        {
            base.Init(command);
            if (InitValue != 0)
            {
                NormalizedTime = 1 - InitValue;
            }
        }
    }
}
