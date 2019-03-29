using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Core.Fsm;
using UnityEngine;
using Utils.CharacterState;

namespace Core.CharacterState.Action.States.SpecialFire
{
    class SpecialFireState : ActionState
    {
        public SpecialFireState(ActionStateId id) : base(id)
        {
            #region SpecialFire To SpecialFireHold

            AddTransition(
                (command, addOutput) => FsmTransition.SimpleCommandHandler(command, FsmInput.FireFinished),
                null, (int)ActionStateId.SpecialFireHold, null, 0, new[] { FsmInput.FireFinished });

            #endregion
        }

        public override void DoBeforeEntering(IFsmInputCommand command, Action<FsmOutput> addOutput)
        {
            base.DoBeforeEntering(command, addOutput);

            FsmOutput.Cache.SetValue(AnimatorParametersHash.Instance.FireHoldHash,
                                     AnimatorParametersHash.Instance.FireHoldName,
                                     AnimatorParametersHash.Instance.FireHoldEnableValue,
                                     CharacterView.FirstPerson | CharacterView.ThirdPerson, false);
            addOutput(FsmOutput.Cache);
        }

        public override void DoBeforeLeaving(Action<FsmOutput> addOutput)
        {
            base.DoBeforeLeaving(addOutput);
            
            FsmOutput.Cache.SetValue(AnimatorParametersHash.Instance.FireHash,
                                     AnimatorParametersHash.Instance.FireName,
                                     AnimatorParametersHash.Instance.FireDisableValue,
                                     CharacterView.FirstPerson | CharacterView.ThirdPerson, false);
            addOutput(FsmOutput.Cache);
            
            FsmOutput.Cache.SetValue(AnimatorParametersHash.Instance.SightsFireHash,
                                     AnimatorParametersHash.Instance.SightsFireName,
                                     AnimatorParametersHash.Instance.SightsFireDisableValue,
                                     CharacterView.FirstPerson | CharacterView.ThirdPerson, false);
            addOutput(FsmOutput.Cache);
        }
    }
}
