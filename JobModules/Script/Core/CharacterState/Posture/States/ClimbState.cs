using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Core.Fsm;
using Core.Utils;
using UnityEngine;
using Utils.CharacterState;
using Utils.Configuration;
using XmlConfig;

namespace Core.CharacterState.Posture.States
{
    class ClimbState : PostureState
    {
        private static LoggerAdapter _logger = new LoggerAdapter(typeof(ClimbState));

        public ClimbState(PostureStateId id) : base(id)
        {
            #region climb to stand

            AddTransition(
                (command, addOutput) => FsmTransition.SimpleCommandHandler(command, FsmInput.GenericActionFinished),
                null, (int) PostureStateId.Stand, null, 0, new[] {FsmInput.GenericActionFinished});

            AddTransition(
                (command, addOutput) =>
                {
                    if (command.IsMatch(FsmInput.Freefall))
                    {
                        FsmOutput.Cache.SetValue(AnimatorParametersHash.Instance.FreeFallHash,
                            AnimatorParametersHash.Instance.FreeFallName,
                            AnimatorParametersHash.Instance.FreeFallEnable,
                            CharacterView.FirstPerson | CharacterView.ThirdPerson, false);
                        addOutput(FsmOutput.Cache);

                        FsmOutput.Cache.SetValue(AnimatorParametersHash.Instance.JumpStateHash,
                            AnimatorParametersHash.Instance.JumpStateName,
                            AnimatorParametersHash.Instance.JumpStateNormal,
                            CharacterView.ThirdPerson);
                        addOutput(FsmOutput.Cache);

                        FsmOutput.Cache.SetValue(AnimatorParametersHash.Instance.MoveJumpStateHash,
                            AnimatorParametersHash.Instance.MoveJumpStateName,
                            AnimatorParametersHash.Instance.MoveJumpStateNormal,
                            CharacterView.ThirdPerson);
                        addOutput(FsmOutput.Cache);

                        command.Handled = true;

                        return true;
                    }

                    return false;
                }, null, (int) PostureStateId.Freefall, null, 0, new[] {FsmInput.Freefall});

            #endregion
        }

        public override void DoBeforeLeaving(Action<FsmOutput> addOutput)
        {
            FsmOutput.Cache.SetValue(AnimatorParametersHash.Instance.ClimbHash,
                AnimatorParametersHash.Instance.ClimbName,
                AnimatorParametersHash.Instance.ClimbDisableValue,
                CharacterView.ThirdPerson | CharacterView.FirstPerson, false);
            addOutput(FsmOutput.Cache);
            
            FsmOutput.Cache.SetValue(AnimatorParametersHash.Instance.ClimbEndHash,
                AnimatorParametersHash.Instance.ClimbEndName,
                AnimatorParametersHash.Instance.ClimbEndDisableValue,
                CharacterView.ThirdPerson, false);
            addOutput(FsmOutput.Cache);

            base.DoBeforeLeaving(addOutput);
        }
    }
}