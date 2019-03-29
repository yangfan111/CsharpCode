using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Core.Fsm;
using Core.Utils;
using UnityEngine;
using Utils.CharacterState;
using Utils.Configuration;
using Utils.Singleton;
using XmlConfig;

namespace Core.CharacterState.Posture.States
{
    class JumpStartState : PostureState
    {
        private static LoggerAdapter _logger = new LoggerAdapter(typeof(JumpStartState));

        private bool isBigJump = false;

        public JumpStartState(PostureStateId id) : base(id)
        {
            #region jumpstart to jumpend

            AddTransition(
                (command, addOutput) =>
                {
                    if (command.IsMatch(FsmInput.Land))
                    {
                        FsmOutput.Cache.SetValue(AnimatorParametersHash.Instance.JumpStartHash,
                                                 AnimatorParametersHash.Instance.JumpStartName,
                                                 AnimatorParametersHash.Instance.JumpStartDisable,
                                                 CharacterView.FirstPerson | CharacterView.ThirdPerson, false);
                        addOutput(FsmOutput.Cache);

                        return FsmStateResponseType.Reenter;
                    }

                    return FsmStateResponseType.Pass;
                },
                (command, addOutput) => FsmTransitionResponseType.NoResponse,
                (int)PostureStateId.JumpEnd, null, 0, new[] { FsmInput.Land });

            #endregion

            #region jumpstart to freefall

            AddTransition(
                (command, addOutput) => FsmTransition.SimpleCommandHandler(command, FsmInput.Freefall),
                (command, addOutput) => FsmTransitionResponseType.NoResponse,
                (int)PostureStateId.Freefall, null, 0, new[] { FsmInput.Freefall });

            #endregion

            #region jumpStart to dying
            AddTransitionFromJumpToDying(this);
            #endregion
        }

        public override void DoBeforeEntering(IFsmInputCommand command, Action<FsmOutput> addOutput)
        {
            isBigJump = false;
            base.DoBeforeEntering(command, addOutput);
        }

        public override void DoBeforeLeaving(Action<FsmOutput> addOutput)
        {
            FsmOutput.Cache.SetValue(FsmOutputType.CharacterControllerHeight, SingletonManager.Get<CharacterStateConfigManager>().GetCharacterControllerCapsule(PostureInConfig.Stand).Height);
            addOutput(FsmOutput.Cache);
            base.DoBeforeLeaving(addOutput);
        }

        public override FsmStateResponseType HandleInput(IFsmInputCommand command, Action<FsmOutput> addOutput)
        {
            if (command.IsMatch(FsmInput.Crouch) && !isBigJump)
            {
                isBigJump = true;
                FsmOutput.Cache.SetValue(FsmOutputType.CharacterControllerJumpHeight, SingletonManager.Get<CharacterStateConfigManager>().CharacterControllerJumpHeight);
                addOutput(FsmOutput.Cache);
            }

            return base.HandleInput(command, addOutput);
        }

        
    }
}
