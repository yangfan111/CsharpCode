using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Core.Fsm;
using Utils.CharacterState;

namespace Core.CharacterState.Posture.States
{
    class ProneTransitState : PostureState
    {
        public ProneTransitState(PostureStateId id) : base(id)
        {
            #region proneTransit to prone

            AddTransition(
                (command, addOutput) =>
                {
                    var ret = command.IsMatch(FsmInput.ToProneTransitFinish);
                    if (ret)
                    {
                        FsmOutput.Cache.SetValue(AnimatorParametersHash.Instance.PostureHash,
                                                 AnimatorParametersHash.Instance.PostureName,
                                                 AnimatorParametersHash.Instance.ProneValue,
                                                 CharacterView.FirstPerson | CharacterView.ThirdPerson);
                        addOutput(FsmOutput.Cache);
                        command.Handled = true;
                    }

                    return ret;
                },
                null, (int)PostureStateId.Prone, null, 0, new[] { FsmInput.ToProneTransitFinish });

            #endregion
        }
    }
}
