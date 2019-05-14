﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Core.Fsm;
using UnityEngine;
using Utils.CharacterState;
using Utils.Configuration;
using Utils.Singleton;
using XmlConfig;

namespace Core.CharacterState.Posture.States
{
    class FreefallState : PostureState
    {
        private bool isBigJump = false;
        public FreefallState(PostureStateId id) : base(id)
        {
			InitSpecial();
            InitCommon();
            
            #region jumpstart to stand

            AddTransition(
                (command, addOutput) =>
                {
                    if (command.IsMatch(FsmInput.MiddleEnterLadder) || 
                        command.IsMatch(FsmInput.EnterLadder))
                    {
                        FsmOutput.Cache.SetValue(AnimatorParametersHash.Instance.JumpStartHash,
                            AnimatorParametersHash.Instance.JumpStartName,
                            AnimatorParametersHash.Instance.JumpStartDisable,
                            CharacterView.FirstPerson | CharacterView.ThirdPerson, false);
                        addOutput(FsmOutput.Cache);
                        
                        FsmOutput.Cache.SetValue(AnimatorParametersHash.Instance.FreeFallHash,
                            AnimatorParametersHash.Instance.FreeFallName,
                            AnimatorParametersHash.Instance.FreeFallDisable,
                            CharacterView.FirstPerson | CharacterView.ThirdPerson, false);
                        addOutput(FsmOutput.Cache);

                        return true;
                    }
                    return false;
                },
                (command, addOutput) => FsmTransitionResponseType.NoResponse,
                (int)PostureStateId.Ladder, null, 0, new[] { FsmInput.MiddleEnterLadder, FsmInput.EnterLadder });

            #endregion
        }

        private void InitSpecial()
        {
            #region freefall to jumpend

            AddTransition(
                (command, addOutput) =>
                {
                    if (command.IsMatch(FsmInput.Land))
                    {
                        return FsmStateResponseType.Reenter;
                    }

                    return FsmStateResponseType.Pass;
                },
                null, (int)PostureStateId.JumpEnd, null, 0, new[] { FsmInput.Land });

            #endregion
        }

        protected void InitCommon()
        {
            
            #region freefall to swim

            
            AddTransition(
                (command, addOutput) =>
                {
                    var ret = command.IsMatch(FsmInput.Swim);

                    if (ret)
                    {
                        FsmOutput.Cache.SetValue(AnimatorParametersHash.Instance.SwimStateHash,
                                                 AnimatorParametersHash.Instance.SwimStateName,
                                                 AnimatorParametersHash.Instance.SwimStateSwimValue,
                                                 CharacterView.ThirdPerson);
                        addOutput(FsmOutput.Cache);

                        command.Handled = true;
                    }

                    return ret;
                },
                null, 
                (int)PostureStateId.Swim, 
                (normalizedTime, addOutput) =>
                {
                    FsmOutput.Cache.SetLayerWeight(AnimatorParametersHash.Instance.SwimLayer,
                        Mathf.Lerp(AnimatorParametersHash.Instance.SwimDisableValue,
                            AnimatorParametersHash.Instance.SwimEnableValue,
                            Mathf.Clamp01(normalizedTime)),
                        CharacterView.ThirdPerson);
                    addOutput(FsmOutput.Cache);
                    
                },
                SingletonManager.Get<CharacterStateConfigManager>().GetPostureTransitionTime(PostureInConfig.Jump, PostureInConfig.Swim),
                new[] { FsmInput.Swim });

            #endregion

            #region freefall to dive

            AddTransition(
                (command, addOutput) =>
                {
                    var ret = command.IsMatch(FsmInput.Dive);

                    if (ret)
                    {
                        FsmOutput.Cache.SetLayerWeight(AnimatorParametersHash.Instance.SwimLayer,
                                                       AnimatorParametersHash.Instance.SwimEnableValue,
                                                       CharacterView.ThirdPerson);
                        addOutput(FsmOutput.Cache);

                        FsmOutput.Cache.SetValue(AnimatorParametersHash.Instance.SwimStateHash,
                                                 AnimatorParametersHash.Instance.SwimStateName,
                                                 AnimatorParametersHash.Instance.SwimStateDiveValue,
                                                 CharacterView.ThirdPerson);
                        addOutput(FsmOutput.Cache);

                        command.Handled = true;
                    }

                    return ret;
                },
                null, (int)PostureStateId.Dive, null, 0, new[] { FsmInput.Dive });

            #endregion

            #region freefall to dyingtrasition
            
            AddTransition(
                (command, addOutput) =>
                {
                    var ret = command.IsMatch(FsmInput.Dying);

                    if (ret)
                    {
                        FsmOutput.Cache.SetValue(AnimatorParametersHash.Instance.InjuredMoveHash,
                            AnimatorParametersHash.Instance.InjuredMoveName,
                            AnimatorParametersHash.Instance.InjuredMoveEnableValue,
                            CharacterView.ThirdPerson,
                            false);
						addOutput(FsmOutput.Cache);
                        command.Handled = true;
                    }

                    return ret;
                },
                null, (int)PostureStateId.DyingTransition, null, 0, new[] { FsmInput.Dying });

            #endregion
        }

        public override void DoBeforeEntering(IFsmInputCommand command, Action<FsmOutput> addOutput)
        {
            FsmOutput.Cache.SetValue(FsmOutputType.CharacterControllerJumpHeight, _characterInfo.GetStandCapsule().Height);
            addOutput(FsmOutput.Cache);
            base.DoBeforeEntering(command, addOutput);
        }

        public override void DoBeforeLeaving(Action<FsmOutput> addOutput)
        {
            FsmOutput.Cache.SetValue(AnimatorParametersHash.Instance.FreeFallHash,
                                     AnimatorParametersHash.Instance.FreeFallName,
                                     AnimatorParametersHash.Instance.FreeFallDisable,
                                     CharacterView.FirstPerson | CharacterView.ThirdPerson, false);
            addOutput(FsmOutput.Cache);

            FsmOutput.Cache.SetValue(AnimatorParametersHash.Instance.JumpStartHash,
                                     AnimatorParametersHash.Instance.JumpStartName,
                                     AnimatorParametersHash.Instance.JumpStartDisable,
                                     CharacterView.FirstPerson | CharacterView.ThirdPerson, false);
            addOutput(FsmOutput.Cache);
            
            FsmOutput.Cache.SetValue(FsmOutputType.CharacterControllerJumpHeight, _characterInfo.GetStandCapsule().Height);
            addOutput(FsmOutput.Cache);
            
            isBigJump = false;
            base.DoBeforeLeaving(addOutput);
        }
        
        public override FsmStateResponseType HandleInput(IFsmInputCommand command, Action<FsmOutput> addOutput)
        {
            if (command.IsMatch(FsmInput.BigJump) && !isBigJump)
            {
                isBigJump = true;
                //FsmOutput.Cache.SetValue(FsmOutputType.CharacterControllerJumpHeight, _characterInfo.GetBigJumpHeight());
                //addOutput(FsmOutput.Cache);
            }
            return base.HandleInput(command, addOutput);
        }
    }
}
