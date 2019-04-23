using System;
using Core.Fsm;
using Utils.CharacterState;

namespace Core.CharacterState.Posture.States
{
    class DyingState:PostureState
    {
        public DyingState(PostureStateId id) : base(id)
        {
            #region dying to crouch

            AddTransition(
                (command, addOutput) =>
                {
                    var ret = command.IsMatch(FsmInput.Revive);

                    if (ret)
                    {
                        command.Handled = true;
                    }

                    return ret;
                },
                (command, addOutput) => FsmTransitionResponseType.NoResponse,
                (int) PostureStateId.Stand, null, 0, new[] {FsmInput.Revive});

            #endregion
        }

        public override void DoBeforeLeaving(Action<FsmOutput> addOutput)
        {
            base.DoBeforeLeaving(addOutput);
            FsmOutput.Cache.SetValue(AnimatorParametersHash.Instance.InjuredMoveHash,
                AnimatorParametersHash.Instance.InjuredMoveName,
                AnimatorParametersHash.Instance.InjuredMoveDisableValue,
                CharacterView.ThirdPerson,
                false);
            addOutput(FsmOutput.Cache);
        }
    }
}