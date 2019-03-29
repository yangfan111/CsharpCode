using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Core.Fsm;
using UnityEngine;
using Utils.CharacterState;

namespace Core.CharacterState.Action.States
{
    class InjuryState : ActionState
    {
        public InjuryState(ActionStateId id) : base(id)
        {
            #region Injury To CommonNull

            AddTransition(
                (command, addOutput) => FsmTransition.SimpleCommandHandler(command, FsmInput.InjuryFinished),
                null, (int) ActionStateId.CommonNull, null, 0, new[] { FsmInput.InjuryFinished });

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
            
            FsmOutput.Cache.SetValue(AnimatorParametersHash.Instance.InjuryHash,
                                     AnimatorParametersHash.Instance.InjuryName,
                                     AnimatorParametersHash.Instance.InjuryEndValue,
                                     CharacterView.ThirdPerson, false);
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
