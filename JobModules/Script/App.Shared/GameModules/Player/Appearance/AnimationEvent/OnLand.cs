using Core;
using UnityEngine;
using XmlConfig;

namespace App.Shared.GameModules.Player.Appearance.AnimationEvent
{


    public class OnStep : AbstractAninamtionEvent, IAnimationEventCallback
    {

        protected override bool Filter(PlayerEntity player, string param, UnityEngine.AnimationEvent eventParam)
        {
            return !SharedConfig.IsServer && lastFrameCount != Time.frameCount &&player.IsStepAudioValied();
                            
        }

        protected override void DoAnimationEvent(PlayerEntity player, UnityEngine.AnimationEvent eventParam)
        {
            if (eventParam.animatorClipInfo.weight > 0.5f||player.StateInteractController().GetCurrStates().Contains(EPlayerState.ProneMove))
            {
                var audioController = player.AudioController();
                audioController.StopSwimAudio();
                AudioGrp_Footstep stepState = player.GetFootStepState();
                audioController.PlayFootstepAudioC(stepState);
                lastFrameCount = Time.frameCount;
            }
          
        }
        
        
        
        
        
        
    }

    public class OnStand : AbstractAninamtionEvent, IAnimationEventCallback
    {
        protected override void DoAnimationEvent(PlayerEntity player, UnityEngine.AnimationEvent eventParam)
        {
            player.AudioController().PlaySimpleAudio(EAudioUniqueId.ProneToStand, true);
        }
    }

    public class JumpDown : AbstractAninamtionEvent, IAnimationEventCallback
    {
        protected override void DoAnimationEvent(PlayerEntity player, UnityEngine.AnimationEvent eventParam)
        {
            player.AudioController().PlayJumpStepAudio();
        }
    }
    
    public class JumpUp : AbstractAninamtionEvent, IAnimationEventCallback
    {
        protected override void DoAnimationEvent(PlayerEntity player, UnityEngine.AnimationEvent eventParam)
        {
        }
    }
    public class OnProne : AbstractAninamtionEvent, IAnimationEventCallback
    {
        protected override void DoAnimationEvent(PlayerEntity player, UnityEngine.AnimationEvent eventParam)
        {
            player.AudioController().PlaySimpleAudio(EAudioUniqueId.Prone,true);
            
        }
    }
}
