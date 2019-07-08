using Core.Utils;
using UnityEngine;

namespace App.Shared.GameModules.Player.Appearance.AnimationEvent
{
    public abstract class AbstractAninamtionEvent 
    {
        protected int lastFrameCount;
        private static readonly LoggerAdapter Logger = new LoggerAdapter(typeof(AbstractAninamtionEvent));
        public void AnimationEventCallback(PlayerEntity player, string param, UnityEngine.AnimationEvent eventParam)
        {
            if (Filter(player, param, eventParam))
            {
                DoAnimationEvent(player);
                lastFrameCount = Time.frameCount;
            }
        }

        protected abstract void DoAnimationEvent(PlayerEntity playerEntity);

        protected virtual bool Filter(PlayerEntity player, string param, UnityEngine.AnimationEvent eventParam)
        {
            if (SharedConfig.IsServer || SharedConfig.IsMute || lastFrameCount == Time.frameCount)
                return false;
            return true;
        }
    }
}