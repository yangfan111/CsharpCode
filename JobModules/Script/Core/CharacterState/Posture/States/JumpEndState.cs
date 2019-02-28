using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Core.CharacterState.Posture.Transitions;
using Core.Fsm;
using Utils.CharacterState;
using Utils.Configuration;
using Utils.Singleton;
using XmlConfig;

namespace Core.CharacterState.Posture.States
{
    class JumpEndState:PostureState
    {
        public JumpEndState(PostureStateId id) : base(id)
        {
            #region jumpend to jumpstart

            AddTransition(
                (command, addOutput) =>
                {
                    var ret = command.IsMatch(FsmInput.Jump);

                    if (ret)
                    {
                        FsmOutput.Cache.SetValue(AnimatorParametersHash.Instance.JumpStartHash,
                            AnimatorParametersHash.Instance.JumpStartName,
                            AnimatorParametersHash.Instance.JumpStartEnable,
                            CharacterView.FirstPerson | CharacterView.ThirdPerson, false);
                        addOutput(FsmOutput.Cache);
                        
                        SetJumpMoveState(command.AdditioanlValue, command.AlternativeAdditionalValue, addOutput);

                        command.Handled = true;
                    }

                    return ret;
                },
                (command, addOutput) => FsmTransitionResponseType.NoResponse,
                (int)PostureStateId.JumpStart, null, 0, new[] { FsmInput.Jump });

            #endregion

            #region jumpend to stand

            AddTransition(new JumpEndToStandTransition(AvailableTransitionId(),
                    (int)PostureStateId.Stand,
                    AnimatorParametersHash.ImpossibleTransitionTime),
                new[] { FsmInput.Land , FsmInput.SlideEnd });

            #endregion

            #region jumpend to dying
            AddTransitionFromJumpToDying(this);
            #endregion
        }

        private static void SetJumpMoveState(float value, float moveJumpState, Action<FsmOutput> addOutput)
        {
            FsmOutput.Cache.SetValue(AnimatorParametersHash.Instance.JumpStateHash,
                AnimatorParametersHash.Instance.JumpStateName,
                value,
                CharacterView.ThirdPerson);
            addOutput(FsmOutput.Cache);
            
            FsmOutput.Cache.SetValue(AnimatorParametersHash.Instance.MoveJumpStateHash,
                AnimatorParametersHash.Instance.MoveJumpStateName,
                moveJumpState,
                CharacterView.ThirdPerson);
            addOutput(FsmOutput.Cache);
        }

        public override void DoBeforeEntering(IFsmInputCommand command, Action<FsmOutput> addOutput)
        {
            FsmOutput.Cache.SetValue(FsmOutputType.CharacterControllerJumpHeight, SingletonManager.Get<CharacterStateConfigManager>().GetCharacterControllerCapsule(PostureInConfig.Stand).Height);
            addOutput(FsmOutput.Cache);
            base.DoBeforeEntering(command, addOutput);
        }
    }
}
