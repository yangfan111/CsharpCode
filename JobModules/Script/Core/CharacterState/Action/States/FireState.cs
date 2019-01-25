using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Core.Fsm;
using UnityEngine;
using Utils.CharacterState;

namespace Core.CharacterState.Action.States
{
    class FireState : ActionState
    {
        public FireState(ActionStateId id) : base(id)
        {
            #region Fire To CommonNull(due to animation end)

            AddTransition(
                (command, addOutput) => FsmTransition.SimpleCommandHandler(command, FsmInput.FireFinished),
                null, (int) ActionStateId.CommonNull, null, 0, new[] { FsmInput.FireFinished });

            #endregion
            
            #region Fire to CommonNull(due to interrupt)
            
            AddTransition(
                (command, addOutput) =>
                {
                    if (command.IsMatch(FsmInput.Fire))
                    {
                        FsmOutput.Cache.SetValue(AnimatorParametersHash.Instance.FireEndHash,
                                                 AnimatorParametersHash.Instance.FireEndName,
                                                 AnimatorParametersHash.Instance.FireEndEnableValue,
                                                 CharacterView.FirstPerson | CharacterView.ThirdPerson, true);
                        addOutput(FsmOutput.Cache);

                        return FsmStateResponseType.Reenter;
                    }

                    return FsmStateResponseType.Pass;
                },
                null, (int) ActionStateId.CommonNull, null, 0, new[] { FsmInput.Fire });

            AddTransition(
                (command, addOutput) =>
                {
                    if (command.IsMatch(FsmInput.SightsFire))
                    {
                        FsmOutput.Cache.SetValue(AnimatorParametersHash.Instance.FireEndHash,
                                                 AnimatorParametersHash.Instance.FireEndName,
                                                 AnimatorParametersHash.Instance.FireEndEnableValue,
                                                 CharacterView.FirstPerson | CharacterView.ThirdPerson, true);
                        addOutput(FsmOutput.Cache);

                        return FsmStateResponseType.Reenter;
                    }

                    return FsmStateResponseType.Pass;
                },
                null, (int)ActionStateId.CommonNull, null, 0, new[] { FsmInput.SightsFire });

            #endregion
        }

        public override void DoBeforeEntering(IFsmInputCommand command, Action<FsmOutput> addOutput)
        {
            base.DoBeforeEntering(command, addOutput);

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

        public override void DoBeforeLeaving(Action<FsmOutput> addOutput)
        {
            base.DoBeforeLeaving(addOutput);

            FsmOutput.Cache.SetValue(AnimatorParametersHash.Instance.FireEndHash,
                                     AnimatorParametersHash.Instance.FireEndName,
                                     AnimatorParametersHash.Instance.FireEndDisableValue,
                                     CharacterView.FirstPerson | CharacterView.ThirdPerson, false);
            addOutput(FsmOutput.Cache);
        }
    }
}
