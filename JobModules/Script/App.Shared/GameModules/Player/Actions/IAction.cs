using UnityEngine;

namespace App.Shared.GameModules.Player.Actions
{
    public interface IAction
    {
        void Update();
        void ActionInput(PlayerEntity player);
        void AnimationBehaviour();
        void ResetConcretAction();
        bool PlayingAnimation { get; }
        Vector3 MatchTarget { set; get; }
        bool CanTriggerAction { set; get; }
    }
}
