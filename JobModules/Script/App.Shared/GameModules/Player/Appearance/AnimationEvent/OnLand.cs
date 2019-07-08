using Core;
using UnityEngine;

namespace App.Shared.GameModules.Player.Appearance.AnimationEvent
{


    public class OnStep : AbstractAninamtionEvent, IAnimationEventCallback
    {

        protected override bool Filter(PlayerEntity player, string param, UnityEngine.AnimationEvent eventParam)
        {
            return base.Filter(player, param, eventParam) && eventParam.animatorClipInfo.weight > 0.5f &&
                            player.IsStepAudioValied();
        }

        protected override void DoAnimationEvent(PlayerEntity player)
        {
            player.AudioController().StopSwimAudio();
            AudioGrp_Footstep stepState = player.GetFootStepState();
            player.AudioController().PlayFootstepAudioC(stepState);
        }
    }

    public class OnStand : AbstractAninamtionEvent, IAnimationEventCallback
    {
        protected override void DoAnimationEvent(PlayerEntity player)
        {
            player.AudioController().PlaySimpleAudio(EAudioUniqueId.ProneToStand, true);
        }
    }

    public class JumpDown : AbstractAninamtionEvent, IAnimationEventCallback
    {
        protected override void DoAnimationEvent(PlayerEntity player)
        {
            player.AudioController().PlayJumpStepAudio();
        }
    }
    
    public class JumpUp : AbstractAninamtionEvent, IAnimationEventCallback
    {
        protected override void DoAnimationEvent(PlayerEntity player)
        {
        }
    }
    public class OnProne : AbstractAninamtionEvent, IAnimationEventCallback
    {
        protected override void DoAnimationEvent(PlayerEntity player)
        {
            player.AudioController().PlaySimpleAudio(EAudioUniqueId.Prone,true);
        }
    }
}
