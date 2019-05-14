using App.Shared.Components;
using App.Shared.Components.Player;
using App.Shared.Configuration;
using App.Shared.Terrains;
using Core.Utils;
using Utils.Singleton;
using Utils.Utils;
using XmlConfig;
using App.Shared;
using App.Shared.Audio;
using Core;
using UnityEngine;

namespace App.Shared.GameModules.Player.Appearance.AnimationEvent
{
    public class JumpUp : IAnimationEventCallback
    {
        private int lastFrameCount;
        public void AnimationEventCallback(PlayerEntity player, string param, UnityEngine.AnimationEvent eventParam)
        {
        //    if (lastFrameCount == Time.frameCount)
        //        return;
        //    lastFrameCount = Time.frameCount;
        ////    DebugUtil.MyLog("Jump Up");
        //    if(player.AudioController() != null)
        //        player.AudioController().PlayJumpStepAudio();

        }
    }
}
