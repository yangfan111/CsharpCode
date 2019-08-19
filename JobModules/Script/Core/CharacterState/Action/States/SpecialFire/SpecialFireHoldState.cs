using Core.Fsm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Utils.CharacterState;

namespace Core.CharacterState.Action.States.SpecialFire
{
    class SpecialFireHoldState : ActionState
    {
        public SpecialFireHoldState(ActionStateId id) : base(id)
        {
            #region SpecialFireHold To SpecialFireEnd

            AddTransition(
                (command, addOutput) =>
                {
                    if (command.IsMatch(FsmInput.SpecialFireEnd))
                    {
                        FsmOutput.Cache.SetValue(AnimatorParametersHash.Instance.FireEndHash,
                                                 AnimatorParametersHash.Instance.FireEndName,
                                                 AnimatorParametersHash.Instance.FireEndEnableValue,
                                                 CharacterView.FirstPerson | CharacterView.ThirdPerson, false);
                        addOutput(FsmOutput.Cache);

                        return true;
                    }

                    return false;
                },
                null, (int)ActionStateId.SpecialFireEnd, null, 0, new[] { FsmInput.SpecialFireEnd });

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

                        //FsmOutput.Cache.SetValue(FsmOutputType.InterruptAction,
                        //    (int)FsmInput.FireEndFinished);
                        //addOutput(FsmOutput.Cache);

                        command.Handled = true;
                        return FsmStateResponseType.Reenter;
                    }

                    return FsmStateResponseType.Pass;
                },
                null, (int)ActionStateId.CommonNull, null, 0, new[] { FsmInput.InterruptAction });

            #endregion
        }

        public override void DoBeforeEntering(IFsmInputCommand command, Action<FsmOutput> addOutput)
        {
            base.DoBeforeEntering(command, addOutput);
            
            FsmOutput.Cache.SetValue(AnimatorParametersHash.Instance.FireHoldHash,
                                     AnimatorParametersHash.Instance.FireHoldName,
                                     AnimatorParametersHash.Instance.FireHoldDisableValue,
                                     CharacterView.FirstPerson | CharacterView.ThirdPerson, false);
            addOutput(FsmOutput.Cache);
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
