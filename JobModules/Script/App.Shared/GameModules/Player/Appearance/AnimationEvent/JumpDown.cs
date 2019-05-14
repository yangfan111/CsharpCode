using App.Shared.Components;
using App.Shared.Components.Player;
using App.Shared.Configuration;
using App.Shared.Terrains;
using Core.Utils;
using Utils.Singleton;
using Utils.Utils;
using XmlConfig;
using App.Shared;
using Core;
using UnityEngine;

namespace App.Shared.GameModules.Player.Appearance.AnimationEvent
{
    public class JumpDown : IAnimationEventCallback
    {
        private int lastFrameCount;
        public void AnimationEventCallback(PlayerEntity player, string param, UnityEngine.AnimationEvent eventParam)
        {
            if (lastFrameCount == Time.frameCount)
               return;
//            DebugUtil.MyLog("Jump down");
            lastFrameCount = Time.frameCount;
            if(player.AudioController() != null)
                player.AudioController().PlayJumpStepAudio();
        }
    }
}
