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
                if (SharedConfig.ShowAnimationClipEvent)
                    Logger.InfoFormat("AnimationClipEvent-- StringName:  {0}  clipName:  {1} clipWeight:  {2}",
                        eventParam.stringParameter,
                        eventParam.animatorClipInfo.clip.name,
                        eventParam.animatorClipInfo.weight);
                
                DoAnimationEvent(player, eventParam);
                lastFrameCount = Time.frameCount;
            }
        }

        protected abstract void DoAnimationEvent(PlayerEntity playerEntity, UnityEngine.AnimationEvent eventParam);

        protected virtual bool Filter(PlayerEntity player, string param, UnityEngine.AnimationEvent eventParam)
        {
            if (SharedConfig.IsServer ||  lastFrameCount == Time.frameCount ||
                eventParam.animatorClipInfo.weight <= 0.5f)
                return false;
            return true;
        }
    }
}
