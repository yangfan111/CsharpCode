using Core.CharacterState;
using Core.Configuration;
using Core.Fsm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Utils.Appearance;
using Utils.CharacterState;

namespace Core.Animation
{
    public class AnimatorClipManager
    {
        private readonly AnimatorClipMonitor _thirdClipMonitor = new AnimatorClipMonitor();
        private readonly AnimatorClipMonitor _firstClipMonitor = new AnimatorClipMonitor();

        private float _reloadSpeedBuff = 1.0f;

        public AnimatorClipManager()
        {
        }

        public void SetAnimationCleanEventCallback(Action<AnimationEvent> animationEventCallback)
        {
            _thirdClipMonitor.SetAnimationCleanEventCallback(animationEventCallback);
            _firstClipMonitor.SetAnimationCleanEventCallback(animationEventCallback);
        }

        public void Update(IAdaptiveContainer<IFsmInputCommand> commands, Action<FsmOutput> addOutput, Animator thirdAnimator, Animator firstAnimator, 
            int? weaponId, bool needRewind)
        {
            _thirdClipMonitor.UpdateClipBehavior(thirdAnimator);
            _thirdClipMonitor.Update(addOutput, CharacterView.ThirdPerson, _reloadSpeedBuff);

            _firstClipMonitor.UpdateClipBehavior(firstAnimator);
            _firstClipMonitor.Update(addOutput, CharacterView.FirstPerson, _reloadSpeedBuff);

            GetAnimatorClipTimesByWeaponId(commands, needRewind, weaponId);
        }

        public void SetReloadSpeedBuff(float value)
        {
            _reloadSpeedBuff = value;
        }

        public void ResetReloadSpeedBuff()
        {
            _reloadSpeedBuff = AnimatorParametersHash.DefaultAnimationSpeed;
        }

        private void GetAnimatorClipTimesByWeaponId(IAdaptiveContainer<IFsmInputCommand> commands, bool needRewind, int? weaponId)
        {
            int lenght = commands.Length;
            for (int i = 0; i < lenght; ++i)
            {
                var cmd = commands[i];
                if (needRewind || FsmInput.PickUpProgressP3 == cmd.Type ||
                    FsmInput.SelectProgressP3 == cmd.Type ||
                    FsmInput.HolsterProgressP3 == cmd.Type
                    )
                {
                    _thirdClipMonitor.SetAnimatorClipsTime(weaponId);
                    _firstClipMonitor.SetAnimatorClipsTime(weaponId);
                }
            }
        }
    }
}
