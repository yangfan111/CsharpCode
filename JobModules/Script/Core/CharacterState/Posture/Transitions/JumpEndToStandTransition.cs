using Core.Fsm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Utils.CharacterState;
using Utils.Configuration;
using Utils.Singleton;

namespace Core.CharacterState.Posture.Transitions
{
    class JumpEndToStandTransition : FsmTransition
    {
        public JumpEndToStandTransition(short id, short target, int duration) : base(id, target, duration)
        {
            _simpleTransferCondition = (command, addOutput) =>
            {
                var ret = SimpleCommandHandler(command, FsmInput.Land);
                return ret;
            };
            _interruptCondition = (command, addOutput) =>
            {
                // 每帧根据动画剩余时间更新NormalizedTime
                if (command.IsMatch(FsmInput.LandProgressP3))
                {
                    NormalizedTime = CalcNormalizedTime(command.AdditioanlValue);
                }
                else if (command.IsMatch(FsmInput.JumpEndFinished))
                {
                    FsmOutput.Cache.SetValue(AnimatorParametersHash.Instance.PostureHash,
                                             AnimatorParametersHash.Instance.PostureName,
                                             AnimatorParametersHash.Instance.StandValue,
                                             CharacterView.FirstPerson | CharacterView.ThirdPerson);
                    addOutput(FsmOutput.Cache);
                    
//                    FsmOutput.Cache.SetValue(AnimatorParametersHash.Instance.JumpStateHash,
//                        AnimatorParametersHash.Instance.JumpStateName,
//                        AnimatorParametersHash.Instance.JumpStateNormal,
//                       CharacterView.ThirdPerson);
//                    addOutput(FsmOutput.Cache);
//                    
//                    FsmOutput.Cache.SetValue(AnimatorParametersHash.Instance.MoveJumpStateHash,
//                        AnimatorParametersHash.Instance.MoveJumpStateName,
//                        AnimatorParametersHash.Instance.MoveJumpStateNormal,
//                        CharacterView.ThirdPerson);
//                    addOutput(FsmOutput.Cache);

                    return FsmTransitionResponseType.ExternalEnd;
                }
                else if (command.IsMatch(FsmInput.Jump))
                {
                    return FsmTransitionResponseType.ForceEnd;
                }
                else if (command.IsMatch(FsmInput.Dying))
                {
                    return FsmTransitionResponseType.ChangeRoad;
                }

                return FsmTransitionResponseType.NoResponse;
            };
        }

        public override void Init(IFsmInputCommand command)
        {
            base.Init(command);
            NormalizedTime = CalcNormalizedTime(0.0f);

        }

        public override bool Update(int frameInterval, Action<FsmOutput> addOutput)
        {
            return false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="normalizedTime">normalizedTime [0,1]区间</param>
        /// <returns></returns>
        private float CalcNormalizedTime(float normalizedTime)
        {
            return 1 - (SingletonManager.Get<CharacterStateConfigManager>().LandSlowDownTime * (1 - normalizedTime) / Duration);
        }
    }
}
