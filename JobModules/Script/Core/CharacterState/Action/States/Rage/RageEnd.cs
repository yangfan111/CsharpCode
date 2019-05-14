using Core.Fsm;
using Utils.CharacterState;

namespace Core.CharacterState.Action.States.Rage
{
    class RageEnd : ActionState
    {
        public RageEnd(ActionStateId id) : base(id)
        {
            #region RageEnd to KeepNull

            AddTransition(
                (command, addOutput) =>
                {
                    if (command.IsMatch(FsmInput.RageEndFinished))
                    {
                        FsmOutput.Cache.SetValue(AnimatorParametersHash.Instance.RageEndHash,
                            AnimatorParametersHash.Instance.RageEndName,
                            AnimatorParametersHash.Instance.RageEndDisable,
                            CharacterView.ThirdPerson, false);
                        addOutput(FsmOutput.Cache);
                        
                        return true;
                    }
                    return false;
                },
                null, (int)ActionStateId.KeepNull, null, 0, new[] { FsmInput.RageEndFinished });

            #endregion
        }
    }
}
