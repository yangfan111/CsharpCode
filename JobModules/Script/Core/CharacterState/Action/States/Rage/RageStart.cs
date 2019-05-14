using System;
using Core.Fsm;
using Utils.CharacterState;

namespace Core.CharacterState.Action.States.Rage
{
    class RageStart : ActionState
    {
        public RageStart(ActionStateId id) : base(id)
        {
            #region RageStart to RageLoop

            AddTransition(
                (command, addOutput) =>
                {
                    if (command.IsMatch(FsmInput.RageStartFinished))
                        return true;
                    return false;
                },
                null, (int)ActionStateId.RageLoop, null, 0, new[] { FsmInput.RageStartFinished });

            #endregion
        }

        public override void DoBeforeLeaving(Action<FsmOutput> addOutput)
        {
            base.DoBeforeLeaving(addOutput);
            
            FsmOutput.Cache.SetValue(AnimatorParametersHash.Instance.RageStartHash,
                AnimatorParametersHash.Instance.RageStartName,
                AnimatorParametersHash.Instance.RageStartDisable,
                CharacterView.ThirdPerson, false);
            addOutput(FsmOutput.Cache);
        }
    }
}
