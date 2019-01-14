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
                (command, addOutput) =>
                {
                    if (command.IsMatch(FsmInput.GenericActionFinished))
                    {
                        FsmOutput.Cache.SetValue(AnimatorParametersHash.Instance.ClimbHash,
                                                 AnimatorParametersHash.Instance.ClimbName,
                                                 AnimatorParametersHash.Instance.ClimbDisableValue,
                                                 CharacterView.ThirdPerson, false);
                        addOutput(FsmOutput.Cache);

                        return FsmStateResponseType.Reenter;
                    }

                    return FsmStateResponseType.Pass;
                },
                (command, addOutput) => FsmTransitionResponseType.NoResponse,
                (int)PostureStateId.Stand, null, 0, new[] { FsmInput.GenericActionFinished });

            #endregion
        }

        public override void DoBeforeEntering(IFsmInputCommand command, Action<FsmOutput> addOutput)
        {
            base.DoBeforeEntering(command, addOutput);
        }

        public override void DoBeforeLeaving(Action<FsmOutput> addOutput)
        {
            base.DoBeforeLeaving(addOutput);
        }
    }
}
