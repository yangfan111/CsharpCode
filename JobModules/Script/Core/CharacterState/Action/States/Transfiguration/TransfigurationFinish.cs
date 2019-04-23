using System;
using Core.Fsm;
using Utils.CharacterState;

namespace Core.CharacterState.Action.States.Transfiguration
{
    class TransfigurationFinish : ActionState
    {
        public TransfigurationFinish(ActionStateId id) : base(id)
        {
            #region TransfigurationStart to TransfigurationFinish

            AddTransition(
                (command, addOutput) =>
                {
                    if (command.IsMatch(FsmInput.TransfigurationFinishEnd))
                        return true;
                    return false;
                },
                null, (int)ActionStateId.CommonNull, null, 0, new[] { FsmInput.TransfigurationFinishEnd });

            #endregion
        }

        public override void DoBeforeLeaving(Action<FsmOutput> addOutput)
        {
            base.DoBeforeLeaving(addOutput);
            
            FsmOutput.Cache.SetValue(AnimatorParametersHash.Instance.TransfigurationFinishHash,
                AnimatorParametersHash.Instance.TransfigurationFinishName,
                AnimatorParametersHash.Instance.TransfigurationFinishDisable,
                CharacterView.ThirdPerson, false);
            addOutput(FsmOutput.Cache);
        }
    }
}
