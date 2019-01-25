using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Core.Fsm;

namespace Core.CharacterState.Posture.Transitions
{
    class NoPeekToPeekRightTransition : FsmTransition
    {
        public NoPeekToPeekRightTransition(short id, short target, int duration) : base(id, target, duration)
        {
            _simpleTransferCondition = (command, addOoutput) => SimpleCommandHandler(command, FsmInput.PeekRight);
            _interruptCondition = (command, addOoutput) =>
            {
                if (command.IsMatch(FsmInput.NoPeek))
                {
                    command.AdditioanlValue = NormalizedTime;
                    return FsmTransitionResponseType.ForceEnd;
                }
                return FsmTransitionResponseType.NoResponse;
            };
            _update = GetLerpFunc(FsmOutputType.Peek, FsmOutput.NoPeekDegree, FsmOutput.PeekRightDegree);
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
