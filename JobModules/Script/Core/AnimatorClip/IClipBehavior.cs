using Core.Fsm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Utils.CharacterState;

namespace Core.Animation
{
    public interface IClipBehavior
    {
        void OnClipEnter(Animator animator, AnimatorClipInfo clipInfo, int layerIndex);
        void OnClipExit(Animator animator, AnimatorClipInfo clipInfo, AnimatorStateInfo stateInfo, int layerIndex);
        void ChangeSpeedMultiplier(float speed, bool reset = false);
        void UpdateClipBehavior(Animator animator);
        void Update(Action<FsmOutput> addOutput, CharacterView view, float stateSpeedBuff);
    }
}
