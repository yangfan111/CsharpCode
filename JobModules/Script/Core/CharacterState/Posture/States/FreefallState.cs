using System;
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
        
        public FreefallState(PostureStateId id) : base(id)
        {
			InitSpecial();
            InitCommon();
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

            #region freefall to dying
            
            AddTransition(
                (command, addOutput) =>
                {
                    var ret = command.IsMatch(FsmInput.Dying);

                    if (ret)
                    {
                        command.Handled = true;

                        FsmOutput.Cache.SetLayerWeight(AnimatorParametersHash.Instance.DyingLayer,
                            AnimatorParametersHash.Instance.DyingEnableValue,
                            CharacterView.ThirdPerson);
                        addOutput(FsmOutput.Cache);
                    }

                    return ret;
                },
                null, (int)PostureStateId.Dying, null, 0, new[] { FsmInput.Dying });

            #endregion
        }

        public override void DoBeforeEntering(IFsmInputCommand command, Action<FsmOutput> addOutput)
        {
            FsmOutput.Cache.SetValue(FsmOutputType.CharacterControllerJumpHeight, SingletonManager.Get<CharacterStateConfigManager>().GetCharacterControllerCapsule(PostureInConfig.Stand).Height);
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
            base.DoBeforeLeaving(addOutput);
        }
    }
}
