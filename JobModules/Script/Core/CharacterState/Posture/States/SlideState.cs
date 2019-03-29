using System;
using Core.Fsm;
using Core.Utils;
using UnityEngine;
using Utils.CharacterState;
using Utils.Configuration;
using Utils.Singleton;
using XmlConfig;

namespace Core.CharacterState.Posture.States
{
    class SlideState:PostureState
    {
        private static readonly LoggerAdapter Logger = new LoggerAdapter(typeof(SlideState));
        private static readonly int ToJumpEndDuration = 0;

        public SlideState(PostureStateId id) : base(id)
        {
            InitSpecial();

            InitCommon();
            
        }

        private void InitCommon()
        {
            #region SlideState to swim

            
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

            #region SlideState to dive

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

            #region SlideState to dying
            
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

        private void InitSpecial()
        {
            #region SlideState to jumpend

            AddTransition(
                (command, addOutput) =>
                {
                    if (command.IsMatch(FsmInput.SlideEnd))
                    {
                        //Logger.InfoFormat("Slide End!!!!!, SlideState To JumpEnd");
                        return FsmStateResponseType.Reenter;
                    }

                    return FsmStateResponseType.Pass;
                },
                null,
                (int)PostureStateId.JumpEnd, null, ToJumpEndDuration, new[] { FsmInput.SlideEnd });

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

            FsmOutput.Cache.SetValue(AnimatorParametersHash.Instance.SlideHash,
                AnimatorParametersHash.Instance.SlideName,
                AnimatorParametersHash.Instance.SlideDisable,
                CharacterView.ThirdPerson, false);
            addOutput(FsmOutput.Cache);
            base.DoBeforeLeaving(addOutput);
        }
        
    }
}