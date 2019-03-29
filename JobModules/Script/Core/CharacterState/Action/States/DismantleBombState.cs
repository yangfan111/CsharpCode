using System;
using Core.Fsm;
using Utils.CharacterState;

namespace Core.CharacterState.Action.States
{
    class DismantleBombState : ActionState
    {
        public DismantleBombState(ActionStateId id) : base(id)
        {
            #region Reload to CommonNull

            AddTransition(
                (command, addOutput) =>
                {
                    if (command.IsMatch(FsmInput.DismantleBombFinished))
                    {
                        command.Handled = true;
                        return true;
                    }

                    return false;
                },
                null, (int) ActionStateId.CommonNull, null, 0, new[] {FsmInput.DismantleBombFinished});

            #endregion

            #region Interrupt

            AddTransition(
                (command, addOutput) =>
                {
                    if (command.IsMatch(FsmInput.InterruptAction))
                    {
                        FsmOutput.Cache.SetValue(AnimatorParametersHash.Instance.InterruptHash,
                            AnimatorParametersHash.Instance.InterruptName,
                            AnimatorParametersHash.Instance.InterruptEnable,
                            CharacterView.FirstPerson | CharacterView.ThirdPerson, true);
                        addOutput(FsmOutput.Cache);

                        FsmOutput.Cache.SetValue(FsmOutputType.InterruptAction,
                            (int) FsmInput.DismantleBombFinished);
                        addOutput(FsmOutput.Cache);

                        command.Handled = true;
                        return FsmStateResponseType.Reenter;
                    }

                    return FsmStateResponseType.Pass;
                },
                null, (int) ActionStateId.CommonNull, null, 0, new[] {FsmInput.InterruptAction});

            #endregion
        }

        public override void Update(int frameInterval, Action<FsmOutput> addOutput)
        {
            // 动画已经播放过一帧
            if (UpdateForTheFirstTime)
            {
                FsmOutput.Cache.SetValue(AnimatorParametersHash.Instance.DismantleHash,
                    AnimatorParametersHash.Instance.DismantleName,
                    AnimatorParametersHash.Instance.DismantleDisableValue,
                    CharacterView.FirstPerson | CharacterView.ThirdPerson, false);
                addOutput(FsmOutput.Cache);
            }

            base.Update(frameInterval, addOutput);
        }

        public override void DoBeforeLeaving(Action<FsmOutput> addOutput)
        {
            base.DoBeforeLeaving(addOutput);

            FsmOutput.Cache.SetValue(AnimatorParametersHash.Instance.InterruptHash,
                AnimatorParametersHash.Instance.InterruptName,
                AnimatorParametersHash.Instance.InterruptDisable,
                CharacterView.FirstPerson | CharacterView.ThirdPerson, false);
            addOutput(FsmOutput.Cache);

            FsmOutput.Cache.SetValue(AnimatorParametersHash.Instance.DismantleHash,
                AnimatorParametersHash.Instance.DismantleName,
                AnimatorParametersHash.Instance.DismantleDisableValue,
                CharacterView.FirstPerson | CharacterView.ThirdPerson, false);
            addOutput(FsmOutput.Cache);
        }
    }
}