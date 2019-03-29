using Core.Fsm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Utils.CharacterState;

namespace Core.CharacterState.Action.Transitions
{
    class OverlayNullToSightTransition : FsmTransition
    {
        public OverlayNullToSightTransition(short id, short target, int duration) : base(id, target, duration)
        {
            _simpleTransferCondition = (command, addOutput) =>
            {
                if (command.IsMatch(FsmInput.Sight))
                {
                    FsmOutput.Cache.SetLayerWeight(AnimatorParametersHash.Instance.ADSLayer,
                                                   AnimatorParametersHash.Instance.ADSEnableValue,
                                                   CharacterView.ThirdPerson);
                    addOutput(FsmOutput.Cache);

                    FsmOutput.Cache.SetLayerWeight(AnimatorParametersHash.Instance.ADSLayerP1,
                                                   AnimatorParametersHash.Instance.ADSEnableValue,
                                                   CharacterView.FirstPerson);
                    addOutput(FsmOutput.Cache);

                    command.Handled = true;
                    // this is why need a class
                    _speedRatio = command.AdditioanlValue;

                    return true;
                }

                return false;
            };

            _interruptCondition = (command, addOutput) =>
            {
                if (command.IsMatch(FsmInput.CancelSight))
                {
                    command.AdditioanlValue = NormalizedTime;
                    return FsmTransitionResponseType.ForceEnd;
                }
                return FsmTransitionResponseType.NoResponse;
            };

            _update = GetLerpFunc(FsmOutputType.FirstPersonSight, 0, 1);
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
