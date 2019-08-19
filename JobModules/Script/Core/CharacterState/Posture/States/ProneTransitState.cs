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
                    var ret = command.IsMatch(FsmInput.ToProneTransitFinish) || 
                              command.IsMatch(FsmInput.Dying);
                    if (command.IsMatch(FsmInput.ToProneTransitFinish))
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
                null, (int)PostureStateId.Prone, null, 0, new[] { FsmInput.ToProneTransitFinish, FsmInput.Dying });

            #endregion
        }
    }
}
