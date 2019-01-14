using Core.Fsm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Core.Configuration;
using Utils.CharacterState;
using Utils.Configuration;
using Utils.Singleton;

namespace Core.CharacterState.Action.States
{
    class PropsState : ActionState
    {
        public PropsState(ActionStateId id) : base(id)
        {
            #region Props to CommonNull

            AddTransition(
                (command, addOutput) =>
                {
                    if (command.IsMatch(FsmInput.PropsEnd))
                    {
                        command.Handled = true;
                        return true;
                    }

                    return false;
                },
                null, (int)ActionStateId.CommonNull, null, 0, new[] { FsmInput.PropsEnd });

            #endregion

            #region finishProps
            AddTransition(
                (command, addOutput) =>
                {
                    if (command.IsMatch(FsmInput.FinishProps))
                    {
                        FsmOutput.Cache.SetValue(AnimatorParametersHash.Instance.PropsEndHash,
                                                 AnimatorParametersHash.Instance.PropsEndName,
                                                 AnimatorParametersHash.Instance.PropsEndEnable,
                                                 CharacterView.FirstPerson | CharacterView.ThirdPerson, true);
                        addOutput(FsmOutput.Cache);
                        command.Handled = true;
                        return true;
                    }

                    return false;
                },
                null, (int)ActionStateId.CommonNull, null, 0, new[] { FsmInput.FinishProps });
            #endregion

            #region Props to CommonNull(due to interrupt)

            AddTransition(
                (command, addOutput) =>
                {
                    if (command.IsMatch(FsmInput.Props))
                    {
                        FsmOutput.Cache.SetValue(AnimatorParametersHash.Instance.PropsEndHash,
                                                 AnimatorParametersHash.Instance.PropsEndName,
                                                 AnimatorParametersHash.Instance.PropsEndEnable,
                                                 CharacterView.FirstPerson | CharacterView.ThirdPerson, true);
                        addOutput(FsmOutput.Cache);

                        return FsmStateResponseType.Reenter;
                    }

                    return FsmStateResponseType.Pass;
                },
                null, (int)ActionStateId.CommonNull, null, 0, new[] { FsmInput.Props });

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
                            (int)FsmInput.PropsEnd);
                        addOutput(FsmOutput.Cache);

                        command.Handled = true;
                        return FsmStateResponseType.Reenter;
                    }

                    return FsmStateResponseType.Pass;
                },
                null, (int)ActionStateId.CommonNull, null, 0, new[] { FsmInput.InterruptAction });

            #endregion
        }

        public override FsmStateResponseType HandleInput(IFsmInputCommand command, Action<FsmOutput> addOutput)
        {
            if (command.IsMatch(FsmInput.PropsProgressP3))
            {
                LerpUpperBodyLayerWeight(command, addOutput, SingletonManager.Get<CharacterStateConfigManager>().LongLayerWeightTransitionTime);
            }
            return base.HandleInput(command, addOutput);
        }

        public override void Update(int frameInterval, Action<FsmOutput> addOutput)
        {
            if (UpdateForTheFirstTime)
            {
                FsmOutput.Cache.SetValue(AnimatorParametersHash.Instance.PropsHash,
                                         AnimatorParametersHash.Instance.PropsName,
                                         AnimatorParametersHash.Instance.PropsDisable,
                                         CharacterView.FirstPerson | CharacterView.ThirdPerson, false);
                addOutput(FsmOutput.Cache);
            }

            base.Update(frameInterval, addOutput);
        }

        public override void DoBeforeLeaving(Action<FsmOutput> addOutput)
        {
            base.DoBeforeLeaving(addOutput);

            FsmOutput.Cache.SetValue(AnimatorParametersHash.Instance.PropsEndHash,
                                                 AnimatorParametersHash.Instance.PropsEndName,
                                                 AnimatorParametersHash.Instance.PropsEndDisable,
                                                 CharacterView.FirstPerson | CharacterView.ThirdPerson, false);
            addOutput(FsmOutput.Cache);

            FsmOutput.Cache.SetValue(AnimatorParametersHash.Instance.InterruptHash,
                                                 AnimatorParametersHash.Instance.InterruptName,
                                                 AnimatorParametersHash.Instance.InterruptDisable,
                                                 CharacterView.FirstPerson | CharacterView.ThirdPerson, false);
            addOutput(FsmOutput.Cache);
            
            TurnOffUpperBodyOverlay(addOutput);
        }
    }
}
