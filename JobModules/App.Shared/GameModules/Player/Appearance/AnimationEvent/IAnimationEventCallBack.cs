using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace App.Shared.GameModules.Player.Appearance.AnimationEvent
{
    public interface IAnimationEventCallback
    {
        void AnimationEventCallback(PlayerEntity player, string param, UnityEngine.AnimationEvent eventParam);
    }
}
