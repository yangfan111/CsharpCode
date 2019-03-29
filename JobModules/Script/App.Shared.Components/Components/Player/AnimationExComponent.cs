using Entitas;
using Entitas.CodeGeneration.Attributes;
using System;

namespace App.Shared.Components.Player
{
    /// <summary>
    /// PlayerEntityAnimationEx 使用的玩家状态信息，需要在死亡等行为后清理
    /// </summary>
    [Player]
    public class AnimationExDataComponent : IComponent
    {
        /// <summary>
        /// 前置行为结束后调调用的动作
        /// </summary>
        [DontInitilize] public Action AnimationWaitToBePlayed;
        /// <summary>
        /// 动作结束后调用的后置行为
        /// </summary>
        [DontInitilize] public Action ActionAfterAllAnimation;

        public void Clear()
        {
            AnimationWaitToBePlayed = null;
            ActionAfterAllAnimation = null;
        }
    }
}
