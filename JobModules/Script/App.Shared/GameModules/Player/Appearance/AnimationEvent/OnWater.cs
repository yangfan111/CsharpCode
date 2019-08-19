using System;
using Core;
using Core.Utils;
using UnityEngine;

namespace App.Shared.GameModules.Player.Appearance.AnimationEvent
{
    /// <summary>
    /// OnSwimIdle 水面待机
    /// OnSwimSide 水面左右方向键控制的游
    /// OnSwimStraight  水面直着游
    /// OnDiveIdle  潜水待机
    /// OnDive      潜水游
    /// </summary>
    public class OnSwimIdle : AbstractAninamtionEvent,IAnimationEventCallback
    {
        protected override void DoAnimationEvent(PlayerEntity player, UnityEngine.AnimationEvent eventParam)
        {
            player.playerAudio.InWaterState = true;
            player.AudioController().PlaySimpleAudio(EAudioUniqueId.SwimIdle);
        }
    }
    public class OnSwimStraight : AbstractAninamtionEvent,IAnimationEventCallback
    {
        protected override void DoAnimationEvent(PlayerEntity player, UnityEngine.AnimationEvent eventParam)
        {
            player.AudioController().PlaySimpleAudio(EAudioUniqueId.SwimStraight);
        }
    }
    public class OnSwimSide : AbstractAninamtionEvent,IAnimationEventCallback
    {
        protected override void DoAnimationEvent(PlayerEntity player, UnityEngine.AnimationEvent eventParam)
        {
            player.AudioController().PlaySimpleAudio(EAudioUniqueId.SwimSide);
        }
    }
    public class OnDive : AbstractAninamtionEvent,IAnimationEventCallback
    {
        protected override void DoAnimationEvent(PlayerEntity player, UnityEngine.AnimationEvent eventParam)
        {
            player.AudioController().PlaySimpleAudio(EAudioUniqueId.SwimSide);
        }
    }
    public class OnDiveIdle : AbstractAninamtionEvent,IAnimationEventCallback
    {
        protected override void DoAnimationEvent(PlayerEntity player, UnityEngine.AnimationEvent eventParam)
        {
            player.playerAudio.InWaterState = true;
            player.AudioController().PlaySimpleAudio(EAudioUniqueId.Dive);
        }
    }
}

