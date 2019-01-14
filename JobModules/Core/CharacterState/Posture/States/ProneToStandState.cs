using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Core.Fsm;
using Utils.CharacterState;

namespace Core.CharacterState.Posture.States
{
    class ProneToStandState : PostureState
    {
        public ProneToStandState(PostureStateId id) : base(id)
        {
            #region proneTransit to stand

            AddTransition(
                (command, addOutput) =>
                {
                    var ret = command.IsMatch(FsmInput.OutProneTransitFinish);
                    if (ret)
                    {
                        FsmOutput.Cache.SetValue(AnimatorParametersHash.Instance.PostureHash,
                                                 AnimatorParametersHash.Instance.PostureName,
                                                 AnimatorParametersHash.Instance.StandValue,
                                                 CharacterView.FirstPerson | CharacterView.ThirdPerson);
                        addOutput(FsmOutput.Cache);
                        command.Handled = true;
                    }

                    return ret;
                },
                null, (int)PostureStateId.Stand, null, 0, new[] { FsmInput.OutProneTransitFinish });

            #endregion
        }
    }
}
