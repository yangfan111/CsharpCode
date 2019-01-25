using Core.Fsm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Utils.CharacterState;

namespace Core.CharacterState.Action.Transitions
{
    class SightToOverlayNullTransition : FsmTransition
    {
        public SightToOverlayNullTransition(short id, short target, int duration) : base(id, target, duration)
        {
            _simpleTransferCondition = (command, addOutput) =>
            {
                var ret = command.IsMatch(FsmInput.CancelSight);

                if (ret)
                {
                    FsmOutput.Cache.SetLayerWeight(AnimatorParametersHash.Instance.ADSLayer,
                                                   AnimatorParametersHash.Instance.ADSDisableValue,
                                                   CharacterView.ThirdPerson);
                    addOutput(FsmOutput.Cache);

                    FsmOutput.Cache.SetLayerWeight(AnimatorParametersHash.Instance.ADSLayerP1,
                                                   AnimatorParametersHash.Instance.ADSDisableValue,
                                                   CharacterView.FirstPerson);
                    addOutput(FsmOutput.Cache);

                    // Stand,Crouch,Prone的姿势都一样
                    FsmOutput.Cache.SetValue(AnimatorParametersHash.Instance.PostureHash,
                                             AnimatorParametersHash.Instance.PostureName,
                                             AnimatorParametersHash.Instance.StandValue,
                                             CharacterView.FirstPerson);
                    addOutput(FsmOutput.Cache);

                    _speedRatio = command.AdditioanlValue;
                    command.Handled = true;
                }

                return ret;
            };
            _update = GetLerpFunc(FsmOutputType.FirstPersonSight, 1, 0);
        }
    }
}
