using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Core.Fsm;

namespace Core.CharacterState.Posture.Transitions
{
    class NoPeekToPeekLeftTransition : FsmTransition
    {
        public NoPeekToPeekLeftTransition(short id, short target, int duration) : base(id, target, duration)
        {
            _simpleTransferCondition = (command, addOutput) => SimpleCommandHandler(command, FsmInput.PeekLeft);
            _interruptCondition = (command, addOutput) =>
            {
                if (command.IsMatch(FsmInput.NoPeek))
                {
                    command.AdditioanlValue = NormalizedTime;
                    return FsmTransitionResponseType.ForceEnd;
                }
                return FsmTransitionResponseType.NoResponse;
            };
            _update = GetLerpFunc(FsmOutputType.Peek, FsmOutput.NoPeekDegree, FsmOutput.PeekLeftDegree);
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
