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
    class PostureLadderState : PostureState
    {
        private static LoggerAdapter _logger = new LoggerAdapter(typeof(PostureLadderState));

        private bool isBigJump = false;

        public PostureLadderState(PostureStateId id) : base(id)
        {
            #region ladder to stand

            AddTransition(
                (command, addOutput) =>
                {
                    if (command.IsMatch(FsmInput.ExitLadder) || 
                        command.IsMatch(FsmInput.InterruptLadder))
                    {
                        FsmOutput.Cache.SetValue(AnimatorParametersHash.Instance.PostureHash,
                            AnimatorParametersHash.Instance.PostureName,
                            AnimatorParametersHash.Instance.StandValue,
                            CharacterView.FirstPerson | CharacterView.ThirdPerson);
                        addOutput(FsmOutput.Cache);

                        return true;
                    }
                    return false;
                },
                (command, addOutput) => FsmTransitionResponseType.NoResponse,
                (int)PostureStateId.Stand, null, 0, new[] { FsmInput.ExitLadder, FsmInput.InterruptLadder });

            #endregion
        }

        public override void DoBeforeEntering(IFsmInputCommand command, Action<FsmOutput> addOutput)
        {
            base.DoBeforeEntering(command, addOutput);

            int a = 0;
        }

        public override void DoBeforeLeaving(Action<FsmOutput> addOutput)
        {
            base.DoBeforeLeaving(addOutput);
            
            int a = 0;
        }
    }
}
