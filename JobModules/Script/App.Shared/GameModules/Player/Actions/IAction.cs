using Core.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace App.Shared.GameModules.Player.Actions
{
    public interface IAction
    {
        void Update();
        void ActionInput(PlayerEntity player);
        void TriggerAnimation();
        void AnimationBehaviour();
        void ResetConcretAction();
        bool PlayingAnimation { get; }
        Vector3 MatchTarget { set; get; }
        bool CanTriggerAction { set; get; }
    }
}
