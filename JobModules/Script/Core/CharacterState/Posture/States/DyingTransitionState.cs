using System;
using Core.Fsm;
using Utils.CharacterState;

namespace Core.CharacterState.Posture.States
{
    class DyingTransitionState:PostureState
    {
        public DyingTransitionState(PostureStateId id) : base(id)
        {
            AddTransition(
                (command, action) => FsmTransition.SimpleCommandHandler(command, FsmInput.DyingTransitionFinished),
                (command, action) => FsmTransitionResponseType.NoResponse,
                (int) PostureStateId.Dying, null, 0, new []{FsmInput.DyingTransitionFinished}
            );
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