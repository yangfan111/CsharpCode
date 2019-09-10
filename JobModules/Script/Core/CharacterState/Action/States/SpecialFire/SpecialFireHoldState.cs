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
    }
}
