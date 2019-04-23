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
        
        float ModelYTranslateOffset { set; get; }
        float ModelYRotationOffset { set; get; }
        
        Vector3 MatchTarget { set; get; }
        Quaternion MatchQuaternion { set; get; }
        bool CanTriggerAction { set; get; }
    }
}
