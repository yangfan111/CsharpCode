using System;
using Core.Fsm;
using Utils.CharacterState;

namespace Core.CharacterState.Action.States
{
    class ReloadState : ActionState
    {
        public ReloadState(ActionStateId id) : base(id)
        {
            #region Reload to CommonNull
            
            AddTransition(
                (command, addOutput) =>
                {
                    if (command.IsMatch(FsmInput.ReloadFinished))
                    {

                        command.Handled = true;

                        return true;
                    }

                    return false;
                },
                null, (int) ActionStateId.CommonNull, null, 0, new[] { FsmInput.ReloadFinished });

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

                        // 添加需要打断的 input
                        // ReloadFinished由动画结束触发
                        // 添加后ReloadFinished会被销毁
                        FsmOutput.Cache.SetValue(FsmOutputType.InterruptAction, 
                            (int)FsmInput.ReloadFinished);
                        addOutput(FsmOutput.Cache);

                        command.Handled = true;
                        return FsmStateResponseType.Reenter;
                    }

                    return FsmStateResponseType.Pass;
                },
                null, (int)ActionStateId.CommonNull, null, 0, new[] { FsmInput.InterruptAction });

            #endregion
        }

        public override void Update(int frameInterval, Action<FsmOutput> addOutput)
        {
            // 动画已经播放过一帧
            if (UpdateForTheFirstTime)
            {
                FsmOutput.Cache.SetValue(AnimatorParametersHash.Instance.ReloadHash,
                                         AnimatorParametersHash.Instance.ReloadName,
                                         AnimatorParametersHash.Instance.ReloadDisableValue,
                                         CharacterView.FirstPerson | CharacterView.ThirdPerson, false);
                addOutput(FsmOutput.Cache);

                FsmOutput.Cache.SetValue(AnimatorParametersHash.Instance.ReloadEmptyHash,
                                         AnimatorParametersHash.Instance.ReloadEmptyName,
                                         AnimatorParametersHash.Instance.ReloadEmptyDisableValue,
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
        }
    }
}