using System;
using Core.Fsm;
using Utils.CharacterState;

namespace Core.CharacterState.Action.States.Rage
{
    class RageLoop : ActionState
    {
        public RageLoop(ActionStateId id) : base(id)
        {
            #region RageLoop to RageEnd

            AddTransition(
                (command, addOutput) =>
                {
                    if (command.IsMatch(FsmInput.RageEnd))
                    {
                        FsmOutput.Cache.SetValue(AnimatorParametersHash.Instance.RageEndHash,
                            AnimatorParametersHash.Instance.RageEndName,
                            AnimatorParametersHash.Instance.RageEndEnable,
                            CharacterView.ThirdPerson, false);
                        addOutput(FsmOutput.Cache);
                        
                        return true;
                    }
                    return false;
                },
                null, (int)ActionStateId.RageEnd, null, 0, new[] { FsmInput.RageEnd });

            #endregion
        }
    }
}
