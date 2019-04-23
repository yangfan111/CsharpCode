using System;
using Core.Fsm;
using Utils.CharacterState;

namespace Core.CharacterState.Action.States.Transfiguration
{
    class TransfigurationStart : ActionState
    {
        public TransfigurationStart(ActionStateId id) : base(id)
        {
            #region TransfigurationStart to TransfigurationFinish

            AddTransition(
                (command, addOutput) =>
                {
                    if (command.IsMatch(FsmInput.TransfigurationStartEnd))
                        return true;
                    return false;
                },
                null, (int)ActionStateId.TransfigurationFinish, null, 0, new[] { FsmInput.TransfigurationStartEnd });

            #endregion
        }

        public override void DoBeforeLeaving(Action<FsmOutput> addOutput)
        {
            base.DoBeforeLeaving(addOutput);
            
            FsmOutput.Cache.SetValue(AnimatorParametersHash.Instance.TransfigurationStartHash,
                AnimatorParametersHash.Instance.TransfigurationStartName,
                AnimatorParametersHash.Instance.TransfigurationStartDisable,
                CharacterView.ThirdPerson, false);
            addOutput(FsmOutput.Cache);
        }
    }
}
